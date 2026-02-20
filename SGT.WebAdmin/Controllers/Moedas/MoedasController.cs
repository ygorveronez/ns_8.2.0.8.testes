using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Moedas
{
    public class MoedasController : BaseController
    {
		#region Construtores

		public MoedasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Moedas/Cotacao")]
        public async Task<IActionResult> Cotacao()
        {
            return View();
        }

        [CustomAuthorize("Moedas/Moeda")]
        public async Task<IActionResult> Moeda()
        {
            return View();
        }
    }
}
