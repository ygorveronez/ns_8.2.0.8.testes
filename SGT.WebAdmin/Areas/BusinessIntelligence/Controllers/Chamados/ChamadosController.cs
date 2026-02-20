using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Chamados
{
	[Area("BusinessIntelligence")]
	public class ChamadosController : BaseController
    {
		#region Construtores

		public ChamadosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("BusinessIntelligence/Chamados/Chamado")]
        public async Task<IActionResult> Chamado()
        {
            return View();
        }
    }
}
