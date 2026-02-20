using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros
{
    public class TerceirosController : BaseController
    {
		#region Construtores

		public TerceirosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Terceiros/ContratoFrete")]
        public async Task<IActionResult> ContratoFrete()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Terceiros/RegraContratoFreteTerceiro")]
        public async Task<IActionResult> RegraContratoFreteTerceiro()
        {
            return View();
        }

        [CustomAuthorize("Terceiros/AutorizacaoContratoFreteTerceiro")]
        public async Task<IActionResult> AutorizacaoContratoFreteTerceiro()
        {
            return View();
        }

        [CustomAuthorize("Terceiros/Imposto")]
        public async Task<IActionResult> Imposto()
        {
            return View();
        }

        [CustomAuthorize("Terceiros/ContratoFreteValorPadrao")]
        public async Task<IActionResult> ContratoFreteValorPadrao()
        {
            return View();
        }

        [CustomAuthorize("Terceiros/ContratoFreteAcrescimoDesconto")]
        public async Task<IActionResult> ContratoFreteAcrescimoDesconto()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

                var configuracoesTransportador = new
                {
                    configuracaoContratoFreteTerceiro.UtilizarFechamentoDeAgregado
                };

                ViewBag.ConfiguracoesTransportador = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesTransportador);
            }

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFreteAcrescimoDesconto");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Terceiros/RegraAutorizacaoContratoFreteAcrescimoDesconto")]
        public async Task<IActionResult> RegraAutorizacaoContratoFreteAcrescimoDesconto()
        {
            return View();
        }

        [CustomAuthorize("Terceiros/AutorizacaoContratoFreteAcrescimoDesconto")]
        public async Task<IActionResult> AutorizacaoContratoFreteAcrescimoDesconto()
        {
            return View();
        }

        [CustomAuthorize("Terceiros/TaxaTerceiro")]
        public async Task<IActionResult> TaxaTerceiro()
        {
            return View();
        }      
        
        [CustomAuthorize("Terceiros/ContratoFreteAcrescimoDescontoAutomatico")]
        public async Task<IActionResult> ContratoFreteAcrescimoDescontoAutomatico()
        {
            return View();
        }

        [CustomAuthorize("Terceiros/AutorizacaoPagamentoContratoFrete")]
        public async Task<IActionResult> AutorizacaoPagamentoContratoFrete()
        {
            return View();
        }

        [CustomAuthorize("Terceiros/FechamentoAgregado")]
        public async Task<IActionResult> FechamentoAgregado()
        {
            return View();
        }
    }
}
