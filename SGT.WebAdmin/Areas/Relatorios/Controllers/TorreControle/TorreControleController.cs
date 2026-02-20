using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.TorreControle
{
    [Area("Relatorios")]
    public class TorreControleController : BaseController
    {
        #region Construtores

        public TorreControleController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Relatorios/TorreControle/ConsultaPorNotaFiscal")]
        public async Task<IActionResult> ConsultaPorNotaFiscal()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/TorreControle/ConsolidadoEntregas")]
        public async Task<IActionResult> ConsolidadoEntregas()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/TorreControle/Permanencias")]
        public async Task<IActionResult> Permanencias()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/TorreControle/DevolucaoNotasFiscais")]
        public async Task<IActionResult> DevolucaoNotasFiscais()
        {
            return View();
        }
    }
}
