using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/UnidadeMedida")]
    public class UnidadeMedidaController : BaseController
    {
		#region Construtores

		public UnidadeMedidaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string sigla = Request.Params("Sigla");
                
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.UnidadeMedida.Sigla, "Sigla", 25, Models.Grid.Align.left, true);

                Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);

                List<Dominio.Entidades.UnidadeDeMedida> listaUnidadeMedida = repUnidadeMedida.Consultar(descricao, sigla, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repUnidadeMedida.ContarConsulta(descricao, sigla));

                var retorno = (from obj in listaUnidadeMedida
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.Sigla
                               }).ToList();

                grid.AdicionaRows(listaUnidadeMedida);

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

        #endregion
    }
}
