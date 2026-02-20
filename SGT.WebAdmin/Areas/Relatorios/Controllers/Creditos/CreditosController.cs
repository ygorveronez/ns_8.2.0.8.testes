using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;


namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Creditos
{
	[Area("Relatorios")]
	public class CreditosController : BaseController
    {
		#region Construtores

		public CreditosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Creditos/CargaComplementosFrete")]
        public async Task<IActionResult> CargaComplementosFrete()
        {
            return View();
        }
    }
}
