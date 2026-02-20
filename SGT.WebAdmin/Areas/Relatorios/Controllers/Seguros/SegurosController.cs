using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;


namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Seguros
{
	[Area("Relatorios")]
	public class SegurosController : BaseController
    {
		#region Construtores

		public SegurosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Seguros/CTesAverbados")]
        public async Task<IActionResult> CTesAverbados()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Seguros/Apolices")]
        public async Task<IActionResult> Apolices()
        {
            return View();
        }
    }
}
