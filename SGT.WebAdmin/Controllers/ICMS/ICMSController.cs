using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ICMS
{
    public class ICMSController : BaseController
    {
		#region Construtores

		public ICMSController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("ICMS/SubstituicaoTributariaEstado")]
        public async Task<IActionResult> SubstituicaoTributariaEstado()
        {
            return View();
        }

        [CustomAuthorize("ICMS/RegraICMS")]
        public async Task<IActionResult> RegraICMS()
        {
            return View();
        }

        [CustomAuthorize("ICMS/AliquotaICMS")]
        public async Task<IActionResult> AliquotaICMS()
        {
            return View();
        }

        [CustomAuthorize("ICMS/RegraAutorizacaoAlteracaoRegraICMS")]
        public async Task<IActionResult> RegraAutorizacaoAlteracaoRegraICMS()
        {
            return View();
        }

        [CustomAuthorize("ICMS/AutorizacaoAlteracaoRegraICMS")]
        public async Task<IActionResult> AutorizacaoAlteracaoRegraICMS()
        {
            return View();
        }

        [CustomAuthorize("ICMS/CoeficientePautaFiscal")]
        public async Task<IActionResult> CoeficientePautaFiscal()
        {
            return View();
        }

        [CustomAuthorize("ICMS/PautaFiscal")]
        public async Task<IActionResult> PautaFiscal()
        {
            return View();
        }

        [CustomAuthorize("ICMS/RegraExtensao")]
        public async Task<IActionResult> RegraExtensao()
        {
            return View();
        }
    }
}
