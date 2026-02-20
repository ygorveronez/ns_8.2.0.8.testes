using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Ocorrencias
{
	[Area("Relatorios")]
	public class OcorrenciasController : BaseController
    {
		#region Construtores

		public OcorrenciasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Ocorrencias/Ocorrencia")]
        public async Task<IActionResult> Ocorrencia()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Ocorrencias/OcorrenciaEntrega")]
        public async Task<IActionResult> OcorrenciaEntrega()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Ocorrencias/TipoOcorrencia")]
        public async Task<IActionResult> TipoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Ocorrencias/RegrasAutorizacaoOcorrencia")]
        public async Task<IActionResult> RegrasAutorizacaoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Ocorrencias/OcorrenciaCentroCusto")]
        public async Task<IActionResult> OcorrenciaCentroCusto()
        {
            return View();
        }
    }
}
