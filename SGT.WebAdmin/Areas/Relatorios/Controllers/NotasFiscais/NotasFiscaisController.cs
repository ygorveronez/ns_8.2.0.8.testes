using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
	[Area("Relatorios")]
	public class NotasFiscaisController : BaseController
    {
		#region Construtores

		public NotasFiscaisController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/NotasFiscais/ItemNaoConformidade")]
        public async Task<IActionResult> ItemNaoConformidade()
        {
            return View();
        }
    }
}
