using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Acerto
{
    public class AcertosController : BaseController
    {
		#region Construtores

		public AcertosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Acertos/AcertoViagem")]
        public async Task<IActionResult> AcertoViagem()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Acertos/TipoBonificacao")]
        public async Task<IActionResult> TipoBonificacao()
        {
            return View();
        }
        [CustomAuthorize("Acertos/TipoDespesa")]
        public async Task<IActionResult> TipoDespesa()
        {
            return View();
        }

        [CustomAuthorize("Acertos/TabelaDiaria")]
        public async Task<IActionResult> TabelaDiaria()
        {
            return View();
        }

        [CustomAuthorize("Acertos/TabelaComissaoMotorista")]
        public async Task<IActionResult> TabelaComissaoMotorista()
        {
            return View();
        }        
    }
}


