using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Frotas/TipoOleo")]
    public class TipoOleoController : BaseController
    {
		#region Construtores

		public TipoOleoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Frotas.TipoOleo repTipoOleo = new Repositorio.Embarcador.Frotas.TipoOleo(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.TipoOleo tipoOleo = new Dominio.Entidades.Embarcador.Frotas.TipoOleo();

                PreencherEntidade(tipoOleo, unitOfWork);

                unitOfWork.Start();

                repTipoOleo.Inserir(tipoOleo, Auditado);

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

                Repositorio.Embarcador.Frotas.TipoOleo repTipoOleo = new Repositorio.Embarcador.Frotas.TipoOleo(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.TipoOleo tipoOleo = repTipoOleo.BuscarPorCodigo(codigo, true);

                if (tipoOleo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(tipoOleo, unitOfWork);

                unitOfWork.Start();

                repTipoOleo.Atualizar(tipoOleo, Auditado);

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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frotas.TipoOleo repTipoOleo = new Repositorio.Embarcador.Frotas.TipoOleo(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.TipoOleo tipoOleo = repTipoOleo.BuscarPorCodigo(codigo, true);

                if (tipoOleo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repTipoOleo.Deletar(tipoOleo, Auditado);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frotas.TipoOleo repTipoOleo = new Repositorio.Embarcador.Frotas.TipoOleo(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.TipoOleo tipoOleo = repTipoOleo.BuscarPorCodigo(codigo, false);

                if (tipoOleo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoOleo.Codigo,
                    tipoOleo.Descricao,
                    tipoOleo.TipoDeOleo,
                    Produto = new { Codigo = tipoOleo.Produto?.Codigo ?? 0, Descricao = tipoOleo.Produto?.Descricao ?? string.Empty },
                    tipoOleo.CodigoIntegracao
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

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Frotas.TipoOleo tipoOleo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

            int codigoProduto = Request.GetIntParam("Produto");
            string descricao = Request.GetStringParam("Descricao");
            string tipoDeOleo = Request.GetStringParam("TipoDeOleo");
            string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");

            tipoOleo.Descricao = !String.IsNullOrEmpty(descricao) ? descricao : throw new ControllerException("A descrição não pode ser nula ou vazia!");
            tipoOleo.TipoDeOleo = !String.IsNullOrEmpty(tipoDeOleo) ? tipoDeOleo : throw new ControllerException("O Tipo de Óleo não pode ser nulo ou vazio!");
            tipoOleo.Produto = codigoProduto > 0 ? repProduto.BuscarPorCodigo(codigoProduto) : throw new ControllerException("Produto não cadastrado!");
            tipoOleo.CodigoIntegracao = codigoIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoOleo ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoOleo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoOleo()
            {
                Descricao = Request.GetStringParam("Descricao"),
                TipoDeOleo = Request.GetStringParam("TipoDeOleo"),
                Produto = Request.GetIntParam("Produto")
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de Óleo", "TipoDeOleo", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Material", "Produto", 20, Models.Grid.Align.left, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frotas.TipoOleo reptipoOleo = new Repositorio.Embarcador.Frotas.TipoOleo(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoOleo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frotas.TipoOleo> listaTipoOleo = reptipoOleo.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = reptipoOleo.ContarConsulta(filtrosPesquisa);

            var dynListaTipoOleo = from tipo in listaTipoOleo
                                    select new
                                    {
                                        tipo.Codigo,
                                        tipo.Descricao,
                                        tipo.TipoDeOleo,
                                        Produto = tipo.Produto != null ? tipo.Produto.Descricao : string.Empty,
                                    };

            return dynListaTipoOleo.ToList();
        }

        #endregion
    }
}
