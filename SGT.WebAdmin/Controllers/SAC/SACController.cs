using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.SAC
{
    public class SACController : BaseController
    {
		#region Construtores

		public SACController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("SAC/AtendimentoCliente")]
        public async Task<IActionResult> AtendimentoCliente()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("SAC/AtendimentoCliente");
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFatura = ObterPermissoesPersonalizadas("SAC/AtendimentoCliente");
            ViewBag.PermissoesPersonalizadasFatura = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasFatura);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");
            ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);

            return View();
        }
    }
}
