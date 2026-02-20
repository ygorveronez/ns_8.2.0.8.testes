using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Creditos
{
    public class CreditosController : BaseController
    {
		#region Construtores

		public CreditosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Creditos/HierarquiaSolicitacaoCredito")]
        public async Task<IActionResult> HierarquiaSolicitacaoCredito()
        {
            return View();
        }

        [CustomAuthorize("Creditos/CreditoDisponivel")]
        public async Task<IActionResult> CreditoDisponivel()
        {
            return View();
        }

        [CustomAuthorize("Creditos/CreditoLiberacao")]
        public async Task<IActionResult> CreditoLiberacao()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }
    }
}
