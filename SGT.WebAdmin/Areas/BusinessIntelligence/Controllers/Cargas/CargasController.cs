using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Cargas
{
	[Area("BusinessIntelligence")]
	public class CargasController : Controller
    {
        [CustomAuthorize("BusinessIntelligence/Cargas/Quantidade")]
        public async Task<IActionResult> Quantidade()
        {
            return View();
        }

        [CustomAuthorize("BusinessIntelligence/Cargas/DirecionamentoOperador")]
        public async Task<IActionResult> DirecionamentoOperador()
        {
            return View();
        }

        [CustomAuthorize("BusinessIntelligence/Cargas/ValorMedioFrete")]
        public async Task<IActionResult> ValorMedioFrete()
        {
            return View();
        }

        [CustomAuthorize("BusinessIntelligence/Cargas/IndiceAtraso")]
        public async Task<IActionResult> IndiceAtraso()
        {
            return View();
        }

        [CustomAuthorize("BusinessIntelligence/Cargas/FaturamentoTransportador")]
        public async Task<IActionResult> FaturamentoTransportador()
        {
            return View();
        }

        [CustomAuthorize("BusinessIntelligence/Cargas/QuantidadePorRota")]
        public async Task<IActionResult> QuantidadePorRota()
        {
            return View();
        }

    }
}