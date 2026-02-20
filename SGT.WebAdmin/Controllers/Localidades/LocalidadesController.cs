using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Localidades
{
    public class LocalidadesController : BaseController
    {
		#region Construtores

		public LocalidadesController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Localidades/Regiao")]
        public async Task<IActionResult> Regiao()
        {
            return View();
        }

        [CustomAuthorize("Localidades/Estado")]
        public async Task<IActionResult> Estado()
        {
            return View();
        }

        [CustomAuthorize("Localidades/Localidade")]
        public async Task<IActionResult> Localidade()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> AlterarSenha()
        {
            return View();
        }

        [CustomAuthorize("Localidades/MesoRegiao")]
        public async Task<IActionResult> MesoRegiao()
        {
            return View();
        }

        [CustomAuthorize("Localidades/Pais")]
        public async Task<IActionResult> Pais()
        {
            return View();
        }

        [CustomAuthorize("Localidades/RegiaoBrasil")]
        public async Task<IActionResult> RegiaoBrasil()
        {
            return View();
        }

        [CustomAuthorize("Localidades/DistribuidorPorRegiao")]
        public async Task<IActionResult> DistribuidorPorRegiao()
        {
            return View();
        }
    }
}
