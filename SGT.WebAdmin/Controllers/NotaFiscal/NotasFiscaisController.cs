using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    public class NotasFiscaisController : BaseController
    {
		#region Construtores

		public NotasFiscaisController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("NotasFiscais/CFOP")]
        public async Task<IActionResult> CFOP()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/ObservacaoFiscal")]
        public async Task<IActionResult> ObservacaoFiscal()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/NaturezaDaOperacao")]
        public async Task<IActionResult> NaturezaDaOperacao()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/Servico")]
        public async Task<IActionResult> Servico()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/GrupoImposto")]
        public async Task<IActionResult> GrupoImposto()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/ModeloDocumentoFiscal")]
        public async Task<IActionResult> ModeloDocumentoFiscal()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/NotaFiscalEletronica")]
        public async Task<IActionResult> NotaFiscalEletronica()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/NotaFiscalObservacaoCartaCorrecao")]
        public async Task<IActionResult> NotaFiscalObservacaoCartaCorrecao()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/NotaFiscalImportacao")]
        public async Task<IActionResult> NotaFiscalImportacao()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/NotaFiscalInutilizar")]
        public async Task<IActionResult> NotaFiscalInutilizar()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/ImpostoIBPTNFe")]
        public async Task<IActionResult> ImpostoIBPTNFe()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/ContratoNotaFiscal")]
        public async Task<IActionResult> ContratoNotaFiscal()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/SpedFiscal")]
        public async Task<IActionResult> SpedFiscal()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/Sintegra")]
        public async Task<IActionResult> Sintegra()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/SpedPISCOFINS")]
        public async Task<IActionResult> SpedPISCOFINS()
        {
            return View();
        }
        
        [CustomAuthorize("NotasFiscais/NotaFiscalSituacao")]
        public async Task<IActionResult> NotaFiscalSituacao()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/ItemNaoConformidade")]
        public async Task<IActionResult> ItemNaoConformidade()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/ConversaoUnidadeMedida")]
        public async Task<IActionResult> ConversaoUnidadeMedida()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/BloqueioEmissaoPorNaoConformidade")]
        public async Task<IActionResult> BloqueioEmissaoPorNaoConformidade()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/RegraAutorizacaoNaoConformidade")]
        public async Task<IActionResult> RegraAutorizacaoNaoConformidade()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/HistoricoNaoConformidade")]
        public async Task<IActionResult> HistoricoNaoConformidade()
        {
            return View();
        }

        [CustomAuthorize("NotasFiscais/RetornoSefaz")]
        public async Task<IActionResult> RetornoSefaz()
        {
            return View();
        }        

        [CustomAuthorize("NotasFiscais/AutorizacaoNaoConformidade")]
        public async Task<IActionResult> AutorizacaoNaoConformidade()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");

            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);

            return View();
        }

    }
}
