using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Interfaces.Database;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Terceiros;
using Dominio.ObjetosDeValor.Relatorios;
using Dominio.ObjetosDeValor.WebService;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Terceiros
{
    public class ContratoFrete : ServicoBase
    {
        #region Propriedades Privadas
        
        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        protected string _adminStringConexao;

        #endregion

        #region Construtores
                
        public ContratoFrete(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public ContratoFrete(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, string stringConexaoAdmin) : base(unitOfWork, tipoServicoMultisoftware, cliente)
        {
            _auditado = auditado;
            _clienteAcesso = clienteURLAcesso;
            _adminStringConexao = stringConexaoAdmin;
        }
        public ContratoFrete(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware) : base(unitOfWork, tipoServicoMultisoftware) { }
        #endregion

        #region Métodos Públicos

        public dynamic ObterDetalhescontratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {

            if (contratoFrete != null)
            {
                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unidadeTrabalho);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = contratoFrete.TransportadorTerceiro.Modalidades.Where(o => o.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro).FirstOrDefault();
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportador = null;

                if (modalidade != null)
                    modalidadeTransportador = repModalidadeTransportador.BuscarPorModalidade(modalidade.Codigo);

                int aprovacoesNecessarias = repContratoFrete.ContarAprovacoesNecessarias(contratoFrete.Codigo);
                int aprovacoes = repContratoFrete.ContarAprovacoes(contratoFrete.Codigo);
                int reprovacoes = repContratoFrete.ContarReprovacoes(contratoFrete.Codigo);

                var retorno = new
                {
                    DataEmissaoContrato = contratoFrete.DataEmissaoContrato.ToString("dd/MM/yyyy"),
                    contratoFrete.Codigo,
                    contratoFrete.Observacao,
                    Transbordo = contratoFrete.Transbordo != null ? contratoFrete.Transbordo.Codigo : 0,
                    Carga = contratoFrete.Carga.Codigo,
                    contratoFrete.SituacaoContratoFrete,
                    contratoFrete.TipoFreteEscolhido,
                    contratoFrete.ValorLiquidoSemAdiantamento,
                    contratoFrete.ValorAdiantamento,
                    contratoFrete.ValorAbastecimento,
                    contratoFrete.Descontos,
                    contratoFrete.DescricaoSituacaoContratoFrete,
                    contratoFrete.PercentualAdiantamento,
                    contratoFrete.PercentualAbastecimento,
                    Terceiro = contratoFrete.TransportadorTerceiro.Nome + " (" + contratoFrete.TransportadorTerceiro.Localidade.DescricaoCidadeEstado + ")" + (modalidadeTransportador != null ? " - " + modalidadeTransportador.DescricaoTipoTransportador : string.Empty),
                    contratoFrete.ValorFreteSubcontratacao,
                    contratoFrete.ValorFreteSubContratacaoTabelaFrete,
                    contratoFrete.DescricaoTipoFreteEscolhido,
                    contratoFrete.ValorOutrosAdiantamento,
                    contratoFrete.ValorPedagio,
                    PossuiCIOT = contratoFrete.Carga.CargaCTes.Any(o => o.CIOTs != null && o.CIOTs.Count() > 0),
                    contratoFrete.Bloqueado,
                    contratoFrete.JustificativaBloqueio,
                    Resumo = new
                    {
                        Solicitante = contratoFrete.Usuario?.Nome ?? string.Empty,
                        DataSolicitacao = contratoFrete.DataEmissaoContrato.ToString("dd/MM/yyyy"),
                        AprovacoesNecessarias = aprovacoesNecessarias,
                        Aprovacoes = aprovacoes,
                        Reprovacoes = reprovacoes,
                        Situacao = contratoFrete.DescricaoSituacaoContratoFrete,
                    },
                    contratoFrete.EnviouContratoAXComSucesso,
                    contratoFrete.EnviouAcertoContasContratoAXComSucesso,
                    contratoFrete.PossuiIntegracaoAX
                };

                return retorno;
            }
            else
            {
                return null;
            }
        }

        public bool GerarMovimentacaoFinanceiraReversaoJustificativas(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro)
        {
            if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimentoFinanceiro = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeTrabalho.StringConexao);

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> justificativas = repContratoFreteValor.BuscarPorContratoFrete(contratoFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor justificativa in justificativas)
            {
                if (!svcMovimentoFinanceiro.GerarMovimentacao(out erro, justificativa.TipoMovimentoReversao, contratoFrete.DataEmissaoContrato, justificativa.Valor, contratoFrete.NumeroContrato.ToString(), "Referente à reversão do valor justificado no contrato de frete nº " + contratoFrete.NumeroContrato + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT SolicitarFinalizacaoPorCIOT(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string retorno)
        {
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(contratoFrete.TransportadorTerceiro, unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);
            retorno = "";

            if (ValidarEncerramentoPorContrato(contratoFrete, out retorno, unitOfWork))
            {
                if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)
                    return cargaCIOT.CIOT;

                if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                {
                    Servicos.Log.TratarErro("SolicitarFinalizacaoPorCIOT codigoCiot" + cargaCIOT.CIOT.Codigo.ToString(), "QuitacaoCIOTCarga");
                    Servicos.Embarcador.CIOT.CIOT serCIOT = new CIOT.CIOT();
                    bool encerrou = serCIOT.EncerrarCIOT(cargaCIOT.CIOT, unitOfWork, tipoServicoMultisoftware, out retorno);
                    if (encerrou)
                        return cargaCIOT.CIOT;
                    else
                        return null;
                }
                else if (cargaCIOT.CIOT.Situacao == SituacaoCIOT.Encerrado)
                    return cargaCIOT.CIOT;
                else
                {
                    retorno = $"A situação do CIOT ({cargaCIOT.CIOT.Situacao.ObterDescricao()}) não permite que ele seja encerrado.";
                    return null;
                }
            }

            return null;
        }

        public void CancelarContratosPorCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOT(ciot.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in cargaCIOTs)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = cargaCIOT.ContratoFrete;

                CancelamentoContratoViaCIOT(contrato, ciot, tipoServicoMultisoftware, unitOfWork);
            }
        }

        public void AplicarDescontoPorCIOT(decimal valorDesconto, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            if (valorDesconto <= 0)
                return;

            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOT(ciot.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in cargaCIOTs)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = cargaCIOT.ContratoFrete;
                if (contrato != null)
                {
                    contrato.Descontos = valorDesconto;
                    repContratoFrete.Atualizar(contrato);
                }
            }
        }

        public void EncerrarContratosPorCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, DateTime dataEncerramento, bool controlarTransacao = true)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOT(ciot.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in cargaCIOTs)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = cargaCIOT.ContratoFrete;
                if (contrato != null && contrato.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                    EncerramentoContratoViaCIOT(contrato, ciot, tipoServicoMultisoftware, unitOfWork, dataEncerramento, controlarTransacao);
            }
        }

        public void CancelamentoContratoViaCIOT(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (contrato.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoFinanceira = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(StringConexao);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira = repConfiguracaoFinanceira.BuscarPrimeiroRegistro();

            if (configuracaoFinanceira != null)
            {
                string obsTotal = "Referente ao cancelamento do contrato de frete nº " + contrato.NumeroContrato + " cancelamento automaticamente no cancelamento do CIOT " + ciot.Numero + ".";

                decimal valor = contrato.ValorAdiantamento;
                if (contrato.SituacaoContratoFrete == SituacaoContratoFrete.Finalizada)
                    valor += contrato.SaldoAReceber;

                Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira configuracaoFinanceiraCIOT = null;
                if (ciot.ConfiguracaoCIOT?.ConfiguracaoMovimentoFinanceiro ?? false)
                {
                    DateTime dataMovimento = contrato.Carga.CargaCTes.FirstOrDefault()?.CTe.DataEmissao.Value ?? DateTime.Now;

                    configuracaoFinanceiraCIOT = ObterCIOTConfiguracaoFinanceira(contrato, ciot.ConfiguracaoCIOT, unitOfWork);
                    if (configuracaoFinanceiraCIOT != null)
                        svcProcessoMovimento.GerarMovimentacao(out _, configuracaoFinanceiraCIOT.TipoMovimentoParaReversao, dataMovimento, valor, contrato.NumeroContrato.ToString(), obsTotal, unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware);
                }

                if (configuracaoFinanceiraCIOT == null)
                {
                    if (configuracaoFinanceira.GerarMovimentoAutomaticoNaGeracaoContratoFrete && configuracaoFinanceira.TipoMovimentoReversaoValorPagoTerceiroCIOT != null)
                    {
                        DateTime dataMovimento = contrato.Carga.CargaCTes.FirstOrDefault()?.CTe.DataEmissao.Value ?? DateTime.Now;
                        svcProcessoMovimento.GerarMovimentacao(out _, configuracaoFinanceira.TipoMovimentoReversaoValorPagoTerceiroCIOT, dataMovimento, valor, contrato.NumeroContrato.ToString(), obsTotal, unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware);
                    }
                    else if (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao)
                    {
                        GerarReversaoMovimentosAprovacaoPorTipoOperacao(contrato, configuracaoFinanceira, unitOfWork, tipoServicoMultisoftware);
                    }
                }
            }

            string mensagemErro = "";
            Servicos.Embarcador.Terceiros.ContratoFrete.RealizarCancelamentoTotvs(contrato, unitOfWork, out mensagemErro);
            if (!string.IsNullOrWhiteSpace(mensagemErro))
                throw new Exception("Retorno TOTVS: " + mensagemErro);

            contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado;

            repContratoFrete.Atualizar(contrato);

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repositorioModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repositorioModalidadeTerceiro.BuscarPorPessoa(contrato.TransportadorTerceiro.CPF_CNPJ);

            if (!(modalidadeTransportadoraPessoas?.GerarPagamentoTerceiro ?? false))
            {
                if (contrato.ConfiguracaoCIOT?.GerarTitulosContratoFrete ?? false)
                {
                    Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);

                    if (!serTituloAPagar.AtualizarTitulos(contrato, unitOfWork, tipoServicoMultisoftware, out string erroTitulos, contrato.Carga?.Empresa?.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Producao, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada))
                    {
                        throw new Exception(erroTitulos);
                    }
                }
            }
        }

        public void EncerramentoContratoViaCIOT(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, DateTime dataEncerramento, bool controlarTransacao = true)
        {
            if (controlarTransacao)
                unitOfWork.Start();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoFinanceira = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repositorioModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(StringConexao);
            Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira = repConfiguracaoFinanceira.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repositorioModalidadeTerceiro.BuscarPorPessoa(contrato.TransportadorTerceiro.CPF_CNPJ);

            if (configuracaoFinanceira != null && (!ciot.EfetivacaoSaldo.HasValue || ciot.EfetivacaoSaldo.Value != PamcardParcelaTipoEfetivacao.Manual))
            {
                string obsTotal = "Referente ao pagamento do restante do valor no contrato de frete nº " + contrato.NumeroContrato + " liberado automaticamente no encerramento do CIOT " + ciot.Numero + ".";

                Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira configuracaoFinanceiraCIOT = null;
                if (ciot.ConfiguracaoCIOT?.ConfiguracaoMovimentoFinanceiro ?? false)
                {
                    configuracaoFinanceiraCIOT = ObterCIOTConfiguracaoFinanceira(contrato, ciot.ConfiguracaoCIOT, unitOfWork);
                    if (configuracaoFinanceiraCIOT != null)
                    {
                        DateTime dataMovimento = ObterDataParaMovimentoFinanceiroDoContrato(contrato, unitOfWork);
                        svcProcessoMovimento.GerarMovimentacao(configuracaoFinanceiraCIOT.TipoMovimentoParaUso, dataMovimento, contrato.SaldoAReceber, contrato.NumeroContrato.ToString(), obsTotal, unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware);
                    }
                }

                if (configuracaoFinanceiraCIOT == null)
                {
                    if (configuracaoFinanceira.GerarMovimentoAutomaticoNaGeracaoContratoFrete && configuracaoFinanceira.TipoMovimentoValorPagoTerceiroCIOT != null)
                    {
                        DateTime dataMovimento = ObterDataParaMovimentoFinanceiroDoContrato(contrato, unitOfWork);
                        svcProcessoMovimento.GerarMovimentacao(configuracaoFinanceira.TipoMovimentoValorPagoTerceiroCIOT, dataMovimento, contrato.SaldoAReceber, contrato.NumeroContrato.ToString(), obsTotal, unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware);
                    }
                    else if (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao)
                    {
                        GerarMovimentosEncerramentoCIOTPorTipoOperacao(ciot, contrato, configuracaoFinanceira, unitOfWork, tipoServicoMultisoftware);
                    }
                }
            }

            RealizarCompensacaoAX(contrato, unitOfWork, out string mensagemRetorno);
            contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada;
            contrato.EmEncerramentoCIOT = false;
            contrato.DataEncerramentoContrato = dataEncerramento;

            repContratoFrete.Atualizar(contrato);

            if (!(modalidadeTransportadoraPessoas?.GerarPagamentoTerceiro ?? false))
            {
                if (contrato.ConfiguracaoCIOT?.GerarTitulosContratoFrete ?? false)
                {
                    if (!serTituloAPagar.AtualizarTitulos(contrato, unitOfWork, tipoServicoMultisoftware, out string erroTitulos, contrato.Carga.Empresa.TipoAmbiente, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada))
                    {
                        unitOfWork.Rollback();
                        throw new Exception(erroTitulos);
                    }
                }
            }

            if (controlarTransacao)
                unitOfWork.CommitChanges();
        }

        public void AprovarContratoViaCIOTAutorizado(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoFinanceira = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(StringConexao);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = cargaCIOT.ContratoFrete;

            if (contrato == null)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira = repConfiguracaoFinanceira.BuscarPrimeiroRegistro();

            if (configuracaoFinanceira != null && (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao || !cargaCIOT.CIOT.EfetivacaoAdiantamento.HasValue || cargaCIOT.CIOT.EfetivacaoAdiantamento != PamcardParcelaTipoEfetivacao.Manual))
            {
                string obsAdiantamento = "Referente ao adiantamento do contrato de frete nº " + contrato.NumeroContrato + " liberado automaticamente no CIOT " + cargaCIOT.CIOT.Numero + ".";

                Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira configuracaoFinanceiraCIOT = null;
                if (cargaCIOT.CIOT.ConfiguracaoCIOT?.ConfiguracaoMovimentoFinanceiro ?? false)
                {
                    configuracaoFinanceiraCIOT = ObterCIOTConfiguracaoFinanceira(contrato, cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);
                    if (configuracaoFinanceiraCIOT != null)
                    {
                        DateTime dataMovimento = ObterDataParaMovimentoFinanceiroDoContrato(contrato, unitOfWork);
                        svcProcessoMovimento.GerarMovimentacao(configuracaoFinanceiraCIOT.TipoMovimentoParaUso, dataMovimento, contrato.ValorAdiantamento, contrato.NumeroContrato.ToString(), obsAdiantamento, unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contrato.TransportadorTerceiro);
                    }
                }

                if (configuracaoFinanceiraCIOT == null)
                {
                    if (configuracaoFinanceira.GerarMovimentoAutomaticoNaGeracaoContratoFrete && configuracaoFinanceira.TipoMovimentoValorPagoTerceiroCIOT != null)
                    {
                        DateTime dataMovimento = ObterDataParaMovimentoFinanceiroDoContrato(contrato, unitOfWork);
                        svcProcessoMovimento.GerarMovimentacao(configuracaoFinanceira.TipoMovimentoValorPagoTerceiroCIOT, dataMovimento, contrato.ValorAdiantamento, contrato.NumeroContrato.ToString(), obsAdiantamento, unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contrato.TransportadorTerceiro);
                    }
                    else if (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao)
                    {
                        GerarMovimentosAprovacaoPorTipoOperacao(contrato, configuracaoFinanceira, unitOfWork, tipoServicoMultisoftware, configuracaoTMS);
                    }
                }
            }

            contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado;
        }

        public void GerarMovimentosAprovacaoPorTipoOperacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao configuracao = ObterConfiguracaoFinanceiraPorTipoOperacao(contratoFrete, configuracaoFinanceira, unitOfWork);

            if (configuracao == null)
                return;

            DateTime dataMovimento = ObterDataParaMovimentoFinanceiroDoContrato(contratoFrete, unitOfWork);

            if (configuracao.DiferenciarMovimentoValorAbastecimento && configuracao.TipoMovimentoValorAbastecimento != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorAbastecimento, dataMovimento, contratoFrete.ValorAbastecimento, contratoFrete.NumeroContrato.ToString(), $"Referente ao vale-abastecimento do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorAbastecimento.Exportar ? configuracao.TipoMovimentoValorAbastecimento.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorAdiantamento && configuracao.TipoMovimentoValorAdiantamento != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorAdiantamento, dataMovimento, contratoFrete.ValorAdiantamento, contratoFrete.NumeroContrato.ToString(), $"Referente ao adiantamento do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorAdiantamento.Exportar ? configuracao.TipoMovimentoValorAdiantamento.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorINSS && configuracao.TipoMovimentoValorINSS != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorINSS, dataMovimento, contratoFrete.ValorINSS, contratoFrete.NumeroContrato.ToString(), $"Referente ao INSS do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorINSS.Exportar ? configuracao.TipoMovimentoValorINSS.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorINSSPatronal && configuracao.TipoMovimentoValorINSSPatronal != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorINSSPatronal, dataMovimento, contratoFrete.ValorINSSPatronal, contratoFrete.NumeroContrato.ToString(), $"Referente ao INSS patronal do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorINSSPatronal.Exportar ? configuracao.TipoMovimentoValorINSSPatronal.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorIRRF && configuracao.TipoMovimentoValorIRRF != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorIRRF, dataMovimento, contratoFrete.ValorIRRF, contratoFrete.NumeroContrato.ToString(), $"Referente ao IRRF do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorIRRF.Exportar ? configuracao.TipoMovimentoValorIRRF.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorLiquido && configuracao.TipoMovimentoValorLiquido != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorLiquido, dataMovimento, contratoFrete.ValorLiquidoSemAdiantamento, contratoFrete.NumeroContrato.ToString(), $"Referente ao valor líquido do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorLiquido.Exportar ? configuracao.TipoMovimentoValorLiquido.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorSaldo && configuracao.TipoMovimentoValorSaldo != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorSaldo, dataMovimento, contratoFrete.SaldoAReceber, contratoFrete.NumeroContrato.ToString(), $"Referente ao saldo do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorSaldo.Exportar ? configuracao.TipoMovimentoValorSaldo.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorSENAT && configuracao.TipoMovimentoValorSENAT != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorSENAT, dataMovimento, contratoFrete.ValorSENAT, contratoFrete.NumeroContrato.ToString(), $"Referente ao SENAT do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorSENAT.Exportar ? configuracao.TipoMovimentoValorSENAT.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorSEST && configuracao.TipoMovimentoValorSEST != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorSEST, dataMovimento, contratoFrete.ValorSEST, contratoFrete.NumeroContrato.ToString(), $"Referente ao SEST do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorSEST.Exportar ? configuracao.TipoMovimentoValorSEST.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorTarifaSaque && configuracao.TipoMovimentoValorTarifaSaque != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorTarifaSaque, dataMovimento, contratoFrete.TarifaSaque, contratoFrete.NumeroContrato.ToString(), $"Referente à tarifa de saque do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorTarifaSaque.Exportar ? configuracao.TipoMovimentoValorTarifaSaque.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorTarifaTransferencia && configuracao.TipoMovimentoValorTarifaTransferencia != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorTarifaTransferencia, dataMovimento, contratoFrete.TarifaTransferencia, contratoFrete.NumeroContrato.ToString(), $"Referente à tarifa de transferência do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorTarifaTransferencia.Exportar ? configuracao.TipoMovimentoValorTarifaTransferencia.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorTotal && configuracao.TipoMovimentoValorTotal != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoValorTotal, dataMovimento, contratoFrete.ValorFreteSubcontratacao, contratoFrete.NumeroContrato.ToString(), $"Referente ao valor total do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoValorTotal.Exportar ? configuracao.TipoMovimentoValorTotal.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete);
        }

        public void GerarReversaoMovimentosAprovacaoPorTipoOperacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao configuracao = ObterConfiguracaoFinanceiraPorTipoOperacao(contratoFrete, configuracaoFinanceira, unitOfWork);

            if (configuracao == null)
                return;

            DateTime dataMovimento = ObterDataParaMovimentoFinanceiroDoContrato(contratoFrete, unitOfWork);

            if (configuracao.DiferenciarMovimentoValorAbastecimento && configuracao.TipoMovimentoReversaoValorAbastecimento != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorAbastecimento, dataMovimento, contratoFrete.ValorAbastecimento, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do vale-abastecimento do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorAbastecimento.Exportar ? configuracao.TipoMovimentoReversaoValorAbastecimento.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorAdiantamento && configuracao.TipoMovimentoReversaoValorAdiantamento != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorAdiantamento, dataMovimento, contratoFrete.ValorAdiantamento, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do adiantamento do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorAdiantamento.Exportar ? configuracao.TipoMovimentoReversaoValorAdiantamento.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorINSS && configuracao.TipoMovimentoReversaoValorINSS != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorINSS, dataMovimento, contratoFrete.ValorINSS, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do INSS do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorINSS.Exportar ? configuracao.TipoMovimentoReversaoValorINSS.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorINSSPatronal && configuracao.TipoMovimentoReversaoValorINSSPatronal != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorINSSPatronal, dataMovimento, contratoFrete.ValorINSSPatronal, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do INSS patronal do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorINSSPatronal.Exportar ? configuracao.TipoMovimentoReversaoValorINSSPatronal.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorIRRF && configuracao.TipoMovimentoReversaoValorIRRF != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorIRRF, dataMovimento, contratoFrete.ValorIRRF, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do IRRF do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorIRRF.Exportar ? configuracao.TipoMovimentoReversaoValorIRRF.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorLiquido && configuracao.TipoMovimentoReversaoValorLiquido != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorLiquido, dataMovimento, contratoFrete.ValorLiquidoSemAdiantamento, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do valor líquido do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorLiquido.Exportar ? configuracao.TipoMovimentoReversaoValorLiquido.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorSaldo && configuracao.TipoMovimentoReversaoValorSaldo != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorSaldo, dataMovimento, contratoFrete.SaldoAReceber, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do saldo do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorSaldo.Exportar ? configuracao.TipoMovimentoReversaoValorSaldo.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorSENAT && configuracao.TipoMovimentoReversaoValorSENAT != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorSENAT, dataMovimento, contratoFrete.ValorSENAT, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do SENAT do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorSENAT.Exportar ? configuracao.TipoMovimentoReversaoValorSENAT.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorSEST && configuracao.TipoMovimentoReversaoValorSEST != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorSEST, dataMovimento, contratoFrete.ValorSEST, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do SEST do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorSEST.Exportar ? configuracao.TipoMovimentoReversaoValorSEST.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorTarifaSaque && configuracao.TipoMovimentoReversaoValorTarifaSaque != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorTarifaSaque, dataMovimento, contratoFrete.TarifaSaque, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão da tarifa de saque do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorTarifaSaque.Exportar ? configuracao.TipoMovimentoReversaoValorTarifaSaque.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorTarifaTransferencia && configuracao.TipoMovimentoReversaoValorTarifaTransferencia != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorTarifaTransferencia, dataMovimento, contratoFrete.TarifaTransferencia, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão da tarifa de transferência do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorTarifaTransferencia.Exportar ? configuracao.TipoMovimentoReversaoValorTarifaTransferencia.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);

            if (configuracao.DiferenciarMovimentoValorTotal && configuracao.TipoMovimentoReversaoValorTotal != null)
                svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoReversaoValorTotal, dataMovimento, contratoFrete.ValorFreteSubcontratacao, contratoFrete.NumeroContrato.ToString(), $"Referente à reversão do valor total do contrato de frete nº {contratoFrete.NumeroContrato}.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoReversaoValorTotal.Exportar ? configuracao.TipoMovimentoReversaoValorTotal.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete);
        }

        public static void GerarMovimentosEncerramentoCIOTPorTipoOperacao(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao configuracao = ObterConfiguracaoFinanceiraPorTipoOperacao(contratoFrete, configuracaoFinanceira, unitOfWork);

            if (configuracao == null || configuracao.TipoMovimentoPagamentoViaCIOT == null)
                return;

            DateTime dataMovimento = ciot.DataEncerramento.Value;

            svcProcessoMovimento.GerarMovimentacao(configuracao.TipoMovimentoPagamentoViaCIOT, dataMovimento, contratoFrete.ValorFreteSubcontratacao, contratoFrete.NumeroContrato.ToString(), $"Referente ao encerramento do contrato de frete nº {contratoFrete.NumeroContrato} via encerramento do CIOT.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro, null, null, null, configuracao.TipoMovimentoPagamentoViaCIOT.Exportar ? configuracao.TipoMovimentoPagamentoViaCIOT.ContasExportacao.ToList() : null, contratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.PagamentoContratoFrete);
        }

        public static Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao ObterConfiguracaoFinanceiraPorTipoOperacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = contratoFrete.Carga?.TipoOperacao;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao repConfiguracaoTipoOperacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao> configuracoes = repConfiguracaoTipoOperacao.BuscarPorTipoOperacao(configuracaoFinanceira.Codigo, tipoOperacao?.Codigo ?? 0);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao configuracao = configuracoes.Where(o => o.TipoOperacao != null).FirstOrDefault();

            if (configuracao == null)
                configuracao = configuracoes.FirstOrDefault();

            return configuracao;
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira ObterCIOTConfiguracaoFinanceira(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            if (!contratoFrete.TipoPagamentoCIOT.HasValue || contratoFrete.TipoPagamentoCIOT.Value == TipoPagamentoCIOT.SemPgto)
                return null;

            Repositorio.Embarcador.CIOT.CIOTConfiguracaoFinanceira repositorioCIOTConfiguracaoFinanceira = new Repositorio.Embarcador.CIOT.CIOTConfiguracaoFinanceira(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira configuracaoFinanceira = repositorioCIOTConfiguracaoFinanceira.BuscarPorConfiguracaoCIOTETipoPagamento(configuracaoCIOT.Codigo, contratoFrete.TipoPagamentoCIOT.Value);

            return configuracaoFinanceira;
        }

        public string SolicitarFinalizacaoContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            string retorno = "";

            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);

            if (contratoFrete.ConfiguracaoCIOT != null && cargaCIOT != null)
            {
                ValidarFinalizacaoPorCIOT(cargaCIOT, contratoFrete, tipoServicoMultisoftware, unitOfWork, out retorno);
            }
            else
            {
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);

                if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                {
                    RealizarCompensacaoAX(contratoFrete, unitOfWork, out string mensagemRetorno);
                    contratoFrete.DataEncerramentoContrato = DateTime.Now;
                    contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada;
                    repContratoFrete.Atualizar(contratoFrete);

                    Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTerceiro.BuscarPorPessoa(contratoFrete.TransportadorTerceiro.CPF_CNPJ);

                    if (!(modalidadeTransportadoraPessoas?.GerarPagamentoTerceiro ?? false))
                        serTituloAPagar.AtualizarTitulos(contratoFrete, unitOfWork, tipoServicoMultisoftware, out retorno, Dominio.Enumeradores.TipoAmbiente.Producao, auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada);

                }
            }
            return retorno;
        }

        public void GerarPreCTesSubContratacao(Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorTransbordo(transbordo.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = transbordo.CargaCTesTransbordados.ToList();
            gerarPreCTe(cargaCTes, contratoFrete, transbordo.Veiculo?.Proprietario ?? transbordo.Carga?.ProvedorOS, unitOfWork);
        }

        public void GerarPreCTesSubContratacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = repModalidadeTerceiro.BuscarPorPessoa(contratoFrete.TransportadorTerceiro.CPF_CNPJ);

            if (!(modalidadeTerceiro?.GerarCIOT ?? false))// se tiver CIOT não gera pré cte de subcontratação.
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(carga.Codigo, false, false, true);
                gerarPreCTe(cargaCTes, contratoFrete, contratoFrete.TransportadorTerceiro ?? carga.ProvedorOS, unitOfWork);
            }
        }

        public static byte[] GerarRomaneioEntrega(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete)
        {
            return ReportRequest.WithType(ReportType.RomaneioEntrega)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("codigoContratoFrete", contratoFrete.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public static string ObterObservacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete)
        {
            if (string.IsNullOrWhiteSpace(contratoFrete.Observacao))
                return string.Empty;

            return contratoFrete.Observacao.Replace("#ValorTotal", contratoFrete.ValorFreteSubcontratacao.ToString("n2"))
                                           .Replace("#PercentualAdiantamento", contratoFrete.PercentualAdiantamento.ToString("n2"))
                                           .Replace("#PercentualAbastecimento", contratoFrete.PercentualAbastecimento.ToString("n2"))
                                           .Replace("#ValorAdiantamento", contratoFrete.ValorAdiantamento.ToString("n2"))
                                           .Replace("#ValorAbastecimento", contratoFrete.ValorAbastecimento.ToString("n2"))
                                           .Replace("#SaldoReceber", contratoFrete.SaldoAReceber.ToString("n2"))
                                           .Replace("#VencimentoAdiantamento", DateTime.Now.AddDays(contratoFrete.DiasVencimentoAdiantamento).ToString("dd/MM/yyyy"))
                                           .Replace("#VencimentoSaldo", Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(contratoFrete).ToString("dd/MM/yyyy"))
                                           .Replace("#OperadoraValePedagio", contratoFrete.TipoIntegracaoValePedagio?.Descricao ?? "")
                                           .Replace("#CartaoAbastecimento", contratoFrete.Carga.Veiculo.NumeroCartaoAbastecimento ?? "");
        }

        public static List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> VerificarRegrasAutorizacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);
            Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro repRegra = new Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro(unitOfWork);

            List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> listaRegras = new List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro>();
            List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> listaFiltrada = new List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro>();
            List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> alcadasCompativeis;

            alcadasCompativeis = repRegra.AlcadasPorValor(contrato.ValorFreteSubcontratacao, contrato.DataEmissaoContrato);
            listaRegras.AddRange(alcadasCompativeis);

            alcadasCompativeis = repRegra.AlcadasPorAcrescimo(contrato.ValorFreteSubcontratacao, contrato.DataEmissaoContrato);
            listaRegras.AddRange(alcadasCompativeis);

            alcadasCompativeis = repRegra.AlcadasPorDesconto(contrato.ValorFreteSubcontratacao, contrato.DataEmissaoContrato);
            listaRegras.AddRange(alcadasCompativeis);

            alcadasCompativeis = repRegra.AlcadasPorTerceiro(contrato.TransportadorTerceiro.CPF_CNPJ, contrato.DataEmissaoContrato);
            listaRegras.AddRange(alcadasCompativeis);

            listaRegras = listaRegras.Distinct().ToList();
            if (listaRegras.Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras);
                foreach (Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regra in listaRegras)
                {
                    if (regra.RegraPorValorContrato)
                    {
                        bool valido = false;
                        if (regra.AlcadasValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor == contrato.ValorFreteSubcontratacao))
                            valido = true;
                        else if (regra.AlcadasValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor == contrato.ValorFreteSubcontratacao))
                            valido = true;
                        else if (regra.AlcadasValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor != contrato.ValorFreteSubcontratacao))
                            valido = true;
                        else if (regra.AlcadasValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor != contrato.ValorFreteSubcontratacao))
                            valido = true;
                        if (regra.AlcadasValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && contrato.ValorFreteSubcontratacao >= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && contrato.ValorFreteSubcontratacao >= o.Valor))
                            valido = true;
                        if (regra.AlcadasValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && contrato.ValorFreteSubcontratacao <= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && contrato.ValorFreteSubcontratacao <= o.Valor))
                            valido = true;
                        if (regra.AlcadasValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && contrato.ValorFreteSubcontratacao > o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && contrato.ValorFreteSubcontratacao > o.Valor))
                            valido = true;
                        if (regra.AlcadasValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && contrato.ValorFreteSubcontratacao < o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && contrato.ValorFreteSubcontratacao < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorValorAcrescimo)
                    {
                        decimal valorAcrescimo = repContratoFreteValor.BuscarValorPorContratoFrete(contrato.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo);
                        bool valido = false;

                        if (regra.AlcadasValorAcrescimo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor == valorAcrescimo))
                            valido = true;
                        else if (regra.AlcadasValorAcrescimo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor == valorAcrescimo))
                            valido = true;
                        else if (regra.AlcadasValorAcrescimo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor != valorAcrescimo))
                            valido = true;
                        else if (regra.AlcadasValorAcrescimo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor != valorAcrescimo))
                            valido = true;
                        if (regra.AlcadasValorAcrescimo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorAcrescimo >= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorAcrescimo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorAcrescimo >= o.Valor))
                            valido = true;
                        if (regra.AlcadasValorAcrescimo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorAcrescimo <= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorAcrescimo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorAcrescimo <= o.Valor))
                            valido = true;
                        if (regra.AlcadasValorAcrescimo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorAcrescimo > o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorAcrescimo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorAcrescimo > o.Valor))
                            valido = true;
                        if (regra.AlcadasValorAcrescimo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorAcrescimo < o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorAcrescimo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorAcrescimo < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorValorDesconto)
                    {
                        decimal valorDesconto = repContratoFreteValor.BuscarValorPorContratoFrete(contrato.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto);
                        bool valido = false;

                        if (regra.AlcadasValorDesconto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor == valorDesconto))
                            valido = true;
                        else if (regra.AlcadasValorDesconto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor == valorDesconto))
                            valido = true;
                        else if (regra.AlcadasValorDesconto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor != valorDesconto))
                            valido = true;
                        else if (regra.AlcadasValorDesconto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor != valorDesconto))
                            valido = true;
                        if (regra.AlcadasValorDesconto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorDesconto >= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorDesconto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorDesconto >= o.Valor))
                            valido = true;
                        if (regra.AlcadasValorDesconto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorDesconto <= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorDesconto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorDesconto <= o.Valor))
                            valido = true;
                        if (regra.AlcadasValorDesconto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorDesconto > o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorDesconto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorDesconto > o.Valor))
                            valido = true;
                        if (regra.AlcadasValorDesconto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorDesconto < o.Valor))
                            valido = true;
                        else if (regra.AlcadasValorDesconto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorDesconto < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTerceiros)
                    {
                        bool valido = false;
                        if (regra.AlcadasTerceiros.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Terceiro.CPF_CNPJ == contrato.TransportadorTerceiro.CPF_CNPJ))
                            valido = true;
                        else if (regra.AlcadasTerceiros.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Terceiro.CPF_CNPJ == contrato.TransportadorTerceiro.CPF_CNPJ))
                            valido = true;
                        else if (regra.AlcadasTerceiros.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Terceiro.CPF_CNPJ != contrato.TransportadorTerceiro.CPF_CNPJ))
                            valido = true;
                        else if (regra.AlcadasTerceiros.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Terceiro.CPF_CNPJ != contrato.TransportadorTerceiro.CPF_CNPJ))
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

        public static void AlcadasContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            if (contratoFrete == null)
                return;

            List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> listaFiltrada = Servicos.Embarcador.Terceiros.ContratoFrete.VerificarRegrasAutorizacao(contratoFrete, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                if (!Servicos.Embarcador.Terceiros.ContratoFrete.CriarRegrasAutorizacao(listaFiltrada, contratoFrete, contratoFrete.Carga.Operador, tipoServicoMultisoftware, unitOfWork.StringConexao, unitOfWork))
                    contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado;
                else
                    contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao;
            }
            else
                contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.SemRegra;

            repContratoFreteTerceiro.Atualizar(contratoFrete);
        }

        public static bool CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> listaFiltrada, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiRegraPendente = false;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = null;
            if (tipoServicoMultisoftware.HasValue)
                serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware.Value, string.Empty);
            Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

            if (listaFiltrada == null || listaFiltrada.Count() == 0)
                throw new ArgumentException("Lista de Regras deve ser maior que 0");

            foreach (Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    possuiRegraPendente = true;
                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete autorizacao = new Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete
                        {
                            ContratoFrete = contrato,
                            Usuario = aprovador,
                            RegraContratoFreteTerceiro = regra,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                        };
                        repAprovacaoAlcadaContratoFrete.Inserir(autorizacao);

                        string nota = string.Empty;
                        nota = string.Format(Localization.Resources.Fretes.ContratoFrete.ContratoFreteAguardandoAprovacao, contrato.NumeroContrato.ToString());

                        if (serNotificacao != null)
                            serNotificacao.GerarNotificacaoEmail(aprovador, usuario, contrato.Codigo, "Terceiros/ContratoFrete", string.Empty, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware.Value, unitOfWork);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete autorizacao = new Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete
                    {
                        ContratoFrete = contrato,
                        Usuario = null,
                        RegraContratoFreteTerceiro = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = "Alçada aprovada pela Regra " + regra.Descricao
                    };
                    repAprovacaoAlcadaContratoFrete.Inserir(autorizacao);
                }
            }

            return possuiRegraPendente;
        }

        public static bool GerarMovimentacaoFinanceiraJustificativas(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro)
        {
            if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimentoFinanceiro = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeTrabalho.StringConexao);

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> justificativas = repContratoFreteValor.BuscarPorContratoFrete(contratoFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor justificativa in justificativas)
            {
                if (!svcMovimentoFinanceiro.GerarMovimentacao(out erro, justificativa.TipoMovimentoUso, contratoFrete.DataEmissaoContrato, justificativa.Valor, contratoFrete.NumeroContrato.ToString(), "Referente ao valor justificado no contrato de frete nº " + contratoFrete.NumeroContrato + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoFrete.TransportadorTerceiro))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        public static bool ProcessarContratoAprovado(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Enumeradores.TipoAmbiente tipoAmbienteEmpresa, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string stringConexao, out string erro)
        {
            erro = "";

            if (contrato.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                return true;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoFinanceiraContratoFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(unitOfWork);
            Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFreteTerceiros = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira = repConfiguracaoFinanceiraContratoFrete.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao configuracao = null;

            Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                tipoAmbiente = tipoAmbienteEmpresa;

            if (!CargaPermiteAlterarContratoFrete(contrato.Carga))
            {
                erro = FormataErroAlcada(contrato, "a situação atual da carga não permite");
                return false;
            }

            if (!configuracaoTMS.NaoValidarDadosBancariosContratoFrete && !ValidarDadosBancarios(contrato.TransportadorTerceiro))
            {
                erro = FormataErroAlcada(contrato, "o terceiro não possui os dados bancários cadastrados");
                return false;
            }

            if (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao)
                configuracao = Servicos.Embarcador.Terceiros.ContratoFrete.ObterConfiguracaoFinanceiraPorTipoOperacao(contrato, configuracaoFinanceira, unitOfWork);

            if (configuracaoFinanceira == null ||
                (configuracaoFinanceira.GerarMovimentoAutomaticoNaGeracaoContratoFrete && (configuracaoFinanceira.TipoMovimentoReversaoValorPagoTerceiro == null || configuracaoFinanceira.TipoMovimentoValorPagoTerceiro == null)) ||
                (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao && (configuracao == null || configuracao.TipoMovimentoGeracaoTitulo == null || configuracao.TipoMovimentoReversaoGeracaoTitulo == null)) ||
                (!configuracaoFinanceira.GerarMovimentoAutomaticoNaGeracaoContratoFrete && !configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao))
            {
                erro = FormataErroAlcada(contrato, "a configuração para geração dos movimentos financeiros não foi realizada");
                return false;
            }

            if (contrato.Transbordo != null)
            {
                contrato.Transbordo.SituacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.EmTransporte;
                repTransbordo.Atualizar(contrato.Transbordo);
                serContratoFreteTerceiros.GerarPreCTesSubContratacao(contrato.Transbordo, unitOfWork);
            }
            else
            {
                serContratoFreteTerceiros.GerarPreCTesSubContratacao(contrato.Carga, unitOfWork);
            }

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTerceiro.BuscarPorPessoa(contrato.TransportadorTerceiro.CPF_CNPJ);
            if (!modalidadeTransportadoraPessoas?.GerarPagamentoTerceiro ?? false)
            {
                if (!serTituloAPagar.AtualizarTitulos(contrato, unitOfWork, tipoServicoMultisoftware, out string erroTitulos, tipoAmbiente, auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada))
                {
                    erro = FormataErroAlcada(contrato, erroTitulos, ". Erro: ");
                    return false;
                }
            }

            if (!GerarMovimentacaoFinanceiraJustificativas(contrato, unitOfWork, tipoServicoMultisoftware, out string erroMovimentos))
            {
                erro = FormataErroAlcada(contrato, erroMovimentos, ". Erro: ");
                return false;
            }

            serContratoFreteTerceiros.GerarMovimentosAprovacaoPorTipoOperacao(contrato, configuracaoFinanceira, unitOfWork, tipoServicoMultisoftware, configuracaoTMS);

            serHubCarga.InformarCargaAtualizada(contrato.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);

            return true;
        }

        public static bool CargaPermiteAlterarContratoFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacaoQueNaoPermite = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
            };

            return !situacaoQueNaoPermite.Contains(carga.SituacaoCarga);
        }

        public bool AdicionarValorAoContrato(out string erro, ref Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, decimal valor, int codigoJustificativa, string observacao, int codigoTaxaTerceiro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool ajustarAdiantamento = false, bool gerouAutomaticamente = false, Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo pendenciaContratoFreteFuturo = null)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(_unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(_unitOfWork);
            Repositorio.Embarcador.Terceiros.TaxaTerceiro repTaxaTerceiro = new Repositorio.Embarcador.Terceiros.TaxaTerceiro(_unitOfWork);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor()
            {
                ContratoFrete = contratoFrete,
                Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa),
                Valor = valor,
                Observacao = observacao,
                TaxaTerceiro = codigoTaxaTerceiro > 0 ? repTaxaTerceiro.BuscarPorCodigo(codigoTaxaTerceiro, false) : null,
                GeradoAutomaticamente = gerouAutomaticamente,
                PendenciaContratoFrete = pendenciaContratoFreteFuturo
            };

            if (!contratoFreteValor.Justificativa.GerarMovimentoAutomatico && _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            {
                erro = "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.";
                return false;
            }

            contratoFreteValor.TipoJustificativa = contratoFreteValor.Justificativa.TipoJustificativa;
            contratoFreteValor.AplicacaoValor = contratoFreteValor.Justificativa.AplicacaoValorContratoFrete.HasValue ? contratoFreteValor.Justificativa.AplicacaoValorContratoFrete.Value : AplicacaoValorJustificativaContratoFrete.NoAdiantamento;
            contratoFreteValor.TipoMovimentoUso = contratoFreteValor.Justificativa.TipoMovimentoUsoJustificativa;
            contratoFreteValor.TipoMovimentoReversao = contratoFreteValor.Justificativa.TipoMovimentoReversaoUsoJustificativa;

            repContratoFreteValor.Inserir(contratoFreteValor, auditado);

            if (contratoFreteValor.AplicacaoValor == AplicacaoValorJustificativaContratoFrete.NoTotal)
            {
                if (contratoFreteValor.TipoJustificativa == TipoJustificativa.Acrescimo)
                    contratoFrete.ValorFreteSubcontratacao += valor;
                else
                    contratoFrete.ValorFreteSubcontratacao -= valor;
            }
            else if (ajustarAdiantamento)
            {
                if (contratoFreteValor.AplicacaoValor == AplicacaoValorJustificativaContratoFrete.NoAdiantamento)
                {
                    if (contratoFreteValor.TipoJustificativa == TipoJustificativa.Acrescimo)
                        contratoFrete.ValorAdiantamento += valor;
                    else
                        contratoFrete.ValorAdiantamento -= valor;
                }
            }

            erro = string.Empty;
            return true;
        }

        public static bool ReverterValorAoContrato(out string erro, ref Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, decimal valor, int codigoJustificativa, string observacao, int codigoTaxaTerceiro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            Repositorio.Embarcador.Terceiros.TaxaTerceiro repTaxaTerceiro = new Repositorio.Embarcador.Terceiros.TaxaTerceiro(unitOfWork);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = repContratoFreteValor.BuscarPorContratoFreteJustificativaEValor(contratoFrete.Codigo, codigoJustificativa, valor);
            if (contratoFreteValor != null)
                repContratoFreteValor.Deletar(contratoFreteValor);

            if (contratoFreteValor.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal)
            {
                if (contratoFreteValor.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                    contratoFrete.ValorFreteSubcontratacao -= valor;
                else
                    contratoFrete.ValorFreteSubcontratacao += valor;
            }

            erro = string.Empty;
            return true;
        }
        public static decimal RetornarValorFreteSubcontratacaoSemAcrescimoDesconto(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete)
        {
            decimal valorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao;

            if (contratoFrete.ValoresAdicionais != null && contratoFrete.ValoresAdicionais.Count > 0)
            {
                foreach (var valor in contratoFrete.ValoresAdicionais)
                {
                    if (valor.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal)
                    {
                        if (valor.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                            valorFreteSubcontratacao -= valor.Valor;
                        else
                            valorFreteSubcontratacao += valor.Valor;
                    }
                }
            }


            return valorFreteSubcontratacao;
        }

        public bool AtualizarValorDoContrato(out string erro, ref Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, ref Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor, decimal valor, int codigoJustificativa, int codigoTaxaTerceiro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(_unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(_unitOfWork);
            Repositorio.Embarcador.Terceiros.TaxaTerceiro repTaxaTerceiro = new Repositorio.Embarcador.Terceiros.TaxaTerceiro(_unitOfWork);

            decimal valorAnterior = contratoFreteValor.Valor;

            contratoFreteValor.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

            if (!contratoFreteValor.Justificativa.GerarMovimentoAutomatico && _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            {
                erro = "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.";
                return false;
            }

            contratoFreteValor.TipoJustificativa = contratoFreteValor.Justificativa.TipoJustificativa;
            contratoFreteValor.AplicacaoValor = contratoFreteValor.Justificativa.AplicacaoValorContratoFrete.HasValue ? contratoFreteValor.Justificativa.AplicacaoValorContratoFrete.Value : AplicacaoValorJustificativaContratoFrete.NoAdiantamento;
            contratoFreteValor.TipoMovimentoUso = contratoFreteValor.Justificativa.TipoMovimentoUsoJustificativa;
            contratoFreteValor.TipoMovimentoReversao = contratoFreteValor.Justificativa.TipoMovimentoReversaoUsoJustificativa;
            contratoFreteValor.Valor = valor;
            contratoFreteValor.TaxaTerceiro = codigoTaxaTerceiro > 0 ? repTaxaTerceiro.BuscarPorCodigo(codigoTaxaTerceiro, false) : null;

            repContratoFreteValor.Atualizar(contratoFreteValor, auditado);

            if (contratoFreteValor.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal)
            {
                if (contratoFreteValor.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                    contratoFrete.ValorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao - valorAnterior + valor;
                else
                    contratoFrete.ValorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao + valorAnterior - valor;

                //decimal valorTotalDescontoAdiantamento = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento);
                //decimal valorTotalAcrescimoAdiantamento = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento);
                //decimal valorTotal = contratoFrete.ValorFreteSubcontratacao + contratoFrete.ValorPedagio;

                //contratoFrete.ValorAdiantamento = ((valorTotal * contratoFrete.PercentualAdiantamento) / 100) + valorTotalAcrescimoAdiantamento - valorTotalDescontoAdiantamento;
            }
            else if (contratoFreteValor.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento)
            {
                //decimal valorTotal = contratoFrete.ValorFreteSubcontratacao + contratoFrete.ValorPedagio - contratoFrete.Descontos;
                //decimal valorTotalDescontoAdiantamento = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento);
                //decimal valorTotalAcrescimoAdiantamento = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento);

                //contratoFrete.ValorAdiantamento = ((valorTotal * contratoFrete.PercentualAdiantamento) / 100) + valorTotalAcrescimoAdiantamento - valorTotalDescontoAdiantamento;
            }

            erro = string.Empty;
            return true;
        }

        public static bool RemoverValorDoContrato(out string erro, ref Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, ref Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);

            if (contratoFreteValor.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal)
            {
                if (contratoFreteValor.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                    contratoFrete.ValorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao - contratoFreteValor.Valor;
                else
                    contratoFrete.ValorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao + contratoFreteValor.Valor;
            }

            repContratoFreteValor.Deletar(contratoFreteValor, auditado);

            erro = string.Empty;
            return true;
        }

        public static void CalcularImpostos(ref Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool alterarAdiantamento = true)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repconfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repconfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

            #region Valor Total Para Calculo
            bool incluirTodosAcrescimosEDescontosNoCalculoDeImpostos = configuracaoTMS.IncluirTodosAcrescimosEDescontosNoCalculoDeImpostos;

            contratoFrete.ValorTotalDescontoAdiantamento = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento);
            contratoFrete.ValorTotalAcrescimoAdiantamento = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento);
            contratoFrete.ValorTotalDescontoSaldo = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo);
            contratoFrete.ValorTotalAcrescimoSaldo = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo);

            decimal valorTotalParaCalculo = contratoFrete.ValorFreteSubcontratacao + contratoFrete.TarifaTransferencia - contratoFrete.TarifaSaque;

            if (!configuracaoContratoFreteTerceiro.EmAcrescimoDescontoCiotNaoAlteraImpostos)
            {
                if (contratoFrete.NaoConsiderarDescontoCalculoImpostos)
                {
                    valorTotalParaCalculo += contratoFrete.ValorTotalAcrescimoSaldo + contratoFrete.ValorTotalAcrescimoAdiantamento;
                }
                else
                {
                    if (incluirTodosAcrescimosEDescontosNoCalculoDeImpostos)
                    {
                        decimal valorTotalDesconto = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto);
                        decimal valorTotalAcrescimo = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo);

                        valorTotalParaCalculo += valorTotalDesconto - valorTotalAcrescimo;
                    }
                    else
                        valorTotalParaCalculo += contratoFrete.ValorTotalAcrescimoSaldo - contratoFrete.ValorTotalDescontoSaldo + contratoFrete.ValorTotalAcrescimoAdiantamento - contratoFrete.ValorTotalDescontoAdiantamento;
                }
            }

            if (contratoFrete.TipoIntegracaoValePedagio != null && !contratoFrete.TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato && !(contratoFrete.Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false) && !(configuracaoContratoFreteTerceiro.NaoSubtrairValePedagioDoContrato))
                valorTotalParaCalculo -= contratoFrete.ValorPedagio;

            if (contratoFrete.PercentualAbastecimento > 0m)
            {
                decimal valorParaAbastecimento = contratoFrete.ValorFreteSubcontratacao - contratoFrete.TarifaSaque - contratoFrete.TarifaTransferencia;

                if (contratoFrete.TipoIntegracaoValePedagio != null && !contratoFrete.TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato && !(contratoFrete.Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false) && !(configuracaoContratoFreteTerceiro.NaoSubtrairValePedagioDoContrato))
                    valorParaAbastecimento -= contratoFrete.ValorPedagio;

                contratoFrete.ValorAbastecimento = Math.Round(valorParaAbastecimento * (contratoFrete.PercentualAbastecimento / 100), 2, MidpointRounding.ToEven);
                valorTotalParaCalculo -= contratoFrete.ValorAbastecimento;
            }
            #endregion

            CalcularImpostosParametros calcularImpostosParametros = new CalcularImpostosParametros();
            calcularImpostosParametros.origemCalcularImposto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCalcularImposto.ContratoFrete;
            calcularImpostosParametros.codigoContratoFrete = contratoFrete.Codigo;
            calcularImpostosParametros.codigoPagamentoMotoristaTMS = 0;
            calcularImpostosParametros.cpfCnpjTerceiro = contratoFrete.TransportadorTerceiro.CPF_CNPJ;
            calcularImpostosParametros.codigoTipoTerceiro = contratoFrete.TipoTerceiro?.Codigo;
            calcularImpostosParametros.valorTotalParaCalculo = valorTotalParaCalculo;

            Dominio.ObjetosDeValor.Embarcador.Terceiros.CalcularImpostosRetorno calculoImpostosRetorno = CalcularImpostos(calcularImpostosParametros, contratoFrete, configuracaoTMS, configuracaoContratoFreteTerceiro, unitOfWork, tipoServicoMultisoftware);

            contratoFrete.BaseCalculoINSS = calculoImpostosRetorno.BaseCalculoINSS;
            contratoFrete.AliquotaINSS = calculoImpostosRetorno.AliquotaINSS;
            contratoFrete.ValorINSS = calculoImpostosRetorno.ValorINSS;

            contratoFrete.BaseCalculoSEST = calculoImpostosRetorno.BaseCalculoSEST;
            contratoFrete.AliquotaSEST = calculoImpostosRetorno.AliquotaSEST;
            contratoFrete.ValorSEST = calculoImpostosRetorno.ValorSEST;

            contratoFrete.BaseCalculoSENAT = calculoImpostosRetorno.BaseCalculoSENAT;
            contratoFrete.AliquotaSENAT = calculoImpostosRetorno.AliquotaSENAT;
            contratoFrete.ValorSENAT = calculoImpostosRetorno.ValorSENAT;

            contratoFrete.BaseCalculoIRRF = calculoImpostosRetorno.BaseCalculoIRRF;
            contratoFrete.AliquotaIRRF = calculoImpostosRetorno.AliquotaIRRF;
            contratoFrete.ValorIRRF = calculoImpostosRetorno.ValorIRRF;

            contratoFrete.BaseCalculoIRRFSemDesconto = calculoImpostosRetorno.BaseCalculoIRRFSemDesconto;
            contratoFrete.BaseCalculoIRRFSemAcumulo = calculoImpostosRetorno.BaseCalculoIRRFSemAcumulo;
            contratoFrete.ValorIRRFSemDesconto = calculoImpostosRetorno.ValorIRRFSemDesconto;
            contratoFrete.ValorIRRFPeriodo = calculoImpostosRetorno.ValorIRRFPeriodo;

            contratoFrete.AliquotaINSSPatronal = calculoImpostosRetorno.AliquotaINSSPatronal;
            contratoFrete.ValorINSSPatronal = calculoImpostosRetorno.ValorINSSPatronal;

            contratoFrete.AliquotaCOFINS = calculoImpostosRetorno.AliquotaCOFINS;
            contratoFrete.AliquotaPIS = calculoImpostosRetorno.AliquotaPIS;
            contratoFrete.CodigoIntegracaoTributaria = calculoImpostosRetorno.CodigoIntegracaoTributaria;

            contratoFrete.QuantidadeDependentes = calculoImpostosRetorno.QuantidadeDependentes;
            contratoFrete.ValorPorDependente = calculoImpostosRetorno.ValorPorDependente;
            contratoFrete.ValorTotalDependentes = calculoImpostosRetorno.ValorTotalDependentes;

            CalcularAdiantamento(ref contratoFrete, unitOfWork, alterarAdiantamento);
        }

        public static Dominio.ObjetosDeValor.Embarcador.Terceiros.CalcularImpostosRetorno CalcularImpostos(Dominio.ObjetosDeValor.Embarcador.Terceiros.CalcularImpostosParametros calcularImpostosParametros, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool alterarAdiantamento = true)
        {
            Dominio.ObjetosDeValor.Embarcador.Terceiros.CalcularImpostosRetorno retorno = new Dominio.ObjetosDeValor.Embarcador.Terceiros.CalcularImpostosRetorno();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repconfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);
            Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);
            Repositorio.INSSImpostoContratoFrete repINSSImpostoContratoFrete = new Repositorio.INSSImpostoContratoFrete(unitOfWork);
            Repositorio.IRImpostoContratoFrete repIRImpostoContratoFrete = new Repositorio.IRImpostoContratoFrete(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            if (configuracaoTMS == null)
                configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (configuracaoContratoFreteTerceiro == null)
                configuracaoContratoFreteTerceiro = repconfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

            if (contratoFrete == null)
                contratoFrete = repContratoFrete.BuscarPorCodigo(calcularImpostosParametros.codigoContratoFrete);

            bool incluirTodosAcrescimosEDescontosNoCalculoDeImpostos = configuracaoTMS.IncluirTodosAcrescimosEDescontosNoCalculoDeImpostos;

            Dominio.Entidades.Cliente transportadorTerceiro = repCliente.BuscarPorCPFCNPJ(calcularImpostosParametros.cpfCnpjTerceiro);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = transportadorTerceiro.Modalidades.Where(o => o.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro).FirstOrDefault();
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportador = null;

            if (modalidade != null)
                modalidadeTransportador = repModalidadeTransportador.BuscarPorModalidade(modalidade.Codigo);

            Dominio.Entidades.ImpostoContratoFrete impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(transportadorTerceiro.CPF_CNPJ, calcularImpostosParametros.codigoTipoTerceiro ?? 0, null, "");

            if (impostoContratoFrete == null)
                impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(transportadorTerceiro.CPF_CNPJ, 0, null, "");

            if (impostoContratoFrete == null)
                impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(0D, calcularImpostosParametros.codigoTipoTerceiro ?? 0, transportadorTerceiro.RegimeTributario, transportadorTerceiro.Tipo);

            if (impostoContratoFrete == null)
                impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(0D, 0, transportadorTerceiro.RegimeTributario, transportadorTerceiro.Tipo);

            if (impostoContratoFrete == null)
                impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(0D, calcularImpostosParametros.codigoTipoTerceiro ?? 0, null, transportadorTerceiro.Tipo);

            if (impostoContratoFrete == null)
                impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(0D, 0, null, transportadorTerceiro.Tipo);

            if (impostoContratoFrete == null)
                impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(0D, calcularImpostosParametros.codigoTipoTerceiro ?? 0, transportadorTerceiro.RegimeTributario, "");

            if (impostoContratoFrete == null)
                impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(0D, 0, transportadorTerceiro.RegimeTributario, "");

            if (impostoContratoFrete == null)
                impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(0D, calcularImpostosParametros.codigoTipoTerceiro ?? 0, null, "");

            if (impostoContratoFrete == null)
                impostoContratoFrete = repImpostoContratoFrete.BuscarPorTerceiro(0D, 0, null, "");

            if (transportadorTerceiro.Tipo != "F" && impostoContratoFrete != null)//se é pessoa jurídica não gera impostos, por lei deve emitir um CT-e de subcontratação
            {
                if (impostoContratoFrete.TipoPessoa != transportadorTerceiro.Tipo && impostoContratoFrete.Terceiro == null)
                    impostoContratoFrete = null;//só segue com a tabela de imposto se realmente tem uma para juridica ou exterior ou se tem uma exclusiva para o terceiro
            }

            if (impostoContratoFrete == null)
            {
                retorno.AliquotaINSS =
                retorno.AliquotaIRRF =
                retorno.AliquotaSENAT =
                retorno.AliquotaSEST =
                retorno.AliquotaINSSPatronal =
                retorno.BaseCalculoINSS =
                retorno.BaseCalculoIRRF =
                retorno.BaseCalculoSENAT =
                retorno.BaseCalculoSEST =
                retorno.ValorINSS =
                retorno.ValorINSSPatronal =
                retorno.ValorIRRF =
                retorno.ValorSENAT =
                retorno.ValorSEST = 0m;

                if (modalidadeTransportador != null)
                {
                    retorno.AliquotaCOFINS = modalidadeTransportador.AliquotaCOFINS;
                    retorno.AliquotaPIS = modalidadeTransportador.AliquotaPIS;
                    retorno.CodigoIntegracaoTributaria = modalidadeTransportador.CodigoIntegracaoTributaria;
                }
            }
            else
            {
                decimal baseAcumuladaINSS = 0m;
                decimal valoresPeriodo = 0m;
                int codigoContratoFretePeriodo = calcularImpostosParametros.origemCalcularImposto == OrigemCalcularImposto.PagamentoMotoristaTMS ? calcularImpostosParametros.codigoContratoFrete : 0;
                int codigoPagamentoMotoristaTMSPeriodo = calcularImpostosParametros.origemCalcularImposto == OrigemCalcularImposto.ContratoFrete ? calcularImpostosParametros.codigoPagamentoMotoristaTMS : 0;

                DateTime primeiroDiaMesAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime primeiroDiaProximoMes = primeiroDiaMesAtual.AddMonths(1);

                if (configuracaoContratoFreteTerceiro?.RetencaoPorRaizCNPJ != null && configuracaoContratoFreteTerceiro.RetencaoPorRaizCNPJ && contratoFrete.Carga.Empresa != null)
                {
                    retorno.ValorIRRFPeriodo = repContratoFrete.BuscarIRRFPorRaizCNPJPorPeriodo(contratoFrete.Carga.Empresa.CNPJ, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, calcularImpostosParametros.codigoContratoFrete);
                    retorno.ValorIRRFPeriodo += repPagamentoMotoristaTMS.BuscarIRRFPorRaizCNPJPorPeriodo(contratoFrete.Carga.Empresa.CNPJ, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, codigoPagamentoMotoristaTMSPeriodo);

                    if (impostoContratoFrete.UtilizarBaseCalculoAcumulada)
                    {
                        valoresPeriodo = repContratoFrete.BuscarValorPorRaizCNPJPorPeriodo(contratoFrete.Carga.Empresa.CNPJ, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes);
                        valoresPeriodo += repPagamentoMotoristaTMS.BuscarValorPorRaizCNPJPorPeriodo(contratoFrete.Carga.Empresa.CNPJ, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes);

                        valoresPeriodo += CalcularValoresPeriodo(incluirTodosAcrescimosEDescontosNoCalculoDeImpostos, codigoContratoFretePeriodo, contratoFrete.Carga.Empresa.CNPJ, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, unitOfWork);
                    }
                }
                else
                {
                    retorno.ValorIRRFPeriodo = repContratoFrete.BuscarIRRFPorTerceiroPorPeriodo(transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, codigoContratoFretePeriodo);
                    retorno.ValorIRRFPeriodo += repPagamentoMotoristaTMS.BuscarIRRFPorTerceiroPorPeriodo(transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, codigoPagamentoMotoristaTMSPeriodo);

                    if (impostoContratoFrete.UtilizarBaseCalculoAcumulada && transportadorTerceiro != null)
                    {
                        valoresPeriodo = repContratoFrete.BuscarValorPorTerceiroPorPeriodo(transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes);
                        valoresPeriodo += repPagamentoMotoristaTMS.BuscarValorPorTerceiroPorPeriodo(transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes);

                        if (incluirTodosAcrescimosEDescontosNoCalculoDeImpostos)
                        {
                            decimal valorTotalDescontoPeriodo = repContratoFreteValor.BuscarValorPorTerceiroPorPeriodo(codigoContratoFretePeriodo, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal });
                            decimal valorTotalAcrescimoPeriodo = repContratoFreteValor.BuscarValorPorTerceiroPorPeriodo(codigoContratoFretePeriodo, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal });

                            valoresPeriodo += valorTotalDescontoPeriodo - valorTotalAcrescimoPeriodo;
                        }
                        else
                        {
                            decimal valorTotalDescontoAdiantamentoSaldoPeriodo = repContratoFreteValor.BuscarValorPorTerceiroPorPeriodo(codigoContratoFretePeriodo, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo });
                            decimal valorTotalAcrescimoAdiantamentoSaldoPeriodo = repContratoFreteValor.BuscarValorPorTerceiroPorPeriodo(codigoContratoFretePeriodo, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo });

                            valoresPeriodo += valorTotalAcrescimoAdiantamentoSaldoPeriodo - valorTotalDescontoAdiantamentoSaldoPeriodo;
                        }
                    }
                }

                /*CALCULO DO INSS*/

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = contratoFrete.Carga.Empresa != null ? repGrupoPessoas.BuscarPorRaizCNPJ(contratoFrete.Carga.Empresa.RaizCnpj) : null;

                if (impostoContratoFrete.CalcularPorRaizCNPJ && grupoPessoas != null)
                {
                    decimal valoresPeriodoRaiz = repContratoFrete.BuscarValorPorRaizCNPJPorPeriodo(contratoFrete.Carga.Empresa.CNPJ, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes);

                    valoresPeriodoRaiz += repPagamentoMotoristaTMS.BuscarValorPorRaizCNPJPorPeriodo(contratoFrete.Carga.Empresa.CNPJ, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes);
                    valoresPeriodoRaiz += CalcularValoresPeriodo(incluirTodosAcrescimosEDescontosNoCalculoDeImpostos, codigoContratoFretePeriodo, contratoFrete.Carga.Empresa.CNPJ, transportadorTerceiro.CPF_CNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, unitOfWork);

                    baseAcumuladaINSS = valoresPeriodoRaiz > 0 ? Math.Round((valoresPeriodoRaiz * (impostoContratoFrete.PercentualBCINSS / 100)), 2, MidpointRounding.ToEven) : 0;
                }
                else
                {
                    baseAcumuladaINSS = valoresPeriodo > 0 ? Math.Round((valoresPeriodo * (impostoContratoFrete.PercentualBCINSS / 100)), 2, MidpointRounding.ToEven) : 0;
                }

                retorno.BaseCalculoINSS = Math.Round(calcularImpostosParametros.valorTotalParaCalculo * (impostoContratoFrete.PercentualBCINSS / 100m), 2, MidpointRounding.ToEven);

                decimal baseINSSPaga = baseAcumuladaINSS > 0 ? baseAcumuladaINSS - retorno.BaseCalculoINSS : 0;
                Dominio.Entidades.INSSImpostoContratoFrete inssImpostoContratoFrete = repINSSImpostoContratoFrete.BuscarPorImpostoEFaixa(impostoContratoFrete.Codigo, baseINSSPaga > 0 ? baseINSSPaga : retorno.BaseCalculoINSS);

                decimal valorINSSJaPago = 0m;

                if (inssImpostoContratoFrete != null)
                {
                    retorno.AliquotaINSS = inssImpostoContratoFrete.PercentualAplicar;

                    valorINSSJaPago = Math.Round(baseINSSPaga * (inssImpostoContratoFrete.PercentualAplicar / 100), 2, MidpointRounding.ToEven);

                    if (valorINSSJaPago < impostoContratoFrete.ValorTetoRetencaoINSS)
                    {
                        decimal valorINSS = Math.Round(retorno.BaseCalculoINSS * (retorno.AliquotaINSS / 100), 2, MidpointRounding.ToEven);

                        valorINSSJaPago += valorINSS;

                        if (valorINSSJaPago > impostoContratoFrete.ValorTetoRetencaoINSS)
                        {
                            retorno.ValorINSS = valorINSS - (valorINSSJaPago - impostoContratoFrete.ValorTetoRetencaoINSS);
                            valorINSSJaPago = impostoContratoFrete.ValorTetoRetencaoINSS;
                        }
                        else
                            retorno.ValorINSS = valorINSS;
                    }
                    else
                    {
                        valorINSSJaPago = impostoContratoFrete.ValorTetoRetencaoINSS;
                        retorno.ValorINSS = 0;
                    }
                }
                else
                {
                    retorno.AliquotaINSS = 0m;
                    retorno.ValorINSS = 0m;
                }

                /*CALCULO DO IRRF*/
                decimal baseCalculoIRRFTotalSemDescontos = 0m;
                retorno.BaseCalculoIRRFSemAcumulo = 0;

                if (impostoContratoFrete.UtilizarBaseCalculoAcumulada && valoresPeriodo > 0m)
                {
                    baseCalculoIRRFTotalSemDescontos = Math.Round(valoresPeriodo * (impostoContratoFrete.PercentualBCIR / 100m), 2, MidpointRounding.ToEven);
                    retorno.BaseCalculoIRRF = Math.Round(baseCalculoIRRFTotalSemDescontos - valorINSSJaPago, 2, MidpointRounding.AwayFromZero);

                    retorno.BaseCalculoIRRFSemAcumulo = Math.Round((calcularImpostosParametros.valorTotalParaCalculo * (impostoContratoFrete.PercentualBCIR / 100m)) - retorno.ValorINSS, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    retorno.BaseCalculoIRRF = Math.Round((calcularImpostosParametros.valorTotalParaCalculo * (impostoContratoFrete.PercentualBCIR / 100m)) - retorno.ValorINSS, 2, MidpointRounding.AwayFromZero);
                    retorno.BaseCalculoIRRFSemAcumulo = retorno.BaseCalculoIRRF;
                }

                //Desconto dependentes terceiro
                retorno.BaseCalculoIRRFSemDesconto = retorno.BaseCalculoIRRF;
                retorno.ValorIRRFSemDesconto = 0m;
                if (modalidadeTransportador?.QuantidadeDependentes > 0 && impostoContratoFrete.ValorPorDependenteDescontoIRRF > 0)
                {
                    retorno.QuantidadeDependentes = modalidadeTransportador.QuantidadeDependentes;
                    retorno.ValorPorDependente = impostoContratoFrete.ValorPorDependenteDescontoIRRF;
                    retorno.ValorTotalDependentes = retorno.QuantidadeDependentes.Value * retorno.ValorPorDependente;
                    retorno.BaseCalculoIRRF = retorno.BaseCalculoIRRF - retorno.ValorTotalDependentes;
                }

                if (retorno.BaseCalculoIRRF < 0m)
                    retorno.BaseCalculoIRRF = 0m;

                Dominio.Entidades.IRImpostoContratoFrete irImpostoContratoFrete = null;

                if ((impostoContratoFrete.UtilizarBaseCalculoAcumulada && !impostoContratoFrete.UtilizarCalculoIrSobreFaixaValorTotal && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && valoresPeriodo > 0m)
                    irImpostoContratoFrete = repIRImpostoContratoFrete.BuscarPorImpostoEFaixa(impostoContratoFrete.Codigo, baseCalculoIRRFTotalSemDescontos);
                else
                    irImpostoContratoFrete = repIRImpostoContratoFrete.BuscarPorImpostoEFaixa(impostoContratoFrete.Codigo, retorno.BaseCalculoIRRF);

                if (irImpostoContratoFrete != null)
                {
                    retorno.AliquotaIRRF = irImpostoContratoFrete.PercentualAplicar;
                    retorno.ValorIRRF = Math.Round((retorno.BaseCalculoIRRF * (retorno.AliquotaIRRF / 100m)) - irImpostoContratoFrete.ValorDeduzir, 2, MidpointRounding.AwayFromZero);

                    if (impostoContratoFrete.UtilizarBaseCalculoAcumulada && retorno.ValorIRRFPeriodo > 0m)
                        retorno.ValorIRRF -= retorno.ValorIRRFPeriodo;

                    if (retorno.ValorIRRF < 0m)
                        retorno.ValorIRRF = 0m;

                    retorno.ValorIRRFSemDesconto = Math.Round((retorno.BaseCalculoIRRFSemDesconto * (retorno.AliquotaIRRF / 100m)) - irImpostoContratoFrete.ValorDeduzir, 2, MidpointRounding.AwayFromZero);
                    if (impostoContratoFrete.UtilizarBaseCalculoAcumulada && retorno.ValorIRRFPeriodo > 0m)
                        retorno.ValorIRRFSemDesconto -= retorno.ValorIRRFPeriodo;
                    if (retorno.ValorIRRFSemDesconto < 0m)
                        retorno.ValorIRRFSemDesconto = 0m;

                }
                else
                {
                    retorno.AliquotaIRRF = 0m;
                    retorno.ValorIRRF = 0m;
                }

                /*CALCULO DO SEST SENAT*/
                retorno.BaseCalculoSEST = Math.Round(calcularImpostosParametros.valorTotalParaCalculo * (impostoContratoFrete.PercentualBCINSS / 100m), 2, MidpointRounding.AwayFromZero);
                retorno.BaseCalculoSENAT = Math.Round(calcularImpostosParametros.valorTotalParaCalculo * (impostoContratoFrete.PercentualBCINSS / 100m), 2, MidpointRounding.AwayFromZero);

                retorno.AliquotaSENAT = impostoContratoFrete.AliquotaSENAT;
                retorno.AliquotaSEST = impostoContratoFrete.AliquotaSEST;
                retorno.AliquotaPIS = impostoContratoFrete.AliquotaPIS;
                retorno.AliquotaCOFINS = impostoContratoFrete.AliquotaCOFINS;

                if (modalidadeTransportador != null && !string.IsNullOrWhiteSpace(modalidadeTransportador.CodigoIntegracaoTributaria))
                    retorno.CodigoIntegracaoTributaria = modalidadeTransportador.CodigoIntegracaoTributaria;
                if (string.IsNullOrWhiteSpace(retorno.CodigoIntegracaoTributaria))
                    retorno.CodigoIntegracaoTributaria = impostoContratoFrete.CodigoIntegracaoTributaria;

                if (modalidadeTransportador != null && modalidadeTransportador.AliquotaPIS > 0)
                    retorno.AliquotaPIS = modalidadeTransportador.AliquotaPIS;
                if (modalidadeTransportador != null && modalidadeTransportador.AliquotaCOFINS > 0)
                    retorno.AliquotaCOFINS = modalidadeTransportador.AliquotaCOFINS;

                retorno.ValorSEST = Math.Round(retorno.BaseCalculoSEST * (retorno.AliquotaSEST / 100m), 2, MidpointRounding.AwayFromZero);
                retorno.ValorSENAT = Math.Round(retorno.BaseCalculoSENAT * (retorno.AliquotaSENAT / 100m), 2, MidpointRounding.AwayFromZero);

                /*CALCULO DO INSS Patronal*/
                if (contratoFrete.ReterImpostosContratoFrete && impostoContratoFrete.AliquotaINSSPatronal > 0m)
                {
                    retorno.AliquotaINSSPatronal = impostoContratoFrete.AliquotaINSSPatronal;
                    retorno.ValorINSSPatronal = Math.Round(retorno.BaseCalculoINSS * (retorno.AliquotaINSSPatronal / 100m), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    retorno.AliquotaINSSPatronal = 0m;
                    retorno.ValorINSSPatronal = 0m;
                }
            }

            return retorno;
        }

        public static void AdicionarValePedagioContratoFrete(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, decimal valorValePedagio = 0)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(cargaValePedagio.Carga.Codigo);

            if (contratoFrete != null)
            {
                contratoFrete.ValorPedagio = valorValePedagio != 0 ? valorValePedagio : ObterValorPedagioContratoFrete(unitOfWork, cargaValePedagio.Carga);
                contratoFrete.TipoIntegracaoValePedagio = cargaValePedagio.TipoIntegracao;

                CalcularImpostos(ref contratoFrete, unitOfWork, tipoServicoMultisoftware);

                repContratoFrete.Atualizar(contratoFrete);
            }
        }

        public bool ReabrirContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteCTe repContratoFreteCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);

            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado &&
                contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Rejeitado &&
                contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada)
            {
                mensagemErro = "Não é possível reabrir o contrato em sua atual situação.";
                return false;
            }

            bool contratoComCiot = false;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                contratoComCiot = repContratoFrete.BuscarCiotPorContrato(contratoFrete.Codigo);
            else
                contratoComCiot = contratoFrete.ConfiguracaoCIOT != null;

            if (contratoComCiot)
            {
                mensagemErro = "Não é possível reabrir o contrato pois o mesmo está vinculado à um CIOT.";
                return false;
            }

            if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado ||
                contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoFinanceiraContratoFreteTerceiros = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceiraContratoFreteTerceiros = repConfiguracaoFinanceiraContratoFreteTerceiros.BuscarPrimeiroRegistro();

                contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto;

                repContratoFrete.Atualizar(contratoFrete);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = usuario?.Empresa?.TipoAmbiente ?? 0;

                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTerceiro.BuscarPorPessoa(contratoFrete.TransportadorTerceiro.CPF_CNPJ);
                if (!(modalidadeTransportadoraPessoas?.GerarPagamentoTerceiro ?? false))
                {
                    if (!serTituloAPagar.AtualizarTitulos(contratoFrete, unitOfWork, tipoServicoMultisoftware, out mensagemErro, tipoAmbiente, auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada))
                    {
                        return false;
                    }

                    if (!serContratoFrete.GerarMovimentacaoFinanceiraReversaoJustificativas(contratoFrete, unitOfWork, tipoServicoMultisoftware, out mensagemErro))
                    {
                        return false;
                    }
                }

                if (configuracaoFinanceiraContratoFreteTerceiros.GerarMovimentoAutomaticoPorTipoOperacao)
                    servicoContratoFrete.GerarReversaoMovimentosAprovacaoPorTipoOperacao(contratoFrete, configuracaoFinanceiraContratoFreteTerceiros, unitOfWork, tipoServicoMultisoftware);

                repContratoFreteCTe.DeletarPorContratoFrete(contratoFrete.Codigo);
            }
            else if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Rejeitado)
            {
                contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto;

                repContratoFrete.Atualizar(contratoFrete);
            }

            if (auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, contratoFrete, null, "Reabriu o Contrato.", unitOfWork);

            return true;

        }

        public static void RealizarCompensacaoAX(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = string.Empty;
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            if (integracao.PossuiIntegracaoAX && !string.IsNullOrWhiteSpace(integracao.URLAXCompansacao))
                Servicos.Embarcador.Integracao.AX.IntegracaoAX.CompensacaoContratoFrete(contratoFrete, integracao.URLAXContratoFrete, integracao.URLAXCompansacao, integracao.UsuarioAX, integracao.SenhaAX, out mensagemErro, unitOfWork);
        }

        public static void RealizarCancelamentoTotvs(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = string.Empty;
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
            Servicos.Embarcador.Integracao.Totvs.Movimento svsMovimentoTotvs = new Servicos.Embarcador.Integracao.Totvs.Movimento();

            if ((integracao?.PossuiIntegracaoDeTotvs ?? false) && !string.IsNullOrWhiteSpace(contratoFrete.CodigoCompanhia) && !string.IsNullOrWhiteSpace(contratoFrete.CodigoIntegracao))
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCargaCTe.BuscarPrimeiroCTePorCarga(contratoFrete.Carga.Codigo);
                if (cte != null)
                    svsMovimentoTotvs.IntegrarCancelamentoContratoTerceiro(contratoFrete, cte, contratoFrete.TransportadorTerceiro, contratoFrete.Carga.TipoOperacao, integracao.URLIntegracaoTotvs, integracao.UsuarioTotvs, integracao.SenhaTotvs, integracao.ContextoTotvs, unitOfWork, out mensagemErro);
            }
        }

        public DateTime ObterDataParaMovimentoFinanceiroDoContrato(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            if (contrato.UtilizarDataEmissaoParaMovimentoFinanceiro)
                return contrato.DataEmissaoContrato;

            return repCargaCTe.BuscarUltimaDataEmissaoNullablePorCarga(contrato.Carga.Codigo, "A") ?? contrato.DataEmissaoContrato;
        }

        public DateTime ObterDataParaMovimentoFinanceiroDoContrato(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            if (justificativa.UsarDataAutorizacaoParaMovimentoAcrescimoDesconto)
                return DateTime.Now;

            if (contrato.UtilizarDataEmissaoParaMovimentoFinanceiro)
                return contrato.DataEmissaoContrato;

            return repCargaCTe.BuscarUltimaDataEmissaoNullablePorCarga(contrato.Carga.Codigo, "A") ?? contrato.DataEmissaoContrato;

        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoEncerramentoCIOT> EncerrarCIOTPeloContratoTerfeito(int protocoloContratoTerceiro)
        {
            Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoEncerramentoCIOT> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoEncerramentoCIOT>()
            {
                Objeto = new Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoEncerramentoCIOT(),
                Mensagem = "Nenhum CIOT pendente de encerramento localizado.",
                Status = false,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            };

            try
            {
                Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros = new Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT()
                {
                    CodigoContratoTerceiro = protocoloContratoTerceiro
                };

                List<int> codigosCIOTs = repCIOT.ObterCodigosConsulta(filtros);

                if (codigosCIOTs.Count <= 0)
                {
                    retorno.Mensagem = "Não foram encontrados CIOT's para o contrato de terceiro.";
                    return retorno;
                }

                foreach (int codigoCIOT in codigosCIOTs)
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                    if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado &&
                        ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                        continue;

                    string mensagemErro = "";

                    Servicos.Log.TratarErro("EncerrarCIOTPeloContratoTerfeito codigoCiot" + ciot.Codigo.ToString(), "QuitacaoCIOTCarga");
                    if (svcCIOT.EncerrarCIOT(ciot, _unitOfWork, _tipoServicoMultisoftware, out mensagemErro))
                    {
                        Servicos.Auditoria.Auditoria.Auditar(_auditado, ciot, null, "Encerrou o CIOT via integração.", _unitOfWork);

                        retorno.Objeto.CodigoCIOT = codigoCIOT;
                        retorno.Objeto.MensagemRetorno = mensagemErro;
                        retorno.Objeto.Sucesso = true;

                        retorno.Status = true;
                        retorno.Mensagem = mensagemErro;
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Mensagem = mensagemErro;

                        retorno.Objeto.CodigoCIOT = codigoCIOT;
                        retorno.Objeto.MensagemRetorno = mensagemErro;
                        retorno.Objeto.Sucesso = false;

                        ciot.Mensagem = mensagemErro;
                        repCIOT.Atualizar(ciot);

                        continue;
                    }

                    _unitOfWork.FlushAndClear();
                }

                return retorno;
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                retorno.Mensagem = "Ocorreu uma falha ao encerrar o CIOT.";
                return retorno;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoAutorizacaoPagamento> AutorizarPagamento(AutorizacaoPagamento autorizacaoPagamento)
        {
            Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoAutorizacaoPagamento> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoAutorizacaoPagamento>()
            {
                Objeto = new Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoAutorizacaoPagamento(),
                Mensagem = "",
                Status = false,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            };

            try
            {
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros = new Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT()
                {
                    CodigoContratoTerceiro = autorizacaoPagamento.ProtocoloContratoFrete,
                    TipoAutorizacaoPagamentoCIOTParcela = autorizacaoPagamento.TipoAutorizacao
                };

                List<int> codigosCIOTs = repCIOT.ObterCodigosConsulta(filtros);

                if (codigosCIOTs.Count <= 0)
                {
                    retorno.Mensagem = "Não foram encontrados CIOT's encerrados com os filtros realizados para autorizar o pagamento.";
                    return retorno;
                }

                if (autorizacaoPagamento.TipoAutorizacao != TipoAutorizacaoPagamentoCIOTParcela.Adiantamento && autorizacaoPagamento.TipoAutorizacao != TipoAutorizacaoPagamentoCIOTParcela.Saldo)
                {
                    retorno.Mensagem = "Tipo de Autorização não implementado.";
                    return retorno;
                }

                foreach (int codigoCIOT in codigosCIOTs)
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                    if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado &&
                        ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                        continue;

                    if (ciot.SaldoPago && autorizacaoPagamento.TipoAutorizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Saldo)
                        continue;

                    if (ciot.AdiantamentoPago && autorizacaoPagamento.TipoAutorizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Adiantamento)
                        continue;

                    if (ciot.AbastecimentoPago && autorizacaoPagamento.TipoAutorizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Abastecimento)
                        continue;

                    if (!Servicos.Embarcador.CIOT.CIOT.IntegrarAutorizacaoPagamentoParcela(out string mensagemErro, autorizacaoPagamento.TipoAutorizacao, ciot, null, _auditado, _tipoServicoMultisoftware, _unitOfWork))
                    {
                        retorno.Status = false;
                        retorno.Mensagem = mensagemErro;

                        retorno.Objeto.CodigoCIOT = codigoCIOT;
                        retorno.Objeto.MensagemRetorno = mensagemErro;
                        retorno.Objeto.Sucesso = false;

                        _unitOfWork.Start();

                        ciot.Mensagem = mensagemErro;

                        repCIOT.Atualizar(ciot);

                        _unitOfWork.CommitChanges();

                        continue;
                    }
                    else
                    {
                        retorno.Objeto.CodigoCIOT = codigoCIOT;
                        retorno.Objeto.MensagemRetorno = mensagemErro;
                        retorno.Objeto.Sucesso = true;

                        retorno.Status = true;
                        retorno.Mensagem = mensagemErro;

                        Servicos.Auditoria.Auditoria.Auditar(_auditado, ciot, mensagemErro, _unitOfWork);
                    }

                    _unitOfWork.FlushAndClear();
                }

                return retorno;
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                retorno.Mensagem = "Ocorreu uma falha ao autorizar o pagamento.";
                return retorno;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public void AutorizarPagamentoContratoFretePagamentoCIOT(Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete, List<Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao> pagamentoCIOTIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            var TipoPagamentoAcertoDespesa = autorizacaoPagamentoContratoFrete.TipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento ? TipoAutorizacaoPagamentoCIOTParcela.Adiantamento : TipoAutorizacaoPagamentoCIOTParcela.Saldo;

            foreach (var pagamentoCIOT in pagamentoCIOTIntegracao)
                ProcessarPagamentoCIOT(TipoPagamentoAcertoDespesa, pagamentoCIOT, tipoServicoMultisoftware, unitOfWork);

            GerarPagamentoContratoFreteIntegracao(autorizacaoPagamentoContratoFrete, unitOfWork);
        }

        public void ProcessarPagamentoCIOT(TipoAutorizacaoPagamentoCIOTParcela tipoPagamento, Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao pagamentoCIOTIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao repPagamentoCIOTIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            pagamentoCIOTIntegracao.DataIntegracao = DateTime.Now;
            pagamentoCIOTIntegracao.NumeroTentativas++;

            try
            {
                bool contratoJaFoiPago = false;
                if (pagamentoCIOTIntegracao.AutorizacaoPagamentoContratoFrete.TipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento && pagamentoCIOTIntegracao.ContratoFrete.DataAutorizacaoPagamentoAdiantamento != null)
                    contratoJaFoiPago = true;
                else if (pagamentoCIOTIntegracao.AutorizacaoPagamentoContratoFrete.TipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoSaldo && pagamentoCIOTIntegracao.ContratoFrete.DataAutorizacaoPagamentoSaldo != null)
                    contratoJaFoiPago = true;
                else if (pagamentoCIOTIntegracao.ContratoFrete.DataAutorizacaoPagamento != null)
                    contratoJaFoiPago = true;

                if (contratoJaFoiPago)
                {
                    pagamentoCIOTIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pagamentoCIOTIntegracao.ProblemaIntegracao = "Contrato de frete já foi pago.";
                }
                else
                {
                    if (pagamentoCIOTIntegracao.ContratoFrete.ConfiguracaoCIOT != null)
                    {
                        bool sucessoBuscarCiot = true;
                        if (pagamentoCIOTIntegracao.CIOT == null)
                        {
                            var cargaCIOT = repCargaCIOT.BuscarPorContrato(pagamentoCIOTIntegracao.ContratoFrete.Codigo);
                            if (cargaCIOT == null)
                                sucessoBuscarCiot = false;
                            else
                                pagamentoCIOTIntegracao.CIOT = cargaCIOT.CIOT;
                        }

                        if (!sucessoBuscarCiot)
                        {
                            pagamentoCIOTIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            pagamentoCIOTIntegracao.ProblemaIntegracao = "Contrato de frete configurado para emissão do CIOT e CIOT não encontrado.";
                        }
                        else
                        {
                            bool sucesso = Servicos.Embarcador.CIOT.CIOT.IntegrarAutorizacaoPagamentoParcela(out string mensagemErro, tipoPagamento, pagamentoCIOTIntegracao.CIOT, null, _auditado, tipoServicoMultisoftware, unitOfWork);

                            if (sucesso)
                            {
                                pagamentoCIOTIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                                pagamentoCIOTIntegracao.ProblemaIntegracao = "Registro integrado com sucesso.";
                            }
                            else
                            {
                                pagamentoCIOTIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                                pagamentoCIOTIntegracao.ProblemaIntegracao = mensagemErro;
                            }
                        }
                    }
                    else
                    {
                        if (pagamentoCIOTIntegracao.AutorizacaoPagamentoContratoFrete.TipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento)
                            pagamentoCIOTIntegracao.ContratoFrete.DataAutorizacaoPagamentoAdiantamento = System.DateTime.Now;
                        else if (pagamentoCIOTIntegracao.AutorizacaoPagamentoContratoFrete.TipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoSaldo)
                            pagamentoCIOTIntegracao.ContratoFrete.DataAutorizacaoPagamentoSaldo = System.DateTime.Now;

                        if (pagamentoCIOTIntegracao.ContratoFrete.DataAutorizacaoPagamentoAdiantamento != null && pagamentoCIOTIntegracao.ContratoFrete.DataAutorizacaoPagamentoSaldo != null)
                            pagamentoCIOTIntegracao.ContratoFrete.DataAutorizacaoPagamento = System.DateTime.Now;

                        repContratoFrete.Atualizar(pagamentoCIOTIntegracao.ContratoFrete);

                        pagamentoCIOTIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        pagamentoCIOTIntegracao.ProblemaIntegracao = "Contrato não possui CIOT";
                    }
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                pagamentoCIOTIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoCIOTIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar o pagamento do CIOT.";
            }

            repPagamentoCIOTIntegracao.Atualizar(pagamentoCIOTIntegracao);
        }

        public void GerarPagamentoContratoFreteIntegracao(Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao repPagamentoCIOTIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao repPagamentoContratoIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            var listaSituacaoIntegracao = new List<SituacaoIntegracao>() { SituacaoIntegracao.AgIntegracao, SituacaoIntegracao.AgRetorno, SituacaoIntegracao.ProblemaIntegracao };
            int contarIntegracaoPendente = repPagamentoCIOTIntegracao.ContarPorAutorizacaoPagamentoSituacaoIntegracao(autorizacaoPagamentoContratoFrete.Codigo, listaSituacaoIntegracao);

            if (contarIntegracaoPendente == 0)
            {
                foreach (var contratoFrete in autorizacaoPagamentoContratoFrete.ContratoFrete)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesAGerarPorTipoDocumento = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() {
                           Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM
                        };

                    foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracoesAGerarPorTipoDocumento)
                    {
                        // pendencia testar se ambiente esta configurado para realizar integração   
                        Dominio.Entidades.Embarcador.Cargas.TipoIntegracao existeTipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);
                        if (existeTipoIntegracao == null)
                            continue;

                        if (existeTipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM)
                        {
                            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

                            if (!(configuracaoIntegracaoKMM?.PossuiIntegracao ?? false))
                                continue;
                        }

                        var pagamentoCIOTIntegracao = new Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao();
                        pagamentoCIOTIntegracao.TipoIntegracao = existeTipoIntegracao;
                        pagamentoCIOTIntegracao.ProblemaIntegracao = "";
                        pagamentoCIOTIntegracao.NumeroTentativas = 0;
                        pagamentoCIOTIntegracao.AutorizacaoPagamentoContratoFrete = autorizacaoPagamentoContratoFrete;
                        pagamentoCIOTIntegracao.ContratoFrete = contratoFrete;
                        pagamentoCIOTIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        pagamentoCIOTIntegracao.DataIntegracao = DateTime.Now;

                        repPagamentoContratoIntegracao.Inserir(pagamentoCIOTIntegracao);
                    }
                }
            }
        }

        public void AutorizarPagamentoContratoFretePagamentoIntegracao(Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete, List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> pagamentoContratoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            foreach (var pagamentoContrato in pagamentoContratoIntegracao)
                ProcessarPagamentoContratoFreteIntegracao(autorizacaoPagamentoContratoFrete.TipoPagamento, pagamentoContrato, tipoServicoMultisoftware, unitOfWork);
        }

        public void ProcessarPagamentoContratoFreteIntegracao(EnumTipoPagamentoAutorizacaoPagamento tipoPagamento, Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao pagamentoContratoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            switch (pagamentoContratoIntegracao.TipoIntegracao.Tipo)
            {
                case TipoIntegracao.KMM:
                    var kmmService = new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(_unitOfWork ?? unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
                    kmmService.IntegrarLiberacaoPagamentoContratoFrete(tipoPagamento, pagamentoContratoIntegracao);
                    break;
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>> BuscarContratosFretePendentesIntegracao(string dataInicial, string dataFinal, int inicio, int quantidadeRegistros)
        {
            if (quantidadeRegistros > 50)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>>.CriarRetornoDadosInvalidos("Não é possivel retornar mais do que 50 registros por vez");

            Repositorio.Embarcador.Terceiros.ContratoFrete repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);

            int totalRegistroPendenteIntegracao = repositorioContratoFrete.ContarContratosFretePendentesIntegracao();

            Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>(),
                NumeroTotalDeRegistro = totalRegistroPendenteIntegracao
            };

            if (totalRegistroPendenteIntegracao == 0)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>>.CriarRetornoSucesso(retorno);

            retorno.Itens = ConverterEmObjetoContratoFreteTerceiro(repositorioContratoFrete.BuscarContratoFreteNaoIntegrados(dataInicial, dataFinal, inicio, quantidadeRegistros));

            return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>>.CriarRetornoSucesso(retorno);
        }

        public Retorno<bool> ConfirmarIntegracaoContratoFrete(List<int> protocolos)
        {
            if (protocolos.Count == 0)
                return Retorno<bool>.CriarRetornoDadosInvalidos("Sem protocolos de contratos para processar");

            Repositorio.Embarcador.Terceiros.ContratoFrete repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);

            foreach (int protocolo in protocolos)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repositorioContratoFrete.BuscarPorCodigo(protocolo);

                if (contratoFrete == null)
                    continue;

                contratoFrete.Integrado = true;
                repositorioContratoFrete.Atualizar(contratoFrete);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, contratoFrete, null, $"Contrato Nº{contratoFrete.NumeroContrato} Confirmado Via Integração", _unitOfWork);
            }

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public byte[] GerarFaturaPadrao(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, int codigoCarga = 0, bool origemTelaRelatorio = false, string paginaRelatorio = "", int codigoControleGeracao = 0)
        {
            return ReportRequest.WithType(ReportType.FaturaPadrao)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoContratoFrete", contratoFrete.Codigo)
                .AddExtraData("RelatorioTemp", relatorioTemp.ToJson())
                .AddExtraData("CodigoCarga", codigoCarga)
                .AddExtraData("OrigemTelaRelatorio", origemTelaRelatorio)
                .AddExtraData("PaginaRelatorio", paginaRelatorio)
                .AddExtraData("CodigoControleGeracao", codigoControleGeracao)
                .CallReport()
                .GetContentFile();
        }

        public static void RemoverVinculoPendenciaContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor, Repositorio.UnitOfWork unitOfWork)
        {
            if (contratoFreteValor.PendenciaContratoFrete != null)
            {
                Repositorio.Embarcador.Terceiros.PendenciaContratoFreteFuturo repPendenciaContratoFreteFuturo = new Repositorio.Embarcador.Terceiros.PendenciaContratoFreteFuturo(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo pendenciaContratoFreteFuturo = repPendenciaContratoFreteFuturo.BuscarPorCodigo(contratoFreteValor.PendenciaContratoFrete.Codigo);
                pendenciaContratoFreteFuturo.ContratoFreteDestino = null;
                pendenciaContratoFreteFuturo.Ativo = true;

                repPendenciaContratoFreteFuturo.Atualizar(pendenciaContratoFreteFuturo);
            }
        }
        #endregion

        #region Métodos Privados

        private static decimal CalcularValoresPeriodo(bool incluirTodosAcrescimosEDescontosNoCalculoDeImpostos, int codigoContratoFretePeriodo, string contratoFreteCnpj, double terceiroCNPJ, DateTime primeiroDiaMesAtual, DateTime primeiroDiaProximoMes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);

            if (incluirTodosAcrescimosEDescontosNoCalculoDeImpostos)
            {
                decimal valorTotalDescontoPeriodo = repContratoFreteValor.BuscarValorPorRaizCNPJPorPeriodo(codigoContratoFretePeriodo, contratoFreteCnpj, terceiroCNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal });
                decimal valorTotalAcrescimoPeriodo = repContratoFreteValor.BuscarValorPorRaizCNPJPorPeriodo(codigoContratoFretePeriodo, contratoFreteCnpj, terceiroCNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal });

                return valorTotalDescontoPeriodo - valorTotalAcrescimoPeriodo;
            }
            else
            {
                decimal valorTotalDescontoAdiantamentoSaldoPeriodo = repContratoFreteValor.BuscarValorPorRaizCNPJPorPeriodo(codigoContratoFretePeriodo, contratoFreteCnpj, terceiroCNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo });
                decimal valorTotalAcrescimoAdiantamentoSaldoPeriodo = repContratoFreteValor.BuscarValorPorRaizCNPJPorPeriodo(codigoContratoFretePeriodo, contratoFreteCnpj, terceiroCNPJ, primeiroDiaMesAtual, primeiroDiaProximoMes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoSaldo });

                return valorTotalAcrescimoAdiantamentoSaldoPeriodo - valorTotalDescontoAdiantamentoSaldoPeriodo;
            }
        }

        private void gerarPreCTe(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Cliente transportadorTerceiro, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaCTes.Count <= 0)
                return;

            Repositorio.Embarcador.Terceiros.ContratoFreteCTe repContratoFreteCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            Servicos.PreCTe serPreCTe = new PreCTe(unitOfWork);
            Servicos.Embarcador.CTe.CTe serCTe = new Embarcador.CTe.CTe(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe ultimaCargaCTe = cargaCTes.LastOrDefault();
            decimal totalFrete = 0;
            decimal totalPedagio = 0;
            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes.FirstOrDefault();

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe contratoFreteCTeExiste = repContratoFreteCTe.BuscarCargaCTe(cargaCTe.Codigo, contratoFrete.Codigo);
            if (contratoFreteCTeExiste != null)
                repContratoFreteCTe.Deletar(contratoFreteCTeExiste);

            Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe = new Dominio.Entidades.PreConhecimentoDeTransporteEletronico();
            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterEntidadeCTeParaObjeto(cargaCTe.CTe, enviarCTeApenasParaTomador, unitOfWork);
            cteIntegracao.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();

            decimal valorSubContratacao = contratoFrete.ValorFreteSubcontratacao - contratoFrete.Descontos;

            cteIntegracao.ValorFrete.FreteProprio = valorSubContratacao;
            totalFrete += cteIntegracao.ValorFrete.FreteProprio;
            if (cargaCTe.Equals(ultimaCargaCTe))
                cteIntegracao.ValorFrete.FreteProprio += valorSubContratacao - totalFrete;

            cteIntegracao.ValorFrete.ValorPrestacaoServico = cteIntegracao.ValorFrete.FreteProprio;
            if (contratoFrete.ValorPedagio > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                componente.Componente.Descricao = "Pedágio";
                componente.Componente.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO;

                componente.ValorComponente = contratoFrete.ValorPedagio;
                totalPedagio += componente.ValorComponente;
                if (cargaCTe.Equals(ultimaCargaCTe))
                    componente.ValorComponente += contratoFrete.ValorPedagio - totalPedagio;


                cteIntegracao.ValorFrete.ComponentesAdicionais.Add(componente);
                cteIntegracao.ValorFrete.ValorPrestacaoServico += componente.ValorComponente;
            }
            cteIntegracao.ValorFrete.ValorTotalAReceber = cteIntegracao.ValorFrete.ValorPrestacaoServico;
            cteIntegracao.Emitente = serEmpresa.ConverterObjetoEmpresa(transportadorTerceiro);
            cteIntegracao.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cteIntegracao.ValorFrete.ICMS.CST = "40";

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeDoc in cargaCTes)
            {
                cteIntegracao.DocumentosAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior>();
                Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior documentoAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior();
                documentoAnterior.ChaveAcesso = cargaCTeDoc.CTe.Chave;
                documentoAnterior.Emitente = serPessoa.ConverterObjetoEmpresa(cargaCTeDoc.CTe.Empresa);
                cteIntegracao.DocumentosAnteriores.Add(documentoAnterior);
            }

            cteIntegracao.TipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;
            cteIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
            cteIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
            cteIntegracao.Tomador = serPessoa.ConverterObjetoEmpresa(cargaCTe.CTe.Empresa);
            serPreCTe.SalvarDadosPreCTe(ref preCTe, cteIntegracao);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe contratoFreteCTe = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe();
            contratoFreteCTe.PreCTe = preCTe;
            contratoFreteCTe.CargaCTe = cargaCTe;
            contratoFreteCTe.ContratoFrete = contratoFrete;
            repContratoFreteCTe.Inserir(contratoFreteCTe);
        }

        private static void CalcularAdiantamento(ref Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork, bool alterarAdiantamento)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repositorioConfigContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repositorioConfigContratoFreteTerceiro.BuscarConfiguracaoPadrao();

            if (!alterarAdiantamento)
                return;

            decimal valorTotal = contratoFrete.ValorFreteSubcontratacao - contratoFrete.Descontos - contratoFrete.TarifaSaque - contratoFrete.TarifaTransferencia;

            if (contratoFrete.TipoIntegracaoValePedagio != null && !contratoFrete.TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato && !(contratoFrete.Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false) && !(configuracaoContratoFreteTerceiro.NaoSubtrairValePedagioDoContrato))
                valorTotal -= contratoFrete.ValorPedagio;
            else if (!contratoFrete.CalcularAdiantamentoComPedagio)
            {
                if (contratoFrete.TipoIntegracaoValePedagio == null)
                    valorTotal += contratoFrete.ValorPedagio;
            }

            if (contratoFrete.ReterImpostosContratoFrete && contratoFrete.TransportadorTerceiro != null && contratoFrete.TransportadorTerceiro.Tipo == "F")
                valorTotal -= contratoFrete.ValorINSS + contratoFrete.ValorIRRFReter + contratoFrete.ValorSENAT + contratoFrete.ValorSEST;

            contratoFrete.ValorAdiantamento = Math.Round(((valorTotal * contratoFrete.PercentualAdiantamento) / 100) + contratoFrete.ValorTotalAcrescimoAdiantamento - contratoFrete.ValorTotalDescontoAdiantamento, 2, MidpointRounding.AwayFromZero);

            if (contratoFrete.CalcularAdiantamentoComPedagio && contratoFrete.TipoIntegracaoValePedagio == null)
                contratoFrete.ValorAdiantamento += contratoFrete.ValorPedagio;
        }

        private static string FormataErroAlcada(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, string msg, string sufixo = " pois ")
        {
            return "Não é possível alterar o contrato " + contrato.NumeroContrato + sufixo + msg + ".";
        }

        private static bool ValidarDadosBancarios(Dominio.Entidades.Cliente cliente)
        {
            bool valido = true;
            if (cliente.Banco == null)
                valido = false;
            if (string.IsNullOrWhiteSpace(cliente.Agencia))
                valido = false;
            if (string.IsNullOrWhiteSpace(cliente.NumeroConta))
                valido = false;
            return valido;
        }

        private bool ValidarEncerramentoPorContrato(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, out string retorno, Repositorio.UnitOfWork unitOfWork)
        {
            if (contratoFrete.ConfiguracaoCIOT.EncerrarCIOTManualmente)
            {
                retorno = "Devido à configuração do CIOT, é necessário encerrar o CIOT manualmente.";
                return false;
            }

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(contratoFrete.TransportadorTerceiro, unitOfWork);

            bool configGerarPorViagem = contratoFrete.ConfiguracaoCIOT.GerarUmCIOTPorViagem;

            if (modalidade.TipoGeracaoCIOT.HasValue)
                configGerarPorViagem = modalidade.TipoGeracaoCIOT.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoCIOT.PorViagem;

            if (configGerarPorViagem || modalidade.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado)
            {
                retorno = "";
                return true;
            }
            else
            {
                retorno = "A configuração do CIOT deste contrato determina que ele pode ser usado para mais de um contrato de frete, sendo assim, é necessário encerrar diretamente o CIOT";
                return false;

            }

        }

        private void ValidarFinalizacaoPorCIOT(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string retorno)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            retorno = "";

            if (ValidarEncerramentoPorContrato(contratoFrete, out retorno, unitOfWork))
            {
                if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                {
                    contratoFrete.EmEncerramentoCIOT = true;
                    repContratoFrete.Atualizar(contratoFrete);
                }
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro> ConverterEmObjetoContratoFreteTerceiro(List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> listaContratos)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repositorioConfigContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repositorioConfigContratoFreteTerceiro.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro> listaContratoFreteTerceiro = new List<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>();
            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFrete Contrato in listaContratos)
            {
                listaContratoFreteTerceiro.Add(new Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro()
                {
                    Protocolo = Contrato.Codigo,
                    Descontos = Contrato.Descontos,
                    INSS = Contrato.ValorINSS,
                    IRRF = Contrato.ValorIRRF,
                    SEST_SENAT = Contrato.ValorSENAT + Contrato.ValorSEST,
                    TarifaSaque = Contrato.TarifaSaque,
                    TarifaTransferencia = Contrato.TarifaTransferencia,
                    ValePedagio = Contrato.ValorPedagio,
                    SubcontratacaoTerceiro = ObterValorSubContratacao(Contrato, configuracaoContratoFreteTerceiro),
                    Adicionais = ObterAdicionaisContratoFrete(Contrato),
                    Contratante = ConverterObjetoContratante(Contrato),
                    Terceiro = ConverterObjetoTercerio(Contrato),
                    Motoristas = ConverterObjetoListaMotoristaContrato(Contrato),
                    Veiculos = ConverterObjetoVeiculo(Contrato),
                    Destinatario = ConverterObjetoEmpresaContratoDetinatario(Contrato.Carga.Pedidos.FirstOrDefault()),
                    Remetente = ConverterObjetoEmpresaContratoRemetente(Contrato.Carga.Pedidos.FirstOrDefault()),
                    PesoCarga = Contrato.Carga.DadosSumarizados.PesoTotal,
                    ValorCarga = Contrato.Carga.DadosSumarizados.ValorTotalProdutos,
                    VolumesCarga = Contrato.Carga.DadosSumarizados.VolumesTotal,
                    RotaCarga = Contrato.Carga.Rota?.Descricao,
                    DataContrato = Contrato.DataEmissaoContrato.ToString("dd/MM/yyyy"),
                    HoraEmissaoContrato = Contrato.DataEmissaoContrato.ToString("HH:mm"),
                    DestinatariosCTe = ConverterObjetoEmpresaContratoDetinatariosCTe(Contrato.Carga?.CargaCTes?.FirstOrDefault())
                });
            }

            return listaContratoFreteTerceiro;
        }

        private decimal ObterValorSubContratacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete Contrato, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro)
        {
            return Contrato.ValorFreteSubcontratacao - (Contrato.TipoIntegracaoValePedagio != null && !Contrato.TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato && (!Contrato.Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false) && !(configuracaoContratoFreteTerceiro.NaoSubtrairValePedagioDoContrato) ? Contrato.ValorPedagio : 0m) -
                                             Contrato.ValoresAdicionais.Where(o => o.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal && o.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Sum(o => o.Valor) +
                                             Contrato.ValoresAdicionais.Where(o => o.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal && o.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Sum(o => o.Valor);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Terceiros.Adicional> ObterAdicionaisContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete Contrato)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Terceiros.Adicional> listaAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Terceiros.Adicional>();

            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor adicional in Contrato.ValoresAdicionais)
                listaAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Terceiros.Adicional()
                {
                    Valor = adicional.Valor,
                    Descricao = adicional.Justificativa?.Descricao ?? string.Empty,
                    Tipo = adicional.TipoJustificativa
                });

            return listaAdicionais;
        }

        private Contratante ConverterObjetoContratante(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete Contrato)
        {
            return new Contratante()
            {
                CNPJ = Contrato.Carga?.Empresa?.CNPJ_Formatado ?? string.Empty,
                NumeroCarga = Contrato.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                RazaoSocial = Contrato.Carga?.Empresa?.RazaoSocial ?? string.Empty,
                Numero = Contrato.NumeroContrato,
                Endereco = Contrato.Carga.Empresa.Endereco,
                CIOT = Contrato.Carga.CargaCIOTs.Select(o => o.CIOT.Numero).FirstOrDefault(),
                ProtocoloCIOT = Contrato.Carga.CargaCIOTs.Select(o => o.CIOT.ProtocoloAutorizacao + (!string.IsNullOrWhiteSpace(o.CIOT.Digito) ? "-" + o.CIOT.Digito : "")).FirstOrDefault()
            };
        }

        private Terceiro ConverterObjetoTercerio(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete Contrato)
        {
            return new Terceiro()
            {
                Nome = Contrato.TransportadorTerceiro?.Nome ?? string.Empty,
                Endereco = Contrato.TransportadorTerceiro?.EnderecoCompleto ?? string.Empty,
                CPF_CNPJ = Contrato.TransportadorTerceiro?.CPF_CNPJ_Formatado ?? string.Empty
            };
        }

        private List<MotoristaContrato> ConverterObjetoListaMotoristaContrato(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete Contrato)
        {
            List<MotoristaContrato> listaMotoristaContrato = new List<MotoristaContrato>();

            foreach (Dominio.Entidades.Usuario motorista in Contrato.Carga.Motoristas)
                listaMotoristaContrato.Add(new MotoristaContrato()
                {
                    CPF = motorista.CPF,
                    CNH = motorista?.NumeroHabilitacao ?? string.Empty,
                    NomeMotorista = motorista?.Nome ?? string.Empty
                });

            return listaMotoristaContrato;
        }

        private List<VeiculoContrato> ConverterObjetoVeiculo(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete Contrato)
        {
            List<VeiculoContrato> veiculoContratos = new List<VeiculoContrato>();

            if (Contrato.Carga.Veiculo != null)
                veiculoContratos.Add(new VeiculoContrato()
                {
                    Placa = Contrato.Carga?.Veiculo?.Placa ?? string.Empty,
                    Modelo = Contrato.Carga?.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    Renavam = Contrato.Carga?.Veiculo?.Renavam ?? string.Empty
                });

            foreach (Dominio.Entidades.Veiculo veiculo in Contrato.Carga.VeiculosVinculados)
                veiculoContratos.Add(new VeiculoContrato()
                {
                    Modelo = veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    Placa = veiculo.Placa,
                    Renavam = veiculo.Renavam
                });

            return veiculoContratos;
        }

        private EmpresaContrato ConverterObjetoEmpresaContratoDetinatario(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            return new EmpresaContrato()
            {
                Nome = cargaPedido.Pedido?.Destinatario?.Nome ?? string.Empty,
                Municipio = cargaPedido.Pedido?.Destino?.Descricao ?? string.Empty
            };
        }

        private EmpresaContrato ConverterObjetoEmpresaContratoRemetente(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            return new EmpresaContrato()
            {
                Nome = cargaPedido.Pedido?.Remetente?.Nome ?? string.Empty,
                Municipio = cargaPedido.Pedido.Origem.Descricao ?? string.Empty
            };
        }

        private EmpresaContrato ConverterObjetoEmpresaContratoDetinatariosCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            if (cargaCTe == null)
                return new EmpresaContrato()
                {
                    Nome = string.Empty,
                    Municipio = string.Empty
                };

            return new EmpresaContrato()
            {
                Nome = cargaCTe.CTe.Destinatario?.Descricao ?? "",
                Municipio = cargaCTe.CTe.Destinatario.Localidade?.DescricaoCidadeEstado ?? ""
            };
        }

        private static decimal ObterValorPedagioContratoFrete(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

            return repositorioCargaValePedagio.BuscarPorCarga(carga?.Codigo ?? 0, SituacaoIntegracao.Integrado).Sum(x => x.ValorValePedagio);

        }
        #endregion
    }
}
