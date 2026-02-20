using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pessoas
{
	[Area("Relatorios")]
	public class PessoasController : Controller
    {
        [CustomAuthorize("Relatorios/Pessoas/PerfilAcesso")]
        public async Task<IActionResult> PerfilAcesso()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pessoas/Pessoa")]
        public async Task<IActionResult> Pessoa()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pessoas/FuncionarioComissao")]
        public async Task<IActionResult> FuncionarioComissao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pessoas/ColaboradorSituacaoLancamento")]
        public async Task<IActionResult> ColaboradorSituacaoLancamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pessoas/PessoaDescarga")]
        public async Task<IActionResult> PessoaDescarga()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pessoas/GrupoPessoas")]
        public async Task<IActionResult> GrupoPessoas()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pessoas/EnderecoSecundario")]
        public async Task<IActionResult> EnderecoSecundario()
        {
            return View();
        }
    }
}