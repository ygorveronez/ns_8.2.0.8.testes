using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [AllowAnonymous]
    public class NotificacoesController : BaseController
    {
		#region Construtores

		public NotificacoesController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAnonymous]
        public async Task<IActionResult> Notificacao()
        {
            return View();
        }

        [CustomAuthorize("Notificacoes/MensagemAviso")]
        public async Task<IActionResult> MensagemAviso()
        {
            return View();
        }

        [CustomAuthorize("Notificacoes/Chat")]
        public async Task<IActionResult> Chat()
        {
            return View();
        }

        [CustomAuthorize("Notificacoes/ConfiguracaoAlerta")]
        public async Task<IActionResult> ConfiguracaoAlerta()
        {
            return View();
        }

        [CustomAuthorize("Notificacoes/AlertaEmail")]
        public async Task<IActionResult> AlertaEmail()
        {
            return View();
        }

        [CustomAuthorize("Notificacoes/ConfiguracaoNCPendente")]
        public async Task<IActionResult> ConfiguracaoNCPendente()
        {
            return View();
        }
    }
}
