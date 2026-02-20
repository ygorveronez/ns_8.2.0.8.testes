using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Patrimonio
{
    public class PatrimonioController : BaseController
    {
		#region Construtores

		public PatrimonioController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Patrimonio/Bem")]
        public async Task<IActionResult> Bem()
        {
            return View();
        }

        [CustomAuthorize("Patrimonio/ManutencaoBem")]
        public async Task<IActionResult> ManutencaoBem()
        {
            return View();
        }

        [CustomAuthorize("Patrimonio/BaixaBem")]
        public async Task<IActionResult> BaixaBem()
        {
            return View();
        }

        [CustomAuthorize("Patrimonio/TransferenciaBem")]
        public async Task<IActionResult> TransferenciaBem()
        {
            return View();
        }

        [CustomAuthorize("Patrimonio/MotivoDefeito")]
        public async Task<IActionResult> MotivoDefeito()
        {
            return View();
        }

		[CustomAuthorize("Patrimonio/PlanoServico")]
		public async Task<IActionResult> PlanoServico()
		{
			return View();
		}

		[CustomAuthorize("Patrimonio/Pet")]
		public async Task<IActionResult> Pet()
		{
			return View();
		}
	}
}
