using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Canhotos
{
	[Area("Relatorios")]
	public class CanhotosController : BaseController
    {
		#region Construtores

		public CanhotosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Canhotos/Canhoto")]
        public IActionResult Canhoto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Canhotos/HistoricoMovimentacaoCanhoto")]
        public IActionResult HistoricoMovimentacaoCanhoto()
        {
            return View();
        }

    }
}
