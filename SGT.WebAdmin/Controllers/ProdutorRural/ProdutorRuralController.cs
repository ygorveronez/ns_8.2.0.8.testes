using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ProdutorRural
{
    public class ProdutorRuralController : BaseController
    {
		#region Construtores

		public ProdutorRuralController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("ProdutorRural/FechamentoColetaProdutor")]
        public async Task<IActionResult> FechamentoColetaProdutor()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

    }
}
