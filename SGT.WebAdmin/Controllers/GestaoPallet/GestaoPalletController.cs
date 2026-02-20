using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPallet
{
	public class GestaoPalletController : BaseController
	{
		#region Construtores

		public GestaoPalletController(Conexao conexao) : base(conexao) { }

		#endregion


		[CustomAuthorize("GestaoPallet/ControleSaldoPallet")]
		public async Task<IActionResult> ControleSaldoPallet()
		{
			return View();
		}

		[CustomAuthorize("GestaoPallet/AgendamentoPallet")]
		public ActionResult AgendamentoPallet()
		{
			return View();
		}

		[CustomAuthorize("GestaoPallet/ControlePallet")]
		public ActionResult ControlePallet()
		{
			return View();
		}

		[CustomAuthorize("GestaoPallet/AgendamentoColetaPallet")]
		public ActionResult AgendamentoColetaPallet()
		{
			return View();
		}

		[CustomAuthorize("GestaoPallet/ManutencaoPallet")]
		public async Task<IActionResult> ManutencaoPallet()
		{
			return await Task.FromResult(View());
		}
	}
}