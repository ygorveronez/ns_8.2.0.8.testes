using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.AcertoViagem
{
	[Area("Relatorios")]
	public class AcertoViagemController : BaseController
    {
		#region Construtores

		public AcertoViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Acertos/AcertoViagem")]
        public async Task<IActionResult> AcertoFechamentoRelatorio()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/ResultadoAcertoViagem")]
        public async Task<IActionResult> ResultadoAcertoViagem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/DespesaAcertoViagem")]
        public async Task<IActionResult> DespesaAcertoViagem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/AcertoDeViagem")]
        public async Task<IActionResult> AcertoDeViagem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/ResultadoAnualAcertoViagem")]
        public async Task<IActionResult> ResultadoAnualAcertoViagem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/CargaCompartilhada")]
        public async Task<IActionResult> CargaCompartilhada()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/DiariaAcertoViagem")]
        public async Task<IActionResult> DiariaAcertoViagem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/ComissaoAcertoViagem")]
        public async Task<IActionResult> ComissaoAcertoViagem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/UltimoAcertoMotorista")]
        public async Task<IActionResult> UltimoAcertoMotorista()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/TempoDeViagem")]
        public async Task<IActionResult> TempoDeViagem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/JornadaMotorista")]
        public async Task<IActionResult> JornadaMotorista()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/AcertoViagem/BonificacaoAcertoViagem")]
        public async Task<IActionResult> BonificacaoAcertoViagem()
        {
            return View();
        }

    }
}
