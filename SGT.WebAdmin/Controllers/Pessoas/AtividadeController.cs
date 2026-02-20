using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Atividade")]
    public class AtividadeController : BaseController
    {
		#region Construtores

		public AtividadeController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 75, Models.Grid.Align.left, true);

                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                List<Dominio.Entidades.Atividade> listaAtividade = repAtividade.Consultar(descricao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repAtividade.ContarConsulta(descricao);
                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaAtividade select new { Codigo = p.Codigo, Descricao = p.Descricao }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
