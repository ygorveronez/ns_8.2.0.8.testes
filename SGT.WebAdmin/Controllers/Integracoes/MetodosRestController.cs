using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    public class MetodosRestController : BaseController
    {
		#region Construtores

		public MetodosRestController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos
        public async Task<IActionResult> PequisarMetodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.WebService.MetodosRest repositorioMetodosRest = new Repositorio.WebService.MetodosRest(unitOfWork);

                string nomeMetodo = Request.GetStringParam("Nome");

                List<Dominio.Entidades.WebService.MetodosRest> metodosRest = repositorioMetodosRest.Buscar(nomeMetodo);

                Models.Grid.Grid grid = ObterGrid();
                grid.AdicionaRows(metodosRest);
                grid.setarQuantidadeTotal(metodosRest.Count);

                return new JsonpResult(grid);
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                return new JsonpResult(false, exe.Message);
            }
        }
        #endregion

        #region Metodos Privados
        public Models.Grid.Grid ObterGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nome Metodo", "NomeMetodo", 10, Models.Grid.Align.left, true);
            return grid;
        }
        #endregion
    }
}
