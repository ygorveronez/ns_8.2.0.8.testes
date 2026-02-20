using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFe
{
    public class NFeController : BaseController
    {
		#region Construtores

		public NFeController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("NFe/GerarCTePorNFe")]
        public async Task<IActionResult> GerarCTePorNFe()
        {
            return View();
        }

        [CustomAuthorize("NFe/ConsultaSiteSEFAZ")]
        public async Task<IActionResult> ConsultaSiteSEFAZ()
        {
            return View();
        }


        [CustomAuthorize("NFe/UploadNotaXML")]
        public async Task<IActionResult> UploadNotaXML()
        {
            return View();
        }

        [CustomAuthorize("NFe/PainelNFeTransporte")]
        public IActionResult PainelNFeTransporte()
        {
            return View();
        }
    }
}
