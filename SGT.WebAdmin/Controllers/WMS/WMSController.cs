using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    public class WMSController : BaseController
    {
		#region Construtores

		public WMSController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("WMS/Deposito")]
        public async Task<IActionResult> Deposito()
        {
            return View();
        }

        [CustomAuthorize("WMS/TransferenciaMercadoria")]
        public async Task<IActionResult> TransferenciaMercadoria()
        {
            return View();
        }

        [CustomAuthorize("WMS/RecebimentoMercadoria")]
        public async Task<IActionResult> RecebimentoMercadoria()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("WMS/RecebimentoMercadoria");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("WMS/RegraDescarteLoteProduto")]
        public async Task<IActionResult> RegraDescarteLoteProduto()
        {
            return View();
        }

        [CustomAuthorize("WMS/DescarteLoteProduto")]
        public async Task<IActionResult> DescarteLoteProduto()
        {
            return View();
        }
        
        [CustomAuthorize("WMS/MontagemContainer")]
        public async Task<IActionResult> MontagemContainer()
        {
            return View();
        }
        
        [CustomAuthorize("WMS/AutorizacaoDescarteLote")]
        public async Task<IActionResult> AutorizacaoDescarteLote()
        {
            return View();
        }

        [CustomAuthorize("WMS/SeparacaoWMS")]
        public async Task<IActionResult> SeparacaoWMS()
        {
            return View();
        }

        [CustomAuthorize("WMS/SelecaoWMS")]
        public async Task<IActionResult> SelecaoWMS()
        {
            return View();
        }

        [CustomAuthorize("WMS/SeparacaoMercadorias")]
        public async Task<IActionResult> SeparacaoMercadorias()
        {
            return View();
        }

        [CustomAuthorize("WMS/SeparacaoPedido")]
        public async Task<IActionResult> SeparacaoPedido()
        {
            return View();
        }
    }
}
