using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/TipoDeCarga", "Cargas/FaixaTemperatura")]
    public class TipoDeCargaController : BaseController
    {
		#region Construtores

		public TipoDeCargaController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Codigo", "CodigoEmbarcador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);

                Repositorio.TipoCarga repTipoCarga = new Repositorio.TipoCarga(unitOfWork);

                List<Dominio.Entidades.TipoCarga> tiposDeCarga = repTipoCarga.Consultar(descricao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoCarga.ContarConsulta(descricao));

                var retorno = (from obj in tiposDeCarga
                               select new
                               {
                                   obj.Codigo,
                                   obj.CodigoEmbarcador,
                                   obj.Descricao
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
    }
}
