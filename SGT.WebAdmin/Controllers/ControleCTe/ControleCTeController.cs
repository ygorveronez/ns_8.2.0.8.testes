using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.ControleCTe
{
    [CustomAuthorize("ControleCTe/ControleEmissaoCTe")]
    public class ControleCTeController : BaseController
    {
		#region Construtores

		public ControleCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [CustomAuthorize("ControleCTe/ControleEmissaoCTe")]
        public async Task<IActionResult> ControleEmissaoCTe()
        {
            return View();
        }

        #endregion

    }
}
