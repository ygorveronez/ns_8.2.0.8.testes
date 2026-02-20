using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.GerenciamentoIrregularidades
{
	[Area("Relatorios")]
	public class GerenciamentoIrregularidadesController : BaseController
    {
		#region Construtores

		public GerenciamentoIrregularidadesController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/GerenciamentoIrregularidades/ModuloControle")]
        public async Task<IActionResult> ModuloControle()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/GerenciamentoIrregularidades/ProcessamentoModuloControle")]
        public async Task<IActionResult> ProcessamentoModuloControle()
        {
            return View();
        }
    }
}
