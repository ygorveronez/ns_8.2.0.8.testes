using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.EDI
{
    public class EDIController : BaseController
    {
		#region Construtores

		public EDIController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("EDI/LayoutEDI")]
        public async Task<IActionResult> LayoutEDI()
        {
            return View();
        }
    }
}
