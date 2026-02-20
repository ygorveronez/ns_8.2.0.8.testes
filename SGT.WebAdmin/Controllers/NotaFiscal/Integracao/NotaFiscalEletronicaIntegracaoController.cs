using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal.Integracao
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotaisIntegracoes" }, "NotaFiscal/NotaFiscalEletronica")]
    public class NotaFiscalEletronicaIntegracaoController : BaseController
    {
        #region Construtores

        public NotaFiscalEletronicaIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeDeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigo);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repNotaFiscal.BuscarArquivosPorIntegracao(codigo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repNotaFiscal.ContarBuscarArquivosPorIntegracao(codigo));

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

                Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao integracaoNotaFiscalEletronica = repNotaFiscalEletronicaIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracaoNotaFiscalEletronica == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracaoNotaFiscalEletronica.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

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

                Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao integracao = repNotaFiscalEletronicaIntegracao.BuscarIntegracaoPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;

                if (integracao.TipoIntegracao.Tipo == TipoIntegracao.Globus)
                {
                    var servicoIntegracaoGlobus = new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork);

                    if (integracao.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Autorizado)
                        servicoIntegracaoGlobus.IntegrarNotaFiscalEletronica(integracao);
                    else if (integracao.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Cancelado)
                        servicoIntegracaoGlobus.IntegrarCancelamentoNotaFiscalEletronica(integracao);
                }
                else
                {
                    integracao.ProblemaIntegracao = Localization.Resources.Gerais.Geral.IntegracaoNaoDisponivel;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                repNotaFiscalEletronicaIntegracao.Atualizar(integracao);

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
        public async Task<IActionResult> PesquisaNotaFiscalEletronicaIntegracoes()
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

                Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repositorioNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPropriedadeOrdenarPesquisaDocumentoEntradaIntegracoes(grid.header[grid.indiceColunaOrdena].data);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao> listaIntegracoes = repositorioNotaFiscalEletronicaIntegracao.Consultar(codigo, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repositorioNotaFiscalEletronicaIntegracao.ContarConsulta(codigo, situacao);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repositorioNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repositorioNotaFiscalEletronicaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repositorioNotaFiscalEletronicaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repositorioNotaFiscalEletronicaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repositorioNotaFiscalEletronicaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

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

                Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repositorioNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao integracaoNotaFiscalEletronica = repositorioNotaFiscalEletronicaIntegracao.BuscarIntegracaoPorCodigo(codigo);

                if (integracaoNotaFiscalEletronica == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                integracaoNotaFiscalEletronica.DataIntegracao = DateTime.Now;
                integracaoNotaFiscalEletronica.NumeroTentativas += 1;
                integracaoNotaFiscalEletronica.ProblemaIntegracao = motivo.Trim();
                integracaoNotaFiscalEletronica.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repositorioNotaFiscalEletronicaIntegracao.Atualizar(integracaoNotaFiscalEletronica);

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
