using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contatos
{
    public class ContatosController : BaseController
    {
		#region Construtores

		public ContatosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Contatos/TipoContato")]
        public async Task<IActionResult> TipoContato()
        {
            return View();
        }

        [CustomAuthorize("Contatos/SituacaoContato")]
        public async Task<IActionResult> SituacaoContato()
        {
            return View();
        }

        [CustomAuthorize("Contatos/ContatoCliente", "Faturas/Fatura", "Financeiros/Bordero", "Financeiros/Titulo")]
        public async Task<IActionResult> ContatoCliente()
        {
            return View();
        }

        [CustomAuthorize("Contatos/PessoaContato")]
        public async Task<IActionResult> PessoaContato()
        {
            return View();
        }
    }
}
