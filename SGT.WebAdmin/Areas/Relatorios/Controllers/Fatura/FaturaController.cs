using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fatura
{
    public class FaturaController : BaseController
    {
		#region Construtores

		public FaturaController(Conexao conexao) : base(conexao) { }

		#endregion

		[Area("Relatorios")]
		[CustomAuthorize("Faturas/Fatura")]
        public async Task<IActionResult> FaturaRelatorio()
        {
            return View();
        }
    }
}
