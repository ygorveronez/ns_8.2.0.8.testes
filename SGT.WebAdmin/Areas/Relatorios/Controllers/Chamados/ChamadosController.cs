using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Chamados
{
	[Area("Relatorios")]
	public class ChamadosController : BaseController
    {
		#region Construtores

		public ChamadosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Chamados/ChamadoOcorrencia")]
        public async Task<IActionResult> ChamadoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Chamados/ChamadoDevolucao")]
        public async Task<IActionResult> ChamadoDevolucao()
        {
            return View();
        }
    }
}
