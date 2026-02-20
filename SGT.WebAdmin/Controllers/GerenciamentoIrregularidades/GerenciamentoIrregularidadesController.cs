using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GerenciamentoIrregularidades
{
    public class GerenciamentoIrregularidadesController : BaseController
    {
		#region Construtores

		public GerenciamentoIrregularidadesController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("GerenciamentoIrregularidades/PortfolioModuloControle")]
        public async Task<IActionResult> PortfolioModuloControle()
        {
            return View();
        }

        [CustomAuthorize("GerenciamentoIrregularidades/Irregularidade")]
        public async Task<IActionResult> Irregularidade()
        {
            return View();
        }

        [CustomAuthorize("GerenciamentoIrregularidades/MotivosIrregularidades")]
        public async Task<IActionResult> MotivosIrregularidades()
        {
            return View();
        }

        [CustomAuthorize("GerenciamentoIrregularidades/MotivoDesacordo")]
        public async Task<IActionResult> MotivoDesacordo()
        {
            return View();
        }

        [CustomAuthorize("GerenciamentoIrregularidades/DefinicaoTratativasIrregularidade")]
        public async Task<IActionResult> DefinicaoTratativasIrregularidade()
        {
            return View();
        }
    }
}
