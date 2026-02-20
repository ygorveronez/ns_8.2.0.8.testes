using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Frotas/TipoDestinoOleo")]
    public class TipoDestinoOleoController : BaseController
    {
		#region Construtores

		public TipoDestinoOleoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                var propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frotas.TipoDestinoOleo repTipoDestinoOleo = new Repositorio.Embarcador.Frotas.TipoDestinoOleo(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo tipoDestinoOleo = new Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo();

                PreencherEntidade(tipoDestinoOleo, unitOfWork);

                unitOfWork.Start();

                repTipoDestinoOleo.Inserir(tipoDestinoOleo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frotas.TipoDestinoOleo repTipoDestinoOleo = new Repositorio.Embarcador.Frotas.TipoDestinoOleo(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo tipoDestinoOleo = repTipoDestinoOleo.BuscarPorCodigo(codigo, true);

                if (tipoDestinoOleo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(tipoDestinoOleo, unitOfWork);

                unitOfWork.Start();

                repTipoDestinoOleo.Atualizar(tipoDestinoOleo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frotas.TipoDestinoOleo repTipoDestinoOleo = new Repositorio.Embarcador.Frotas.TipoDestinoOleo(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo tipoDestinoOleo = repTipoDestinoOleo.BuscarPorCodigo(codigo, false);

                if (tipoDestinoOleo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoDestinoOleo.Codigo,
                    tipoDestinoOleo.Descricao,
                });
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frotas.TipoDestinoOleo repTipoDestinoOleo = new Repositorio.Embarcador.Frotas.TipoDestinoOleo(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo tipoDestinoOleo = repTipoDestinoOleo.BuscarPorCodigo(codigo, true);

                if (tipoDestinoOleo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repTipoDestinoOleo.Deletar(tipoDestinoOleo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo tipoDestinoOleo, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.GetStringParam("Descricao");

            tipoDestinoOleo.Descricao = !String.IsNullOrEmpty(descricao) ? descricao : throw new ControllerException("A descrição não pode ser nula ou vazia!");
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frotas.TipoDestinoOleo repTipoDestinoOleo = new Repositorio.Embarcador.Frotas.TipoDestinoOleo(unitOfWork);

            string descricao = Request.Params("Descricao");

            List<Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo> listaTipoDestinoOleo = repTipoDestinoOleo.Consultar(descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repTipoDestinoOleo.ContarConsulta(descricao);

            var dynListaTipoDestinoOleo = from tipoDestino in listaTipoDestinoOleo
                                   select new
                                   {
                                       tipoDestino.Codigo,
                                       tipoDestino.Descricao
                                   };

            return dynListaTipoDestinoOleo.ToList();
        }

        #endregion
    }
}
