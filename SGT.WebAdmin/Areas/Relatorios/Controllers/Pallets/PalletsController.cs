using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pallets
{
	[Area("Relatorios")]
	public class PalletsController : Controller
    {
        [CustomAuthorize("Relatorios/Pallets/ValorDescarga")]
        public async Task<IActionResult> ValorDescarga()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/DevolucaoPallets")]
        public async Task<IActionResult> DevolucaoPallets()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/EstoqueFilial")]
        public async Task<IActionResult> EstoqueFilial()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/EstoqueTransportador")]
        public async Task<IActionResult> EstoqueTransportador()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/FechamentoTransportador")]
        public async Task<IActionResult> FechamentoTransportador()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/EstoqueCompraPallet")]
        public async Task<IActionResult> EstoqueCompraPallet()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/ControleReformaPallet")]
        public async Task<IActionResult> ControleReformaPallet()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/ControleTransferenciaPallet")]
        public async Task<IActionResult> ControleTransferenciaPallet()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/ControleValePallet")]
        public async Task<IActionResult> ControleValePallet()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/ControleEntradaSaidaPallet")]
        public async Task<IActionResult> ControleEntradaSaidaPallet()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/ControleAvariaPallet")]
        public async Task<IActionResult> ControleAvariaPallet()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/EstoqueCliente")]
        public async Task<IActionResult> EstoqueCliente()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pallets/TaxasDescarga")]
        public async Task<IActionResult> TaxasDescarga()
        {
            return View();
        }
    }
}