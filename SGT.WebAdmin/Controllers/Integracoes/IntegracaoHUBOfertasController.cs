using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/IntegracaoHUB")]
    public class IntegracaoHUBOfertasController : BaseController
    {
        #region Construtores

        public IntegracaoHUBOfertasController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas repositorioCargaIntegracaoHUBOfertas = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas integracao = repositorioCargaIntegracaoHUBOfertas.BuscarPorCodigo(codigo, true);

                if (integracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (integracao.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Não é possível integrar nessa situação!");

                unitOfWork.Start();

                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                repositorioCargaIntegracaoHUBOfertas.Atualizar(integracao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, "Solicitou o reenvio da integração", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o reenvio da integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarMultiplasIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas repositorioCargaIntegracaoHUBOfertas = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(unitOfWork);

                int countIntegracoes = repositorioCargaIntegracaoHUBOfertas.ContarConsultaPorSituacao(SituacaoIntegracao.ProblemaIntegracao);

                if (countIntegracoes == 0)
                    return new JsonpResult(false, true, "Nenhuma integração com falha foi encontrada.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas> listaIntegracoes = repositorioCargaIntegracaoHUBOfertas.ConsultarPorSituacao(SituacaoIntegracao.ProblemaIntegracao);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas integracao in listaIntegracoes)
                {
                    if (integracao.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                        continue;

                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                    repositorioCargaIntegracaoHUBOfertas.Atualizar(integracao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, "Solicitou o reenvio da integração em lote", unitOfWork);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o reenvio de múltiplas integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas repositorioCargaIntegracaoHUBOfertas = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas integracao = repositorioCargaIntegracaoHUBOfertas.BuscarPorCodigo(codigo, false);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas repositorioCargaIntegracaoHUBOfertas = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas integracao = repositorioCargaIntegracaoHUBOfertas.BuscarPorCodigoArquivo(codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repositorioCargaIntegracaoHUBOfertas.BuscarArquivoHistoricoPorCodigo(codigo);

                if (arquivoIntegracao == null || integracao == null)
                    return new JsonpResult(false, true, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(false, true, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", $"Arquivos Integração {integracao.Empresa.RazaoSocial}.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoHUBOfertas filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Situacao", false);
            grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Mensagem", "Retorno", 30, Models.Grid.Align.left, false);

            Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas repositorioCargaIntegracaoHUBOfertas = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(unitOfWork);

            int countIntegracoes = repositorioCargaIntegracaoHUBOfertas.ContarConsulta(filtrosPesquisa).GetAwaiter().GetResult();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas> listaIntegracoes = countIntegracoes > 0 ? repositorioCargaIntegracaoHUBOfertas.Consultar(filtrosPesquisa, parametrosConsulta).GetAwaiter().GetResult() : new List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>();
            grid.setarQuantidadeTotal(countIntegracoes);

            var retorno = (from integracao in listaIntegracoes
                           select new
                           {
                               integracao.Codigo,
                               Situacao = integracao.SituacaoIntegracao,
                               Carga = integracao.Carga?.CodigoCargaEmbarcador ?? "",
                               Transportador = integracao.Empresa?.NomeFantasia ?? "",
                               SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                               TipoIntegracao = integracao.TipoEnvioHUBOfertas.ObterDescricao(),
                               integracao.NumeroTentativas,
                               Retorno = integracao.ProblemaIntegracao,
                               DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                               DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                               DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                           }).ToList();

            grid.AdicionaRows(retorno);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
            {
                return new JsonpResult(grid);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoHUBOfertas ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoHUBOfertas()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                Carga = Request.GetIntParam("Carga"),
                Situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao"),
                TipoEnvioHUBOfertas = Request.GetNullableEnumParam<TipoEnvioHUBOfertas>("TipoEnvioHUBOfertas")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Empresa")
                return propriedadeOrdenar += ".RazaoSocial";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
