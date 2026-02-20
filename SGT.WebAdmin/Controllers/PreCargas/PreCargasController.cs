using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.PreCargas
{
    public class PreCargasController : BaseController
    {
		#region Construtores

		public PreCargasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("PreCargas/PreCarga")]
        public async Task<IActionResult> PreCarga()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasPreCarga = ObterPermissoesPersonalizadas("PreCargas/PreCarga");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

            ViewBag.PermissoesPersonalizadasPreCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasPreCarga);
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);

            return View();
        }

        [CustomAuthorize("PreCargas/PreCargaTransportador")]
        public async Task<IActionResult> PreCargaTransportador()
        {
            return View();
        }
    }
}
