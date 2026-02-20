using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Servico
{
    public class ServicoController : BaseController
    {
		#region Construtores

		public ServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Servico/FTP")]
        public async Task<IActionResult> FTP()
        {
            return View();
        }
    }
}
