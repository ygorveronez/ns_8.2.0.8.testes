using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/Banco")]
    public class BancoController : BaseController
    {
		#region Construtores

		public BancoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("descricao");
                int numero = 0;
                if (!string.IsNullOrWhiteSpace(Request.Params("Numero")))
                    numero = int.Parse(Request.Params("Numero"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 65, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Banco.Numero, "Numero", 25, Models.Grid.Align.center, false);

                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                List<Dominio.Entidades.Banco> listaBanco = repBanco.Consultar(numero, descricao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int countLayoutEDI = repBanco.ContarConsulta(numero, descricao);

                grid.setarQuantidadeTotal(countLayoutEDI);

                var retorno = (from obj in listaBanco
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.Numero
                               }).ToList();

                grid.AdicionaRows(listaBanco);

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
