using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Sefaz
{
    [CustomAuthorize("Sefaz/Sefaz")]
    public class SefazController : BaseController
    {
		#region Construtores

		public SefazController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Descricao", 60, Models.Grid.Align.left, true);

                string propOrdeno = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Sefaz repSefaz = new Repositorio.Sefaz(unitOfWork);

                List<Dominio.Entidades.Sefaz> sefaz = repSefaz.Consulta(descricao, propOrdeno, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSefaz.ContarConsulta(descricao));

                var lista = (from obj in sefaz
                             select new
                             {
                                 obj.Codigo,
                                 Descricao = obj.Descricao
                             }).ToList();

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

        #endregion
    }
}
