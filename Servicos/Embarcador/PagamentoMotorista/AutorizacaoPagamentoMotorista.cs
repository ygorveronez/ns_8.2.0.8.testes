using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.PagamentoMotorista
{
    public class AutorizacaoPagamentoMotorista
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public AutorizacaoPagamentoMotorista(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void ReverterPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(_unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(_unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);

            if (pagamentoMotorista == null)
                throw new ServicoException("Pagamento não encontrado.");

            if (pagamentoMotorista.Titulo != null && pagamentoMotorista.Titulo.StatusTitulo == StatusTitulo.Quitada)
                throw new ServicoException("O título a pagar gerado se encontra quitado, favor reverta o mesmo.");

            if (pagamentoMotorista.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.FinalizadoPagamento)
                throw new ServicoException("A situação do pagamento não permite reverter o mesmo.");

            if (pagamentoMotorista.PagamentoMotoristaTipo?.NaoPermitirCancelamento ?? false)
                throw new ServicoException("O tipo de pagamento não permite realizar o cancelamento/reversão.");

            if (repAcertoAdiantamento.ContemPagamentoEmAcerto(pagamentoMotorista.Codigo))
                throw new ServicoException("Remova o adiantamento do acerto antes de reverter o mesmo.");

            if (repMovimentoFinanceiro.ContemMovimentacaoConciliacao(pagamentoMotorista.Numero.ToString("D"), TipoDocumentoMovimento.Pagamento, "PAGAMENTO MOTORISTA"))
                throw new ServicoException("O movimento financeiro deste pagamento está Conciliado, favor reverta a conciliação financeira.");

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(pagamentoMotorista.Motorista.Codigo, true);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) > 0)
            {
                TipoMovimentoEntidade tipoMovimentoEntidade = configuracaoTMS.TipoMovimentoReversaoPagamentoMotorista;
                if (pagamentoMotorista.PagamentoMotoristaTipo != null && pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista.HasValue && pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista.Value != TipoMovimentoEntidade.Nenhum)
                {
                    tipoMovimentoEntidade = pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista.Value;
                    if (tipoMovimentoEntidade == TipoMovimentoEntidade.Entrada)
                        tipoMovimentoEntidade = TipoMovimentoEntidade.Saida;
                    else
                        tipoMovimentoEntidade = TipoMovimentoEntidade.Entrada;
                }

                if (pagamentoMotorista.PlanoDeContaDebito != null && pagamentoMotorista.PlanoDeContaCredito != null)
                    servProcessoMovimento.GerarMovimentacao(null, pagamentoMotorista.DataPagamento, pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista), pagamentoMotorista.Numero.ToString("D"), "REVERSÃO PAGAMENTO MOTORISTA " + pagamentoMotorista.PagamentoMotoristaTipo.Descricao + " " + pagamentoMotorista.Observacao, _unitOfWork, TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, pagamentoMotorista.Motorista.Codigo, pagamentoMotorista.PlanoDeContaCredito, pagamentoMotorista.PlanoDeContaDebito, 0, tipoMovimentoEntidade);
                else if (pagamentoMotorista.PagamentoMotoristaTipo.GerarMovimentoAutomatico && pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoLancamento != null)
                    servProcessoMovimento.GerarMovimentacao(null, pagamentoMotorista.DataPagamento, pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista), pagamentoMotorista.Numero.ToString("D"), "REVERSÃO PAGAMENTO MOTORISTA " + pagamentoMotorista.PagamentoMotoristaTipo.Descricao + " " + pagamentoMotorista.Observacao, _unitOfWork, TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, pagamentoMotorista.Motorista.Codigo, pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoLancamento.PlanoDeContaCredito, pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoLancamento.PlanoDeContaDebito, 0, tipoMovimentoEntidade);

                if (pagamentoMotorista.PagamentoMotoristaTipo.GerarTarifaAutomatica && pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoTarifa != null)
                {
                    decimal valorTarifa = Math.Round(pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) * (pagamentoMotorista.PagamentoMotoristaTipo.PercentualTarifa / 100), 2);
                    servProcessoMovimento.GerarMovimentacao(null, pagamentoMotorista.DataPagamento, valorTarifa, pagamentoMotorista.Numero.ToString("D"), "REVERSÃO TARIFA PAGAMENTO MOTORISTA " + pagamentoMotorista.PagamentoMotoristaTipo.Descricao, _unitOfWork, TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, pagamentoMotorista.Motorista.Codigo, pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoTarifa.PlanoDeContaDebito, pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoTarifa.PlanoDeContaCredito, 0, tipoMovimentoEntidade);
                }

                if (pagamentoMotorista.Titulo != null && pagamentoMotorista.Titulo.TipoMovimento != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(pagamentoMotorista.Titulo.Codigo);

                    servProcessoMovimento.GerarMovimentacao(null, pagamentoMotorista.DataPagamento, pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista), pagamentoMotorista.Numero.ToString("D"), "REVERSÃO DO TÍTULO A PAGAR DO PAGAMENTO MOTORISTA " + pagamentoMotorista.PagamentoMotoristaTipo.Descricao + " " + pagamentoMotorista.Observacao, _unitOfWork, TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito);

                    titulo.StatusTitulo = StatusTitulo.Cancelado;
                    titulo.DataCancelamento = DateTime.Now.Date;
                    titulo.DataAlteracao = DateTime.Now;

                    repTitulo.Atualizar(titulo);
                }
            }

            if (!configuracaoTMS.NaoDescontarValorSaldoMotorista && pagamentoMotorista.SaldoDescontado > 0)
            {
                if (pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista == TipoPagamentoMotorista.Adiantamento)
                {
                    motorista.SaldoAdiantamento += pagamentoMotorista.SaldoDescontado;
                    repUsuario.Atualizar(motorista, auditado);

                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista, pagamentoMotorista.SaldoDescontado, motorista, usuario, _unitOfWork, pagamentoMotorista, null, pagamentoMotorista.DataPagamento, auditado, tipoServicoMultisoftware);
                }
                else if (pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista == TipoPagamentoMotorista.Diaria)
                {
                    motorista.SaldoDiaria += pagamentoMotorista.SaldoDescontado;
                    repUsuario.Atualizar(motorista, auditado);

                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista, pagamentoMotorista.SaldoDescontado, motorista, usuario, _unitOfWork, pagamentoMotorista, null, pagamentoMotorista.DataPagamento, auditado, tipoServicoMultisoftware);
                }
            }

            pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Cancelada;

            repPagamentoMotorista.Atualizar(pagamentoMotorista);

            if (auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, pagamentoMotorista, null, "Reverteu Pagamento.", _unitOfWork);
        }

        public void CancelarIntegracaoPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(_unitOfWork);
            TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista?.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;
            bool sucesso = true;
            string mensagemErro = string.Empty;

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(pagamentoMotorista.Codigo);

            if (pagamentoEnvio != null && !pagamentoEnvio.Cancelado)
            {
                if (pagamentoMotorista.CodigoViagem > 0)
                {
                    if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem)
                    {
                        Servicos.Embarcador.CIOT.Pagbem svcPagbem = new Servicos.Embarcador.CIOT.Pagbem();
                        sucesso = svcPagbem.EstornarPagamentoMotorista(pagamentoMotorista, auditado, _unitOfWork, out mensagemErro);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target)
                    {
                        Servicos.Embarcador.PagamentoMotorista.Target svcTarget = new Servicos.Embarcador.PagamentoMotorista.Target(_unitOfWork);
                        sucesso = svcTarget.EstornarPagamentoMotorista(pagamentoMotorista, out mensagemErro, pagamentoEnvio);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta)
                    {
                        Servicos.Embarcador.CIOT.Extratta svcExtratta = new CIOT.Extratta();
                        sucesso = svcExtratta.EstornarPagamentoMotorista(pagamentoMotorista, out mensagemErro, pagamentoEnvio, _unitOfWork);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.RepomFrete)
                    {
                        Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete svcExtratta = new CIOT.RepomFrete.IntegracaoRepomFrete();
                        sucesso = svcExtratta.EstornarPagamentoMotorista(pagamentoMotorista, out mensagemErro, pagamentoEnvio, _unitOfWork);
                    }
                }
                else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta)
                {
                    Servicos.Embarcador.CIOT.Extratta svcExtratta = new CIOT.Extratta();
                    sucesso = svcExtratta.EstornarPagamentoMotorista(pagamentoMotorista, out mensagemErro, pagamentoEnvio, _unitOfWork);
                }

                if (!sucesso)
                    throw new ServicoException($"Falha ao cancelar o pagamento na operadora: {mensagemErro}");

                pagamentoEnvio.Cancelado = true;
                repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
            }

            #region Integrações Adicionais

            sucesso = true;

            TipoIntegracaoPagamentoMotorista[] tipos = new TipoIntegracaoPagamentoMotorista[]
            {
                TipoIntegracaoPagamentoMotorista.KMM
            };

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> integracoesAutorizadas = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamentoETipoIntegracao(pagamentoMotorista.Codigo, tipos);

            foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvio in integracoesAutorizadas)
            {
                if (pagamentoMotoristaIntegracaoEnvio.Cancelado)
                    continue;

                if (pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.KMM)
                {
                    Servicos.Embarcador.Integracao.KMM.IntegracaoKMM svcKMM = new Integracao.KMM.IntegracaoKMM(_unitOfWork);
                    sucesso = svcKMM.EstornarPagamentoMotorista(pagamentoMotorista, pagamentoEnvio, _unitOfWork);
                }

                if (!sucesso)
                    throw new ServicoException($"Falha ao cancelar o pagamento na integração {pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista.ObterDescricao()}: {mensagemErro}");

                pagamentoMotoristaIntegracaoEnvio.Cancelado = true;
                repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoMotoristaIntegracaoEnvio);
            }

            #endregion
        }

        public object ObterSaldoDescontadoMotorista(int codigoMotorista, int codigoTipoPagamentoMotorista, decimal valor, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            decimal saldoDescontado = 0;
            decimal totalPagamento = valor;

            if (codigoMotorista > 0 && codigoTipoPagamentoMotorista > 0 && valor > 0)
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(_unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = repPagamentoMotoristaTipo.BuscarPorCodigo(codigoTipoPagamentoMotorista);

                if (motorista != null && pagamentoMotoristaTipo != null && pagamentoMotoristaTipo.TipoPagamentoMotorista != TipoPagamentoMotorista.Nenhum && pagamentoMotoristaTipo.TipoPagamentoMotorista != TipoPagamentoMotorista.Terceiro)
                {
                    saldoDescontado = pagamentoMotoristaTipo.TipoPagamentoMotorista == TipoPagamentoMotorista.Diaria ? motorista.SaldoDiaria : motorista.SaldoAdiantamento;
                    if (saldoDescontado > valor)
                        saldoDescontado = valor;
                    if (saldoDescontado < 0 && !configuracao.NaoDescontarValorSaldoMotorista)
                        saldoDescontado = saldoDescontado * -1;

                    if (configuracao.NaoDescontarValorSaldoMotorista)
                        totalPagamento = valor;
                    else
                        totalPagamento = (valor - saldoDescontado);

                    if (totalPagamento < 0)
                        totalPagamento = 0;
                }
            }

            return new
            {
                SaldoDescontado = saldoDescontado.ToString("n2"),
                TotalPagamento = totalPagamento.ToString("n2")
            };
        }

        #endregion

        #region Métodos Públicos Estáticos

        public static List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> VerificarRegrasPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista repRegrasPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> listaRegras = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista>();
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> listaFiltrada = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista>();

            //Regra por Empresa
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> listaRegraEmpresa = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista>();
            if (pagamentoMotoristaTMS.Carga != null)
                listaRegraEmpresa = repRegrasPagamentoMotorista.BuscarRegraPorEmpresa(pagamentoMotoristaTMS.Carga.Empresa.Codigo, pagamentoMotoristaTMS.DataPagamento);
            listaRegras.AddRange(listaRegraEmpresa);

            //Regra por Tipo Pagamento
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> listaRegraTipoPagamento = repRegrasPagamentoMotorista.BuscarRegraPorTipoPagamento(pagamentoMotoristaTMS.PagamentoMotoristaTipo.Codigo, pagamentoMotoristaTMS.DataPagamento);
            listaRegras.AddRange(listaRegraTipoPagamento);

            //Regra por valor
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> listaRegraValor = repRegrasPagamentoMotorista.BuscarRegraPorValor(pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista), pagamentoMotoristaTMS.DataPagamento);
            listaRegras.AddRange(listaRegraValor);

            if (listaRegras.Distinct().Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras.Distinct());

                foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regra in listaRegras.Distinct())
                {
                    if (regra.RegraPorEmpresa)
                    {
                        bool valido = false;
                        if (regra.RegrasPagamentoMotoristaEmpresa.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Empresa.Codigo == pagamentoMotoristaTMS.Carga?.Empresa.Codigo))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaEmpresa.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Empresa.Codigo == pagamentoMotoristaTMS.Carga?.Empresa.Codigo))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaEmpresa.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Empresa.Codigo != pagamentoMotoristaTMS.Carga?.Empresa.Codigo))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaEmpresa.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Empresa.Codigo != pagamentoMotoristaTMS.Carga?.Empresa.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                    if (regra.RegraPorTipo)
                    {
                        bool valido = false;
                        if (regra.RegrasPagamentoMotoristaTipo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.PagamentoMotoristaTipo.Codigo == pagamentoMotoristaTMS.PagamentoMotoristaTipo.Codigo))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaTipo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.PagamentoMotoristaTipo.Codigo == pagamentoMotoristaTMS.PagamentoMotoristaTipo.Codigo))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaTipo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.PagamentoMotoristaTipo.Codigo != pagamentoMotoristaTMS.PagamentoMotoristaTipo.Codigo))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaTipo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.PagamentoMotoristaTipo.Codigo != pagamentoMotoristaTMS.PagamentoMotoristaTipo.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorValor)
                    {
                        bool valido = false;
                        if (regra.RegrasPagamentoMotoristaValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Valor == pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista)))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Valor == pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista)))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Valor != pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista)))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Valor != pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista)))
                            valido = true;
                        if (regra.RegrasPagamentoMotoristaValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) >= o.Valor))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) >= o.Valor))
                            valido = true;
                        if (regra.RegrasPagamentoMotoristaValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) <= o.Valor))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) <= o.Valor))
                            valido = true;
                        if (regra.RegrasPagamentoMotoristaValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) > o.Valor))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) > o.Valor))
                            valido = true;
                        if (regra.RegrasPagamentoMotoristaValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) < o.Valor))
                            valido = true;
                        else if (regra.RegrasPagamentoMotoristaValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && pagamentoMotoristaTMS.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                }
            }

            return listaFiltrada;
        }

        public static void CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> listaFiltrada, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, out bool contemAprovadorIgualAoOperador)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            contemAprovadorIgualAoOperador = false;
            foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regra in listaFiltrada)
            {
                foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao autorizacao = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao();
                    autorizacao.PagamentoMotoristaTMS = pagamentoMotorista;
                    autorizacao.Usuario = aprovador;
                    autorizacao.RegrasPagamentoMotorista = regra;
                    autorizacao.EtapaAutorizacaoOcorrencia = EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia;
                    autorizacao.Data = DateTime.Now;
                    repPagamentoMotoristaAutorizacao.Inserir(autorizacao);

                    string titulo = Localization.Resources.PagamentoMotorista.AutorizacaoPagamentoMotorista.PagamentoMotorista;
                    string nota = string.Format(Localization.Resources.PagamentoMotorista.AutorizacaoPagamentoMotorista.SolicitouLiberacaoPagamentoValorMotorista, usuario.Nome, pagamentoMotorista.PagamentoMotoristaTipo.Descricao, pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista).ToString("n2"), (pagamentoMotorista.Motorista?.Nome ?? string.Empty));
                    serNotificacao.GerarNotificacaoEmail(aprovador, usuario, pagamentoMotorista.Codigo, "PagamentosMotoristas/AutorizacaoPagamentoMotorista", titulo, nota, IconesNotificacao.cifra, TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);

                    if (pagamentoMotorista?.PagamentoMotoristaTipo?.HabilitarAprovacaoAutomaticaCasoOperadorSejaIgualDaAlcada ?? false)
                    {
                        if (aprovador.Codigo == (pagamentoMotorista.Usuario?.Codigo ?? 0))
                            contemAprovadorIgualAoOperador = true;
                    }
                }
            }
        }

        public static bool VerificarRegrasAutorizacaoPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, out bool contemAprovadorIgualAoOperador)
        {
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> listaFiltrada = Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarRegrasPagamentoMotorista(pagamentoMotorista, unitOfWork);
            contemAprovadorIgualAoOperador = false;
            if (listaFiltrada.Count() > 0)
            {
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarRegrasAutorizacao(listaFiltrada, pagamentoMotorista, usuario, tipoServicoMultisoftware, stringConexao, unitOfWork, auditado, out contemAprovadorIgualAoOperador);
                return true;
            }

            return false;
        }

        public static void CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista, decimal valor, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, DateTime dataLancamento, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Acerto.HistoricoSaldoMotorista repHistoricoSaldoMotorista = new Repositorio.Embarcador.Acerto.HistoricoSaldoMotorista(unitOfWork);

            Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista historico = new Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista()
            {
                AcertoViagem = acertoViagem,
                Data = DateTime.Now,
                DataLancamento = dataLancamento,
                Motorista = motorista,
                PagamentoMotoristaTMS = pagamentoMotorista,
                Usuario = usuario,
                Valor = valor,
                TipoPagamentoMotorista = tipoPagamentoMotorista
            };

            repHistoricoSaldoMotorista.Inserir(historico, auditado);
        }

        public static bool CriarPagamentoMotoristaSaldo(ref string msgRetorno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista, decimal valor, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, DateTime dataLancamento, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            msgRetorno = "";
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = repPagamentoMotoristaTipo.BuscarPrimeiroPorTipo(tipoPagamentoMotorista);
            if (pagamentoMotoristaTipo == null)
            {
                msgRetorno = "Não foi encontrado nenhum cadastro de Tipo de Pagamento ao motorista.";
                return false;
            }

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaTMS = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS()
            {
                AcertoViagem = acertoViagem,
                Data = DateTime.Now.Date,
                DataPagamento = acertoViagem.DataFechamento.Value,
                DataVencimentoTituloPagar = acertoViagem.DataFechamento.Value,
                EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Iniciada,
                Motorista = motorista,
                Numero = repPagamentoMotoristaTMS.BuscarProximoNumero(),
                Observacao = "GERADO A PARTIR DO ACERTO DE VIAGEM Nº " + acertoViagem.Numero.ToString() + " REFERENTE AO SALDO FALTANTE",
                PagamentoMotoristaTipo = pagamentoMotoristaTipo,
                SaldoDescontado = 0,
                SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgInformacoes,
                Usuario = usuario,
                Valor = valor,
                PlanoDeContaCredito = usuario.PlanoConta != null ? usuario.PlanoConta : null,
                PlanoDeContaDebito = motorista.PlanoAcertoViagem != null ? motorista.PlanoAcertoViagem : null,
                PagamentoLiberado = true,
                SaldoDiariaMotorista = 0
            };

            Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.CalcularImpostos(ref pagamentoMotorista, unitOfWork, tipoServicoMultisoftware);

            repPagamentoMotoristaTMS.Inserir(pagamentoMotoristaTMS);

            if (VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotoristaTMS, tipoServicoMultisoftware, unitOfWork, usuario, stringConexao, auditado, out bool contemAprovadorIgualAoOperador))
            {
                pagamentoMotoristaTMS.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente;
                pagamentoMotoristaTMS.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.AgAutorizacao;
            }
            else
            {
                pagamentoMotoristaTMS.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao;
                pagamentoMotoristaTMS.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Integracao;
            }

            if (contemAprovadorIgualAoOperador)
            {
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPrimeiroPorPagamentoUsuario(pagamentoMotorista.Codigo, pagamentoMotorista.Usuario.Codigo);

                EfetuarAprovacao(pagamentoMotoristaAutorizacao, pagamentoMotorista.Usuario, unitOfWork, stringConexao, tipoServicoMultisoftware, configuracaoTMS);

                msgRetorno = "";
                VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unitOfWork, ref msgRetorno, tipoServicoMultisoftware, auditado, stringConexao, configuracaoTMS, pagamentoMotorista.Usuario);
                Servicos.Auditoria.Auditoria.Auditar(auditado, pagamentoMotorista, null, "Aprovou o pagamento pelo mesmo operadora da alçada.", unitOfWork);
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

            TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista;
            if (tipoIntegracaoPagamentoMotorista.PossuiIntegracao())
            {
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvio = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio();
                pagamentoMotoristaIntegracaoEnvio.Data = DateTime.Now.Date;
                pagamentoMotoristaIntegracaoEnvio.NumeroTentativas = 0;
                pagamentoMotoristaIntegracaoEnvio.PagamentoMotoristaTMS = pagamentoMotoristaTMS;
                pagamentoMotoristaIntegracaoEnvio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista = tipoIntegracaoPagamentoMotorista;

                repPagamentoMotoristaIntegracaoEnvio.Inserir(pagamentoMotoristaIntegracaoEnvio);

                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
            }
            else if (pagamentoMotoristaTMS.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
            {
                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                {
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
                }
                else
                {
                    pagamentoMotoristaTMS.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                    pagamentoMotoristaTMS.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                    if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista?.Codigo ?? pagamentoMotoristaTMS.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, auditado, usuario, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware);
                    }
                }
            }

            return true;
        }

        public static bool ConfirmarPagamentoMotorista(ref string msgRetorno, int codigoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoMovimentoEntidade, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (auditado == null)
            {
                auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                };
            }

            msgRetorno = "";

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repConfiguracaoTipoPagamento = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotorista.BuscarPorCodigo(codigoPagamentoMotorista, true);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo configTipoPagamento = repConfiguracaoTipoPagamento.BuscarPorCodigo(pagamentoMotorista.PagamentoMotoristaTipo.Codigo);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);

            Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado repTipoMovimentoCentroResultado = new Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unidadeDeTrabalho);

            if (pagamentoMotorista == null)
            {
                msgRetorno = "Pagamento não encontrado.";
                return false;
            }

            if (pagamentoMotorista.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Finalizada)
            {
                msgRetorno = "A situação do pagamento não permite a confirmação do pagamento.";
                return false;
            }

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = pagamentoMotorista.PagamentoMotoristaTipo;
            TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista;

            if (pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) > 0)
            {
                DateTime dataMovimentacao = DateTime.Now.Date;
                if (configuracaoTMS.RealizarMovimentacaPagamentoMotoristaPelaDataPagamento)
                    dataMovimentacao = pagamentoMotorista.DataPagamento;
                else if (configTipoPagamento.RealizarMovimentoFinanceiroPelaDataPagamento)
                    dataMovimentacao = pagamentoMotorista.DataPagamento;
                else if (configuracaoTMS.RealizarMovimentacaoPamcardProximoDiaUtil && (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target))
                {
                    dataMovimentacao = DateTime.Now.Date.AddDays(1);
                    dataMovimentacao = RetornarProximaDataValida(dataMovimentacao, unidadeDeTrabalho);
                }

                Dominio.Entidades.Cliente pessoa = null;
                if (pagamentoMotorista.Motorista != null)
                {
                    double.TryParse(pagamentoMotorista.Motorista.CPF, out double cpfMotorosita);
                    pessoa = repCliente.BuscarPorCPFCNPJ(cpfMotorosita);
                }

                if (pagamentoMotoristaTipo != null && pagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista.HasValue && pagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Nenhum)
                    tipoMovimentoEntidade = pagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista.Value;

                if (pagamentoMotorista.PlanoDeContaDebito != null && pagamentoMotorista.PlanoDeContaCredito != null)// && pagamentoMotoristaTipo.GerarMovimentoAutomatico)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = null;
                    if (pagamentoMotoristaTipo.GerarMovimentoAutomatico &&
                        pagamentoMotoristaTipo.TipoMovimentoLancamento?.PlanoDeContaCredito?.Codigo == pagamentoMotorista.PlanoDeContaDebito.Codigo &&
                        pagamentoMotoristaTipo.TipoMovimentoLancamento?.PlanoDeContaDebito?.Codigo == pagamentoMotorista.PlanoDeContaCredito.Codigo)
                        tipoMovimento = pagamentoMotoristaTipo.TipoMovimentoLancamento;

                    servProcessoMovimento.GerarMovimentacao(tipoMovimento, dataMovimentacao, pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista), pagamentoMotorista.Numero.ToString("D"), "PAGAMENTO MOTORISTA " + pagamentoMotoristaTipo.Descricao + " " + pagamentoMotorista.Observacao + " " + pagamentoMotorista.Motorista?.Nome, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, pagamentoMotorista.Motorista.Codigo, pagamentoMotorista.PlanoDeContaDebito, pagamentoMotorista.PlanoDeContaCredito, 0, tipoMovimentoEntidade, pessoa);
                }
                else if (pagamentoMotoristaTipo.GerarMovimentoAutomatico)
                    servProcessoMovimento.GerarMovimentacao(pagamentoMotoristaTipo.TipoMovimentoLancamento, dataMovimentacao, pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista), pagamentoMotorista.Numero.ToString("D"), "PAGAMENTO MOTORISTA " + pagamentoMotoristaTipo.Descricao + " " + pagamentoMotorista.Observacao + " " + pagamentoMotorista.Motorista?.Nome, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, pagamentoMotorista.Motorista.Codigo, null, null, 0, tipoMovimentoEntidade, pessoa);

                if (pagamentoMotoristaTipo.GerarTarifaAutomatica)
                {
                    if (configuracaoTMS.RealizarMovimentacaPagamentoMotoristaPelaDataPagamento)
                        dataMovimentacao = pagamentoMotorista.DataPagamento;
                    else if (configuracaoTMS.RealizarMovimentacaoPamcardProximoDiaUtil && (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target))
                    {
                        dataMovimentacao = dataMovimentacao.AddDays(1);
                        dataMovimentacao = RetornarProximaDataValida(dataMovimentacao, unidadeDeTrabalho);
                    }
                    else
                        dataMovimentacao = DateTime.Now.Date;
                    decimal valorTarifa = Math.Round(pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) * (pagamentoMotoristaTipo.PercentualTarifa / 100), 2);
                    servProcessoMovimento.GerarMovimentacao(null, dataMovimentacao, valorTarifa, pagamentoMotorista.Numero.ToString("D"), "TARIFA PAGAMENTO MOTORISTA " + pagamentoMotoristaTipo.Descricao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, pagamentoMotorista.Motorista.Codigo, pagamentoMotoristaTipo.TipoMovimentoTarifa.PlanoDeContaCredito, pagamentoMotoristaTipo.TipoMovimentoTarifa.PlanoDeContaDebito, 0, tipoMovimentoEntidade, pessoa);
                }

                if (pagamentoMotorista.DataVencimentoTituloPagar != null && pagamentoMotorista.DataVencimentoTituloPagar.Value > DateTime.MinValue && pagamentoMotoristaTipo != null && pagamentoMotoristaTipo.GerarTituloPagar
                    && pagamentoMotoristaTipo.TipoMovimentoTituloPagar != null && (pagamentoMotoristaTipo.Pessoa != null || pagamentoMotorista.PessoaTituloPagar != null))
                {
                    if (configuracaoTMS.RealizarMovimentacaPagamentoMotoristaPelaDataPagamento)
                        dataMovimentacao = pagamentoMotorista.DataPagamento;
                    else if (configuracaoTMS.RealizarMovimentacaoPamcardProximoDiaUtil && (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target))
                    {
                        dataMovimentacao = DateTime.Now.Date;
                        dataMovimentacao = RetornarProximaDataValida(dataMovimentacao, unidadeDeTrabalho);
                    }
                    else
                        dataMovimentacao = pagamentoMotorista.DataPagamento;

                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorPessoaTipoTitulo(pagamentoMotorista.PessoaTituloPagar?.CPF_CNPJ ?? pagamentoMotoristaTipo.Pessoa.CPF_CNPJ, pagamentoMotorista.Numero.ToString("D"), "PAG. MOTORISTA", TipoTitulo.Pagar);

                    if (titulo == null)
                    {
                        titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                        {
                            LiberadoPagamento = false,
                            DataVencimento = pagamentoMotorista.DataVencimentoTituloPagar.Value,
                            DataProgramacaoPagamento = pagamentoMotorista.DataVencimentoTituloPagar.Value,
                            Empresa = null,
                            Observacao = "COBRANÇA AUTOMÁTICA DO PAGAMENTO MOTORISTA " + pagamentoMotoristaTipo.Descricao + " " + pagamentoMotorista.Observacao + (pagamentoMotorista.Carga != null ? " CARGA: " + pagamentoMotorista.Carga.CodigoCargaEmbarcador : ""),
                            Pessoa = pagamentoMotorista.PessoaTituloPagar ?? pagamentoMotoristaTipo.Pessoa,
                            Sequencia = 1,
                            StatusTitulo = StatusTitulo.EmAberto,
                            TipoTitulo = TipoTitulo.Pagar,
                            ValorOriginal = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                            ValorPendente = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                            DataAlteracao = DateTime.Now,
                            Acrescimo = 0,
                            DataEmissao = dataMovimentacao,
                            FormaTitulo = FormaTitulo.Outros,
                            ValorTituloOriginal = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                            Desconto = 0,
                            TipoMovimento = pagamentoMotoristaTipo.TipoMovimentoTituloPagar,
                            Valor = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                            ValorTotal = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                            TipoDocumentoTituloOriginal = "PAG. MOTORISTA",
                            NumeroDocumentoTituloOriginal = pagamentoMotorista.Numero.ToString("D"),
                            DataLancamento = DateTime.Now,
                            Usuario = pagamentoMotorista.Usuario,
                            PagamentoMotorista = pagamentoMotorista
                        };

                        titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;

                        repTitulo.Inserir(titulo);

                        GerarRateioDespesaVeiculo(ref msgRetorno, titulo, auditado, usuario, unidadeDeTrabalho);

                        servProcessoMovimento.GerarMovimentacao(pagamentoMotoristaTipo.TipoMovimentoTituloPagar, dataMovimentacao, pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista), pagamentoMotorista.Numero.ToString("D"), "COBRANÇA AUTOMÁTICA DO PAGAMENTO MOTORISTA " + pagamentoMotoristaTipo.Descricao + " " + pagamentoMotorista.Observacao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, pessoa);

                        GerarRateioDespesaVeiculo(ref msgRetorno, pagamentoMotorista, auditado, usuario, unidadeDeTrabalho);

                        Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa tipoDespesaTipoMovimento = repTipoMovimentoTipoDespesa.BuscarPorTipoMovimento(titulo.TipoMovimento?.Codigo ?? 0).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado centroResultadoTipoMovimento = repTipoMovimentoCentroResultado.BuscarPorTipoMovimento(titulo.TipoMovimento?.Codigo ?? 0).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa centrosTiposDespesas;

                        centrosTiposDespesas = new Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa
                        {
                            Titulo = titulo,
                            CentroResultado = centroResultadoTipoMovimento.CentroResultado,
                            TipoDespesaFinanceira = tipoDespesaTipoMovimento.TipoDespesaFinanceira,
                            Percentual = 100
                        };

                        repTituloCentroResultadoTipoDespesa.Inserir(centrosTiposDespesas);
                    }

                    pagamentoMotorista.Titulo = titulo;
                }

                if (pagamentoMotorista.DataVencimentoTituloPagar != null && pagamentoMotorista.DataVencimentoTituloPagar.Value > DateTime.MinValue && pagamentoMotoristaTipo != null && pagamentoMotoristaTipo.GerarTituloAPagarAoMotorista
                    && pagamentoMotoristaTipo.TipoMovimentoTituloMotorista != null)
                {
                    if (configuracaoTMS.RealizarMovimentacaPagamentoMotoristaPelaDataPagamento)
                        dataMovimentacao = pagamentoMotorista.DataPagamento;
                    else if (configuracaoTMS.RealizarMovimentacaoPamcardProximoDiaUtil && (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target))
                    {
                        dataMovimentacao = DateTime.Now.Date;
                        dataMovimentacao = RetornarProximaDataValida(dataMovimentacao, unidadeDeTrabalho);
                    }
                    else
                        dataMovimentacao = pagamentoMotorista.DataPagamento;

                    if (pagamentoMotoristaTipo.GerarTituloAPagarAoMotorista && pagamentoMotorista.Motorista != null)
                    {
                        double.TryParse(pagamentoMotorista.Motorista.CPF, out double cpfMotorista);
                        Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfMotorista);
                        if (cliente == null)
                        {
                            Dominio.Entidades.Usuario motoristaCadastrar = repUsuario.BuscarPorCPF(pagamentoMotorista.Motorista.CPF);
                            if (motoristaCadastrar != null)
                            {
                                if (motoristaCadastrar.Localidade != null)
                                {
                                    cliente = Servicos.Embarcador.Pessoa.Pessoa.ConverterFuncionario(motoristaCadastrar, unidadeDeTrabalho);
                                    repCliente.Inserir(cliente);
                                    Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, null, "Adicionado automaticamente a partir do cadastro de motoristas.", unidadeDeTrabalho);
                                }
                                else
                                    msgRetorno = $"O Motorista { motoristaCadastrar.Nome } está com endereço incompleto em seu cadastro, assim não foi possível cadastrar como fornecedor! Favor gerar o título a pagar de forma manual. ";
                            }
                        }

                        if (cliente != null)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Titulo tituloMotorista = repTitulo.BuscarPorPessoaTipoTitulo(cliente.CPF_CNPJ, pagamentoMotorista.Numero.ToString("D"), "PAG. MOTORISTA", TipoTitulo.Pagar);

                            if (tituloMotorista == null)
                            {
                                tituloMotorista = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                                {
                                    LiberadoPagamento = false,
                                    DataVencimento = pagamentoMotorista.DataVencimentoTituloPagar.Value,
                                    DataProgramacaoPagamento = pagamentoMotorista.DataVencimentoTituloPagar.Value,
                                    Empresa = null,
                                    Observacao = "COBRANÇA AUTOMÁTICA DO PAGAMENTO MOTORISTA " + pagamentoMotoristaTipo.Descricao + " " + pagamentoMotorista.Observacao + (pagamentoMotorista.Carga != null ? " CARGA: " + pagamentoMotorista.Carga.CodigoCargaEmbarcador : ""),
                                    Pessoa = cliente,
                                    Sequencia = 1,
                                    StatusTitulo = StatusTitulo.EmAberto,
                                    TipoTitulo = TipoTitulo.Pagar,
                                    ValorOriginal = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                                    ValorPendente = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                                    DataAlteracao = DateTime.Now,
                                    Acrescimo = 0,
                                    DataEmissao = dataMovimentacao,
                                    FormaTitulo = FormaTitulo.Outros,
                                    ValorTituloOriginal = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                                    Desconto = 0,
                                    TipoMovimento = pagamentoMotoristaTipo.TipoMovimentoTituloMotorista,
                                    Valor = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                                    ValorTotal = pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista),
                                    TipoDocumentoTituloOriginal = "PAG. MOTORISTA",
                                    NumeroDocumentoTituloOriginal = pagamentoMotorista.Numero.ToString("D"),
                                    DataLancamento = DateTime.Now,
                                    Usuario = pagamentoMotorista.Usuario,
                                    PagamentoMotorista = pagamentoMotorista
                                };

                                tituloMotorista.GrupoPessoas = tituloMotorista.Pessoa.GrupoPessoas;

                                repTitulo.Inserir(tituloMotorista);

                                GerarRateioDespesaVeiculo(ref msgRetorno, tituloMotorista, auditado, usuario, unidadeDeTrabalho);

                                servProcessoMovimento.GerarMovimentacao(pagamentoMotoristaTipo.TipoMovimentoTituloMotorista, dataMovimentacao, pagamentoMotorista.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista), pagamentoMotorista.Numero.ToString("D"), "COBRANÇA AUTOMÁTICA DO PAGAMENTO MOTORISTA " + pagamentoMotoristaTipo.Descricao + " " + pagamentoMotorista.Observacao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, 0, null, null, tituloMotorista.Codigo, null, pessoa);

                                GerarRateioDespesaVeiculo(ref msgRetorno, pagamentoMotorista, auditado, usuario, unidadeDeTrabalho);
                            }

                            pagamentoMotorista.Titulo = tituloMotorista;

                        }
                    }
                }
            }

            pagamentoMotorista.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento;
            repPagamentoMotorista.Atualizar(pagamentoMotorista);

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(pagamentoMotorista.Motorista.Codigo, true);

            if (!configuracaoTMS.NaoDescontarValorSaldoMotorista && pagamentoMotoristaTipo.TipoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento && pagamentoMotorista.SaldoDescontado > 0)
            {
                motorista.SaldoAdiantamento -= pagamentoMotorista.SaldoDescontado;
                repUsuario.Atualizar(motorista, auditado);

                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(pagamentoMotoristaTipo.TipoPagamentoMotorista, (pagamentoMotorista.SaldoDescontado * -1), motorista, usuario, unidadeDeTrabalho, pagamentoMotorista, null, pagamentoMotorista.DataPagamento, auditado, tipoServicoMultisoftware);
            }
            else if (!configuracaoTMS.NaoDescontarValorSaldoMotorista && pagamentoMotoristaTipo.TipoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria && pagamentoMotorista.SaldoDescontado > 0)
            {
                motorista.SaldoDiaria -= pagamentoMotorista.SaldoDescontado;
                repUsuario.Atualizar(motorista, auditado);

                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(pagamentoMotoristaTipo.TipoPagamentoMotorista, (pagamentoMotorista.SaldoDescontado * -1), motorista, usuario, unidadeDeTrabalho, pagamentoMotorista, null, pagamentoMotorista.DataPagamento, auditado, tipoServicoMultisoftware);
            }

            if (auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, pagamentoMotorista, null, "Confirmou Pagamento.", unidadeDeTrabalho);

            return true;
        }

        public static void VerificarSituacaoPagamento(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Repositorio.UnitOfWork unitOfWork, ref string msgRetorno, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string stringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Usuario usuario)
        {
            msgRetorno = string.Empty;

            // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
            if (pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgAprovacao || pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente)
            {
                // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

                // Busca todas regras da ocorrencia (Distintas)
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> regras = repPagamentoMotoristaAutorizacao.BuscarRegrasPagamento(pagamentoMotorista.Codigo);

                // Flag de rejeicao
                bool rejeitada = false;
                bool aprovada = true;

                foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regra in regras)
                {
                    // Quantidade de usuarios que marcaram como aprovado ou rejeitado
                    int pendentes = repPagamentoMotoristaAutorizacao.ContarPendentes(pagamentoMotorista.Codigo, regra.Codigo); // P

                    // Quantidade de aprovacoes
                    int aprovacoes = repPagamentoMotoristaAutorizacao.ContarAprovacoesOcorrencia(pagamentoMotorista.Codigo, regra.Codigo); // A

                    int rejeitadas = repPagamentoMotoristaAutorizacao.ContarRejeitadas(pagamentoMotorista.Codigo, regra.Codigo); // R

                    // Numero de aprovacoes minimas
                    int necessariosParaAprovar = regra.NumeroAprovadores; // N

                    if (rejeitadas > 0)
                        rejeitada = true; // Se uma regra foi reprovada, a carga ocorrencia é reprovada
                    else if (aprovacoes < necessariosParaAprovar) // A >= N -> Aprovacoes > NumeroMinimo
                        aprovada = false; // Se nao esta rejeitada e nem reprovada, esta pendente (nao faz nada)
                }

                // Define situacao da ocorrencia
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgAprovacao;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.AgAutorizacao;

                // Rejeicao na autorizacao
                if (rejeitada && (pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgAprovacao || pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente))
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Rejeitada;

                // Se houve alteracao de status, atualiza etapa da ocorencia
                if (rejeitada || aprovada)
                {
                    // Verifica se a situacao e ag aprovacao para testar a regra de etapa ag emissao
                    if ((pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgAprovacao || pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente) && !rejeitada)
                    {
                        
                        if (pagamentoMotorista.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista.SemIntegracao)
                        {
                            if(PossuiIntegracaoPendente(pagamentoMotorista, unitOfWork))
                            {
                                etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Integracao;
                                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao;
                            }
                            else
                            {
                                etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Integracao;
                                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Finalizada;
                            }
                            
                        }
                        else
                        {
                            etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Integracao;
                            situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao;
                        }
                    }

                    // Seta a nova situacao
                    pagamentoMotorista.SituacaoPagamentoMotorista = situacao;
                    pagamentoMotorista.EtapaPagamentoMotorista = etapa;

                    if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Finalizada && configuracaoEmbarcador.ConfirmarPagamentoMotoristaAutomaticamente)
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, configuracaoEmbarcador.TipoMovimentoPagamentoMotorista, auditado, pagamentoMotorista.Usuario, unitOfWork, stringConexao, tipoServicoMultisoftware);

                    repPagamentoMotoristaTMS.Atualizar(pagamentoMotorista);

                    // Define icone
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                    if (rejeitada)
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                    else
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                    // Emite notificação
                    string mensagem = string.Format(Localization.Resources.PagamentoMotorista.AutorizacaoPagamentoMotorista.PagamentoValorMotoristaFoi, pagamentoMotorista.PagamentoMotoristaTipo.Descricao, pagamentoMotorista.TotalPagamento(configuracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2"), (pagamentoMotorista.Motorista?.Nome ?? string.Empty), rejeitada ? "rejeitado" : "aprovado");
                    serNotificacao.GerarNotificacao(pagamentoMotorista.Usuario, usuario, pagamentoMotorista.Codigo, "PagamentosAgregados/PagamentoAgregado", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                }
            }
        }

        public static bool CriarPagamentoMotoristaAdiantamento(ref string msgRetorno,string observacao, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo, decimal valor, Dominio.Entidades.Usuario motorista,Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            msgRetorno = "";
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (pagamentoMotoristaTipo == null)
            {
                msgRetorno = "Não foi encontrado nenhum cadastro de Tipo de Pagamento ao motorista.";
                return false;
            }

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaTMS = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS()
            {
                Carga = carga,
                Data = DateTime.Now.Date,
                DataPagamento = DateTime.Now.Date,
                EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Iniciada,
                Motorista = motorista,
                Numero = repPagamentoMotoristaTMS.BuscarProximoNumero(),
                Observacao = observacao,
                PagamentoMotoristaTipo = pagamentoMotoristaTipo,
                SaldoDescontado = 0,
                SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgInformacoes,
                Usuario = usuario,
                Valor = valor,
                PlanoDeContaCredito = usuario.PlanoConta != null ? usuario.PlanoConta : null,
                PlanoDeContaDebito = motorista.PlanoAcertoViagem != null ? motorista.PlanoAcertoViagem : null,
                PagamentoLiberado = true,
                SaldoDiariaMotorista = 0
            };

            Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.CalcularImpostos(ref pagamentoMotoristaTMS, unitOfWork, tipoServicoMultisoftware);

            repPagamentoMotoristaTMS.Inserir(pagamentoMotoristaTMS);

            if (VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotoristaTMS, tipoServicoMultisoftware, unitOfWork, usuario, stringConexao, auditado, out bool contemAprovadorIgualAoOperador))
            {
                pagamentoMotoristaTMS.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente;
                pagamentoMotoristaTMS.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.AgAutorizacao;
            }
            else
            {
                pagamentoMotoristaTMS.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao;
                pagamentoMotoristaTMS.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Integracao;
            }

            if (contemAprovadorIgualAoOperador)
            {
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPrimeiroPorPagamentoUsuario(pagamentoMotoristaTMS.Codigo, pagamentoMotoristaTMS.Usuario.Codigo);

                EfetuarAprovacao(pagamentoMotoristaAutorizacao, pagamentoMotoristaTMS.Usuario, unitOfWork, stringConexao, tipoServicoMultisoftware, configuracaoTMS);

                msgRetorno = "";
                VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unitOfWork, ref msgRetorno, tipoServicoMultisoftware, auditado, stringConexao, configuracaoTMS, pagamentoMotoristaTMS.Usuario);
                Servicos.Auditoria.Auditoria.Auditar(auditado, pagamentoMotoristaTMS, null, "Aprovou o pagamento pelo mesmo operadora da alçada.", unitOfWork);
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

            TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista;
            if (tipoIntegracaoPagamentoMotorista.PossuiIntegracao())
            {
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvio = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio();
                pagamentoMotoristaIntegracaoEnvio.Data = DateTime.Now.Date;
                pagamentoMotoristaIntegracaoEnvio.NumeroTentativas = 0;
                pagamentoMotoristaIntegracaoEnvio.PagamentoMotoristaTMS = pagamentoMotoristaTMS;
                pagamentoMotoristaIntegracaoEnvio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista = tipoIntegracaoPagamentoMotorista;

                repPagamentoMotoristaIntegracaoEnvio.Inserir(pagamentoMotoristaIntegracaoEnvio);

                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotoristaTMS, unitOfWork);
            }
            else if (pagamentoMotoristaTMS.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
            {
                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                {
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotoristaTMS, unitOfWork);
                }
                else
                {
                    pagamentoMotoristaTMS.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                    pagamentoMotoristaTMS.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                    if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotoristaTMS?.Codigo ?? pagamentoMotoristaTMS.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, auditado, usuario, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware);
                    }
                }
            }

            return true;
        }
        public static void EfetuarAprovacao(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (pagamentoMotoristaAutorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente && pagamentoMotoristaAutorizacao.Usuario.Codigo == usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                pagamentoMotoristaAutorizacao.Data = DateTime.Now;
                pagamentoMotoristaAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada;

                // Atualiza os dados
                repPagamentoMotoristaAutorizacao.Atualizar(pagamentoMotoristaAutorizacao);

                // Notifica usuario que criou a ocorrencia
                NotificarAlteracao(true, pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, usuario, unitOfWork, stringConexao, tipoServicoMultisoftware, configuracaoEmbarcador);
            }
        }

        public static void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

                // Define icone
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                if (aprovada)
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;
                else
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;

                // Emite notificação
                string titulo = "Pagamento";
                string mensagem = string.Format(Localization.Resources.PagamentoMotorista.AutorizacaoPagamentoMotorista.UsuarioPagamentoValorParaMotorista, (aprovada ? Localization.Resources.Gerais.Geral.Aprovou : Localization.Resources.Gerais.Geral.Rejeitou), pagamentoMotorista.PagamentoMotoristaTipo.Descricao, pagamentoMotorista.TotalPagamento(configuracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2"), (pagamentoMotorista.Motorista?.Nome ?? string.Empty));
                serNotificacao.GerarNotificacaoEmail(pagamentoMotorista.Usuario, usuario, pagamentoMotorista.Codigo, "PagamentosMotoristas/PagamentoMotoristaTMS", titulo, mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

        #region Métodos Privados

        private static bool PossuiIntegracaoPendente(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
            {
                Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
                return true;
            }

            return false;
            
        }
        private static DateTime RetornarProximaDataValida(DateTime dataMovimentacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (dataMovimentacao.DayOfWeek == DayOfWeek.Saturday)
                dataMovimentacao = dataMovimentacao.AddDays(2);
            else if (dataMovimentacao.DayOfWeek == DayOfWeek.Sunday)
                dataMovimentacao = dataMovimentacao.AddDays(1);

            bool dataValida = true;
            Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(unidadeDeTrabalho);

            while (dataValida)
            {
                if (dataMovimentacao.DayOfWeek == DayOfWeek.Saturday || dataMovimentacao.DayOfWeek == DayOfWeek.Sunday)
                    dataMovimentacao = dataMovimentacao.AddDays(1);
                else if (servicoFeriado.VerificarSePossuiFeriado(dataMovimentacao))
                    dataMovimentacao = dataMovimentacao.AddDays(1);
                else
                    dataValida = false;
            }

            return dataMovimentacao;
        }

        private static void GerarRateioDespesaVeiculo(ref string msgRetorno, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (!pagamentoMotorista.PagamentoMotoristaTipo.GerarTituloAPagarAoMotorista || !pagamentoMotorista.PagamentoMotoristaTipo.RealizarRateio || pagamentoMotorista.PagamentoMotoristaTipo.TipoDespesa == null)
                return;

            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo repRateioDespesaVeiculoValorVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Veiculo veiculo = repVeiculoMotorista.BuscarVeiculoPorMotorista(pagamentoMotorista.Motorista.Codigo);
            if (veiculo == null)
                veiculo = repCargaMotorista.BuscarVeiculoUltimaCargaMotorista(pagamentoMotorista.Motorista.Codigo);
            if (veiculo == null)
                return;

            Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo()
            {
                DataInicial = pagamentoMotorista.DataPagamento.Date,
                DataFinal = pagamentoMotorista.DataPagamento.Date,
                Valor = pagamentoMotorista.Valor,
                TipoDespesa = pagamentoMotorista.PagamentoMotoristaTipo.TipoDespesa,
                NumeroDocumento = pagamentoMotorista.Numero.ToString(),
                TipoDocumento = "PAG MOTORISTA",
                Pessoa = pagamentoMotorista.PagamentoMotoristaTipo?.Pessoa != null ? pagamentoMotorista.PagamentoMotoristaTipo?.Pessoa : repCliente.BuscarPorCPFCNPJ(pagamentoMotorista.Motorista.CPF.ToDouble()),
                Colaborador = usuario,
                PagamentoMotorista = pagamentoMotorista,
                Origem = OrigemRateioDespesaVeiculo.PagamentoMotorista
            };

            repRateioDespesaVeiculo.Inserir(rateioDespesaVeiculo);

            Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo rateioDespesaVeiculoValorVeiculo = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo()
            {
                Veiculo = veiculo,
                DespesaVeiculo = rateioDespesaVeiculo,
                Valor = pagamentoMotorista.Valor
            };
            repRateioDespesaVeiculoValorVeiculo.Inserir(rateioDespesaVeiculoValorVeiculo);

            Servicos.Auditoria.Auditoria.Auditar(auditado, rateioDespesaVeiculo, "Adicionado pelo pagamento de motorista " + pagamentoMotorista.Numero, unitOfWork);

            Servicos.Embarcador.Financeiro.RateioDespesaVeiculo servicoRateioDespesaVeiculo = new Servicos.Embarcador.Financeiro.RateioDespesaVeiculo(unitOfWork);
            servicoRateioDespesaVeiculo.RatearValorEntreVeiculos(out msgRetorno, rateioDespesaVeiculo);
        }

        private static void GerarRateioDespesaVeiculo(ref string msgRetorno, Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (!titulo.PagamentoMotorista.PagamentoMotoristaTipo.GerarTituloPagar || !titulo.PagamentoMotorista.PagamentoMotoristaTipo.RealizarRateio || titulo.PagamentoMotorista.PagamentoMotoristaTipo.TipoDespesa == null || titulo.PagamentoMotorista.Motorista == null)
                return;

            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo repRateioDespesaVeiculoValorVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo(unitOfWork);

            Dominio.Entidades.Veiculo veiculo = repVeiculoMotorista.BuscarVeiculoPorMotorista(titulo.PagamentoMotorista.Motorista.Codigo);
            if (veiculo == null)
                veiculo = repCargaMotorista.BuscarVeiculoUltimaCargaMotorista(titulo.PagamentoMotorista.Motorista.Codigo);
            if (veiculo == null)
                return;

            Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo()
            {
                DataInicial = titulo.DataEmissao.Value,
                DataFinal = titulo.DataVencimento.Value,
                Valor = titulo.Valor,
                TipoDespesa = titulo.PagamentoMotorista.PagamentoMotoristaTipo.TipoDespesa,
                NumeroDocumento = titulo.NumeroDocumentoTituloOriginal.ToString(),
                TipoDocumento = "PAG MOTORISTA (TITULO A PAGAR)",
                Pessoa = titulo.Pessoa,
                Colaborador = usuario,
                PagamentoMotorista = titulo.PagamentoMotorista,
                Origem = OrigemRateioDespesaVeiculo.PagamentoMotorista
            };

            repRateioDespesaVeiculo.Inserir(rateioDespesaVeiculo);

            Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo rateioDespesaVeiculoValorVeiculo = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo()
            {
                Veiculo = veiculo,
                DespesaVeiculo = rateioDespesaVeiculo,
                Valor = titulo.Valor
            };
            repRateioDespesaVeiculoValorVeiculo.Inserir(rateioDespesaVeiculoValorVeiculo);

            Servicos.Auditoria.Auditoria.Auditar(auditado, rateioDespesaVeiculo, $"Gerado a partir do título a pagar: {titulo.NumeroDocumentoTituloOriginal}  do Pagamento do Motorista {titulo.PagamentoMotorista.Numero}.", unitOfWork);

            Servicos.Embarcador.Financeiro.RateioDespesaVeiculo servicoRateioDespesaVeiculo = new Servicos.Embarcador.Financeiro.RateioDespesaVeiculo(unitOfWork);
            servicoRateioDespesaVeiculo.RatearValorEntreVeiculos(out msgRetorno, rateioDespesaVeiculo);
        }

        #endregion
    }
}
