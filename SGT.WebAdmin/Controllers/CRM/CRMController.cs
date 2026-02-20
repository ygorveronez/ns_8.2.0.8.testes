using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CRM
{
    public class CRMController : BaseController
    {
		#region Construtores

		public CRMController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("CRM/Prospeccao")]
        public async Task<IActionResult> Prospeccao()
        {
            return View();
        }

        [CustomAuthorize("CRM/ClienteProspect")]
        public async Task<IActionResult> ClienteProspect()
        {
            return View();
        }

        [CustomAuthorize("CRM/OrigemContatoClienteProspect")]
        public async Task<IActionResult> OrigemContatoClienteProspect()
        {
            return View();
        }

        [CustomAuthorize("CRM/ProdutoProspect")]
        public async Task<IActionResult> ProdutoProspect()
        {
            return View();
        }
    }
}
