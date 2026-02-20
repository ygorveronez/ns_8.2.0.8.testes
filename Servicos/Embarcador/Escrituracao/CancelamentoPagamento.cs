using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Escrituracao
{
    public class CancelamentoPagamento
    {
        public static void ValidacaoParaPermitirGeraLoteCancelamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (!repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
                return;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacaoContaPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);


            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentoFaturamento = repositorioDocumentoFaturamento.BuscarPorCancelamentoPagamento(cancelamentoPagamento.Codigo);
            if (documentoFaturamento.Count == 0)
                throw new ServicoException("Documentos faturamento não selecionados para estornar");

            List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> pagamentoDocumentoIntegracao = repositorioPagamentoIntegracao.BuscarPorDocumentosFaturamento(documentoFaturamento.Select(x => x.Codigo).ToList());
            List<string> documentoSemSucesso = documentoFaturamento.Where(x =>
                                !pagamentoDocumentoIntegracao.Any(a => a.DocumentoFaturamento.Codigo == x.Codigo)
                                || pagamentoDocumentoIntegracao.Any(a => a.DocumentoFaturamento.Codigo == x.Codigo &&
                                   a.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado))
                                ?.Select(x => x.Numero)?.ToList() ?? new List<string>();

            if (documentoSemSucesso.Count > 0)
                throw new ServicoException($"Não é possivel gerar cancelamento !. Documentos sem MIRO gerada: {string.Join(", ", documentoSemSucesso)} ");

            if (documentoFaturamento.Any(x => !x.DataMiro.HasValue || x.DataMiro.Value.Month != DateTime.Now.Month))
                throw new ServicoException($"Não é possivel gerar cancelamento !. Data Miro fora do mes corrente");

            if (repositorioMovimentacaoContaPagar.ExisteMovimentacaoPorNumeroMiro(documentoFaturamento.Select(x => x.NumeroMiro).ToList()))
                throw new ServicoException($"Já foi recebida compensação para os documento e não é possivel gerar cancelamento ");

            List<(string numeroDocumento, int codigoCte)> complemento = documentoFaturamento.Where(x => x.CTe.PossuiCTeComplementar).Select(x => (x.Numero, x.CTe.Codigo)).ToList();

            if (complemento.Count > 0)
                throw new ServicoException($"Documento(s) {string.Join(", ", complemento.Select(x => x.numeroDocumento).ToList())} possui documentos complementares {string.Join(", ", repositorioCargaCte.BuscarNumerosCteComplementares(complemento.Select(x => x.codigoCte).ToList()))} já com MIRO gerada e seu estorno deve ser solicitado antes do documento original");

        }

        public static void ProcessarPagamentosEmCancelamento(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> cancelamentoPagamentos = repCancelamentoPagamento.BuscarPagamentosEmCancelamento();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento in cancelamentoPagamentos)
                {
                    GerarFechamentoDocumentosCancelamentoPagamento(cancelamentoPagamento, configuracaoFinanceiro, unidadeTrabalho, stringConexao, tipoServicoMultisoftware);
                }

            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private static void GerarFechamentoDocumentosCancelamentoPagamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Hubs.Pagamento servicoNotificacaoPagamento = new Hubs.Pagamento();

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoPagamentoSumarizada> pagamentosSumarizados = repDocumentoFaturamento.BuscarCodigosPorPagamentoEmCancelamento(cancelamentoPagamento.Codigo);

            int quantidadeDocumentos = cancelamentoPagamento.QuantidadeDocsCancelamentoPagamento;
            int quantidadeGerados = quantidadeDocumentos - pagamentosSumarizados.Count();
            int quantidadeTotal = 0;

            for (var i = 0; i < pagamentosSumarizados.Count(); i++)
            {
                unidadeTrabalho.Start();

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoPagamentoSumarizada pagamentoSumarizado = pagamentosSumarizados[i];

                GerarDocumentoContabil(pagamentoSumarizado, cancelamentoPagamento, unidadeTrabalho);
                repDocumentoFaturamento.SetarDocumentoMovimentoGeradoCancelamento(pagamentoSumarizado.Codigo);
                DisponibilizarDocumentoParaNovoPagamento(pagamentoSumarizado, unidadeTrabalho);

                repCTe.RemoveTitulo(pagamentoSumarizado.CodigoCTe);

                unidadeTrabalho.CommitChanges();

                if (quantidadeDocumentos < 10 || ((quantidadeTotal + 1) % 5) == 0)
                {
                    unidadeTrabalho.FlushAndClear();
                    servicoNotificacaoPagamento.InformarQuantidadeDocumentosProcessadosFechamentoCancelamentoPagamento(cancelamentoPagamento.Codigo, quantidadeDocumentos, ((quantidadeGerados + i) + 1));
                }
                quantidadeTotal++;
            }

            EfetuarIntegracaoCancelamentoPagamento(cancelamentoPagamento, unidadeTrabalho);
            cancelamentoPagamento.GerandoMovimentoFinanceiro = false;
            cancelamentoPagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.AgIntegracao;
            repCancelamentoPagamento.Atualizar(cancelamentoPagamento);

            if (configuracaoFinanceiro.NaoPermitirReenviarIntegracoesPagamentoSeCancelado)
                CancelarPagamentos(cancelamentoPagamento, repositorioPagamento);

            servicoNotificacaoPagamento.InformarCancelamentoPagamentoAtualizada(cancelamentoPagamento.Codigo);
        }

        private static void CancelarPagamentos(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento, Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento)
        {
            List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos = repositorioPagamento.BuscarPorCodigo(cancelamentoPagamento.Pagamentos.Select(o => o.Codigo).ToList());

            if (pagamentos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento in pagamentos)
                {
                    pagamento.Situacao = SituacaoPagamento.Cancelado;

                    repositorioPagamento.Atualizar(pagamento);
                }
            }
        }

        private static void GerarDocumentoContabil(Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoPagamentoSumarizada pagamentoSumarizado, Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamento, Repositorio.UnitOfWork unidadeTrabalho)
        {

            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentoContabils = repDocumentoContabil.BuscarPorDocumentoFaturamento(pagamentoSumarizado.Codigo);
            for (int i = 0; i < documentoContabils.Count; i++)
            {
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil = documentoContabils[i].Clonar();
                documentoContabil.Pagamento = null;
                documentoContabil.PagamentoLiberado = null;
                documentoContabil.CancelamentoPagamento = cancelamento;
                documentoContabil.DataRegistro = DateTime.Now;
                documentoContabil.DataLancamento = cancelamento.DataCriacao;

                if (documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito)
                    documentoContabil.TipoContabilizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Debito;
                else
                    documentoContabil.TipoContabilizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito;
                repDocumentoContabil.Inserir(documentoContabil);
            }

            repDocumentoProvisao.DisponibilizarProvisoesProvionarPorDocumentoFaturamento(pagamentoSumarizado.Codigo);
            repDocumentoProvisao.DisponibilizarProvisoesLiquidarPorDocumentoFaturamento(pagamentoSumarizado.Codigo);
        }

        private static void DisponibilizarDocumentoParaNovoPagamento(Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoPagamentoSumarizada pagamentoSumarizado, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoExiste = repDocumentoFaturamento.BuscarPorCodigo(pagamentoSumarizado.Codigo);
            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento = documentoExiste.Clonar();
            documento.CancelamentoPagamento = null;
            documento.Pagamento = null;
            documento.PagamentoLiberacao = null;
            documento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Autorizado;
            documento.MovimentoFinanceiroGerado = false;
            documento.BloqueioGeracaoAutomaticaPagamento = configFinanceiro.NaoGerarAutomaticamenteLotesCancelados;
            Utilidades.Object.DefinirListasGenericasComoNulas(documento);
            repDocumentoFaturamento.Inserir(documento);
        }

        public static void EfetuarIntegracaoCancelamentoPagamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(unitOfWork);

            if (GerarRegistrosIntegracaoCancelamentoPagamento(cancelamentoPagamento, unitOfWork))
                cancelamentoPagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.AgIntegracao;
            else
                cancelamentoPagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.Cancelado;

            repCancelamentoPagamento.Atualizar(cancelamentoPagamento);
        }

        private static bool EfetuarIntegracaoEDICancelamentoPagamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao repCancelamentoPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            // Atualiza status do cancelamentoProvisao
            cancelamentoPagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.AgIntegracao;

            // Cria entidade para integracao
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = LayoutEDICancelamentoPagamento(cancelamentoPagamento, unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> EDIs = (from o in layouts where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR select o).ToList();
            int count = EDIs.Count();
            for (int i = 0; i < count; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao integracaoEDI = repCancelamentoPagamentoEDIIntegracao.BuscarPorCancelamentoPagamentoELayoutEDI(cancelamentoPagamento.Codigo, EDIs[i].LayoutEDI.Codigo);
                if (integracaoEDI == null)
                {
                    integracaoEDI = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao
                    {
                        CancelamentoPagamento = cancelamentoPagamento,
                        LayoutEDI = EDIs[i].LayoutEDI,
                        TipoIntegracao = EDIs[i].TipoIntegracao,
                        SequenciaIntegracao = 1,
                        ProblemaIntegracao = "",
                        NumeroTentativas = 0,
                        DataIntegracao = DateTime.Now,
                        SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    };

                    repCancelamentoPagamentoEDIIntegracao.Inserir(integracaoEDI);
                }
            }

            return count > 0;
        }

        public static List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> LayoutEDICancelamentoPagamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<Dominio.Entidades.Cliente> tomadores = repDocumentoFaturamento.BuscarTomadoresCancelamentoPagamento(cancelamentoPagamento.Codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = (from tomador in tomadores where tomador.GrupoPessoas != null select tomador.GrupoPessoas).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
            for (int i = 0, s = grupoPessoas.Count(); i < s; i++)
                if (grupoPessoas[i].LayoutsEDI != null)
                    layouts.AddRange(grupoPessoas[i].LayoutsEDI.ToList());

            return layouts;
        }

        #region Metodos Privados
        private static bool GerarRegistrosIntegracaoCancelamentoPagamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            bool gerouIntegracao = false;

            List<TipoIntegracao> tipoPossivesGeraIntegracao = new List<TipoIntegracao>()
            {
                 TipoIntegracao.Unilever,
                 TipoIntegracao.Camil
            };

            foreach (var tipo in tipoPossivesGeraIntegracao)
            {
                if (!repositorioTipoIntegracao.ExistePorTipo(tipo))
                    continue;
                gerouIntegracao = true;
                if (tipo == TipoIntegracao.Unilever || tipo == TipoIntegracao.Camil)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentos = cancelamentoPagamento?.DocumentosFaturamento?.ToList() ?? new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
                    foreach (var documento in documentos)
                        GeraRegistroIntegracao(cancelamentoPagamento, unitOfWork, repositorioTipoIntegracao.BuscarPorTipo(tipo), documento);
                    continue;
                }

                GeraRegistroIntegracao(cancelamentoPagamento, unitOfWork, repositorioTipoIntegracao.BuscarPorTipo(tipo));

            }
            return gerouIntegracao;
        }

        private static Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao GeraRegistroIntegracao(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao repositorioCancelamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao novaintegracao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao();
            novaintegracao.DataIntegracao = DateTime.Now;
            novaintegracao.CancelamentoPagamento = cancelamentoPagamento;
            novaintegracao.ProblemaIntegracao = "";
            novaintegracao.TipoIntegracao = tipoIntegracao;
            novaintegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

            if (documentoFaturamento != null)
                novaintegracao.DocumentoFaturamento = documentoFaturamento;

            repositorioCancelamento.Inserir(novaintegracao);
            return novaintegracao;
        }

        public static void GerarCancelamentoPagamentoAutomatico(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, bool canceladoViaAnulacaoCTe = false)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repositorioCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoCamil repositorioIntegracaoCamil = new Repositorio.Embarcador.Configuracoes.IntegracaoCamil(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            if (!(configuracaoFinanceiro.EfetuarCancelamentoDePagamentoAoCancelarCarga ?? false))
                return;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCamil configuracaoCamil = repositorioIntegracaoCamil.Buscar();
            bool efetuarCancelamentoCamilSemPagamentoGerado = configuracaoCamil != null && configuracaoCamil.PossuiIntegracao;

            List<TipoIntegracao> tiposIntegracaoAceitosParaGerarCancelamentoPagamento = new List<TipoIntegracao>() {
                TipoIntegracao.Camil
            };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoParaGerarCancelamentoPagamento = repositorioTipoIntegracao.BuscarPorTipos(tiposIntegracaoAceitosParaGerarCancelamentoPagamento);
            if (tiposIntegracaoParaGerarCancelamentoPagamento.Count == 0)
                return;

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesParaCancelamentoPagamento = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (cargaCancelamento.CTe != null)
                ctesParaCancelamentoPagamento.Add(cargaCancelamento.CTe);
            else if (cargaCancelamento.Carga?.CargaCTes != null)
                ctesParaCancelamentoPagamento.AddRange(cargaCancelamento.Carga.CargaCTes.Where(cargaCTe => cargaCTe.CTe != null).Select(cargaCTe => cargaCTe.CTe));

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteParaCancelamentoPagamento in ctesParaCancelamentoPagamento)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repositorioDocumentoFaturamento.BuscarPorCTe(cteParaCancelamentoPagamento.Codigo);
                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = documentoFaturamento?.Pagamento;

                if (documentoFaturamento == null || documentoFaturamento.CancelamentoPagamento != null)
                    continue;

                if (pagamento == null && efetuarCancelamentoCamilSemPagamentoGerado)
                {
                    Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento.AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil, unitOfWork);
                    continue;
                }
                else if (pagamento == null)
                    continue;

                if (!tiposIntegracaoParaGerarCancelamentoPagamento
                        .Exists(integracao => repositorioPagamentoIntegracao.BuscarPorPagamentoETipoIntegracao(pagamento.Codigo, integracao.Tipo) != null))
                    continue;

                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamento = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento
                {
                    DataCriacao = DateTime.Now,
                    MotivoCancelamento = configuracaoFinanceiro.MotivoCancelamentoPagamentoPadrao,
                    Numero = repositorioCancelamentoPagamento.ObterProximoNumero(),
                    Carga = cargaCancelamento.Carga,
                    Tomador = pagamento.Tomador,
                    Filial = pagamento.Filial,
                    Empresa = pagamento.Empresa,
                    Pagamentos = new List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>()
                    {
                        pagamento
                    },
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.EmCancelamento
                };

                if (!cancelamento.IsInitialized())
                    cancelamento.Initialize();

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
                repositorioCancelamentoPagamento.Inserir(cancelamento, auditado);

                if (!canceladoViaAnulacaoCTe)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cancelamento, "Adicionado automaticamente pelo Cancelamento de Carga", unitOfWork);
                else
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cancelamento, "Adicionado automaticamente pela substituição ou anulação de CTe", unitOfWork);

                if (documentoFaturamento.Pagamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.Finalizado)
                {
                    documentoFaturamento.CancelamentoPagamento = cancelamento;
                    documentoFaturamento.MovimentoFinanceiroGerado = false;
                    documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.EmCancelamento;
                    repositorioDocumentoFaturamento.Atualizar(documentoFaturamento);

                    cancelamento.ValorCancelamentoPagamento = documentoFaturamento.ValorAFaturar;
                    cancelamento.QuantidadeDocsCancelamentoPagamento = 1;
                }

                cancelamento.GerandoMovimentoFinanceiro = true;
                repositorioCancelamentoPagamento.Atualizar(cancelamento);
            }
        }

        #endregion
    }
}
