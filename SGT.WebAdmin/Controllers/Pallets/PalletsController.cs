using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    public class PalletsController : BaseController
    {
		#region Construtores

		public PalletsController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Pallets/AjusteSaldo")]
        public async Task<IActionResult> AjusteSaldo()
        {
            return View();
        }

        [CustomAuthorize("Pallets/Devolucao")]
        public async Task<IActionResult> Devolucao()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pallets/Devolucao");
            ViewBag.PermissoesPersonalizadasPallets = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Pallets/EstoqueFilial")]
        public async Task<IActionResult> EstoqueFilial()
        {
            return View();
        }

        [CustomAuthorize("Pallets/MotivoAvariaPallet")]
        public async Task<IActionResult> MotivoAvariaPallet()
        {
            return View();
        }

        [CustomAuthorize("Pallets/SituacaoDevolucao")]
        public async Task<IActionResult> SituacaoDevolucao()
        {
            return View();
        }

        [CustomAuthorize("Pallets/Transferencia")]
        public async Task<IActionResult> Transferencia()
        {
            return View();
        }

        [CustomAuthorize("Pallets/RegraAutorizacaoTransferencia")]
        public async Task<IActionResult> RegraAutorizacaoTransferencia()
        {
            return View();
        }

        [CustomAuthorize("Pallets/AutorizacaoTransferencia")]
        public async Task<IActionResult> AutorizacaoTransferencia()
        {
            return View();
        }

        [CustomAuthorize("Pallets/Avaria")]
        public async Task<IActionResult> Avaria()
        {
            return View();
        }

        [CustomAuthorize("Pallets/RegraAutorizacaoAvaria")]
        public async Task<IActionResult> RegraAutorizacaoAvaria()
        {
            return View();
        }

        [CustomAuthorize("Pallets/AutorizacaoAvariaPallet")]
        public async Task<IActionResult> AutorizacaoAvariaPallet()
        {
            return View();
        }

        [CustomAuthorize("Pallets/CompraPallets")]
        public async Task<IActionResult> CompraPallets()
        {
            return View();
        }

        [CustomAuthorize("Pallets/Reforma")]
        public async Task<IActionResult> Reforma()
        {
            return View();
        }

        [CustomAuthorize("Pallets/ValePallet", "Chamados/ChamadoOcorrencia", "Cargas/ControleEntrega", "GestaoPatio/FluxoPatio", "Cargas/GestaoDevolucao")]
        public async Task<IActionResult> ValePallet()
        {
            return View();
        }

        [CustomAuthorize("Pallets/DevolucaoValePallet")]
        public async Task<IActionResult> DevolucaoValePallet()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pallets/DevolucaoValePallet");
            ViewBag.PermissoesPersonalizadasPallets = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Pallets/RegraAutorizacaoDevolucaoValePallet")]
        public async Task<IActionResult> RegraAutorizacaoDevolucaoValePallet()
        {
            return View();
        }

        [CustomAuthorize("Pallets/AutorizacaoDevolucaoValePallet")]
        public async Task<IActionResult> AutorizacaoDevolucaoValePallet()
        {
            return View();
        }

        [CustomAuthorize("Pallets/FechamentoPallets")]
        public async Task<IActionResult> FechamentoPallets()
        {
            return View();
        }
    }
}
