using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Localize
{
    [AllowAuthenticate]
    public class LocalizeController : BaseController
    {
		#region Construtores

		public LocalizeController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> GetResources()
        {
            try
            {
                string[] resources = Request.GetArrayParam<string>("Resources");

                IDictionary<string, string> resourceObject = Localization.Service.JSON.GetJSONResourceObjectDictionary(resources);

                return new JsonpResult(resourceObject);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Failed to load the resource files.");
            }
        }
    }
}
