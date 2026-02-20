using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Escrituracao.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais", "PesquisaHistorico", "DownloadArquivosHistoricoIntegracao" }, "Escrituracao/Pagamento")]
    public class PagamentoIntegracaoController : BaseController
    {
        #region Construtores

        public PagamentoIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Pagamento"), out int codigoPagamento);

                int totalAguardandoIntegracao = repPagamentoIntegracao.ContarPorPagamento(codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repPagamentoIntegracao.ContarPorPagamento(codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repPagamentoIntegracao.ContarPorPagamento(codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações de EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ultragaz);

                Models.Grid.Grid grid = GridPesquisa(tipoIntegracao != null);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Integracao"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao integracao = repPagamentoIntegracao.BuscarPorCodigo(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count);

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Integracao"), out int codigoIntegracao);

                Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao integracao = repPagamentoIntegracao.BuscarPorCodigo(codigoIntegracao);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.FirstOrDefault(o => o.Codigo == codigo);

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Pagamento " + integracao.Pagamento.Descricao + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

                unitOfWork.Start();

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao integracao = repPagamentoIntegracao.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                if (integracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ultragaz)
                {
                    Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.Integracao configIntegracao = repositorioIntegracao.Buscar();

                    if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno
                        && configIntegracao != null && configIntegracao.NaoPermitirReenviarIntegracaoPagamentoAgRetorno)
                        return new JsonpResult(false, true, "Não é permitido reenviar integração que está aguardando retorno");

                    if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                        return new JsonpResult(false, true, "Não é permitido reenviar registro que já foi integrado");
                }

                ValidaReenvioIntegracao(integracao, unitOfWork);

                integracao.Pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.AguardandoIntegracao;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a Integração.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Pagamento, null, "Reenviou a Integração.", unitOfWork);

                repPagamentoIntegracao.Atualizar(integracao);
                repPagamento.Atualizar(integracao.Pagamento);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);

                unitOfWork.Start();

                int.TryParse(Request.Params("Pagamento"), out int codigoPagamento);

                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(codigoPagamento);

                if (pagamento.Integracoes.Count > 0)
                    ValidaReenvioIntegracao(pagamento.Integracoes.FirstOrDefault(), unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao integracao in pagamento.Integracoes)
                {

                    if (integracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ultragaz)
                    {
                        Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.Integracao configIntegracao = repositorioIntegracao.Buscar();

                        if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                            continue;
                        if (integracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ultragaz &&
                            integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno &&
                            configIntegracao != null && configIntegracao.NaoPermitirReenviarIntegracaoPagamentoAgRetorno)
                            continue;
                    }

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.Pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.AguardandoIntegracao;

                    repPagamento.Atualizar(integracao.Pagamento);
                    repPagamentoIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a Integração.", unitOfWork);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamento, null, "Reenviou as integrações.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPagamento;
                int.TryParse(Request.Params("Pagamento"), out codigoPagamento);

                Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao repPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repPagamentoEDIIntegracao.BuscarTipoIntegracaoPorPagamento(codigoPagamento);

                return new JsonpResult(new
                {
                    TiposIntegracoesCTe = tiposIntegracoesCTe,
                    TiposIntegracoesEDI = tiposIntegracoesEDI
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigoPagamento;
                int.TryParse(Request.Params("Pagamento"), out codigoPagamento);

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(codigoPagamento);

                if (pagamento == null)
                    return new JsonpResult(true, false, "Escrituração não encontrada.");

                if (pagamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.AguardandoIntegracao)
                    return new JsonpResult(true, false, "A situação da Escrituração não permite a finalização da etapa.");

                unidadeDeTrabalho.Start();

                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.Finalizado;
                repDocumentoFaturamento.LiberarPagamentosAutomaticamentePorPagamento(pagamento.Codigo);


                repPagamento.Atualizar(pagamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamento, null, "Finalizou.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a etapa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarSituacaoFalhaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("CodigoPagamento");

                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(codigo);

                if (pagamento == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                int integracao = repPagamentoIntegracao.ContarPorPagamentoESituacaoDiff(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

                var retorno = new
                {
                    MostrarBotaoFinalizarLote = integracao > 0
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha a buscar a Situação de Falha de Integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarLoteComFalhaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);

                unitOfWork.Start();

                int codigoPagamento = Request.GetIntParam("CodigoPagamento");

                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(codigoPagamento);

                if (pagamento == null)
                    return new JsonpResult(false, true, "Lote de Pagamento não encontrado.");

                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.Finalizado;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamento, null, "Finalizou o lote com falha na Integração.", unitOfWork);

                repPagamento.Atualizar(pagamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha para finalizar lote com Falha na Integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa(bool exibirCTe)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CT-e", "CTe", 10, Models.Grid.Align.center, false, exibirCTe);
            grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 25, Models.Grid.Align.left, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                situacao = situacaoAux;

            int.TryParse(Request.Params("Pagamento"), out int codigoPagamento);

            List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> listaGrid = repPagamentoIntegracao.Consultar(codigoPagamento, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repPagamentoIntegracao.ContarConsulta(codigoPagamento, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            CTe = obj.DocumentoFaturamento?.CTe.Numero.ToString() ?? "",
                            Situacao = obj.DescricaoSituacaoIntegracao,
                            TipoIntegracao = obj.TipoIntegracao.DescricaoTipo,
                            Retorno = obj.ProblemaIntegracao,
                            Tentativas = obj.NumeroTentativas,
                            DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                            DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                   obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                   Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                            DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "TipoIntegracao")
                propOrdena = "TipoIntegracao.Tipo";
            else if (propOrdena == "Tentativas")
                propOrdena = "NumeroTentativas";
            else if (propOrdena == "DataEnvio")
                propOrdena = "DataIntegracao";
            else if (propOrdena == "Situacao")
                propOrdena = "SituacaoIntegracao";
        }

        private void ValidaReenvioIntegracao(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            if (!configuracaoFinanceiro.BloqueioEnvioIntegracoesCargasAnuladaseCanceladas)
                return;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = integracao.DocumentoFaturamento?.CTe.CargaCTes?.Select(x => x.Carga).FirstOrDefault();

            if (carga == null)
                throw new ServicoException("A carga foi anulada ou cancelada e sua integração não pode ser reenviada.");

            List<SituacaoCarga> situacoesNaoPermitidas = new List<SituacaoCarga>()
            {
                SituacaoCarga.Anulada,
                SituacaoCarga.Cancelada
            };

            if (situacoesNaoPermitidas.Contains(carga.SituacaoCarga))
                throw new ServicoException("A carga foi anulada ou cancelada e sua integração não pode ser reenviada.");

            if (configuracaoFinanceiro.NaoPermitirReenviarIntegracoesPagamentoSeCancelado && integracao.Pagamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.Cancelado)
                throw new ServicoException("A configuração impede o reenvio de integrações de pagamento caso estejam canceladas.");
        }

        #endregion
    }
}
