using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Atendimentos
{
    public class AtendimentosController : BaseController
    {
		#region Construtores

		public AtendimentosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Atendimentos/Atendimento")]
        public async Task<IActionResult> Atendimento()
        {
            return View();
        }

        [CustomAuthorize("Atendimentos/TipoAtendimento")]
        public async Task<IActionResult> TipoAtendimento()
        {
            return View();
        }

        [CustomAuthorize("Atendimentos/Sistema")]
        public async Task<IActionResult> Sistema()
        {
            return View();
        }

        [CustomAuthorize("Atendimentos/Modulo")]
        public async Task<IActionResult> Modulo()
        {
            return View();
        }

        [CustomAuthorize("Atendimentos/Tela")]
        public async Task<IActionResult> Tela()
        {
            return View();
        }

        [CustomAuthorize("Atendimentos/Chamado")]
        public async Task<IActionResult> Chamado()
        {
            return View();
        }
    }
}
