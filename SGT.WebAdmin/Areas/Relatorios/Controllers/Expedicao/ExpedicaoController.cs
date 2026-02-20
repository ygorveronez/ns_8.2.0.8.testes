using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Expedicao
{
    public class ExpedicaoController : BaseController
    {
		#region Construtores

		public ExpedicaoController(Conexao conexao) : base(conexao) { }

		#endregion

		[Area("Relatorios")]
		[CustomAuthorize("Relatorios/Expedicao/ExpedicaoProdutos")]
        public async Task<IActionResult> ExpedicaoProdutos()
        {
            return View();
        }
    }
}
