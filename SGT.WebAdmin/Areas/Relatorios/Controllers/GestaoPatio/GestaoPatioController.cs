using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.GestaoPatio
{
	[Area("Relatorios")]
	public class GestaoPatioController : BaseController
    {
		#region Construtores

		public GestaoPatioController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/GestaoPatio/TemposGestaoPatio")]
        public async Task<IActionResult> TemposGestaoPatio()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/GestaoPatio/FluxoHorario")]
        public async Task<IActionResult> FluxoHorario()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/GestaoPatio/GuaritaCheckList")]
        public async Task<IActionResult> GuaritaCheckList()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/GestaoPatio/ControleVisita")]
        public async Task<IActionResult> ControleVisita()
        {
            return View();
        }
        
        [CustomAuthorize("Relatorios/GestaoPatio/CheckList")]
        public async Task<IActionResult> CheckList()
        {
            return View();
        }
        
    }
}
