using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Canhotos/CanhotoIntegracaoPorCTe")]
    public class CanhotoIntegracaoPorCTeController : BaseController
    {
		#region Construtores

		public CanhotoIntegracaoPorCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(GridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repositorioCTeCanhotoIntegracao = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(unidadeTrabalho);

                Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao canhotoIntegracaoPorCTe = repositorioCTeCanhotoIntegracao.BuscarPorCodigo(codigo);

                canhotoIntegracaoPorCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, canhotoIntegracaoPorCTe, null, "Reenviou integração", unidadeTrabalho);

                repositorioCTeCanhotoIntegracao.Atualizar(canhotoIntegracaoPorCTe);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar reenviar o arquivo para integração.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigos = Request.GetListParam<int>("Codigos");

                Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repositorioCanhotoIntegracaoPorCTe = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao> integracoesPendentes = repositorioCanhotoIntegracaoPorCTe.BuscarPorCodigoSituacaoFalhaIntegracao(codigos);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao cteCanhotoIntegracao in integracoesPendentes)
                {
                    cteCanhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repositorioCanhotoIntegracaoPorCTe.Atualizar(cteCanhotoIntegracao);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu um erro ao reenviar as integrações");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracaoPorCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repositorioCanhotoIntegracaoPorCTe = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao integracao = repositorioCanhotoIntegracaoPorCTe.BuscarPorCodigo(codigo);
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
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExportarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número Documento", "Numero", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Emitente", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "DataIntegracao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 30, Models.Grid.Align.left, false);

                Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repositorioCTeCanhotoIntegracao = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCTeCanhotoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioCTeCanhotoIntegracao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao> listaGrid = repositorioCTeCanhotoIntegracao.Consultar(filtrosPesquisa, parametrosConsulta);

                var lista = from obj in listaGrid
                            select new
                            {
                                obj.Codigo,
                                obj.CTe.Numero,
                                Tipo = obj.TipoRegistro.ObterDescricao(),
                                DataIntegracao = obj.DataIntegracao,
                                CodigoCargaEmbarcador = string.Join(", ", (from p in obj.CTe.CargaCTes select p.Carga.CodigoCargaEmbarcador).ToList()),
                                Situacao = obj.DescricaoSituacaoIntegracao,
                                MensagemRetorno = obj.ProblemaIntegracao,
                                Emitente = obj.CTe?.Empresa?.RazaoSocial,
                                DT_RowClass = this.CorPorCTeCanhotoIntegracao(obj)
                            };

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoPorCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repositorioCanhotoIntegracaoPorCTe = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao integracao = repositorioCanhotoIntegracaoPorCTe.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos " + integracao.CTe.Numero + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo de integração.");
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCTeCanhotoIntegracao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCTeCanhotoIntegracao()
            {
                CodigoCarga = Request.GetIntParam("CodigoCargaEmbarcador"),
                TipoRegistro = Request.GetNullableEnumParam<TipoRegistroIntegracaoCTeCanhoto>("TipoRegistro"),
                Situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao"),
                DataEmissaoNFeInicial = Request.GetDateTimeParam("DataEmissaoNFeInicial"),
                DataEmissaoNFeFinal = Request.GetDateTimeParam("DataEmissaoNFeFinal"),
                DataEntregaInicial = Request.GetDateTimeParam("DataEntregaInicial"),
                DataEntregaFinal = Request.GetDateTimeParam("DataEntregaFinal"),
                DataDigitalizacaoInicial = Request.GetDateTimeParam("DataDigitalizacaoInicial"),
                DataDigitalizacaoFinal = Request.GetDateTimeParam("DataDigitalizacaoFinal"),
                DataAprovacaoInicial = Request.GetDateTimeParam("DataAprovacaoInicial"),
                DataAprovacaoFinal = Request.GetDateTimeParam("DataAprovacaoFinal"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                Emitente = Request.GetIntParam("Emitente"),
                NumeroDocumento = Request.GetIntParam("NumeroDocumento"),
            };
        }

        private string CorPorCTeCanhotoIntegracao(Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao canhotoIntegracaoPorCTe)
        {
            if (canhotoIntegracaoPorCTe.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                return ClasseCorFundo.Primary(IntensidadeCor._100);

            if (canhotoIntegracaoPorCTe.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                return ClasseCorFundo.Sucess(IntensidadeCor._100);

            if (canhotoIntegracaoPorCTe.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                return ClasseCorFundo.Danger(IntensidadeCor._100);

            if (canhotoIntegracaoPorCTe.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno)
                return ClasseCorFundo.Warning(IntensidadeCor._100);

            return "";
        }

        #endregion
    }
}
