using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Dash
{
    public class DashController : BaseController
    {
		#region Construtores

		public DashController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Dash/Documentacao")]
        public async Task<IActionResult> Documentacao()
        {
            return View();
        }
    }
}
