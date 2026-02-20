using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.RH
{
    public class RHController : BaseController
    {
		#region Construtores

		public RHController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("RH/ComissaoFuncionario")]
        public async Task<IActionResult> ComissaoFuncionario()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("RH/FolhaInformacao")]
        public async Task<IActionResult> FolhaInformacao()
        {
            return View();
        }

        [CustomAuthorize("RH/FolhaLancamento")]
        public async Task<IActionResult> FolhaLancamento()
        {
            return View();
        }

        [CustomAuthorize("RH/DiarioBordoSemanal")]
        public async Task<IActionResult> DiarioBordoSemanal()
        {
            return View();
        }

        [CustomAuthorize("RH/TabelaProdutividade")]
        public async Task<IActionResult> TabelaProdutividade()
        {
            return View();
        }

        [CustomAuthorize("RH/TabelaMediaModeloPeso")]
        public async Task<IActionResult> TabelaMediaModeloPeso()
        {
            return View();
        }

        [CustomAuthorize("RH/TabelaPremioProdutividade")]
        public async Task<IActionResult> TabelaPremioProdutividade()
        {
            return View();
        }        

    }
}
