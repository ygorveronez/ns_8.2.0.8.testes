
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers
{
	[Area("Relatorios")]
	public class FiliaisController : BaseController
    {
		#region Construtores

		public FiliaisController(Conexao conexao) : base(conexao) { }

		#endregion

        // GET: Relatorios/Filiais
        public async Task<IActionResult> Filial()
        {
            return View();
        }
    }
}
