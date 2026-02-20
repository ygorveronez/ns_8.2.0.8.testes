
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Avarias
{
	[Area("Relatorios")]
	public class AvariasController : BaseController
    {
		#region Construtores

		public AvariasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Avarias/Analitico")]
        public async Task<IActionResult> Analitico()
        {
            return View();
        }
    }
}
