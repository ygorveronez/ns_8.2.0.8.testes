using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frota
{
	[Area("Relatorios")]
	public class FrotaController : BaseController
    {
		#region Construtores

		public FrotaController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Frota/MovimentacaoPneuVeiculo")]
        public IActionResult MovimentacaoPneuVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frota/PneuHistorico")]
        public IActionResult PneuHistorico()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frota/OrdemServico")]
        public IActionResult OrdemServico()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frota/ServicoVeiculo")]
        public IActionResult ServicoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frota/Sinistro")]
        public IActionResult Sinistro()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frota/PneuCustoEstoque")]
        public IActionResult PneuCustoEstoque()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frota/Pneu")]
        public IActionResult Pneu()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frota/PlanejamentoFrotaDia")]
        public IActionResult PlanejamentoFrotaDia()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frota/PneuPorVeiculo")]
        public IActionResult PneuPorVeiculo()
        {
            return View();
        }
    }
}
