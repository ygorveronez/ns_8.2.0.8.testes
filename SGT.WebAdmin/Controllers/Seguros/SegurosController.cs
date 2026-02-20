using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Seguros
{
    public class SegurosController : BaseController
    {
		#region Construtores

		public SegurosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Seguros/Seguradora")]
        public async Task<IActionResult> Seguradora()
        {
            return View();
        }

        [CustomAuthorize("Seguros/ApoliceSeguro")]
        public async Task<IActionResult> ApoliceSeguro()
        {
            return View();
        }

        [CustomAuthorize("Seguros/FechamentoAverbacao")]
        public async Task<IActionResult> FechamentoAverbacao()
        {
            return View();
        }
    }
}
