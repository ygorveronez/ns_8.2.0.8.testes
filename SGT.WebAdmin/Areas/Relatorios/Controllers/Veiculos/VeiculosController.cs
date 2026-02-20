using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Veiculos
{
	[Area("Relatorios")]
	public class VeiculosController : Controller
    {
        [CustomAuthorize("Relatorios/Veiculos/ClassificacaoVeiculo")]
        public IActionResult ClassificacaoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Veiculos/Veiculo")]
        public IActionResult Veiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Veiculos/Manutencao")]
        public IActionResult Manutencao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Veiculos/ReceitaDespesa")]
        public IActionResult ReceitaDespesa()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Veiculos/ResponsavelVeiculo")]
        public IActionResult ResponsavelVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Veiculos/Equipamento")]
        public IActionResult Equipamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Veiculos/HistoricoVeiculoVinculo")]
        public IActionResult HistoricoVeiculoVinculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Veiculos/Tacografo")]
        public IActionResult Tacografo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Veiculos/HistoricoMotoristaCentro")]
        public IActionResult HistoricoMotoristaCentro()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Veiculos/HistoricoVeiculo")]
        public IActionResult HistoricoVeiculo()
        {
            return View();
        }
    }
}