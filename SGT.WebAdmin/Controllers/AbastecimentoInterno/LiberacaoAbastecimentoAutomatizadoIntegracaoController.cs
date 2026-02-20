using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.AbastecimentoInterno
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "AbastecimentoInterno/LiberacaoAbastecimentoAutomatizado")]
    public class LiberacaoAbastecimentoAutomatizadoIntegracaoController : BaseController
    {
        #region Construtores

        public LiberacaoAbastecimentoAutomatizadoIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(unidadeDeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repLiberacaoAbastecimentoAutomatizadoIntegracao.BuscarArquivosPorLiberacaoAbetecimentoAutomatizado(codigo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repLiberacaoAbastecimentoAutomatizadoIntegracao.ContarBuscarArquivosPorLiberacaoAbetecimentoAutomatizado(codigo));

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

        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repLiberacaoAbastecimentoAutomatizadoIntegracao.BuscarIntegracaoPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Abastecimento.zip");
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

        

        #endregion
    }
}