using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Rateios
{
    public class RateiosController : BaseController
    {
		#region Construtores

		public RateiosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Rateios/RateioFormula")]
        public async Task<IActionResult> RateioFormula()
        {
            return View();
        }
    }
}
