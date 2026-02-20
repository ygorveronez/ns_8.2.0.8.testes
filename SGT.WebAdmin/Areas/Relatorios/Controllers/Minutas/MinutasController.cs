using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Minutas
{
	[Area("Relatorios")]
	public class MinutasController : BaseController
    {
		#region Construtores

		public MinutasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Minutas/Minuta")]
        public async Task<IActionResult> Minuta()
        {
            return View();
        }
    }
}
