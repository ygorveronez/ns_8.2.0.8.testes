using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotaisIntegracoes" }, "Financeiros/DocumentoEntrada")]
    public class DocumentoEntradaIntegracaoController : BaseController
    {
        #region Construtores

        public DocumentoEntradaIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigo);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repDocumentoEntradaTMS.BuscarArquivosPorIntegracao(codigo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repDocumentoEntradaTMS.ContarBuscarArquivosPorIntegracao(codigo));

                var retorno = (from obj in integracoesArquivos
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
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao integracaoDocumentoEntrada = repDocumentoEntradaIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracaoDocumentoEntrada == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracaoDocumentoEntrada.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Motorista.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Integrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao integracao = repDocumentoEntradaIntegracao.BuscarIntegracaoPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;

                if (integracao.TipoIntegracao.Tipo == TipoIntegracao.Globus)
                {
                    var servicoIntegracaoGlobus = new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork);
                    
                    if (integracao.DocumentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
                        servicoIntegracaoGlobus.IntegrarDocumentoEntrada(integracao);
                    else if (integracao.DocumentoEntrada.Situacao == SituacaoDocumentoEntrada.Cancelado)
                        servicoIntegracaoGlobus.IntegrarCancelamentoDocumentoEntrada(integracao);
                }
                else
                {
                    integracao.ProblemaIntegracao = Localization.Resources.Gerais.Geral.IntegracaoNaoDisponivel;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                repDocumentoEntradaIntegracao.Atualizar(integracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                unitOfWork.Rollback();

                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoAdicionarIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumentoEntradaIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.Tentativas, "NumeroTentativas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.DataDoEnvio, "DataIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Mensagem, "Retorno", 30, Models.Grid.Align.left, false);

                int codigo = int.Parse(Request.GetStringParam("Codigo"));

                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repositorioDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPropriedadeOrdenarPesquisaDocumentoEntradaIntegracoes(grid.header[grid.indiceColunaOrdena].data);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao> listaIntegracoes = repositorioDocumentoEntradaIntegracao.Consultar(codigo, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repositorioDocumentoEntradaIntegracao.ContarConsulta(codigo, situacao);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte()
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repositorioDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repositorioDocumentoEntradaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repositorioDocumentoEntradaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repositorioDocumentoEntradaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repositorioDocumentoEntradaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProblemaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.MotivoDeveSerInformado);

                Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repositorioDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao integracaoDocumentoEntrada = repositorioDocumentoEntradaIntegracao.BuscarIntegracaoPorCodigo(codigo);

                if (integracaoDocumentoEntrada == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                integracaoDocumentoEntrada.DataIntegracao = DateTime.Now;
                integracaoDocumentoEntrada.NumeroTentativas += 1;
                integracaoDocumentoEntrada.ProblemaIntegracao = motivo.Trim();
                integracaoDocumentoEntrada.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repositorioDocumentoEntradaIntegracao.Atualizar(integracaoDocumentoEntrada);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenarPesquisaDocumentoEntradaIntegracoes(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TipoIntegracao")
                return "TipoIntegracao.Tipo";

            return propriedadeOrdenar;
        }

    }
    #endregion
}
