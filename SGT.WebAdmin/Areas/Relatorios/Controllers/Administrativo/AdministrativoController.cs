using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Administrativo
{
	[Area("Relatorios")]
	public class AdministrativoController : BaseController
    {
		#region Construtores

		public AdministrativoController(Conexao conexao) : base(conexao) { }

		#endregion

    
        [CustomAuthorize("Relatorios/Administrativo/LogEnvioEmail")]
        public async Task<IActionResult> LogEnvioEmail()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Administrativo/Licenca")]
        public async Task<IActionResult> Licenca()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Administrativo/LogEnvioSMS")]
        public async Task<IActionResult> LogEnvioSMS()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Administrativo/LicencaVeiculo")]
        public async Task<IActionResult> LicencaVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Administrativo/LogAcesso")]
        public async Task<IActionResult> LogAcesso()
        {
            return View();
        }
    }
}
