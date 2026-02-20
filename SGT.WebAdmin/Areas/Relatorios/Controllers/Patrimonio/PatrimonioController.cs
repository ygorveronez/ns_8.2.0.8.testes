using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Patrimonio
{
	[Area("Relatorios")]
	public class PatrimonioController : BaseController
    {
		#region Construtores

		public PatrimonioController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Patrimonio/Bem")]
        public async Task<IActionResult> Bem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Patrimonio/MapaDepreciacao")]
        public async Task<IActionResult> MapaDepreciacao()
        {
            return View();
        }
    }
}
