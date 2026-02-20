using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fechamento
{
    public class FechamentoController : BaseController
    {
		#region Construtores

		public FechamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Fechamento/FechamentoFrete")]
        public async Task<IActionResult> FechamentoFrete()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFechamento = ObterPermissoesPersonalizadas("Fechamento/FechamentoFrete");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");

            ViewBag.PermissoesPersonalizadasFechamento = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasFechamento);
            ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);

            return View();
        }


        [CustomAuthorize("Fechamento/FechamentoJustificativaAcrescimoDesconto")]
        public async Task<IActionResult> FechamentoJustificativaAcrescimoDesconto()
        {
            return View();
        }
    }
}

