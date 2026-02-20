using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscalServico
{
    public class NotasFiscaisServicosController : BaseController
    {
		#region Construtores

		public NotasFiscaisServicosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("NotasFiscaisServicos/NotaFiscalServico")]
        public async Task<IActionResult> NotaFiscalServico()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscaisServicos/ControleEmissaoNFSe")]
        public async Task<IActionResult> ControleEmissaoNFSe()
        {
            return View();
        }
    }
}
