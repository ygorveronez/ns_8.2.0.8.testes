using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Operacional
{
    public class OperacionalController : BaseController
    {
		#region Construtores

		public OperacionalController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Operacional/ConfigOperador")]
        public async Task<IActionResult> ConfigOperador()
        {
            return View();
        }

        [CustomAuthorize("Operacional/RestricaoVisualizacaoCanhotos")]
        public async Task<IActionResult> RestricaoVisualizacaoCanhotos()
        {
            return View();
        }
    }
}
