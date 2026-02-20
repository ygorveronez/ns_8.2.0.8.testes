using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Avarias
{
    public class AvariasController : BaseController
    {
		#region Construtores

		public AvariasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Avarias/ProdutoAvaria")]
        public async Task<IActionResult> ProdutoAvaria()
        {
            return View();
        }

        [CustomAuthorize("Avarias/MotivoAvaria")]
        public async Task<IActionResult> MotivoAvaria()
        {
            return View();
        }

        [CustomAuthorize("Avarias/RegrasAutorizacaoAvaria")]
        public async Task<IActionResult> RegrasAutorizacaoAvaria()
        {
            return View();
        }

        [CustomAuthorize("Avarias/SolicitacaoAvaria")]
        public async Task<IActionResult> SolicitacaoAvaria()
        {
            return View();
        }

        [CustomAuthorize("Avarias/AutorizacaoAvaria")]
        public async Task<IActionResult> AutorizacaoAvaria()
        {
            return View();
        }

        [CustomAuthorize("Avarias/LoteAvarias")]
        public async Task<IActionResult> LoteAvarias()
        {
            return View();
        }

        [CustomAuthorize("Avarias/Lotes")]
        public async Task<IActionResult> Lotes()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");
            ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);

            return View();
        }

        [CustomAuthorize("Avarias/MotivoRemocaoLote")]
        public async Task<IActionResult> MotivoRemocaoLote()
        {
            return View();
        }

        [CustomAuthorize("Avarias/AceiteLoteAvaria")]
        public async Task<IActionResult> AceiteLoteAvaria()
        {
            return View();
        }

        [CustomAuthorize("Avarias/LotesPendentes")]
        public async Task<IActionResult> LotesPendentes()
        {
            return View();
        }

        [CustomAuthorize("Avarias/MotivoDescontoAvaria")]
        public async Task<IActionResult> MotivoDescontoAvaria()
        {
            return View();
        }

        [CustomAuthorize("Avarias/FluxoAvaria")]
        public async Task<IActionResult> FluxoAvaria()
        {
            return View();
        }

    }

}

