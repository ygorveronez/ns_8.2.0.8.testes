using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.MDFe
{
	[Area("Relatorios")]
	public class MdfeController : BaseController
    {
		#region Construtores

		public MdfeController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Mdfe/Mdfes")]
        public async Task<IActionResult> Mdfes()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Mdfe/MDFesAverbados")]
        public async Task<IActionResult> MDFesAverbados()
        {
            return View();
        }
    }
}
