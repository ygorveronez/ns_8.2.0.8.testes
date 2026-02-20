using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Gerais
{
    [CustomAuthorize("Gerais/Encoding")]
    public class EncodingController : BaseController
    {
		#region Construtores

		public EncodingController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                System.Text.EncodingInfo[] encodingInfos = System.Text.Encoding.GetEncodings();

                return new JsonpResult(encodingInfos.OrderBy(o => o.Name).Select(o => new { value = o.Name, text = $"{o.Name.ToUpper()} ({o.DisplayName})" }).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os encodings disponíveis.");
            }
        }
    }
}
