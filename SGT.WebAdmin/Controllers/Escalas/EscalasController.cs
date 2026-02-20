using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escalas
{
    public class EscalasController : BaseController
    {
		#region Construtores

		public EscalasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Escalas/GerarEscala")]
        public async Task<IActionResult> GerarEscala()
        {
            return View();
        }

        [CustomAuthorize("Escalas/Escala")]
        public async Task<IActionResult> Escala()
        {
            return View();
        }

        [CustomAuthorize("Escalas/MotivoRemocaoVeiculoEscala")]
        public async Task<IActionResult> MotivoRemocaoVeiculoEscala()
        {
            return View();
        }

        [CustomAuthorize("Escalas/EscalaVeiculo")]
        public async Task<IActionResult> EscalaVeiculo()
        {
            return View();
        }
    }
}

