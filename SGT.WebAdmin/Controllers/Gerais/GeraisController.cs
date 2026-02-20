using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Gerais
{
    public class GeraisController : BaseController
    {
		#region Construtores

		public GeraisController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Gerais/Encoding")]
        public async Task<IActionResult> Encoding()
        {
            return View();
        }
    }
}
