using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.ControleNFSe
{
    [CustomAuthorize("ControleNFSe/ControleEmissaoNFSe")]
    public class ControleNFSeController : BaseController
    {
		#region Construtores

		public ControleNFSeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [CustomAuthorize("ControleNFSe/ControleEmissaoNFSe")]
        public async Task<IActionResult> ControleEmissaoNFSe()
        {
            return View();
        }

        #endregion

    }
}
