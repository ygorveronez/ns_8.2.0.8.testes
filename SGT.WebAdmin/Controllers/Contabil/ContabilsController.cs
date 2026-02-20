using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    public class ContabilsController : BaseController
    {
		#region Construtores

		public ContabilsController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Contabils/ArquivoEBS")]
        public async Task<IActionResult> ArquivoEBS()
        {
            return View();
        }

        [CustomAuthorize("Contabils/ConsultaValores")]
        public async Task<IActionResult> ConsultaValores()
        {
            return View();
        }

        [CustomAuthorize("Contabils/ConsultaValoresPagar")]
        public async Task<IActionResult> ConsultaValoresPagar()
        {
            return View();
        }

        [CustomAuthorize("Contabils/ArquivoContabil")]
        public async Task<IActionResult> ArquivoContabil()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Contabils/ArquivoContabil");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Contabils/ControleArquivo")]
        public async Task<IActionResult> ControleArquivo()
        {
            return View();
        }

        [CustomAuthorize("Contabils/AlteracaoArquivoMercante")]
        public async Task<IActionResult> AlteracaoArquivoMercante()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Contabils/AlteracaoArquivoMercante");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Contabils/TipoMovimentoArquivoContabil")]
        public async Task<IActionResult> TipoMovimentoArquivoContabil()
        {
            return View();
        }

        [CustomAuthorize("Contabils/CalculoISS")]
        public async Task<IActionResult> CalculoISS()
        {
            return View();
        }

        [CustomAuthorize("Contabils/CalculoPisCofins")]
        public async Task<IActionResult> CalculoPisCofins()
        {
            return View();
        }

        [CustomAuthorize("Contabils/CodigoIntegracaoCFOPCST")]
        public async Task<IActionResult> CodigoIntegracaoCFOPCST()
        {
            return View();
        }

        [CustomAuthorize("Contabils/JustificativaMercante")]
        public async Task<IActionResult> JustificativaMercante()
        {
            return View();
        }

        [CustomAuthorize("Contabils/ImpostoValorAgregado")]
        public async Task<IActionResult> ImpostoValorAgregado()
        {
            return View();
        }

        [CustomAuthorize("Contabils/DireitoFiscal")]
        public async Task<IActionResult> DireitoFiscal()
        {
            return View();
        }

    }
}
