using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.MDFe
{
    [AllowAnonymous]
    public class MDFeController : BaseController
    {
		#region Construtores

		public MDFeController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAnonymous]
        public async Task<IActionResult> EncerramentoTransportador()
        {
            ViewBag.Titulo = "Multi Embarcador";
            ViewBag.Logo = "../img/Logos/LogoEmbarcadorCinza.png";
            ViewBag.Layout = "smart-style-0";
            ViewBag.CorFundoUsuario = "";
            ViewBag.Favicon = "../img/favicon/favicon.ico?v=3";
            
            return View();
        }

        [CustomAuthorize("MDFe/Encerramento")]
        public async Task<IActionResult> Encerramento()
        {
            return View();
        }

        [CustomAuthorize("MDFe/ConsultaMDFe")]
        public async Task<IActionResult> ConsultaMDFe()
        {
            return View();
        }

        [CustomAuthorize("MDFe/ControleEmissaoMDFe")]
        public async Task<IActionResult> ControleEmissaoMDFe()
        {
            return View();
        }

        [CustomAuthorize("MDFe/EncerramentoTMS")]
        public async Task<IActionResult> EncerramentoTMS()
        {
            return View();
        }
    }
}
