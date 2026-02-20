using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Planejamentos
{
    public class PlanejamentosController : BaseController
    {
		#region Construtores

		public PlanejamentosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Planejamentos/PlanejamentoFrota")]
        public async Task<IActionResult> PlanejamentoFrota()
        {
            return View();
        }
    }
}
