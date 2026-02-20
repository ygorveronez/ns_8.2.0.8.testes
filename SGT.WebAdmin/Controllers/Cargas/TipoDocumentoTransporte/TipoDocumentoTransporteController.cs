using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.TipoDocumentoTransporte
{
    [CustomAuthorize("Cargas/TipoDocumentoTransporte")]
    public class TipoDocumentoTransporteController : BaseController
    {
		#region Construtores

		public TipoDocumentoTransporteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unidadeTrabalho, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte repTipoDocumentoTransporte = new Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte tipoDocumentoTransporte = repTipoDocumentoTransporte.BuscarPorCodigo(codigo, true);

                if (tipoDocumentoTransporte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(tipoDocumentoTransporte, unitOfWork);

                repTipoDocumentoTransporte.Atualizar(tipoDocumentoTransporte, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte repTipoDocumentoTransporte = new Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte tipoDocumentoTransporte = new Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte();

                PreencherEntidade(tipoDocumentoTransporte, unitOfWork);

                repTipoDocumentoTransporte.Inserir(tipoDocumentoTransporte, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte repTipoDocumentoTransporte = new Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte tipoDocumentoTransporte = repTipoDocumentoTransporte.BuscarPorCodigo(codigo);

                if (tipoDocumentoTransporte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repTipoDocumentoTransporte.Deletar(tipoDocumentoTransporte, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte repTipoDocumentoTransporte = new Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte tipoDocumentoTransporte = repTipoDocumentoTransporte.BuscarPorCodigo(codigo, true);

                if (tipoDocumentoTransporte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                dynamic dynPedido = new
                {
                    Descricao = tipoDocumentoTransporte.Descricao,
                    CodigoIntegracao = tipoDocumentoTransporte.CodigoIntegracao,
                    Status = tipoDocumentoTransporte.Status
                };

                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metódos Privados

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho, bool exportacao = false)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte repositorioTipoDocumentoTransporte = new Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.TipoDocumentoTransporte.FiltroTipoDocumentoTransporte filtrosPesquisa = ObterFiltrosTipoDocumentoTransporte();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Status", "Status", 15, Models.Grid.Align.center);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioTipoDocumentoTransporte.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte> listaTipoDocumentoTranporte = totalRegistros > 0 ? repositorioTipoDocumentoTransporte.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte>();

                grid.AdicionaRows((
                    from o in listaTipoDocumentoTranporte
                    select new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.CodigoIntegracao,
                        Status = o.Status.ObterDescricaoAtivo()
                    }).ToList()
                );

                grid.setarQuantidadeTotal(totalRegistros);

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

        private Dominio.ObjetosDeValor.Embarcador.Carga.TipoDocumentoTransporte.FiltroTipoDocumentoTransporte ObterFiltrosTipoDocumentoTransporte()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.TipoDocumentoTransporte.FiltroTipoDocumentoTransporte()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Status = Request.GetBoolParam("Status"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao")
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte tipoDocumentoTransporte, Repositorio.UnitOfWork unitOfWork)
        {
            tipoDocumentoTransporte.Status = Request.GetBoolParam("Status");
            tipoDocumentoTransporte.Descricao = Request.GetStringParam("Descricao");
            tipoDocumentoTransporte.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
        }

        #endregion
    }
}
