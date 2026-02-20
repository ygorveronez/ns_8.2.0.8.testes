using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BoletoRetornoComando")]
    public class BoletoRetornoComandoController : BaseController
    {
		#region Construtores

		public BoletoRetornoComandoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string comando = Request.Params("Comando");
                string descricao = Request.Params("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Comando", "Comando", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Financeiro.BoletoRetornoComando repBoletoRetornoComando = new Repositorio.Embarcador.Financeiro.BoletoRetornoComando(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando> listaRetornoComando = repBoletoRetornoComando.Consultar(comando, descricao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBoletoRetornoComando.ContarConsulta(comando, descricao));
                var lista = (from p in listaRetornoComando
                             select new
                             {
                                 p.Codigo,
                                 p.Comando,
                                 p.Descricao
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }
    }
}
