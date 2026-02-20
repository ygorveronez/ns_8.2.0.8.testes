using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Containers
{
    public class ContainersController : BaseController
    {
		#region Construtores

		public ContainersController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Containers/ControleContainer")]
        public async Task<IActionResult> ControleContainer()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Containers/ControleContainer");
            ViewBag.PermissoesPersonalizadasControleContainer = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            
            return View();
        }
        
        [CustomAuthorize("Containers/MovimentacaoAreaContainer")]
        public async Task<IActionResult> MovimentacaoAreaContainer()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega");

            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);
            ViewBag.PermissoesPersonalizadasControleEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
           
            return View();
        }

        [CustomAuthorize("Containers/ContainerRedex")]
        public async Task<IActionResult> ContainerRedex()
        {
            return View();
        }

        [CustomAuthorize("Containers/JustificativaContainer")]
        public async Task<IActionResult> JustificativaContainer()
        {
            return View();
        }

        [CustomAuthorize("Containers/ConferenciaContainer")]
        public async Task<IActionResult> ConferenciaContainer()
        {
            return View();
        }
    }
}
