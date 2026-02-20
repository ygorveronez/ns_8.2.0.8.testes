using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscalConsumidor
{
    public class NotasFiscaisConsumidoresController : BaseController
    {
		#region Construtores

		public NotasFiscaisConsumidoresController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("NotasFiscaisConsumidores/NotaFiscalConsumidor")]
        public async Task<IActionResult> NotaFiscalConsumidor()
        {
            return View();
        }
    }
}
