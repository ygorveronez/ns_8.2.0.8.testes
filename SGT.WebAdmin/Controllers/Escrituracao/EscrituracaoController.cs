using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    public class EscrituracaoController : BaseController
    {
        #region Construtores

        public EscrituracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Escrituracao/LoteEscrituracao")]
        public async Task<IActionResult> LoteEscrituracao()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/LoteEscrituracao");

            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Escrituracao/LoteEscrituracaoCancelamento")]
        public async Task<IActionResult> LoteEscrituracaoCancelamento()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/LoteEscrituracaoCancelamento");

            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Escrituracao/Provisao")]
        public async Task<IActionResult> Provisao()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/Provisao");

            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Escrituracao/Pagamento")]
        public async Task<IActionResult> Pagamento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfigCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfigCanhoto.BuscarPrimeiroRegistro();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/Pagamento");

                var retorno = new
                {
                    PermitirBloquearDocumentoManualmente = configuracaoCanhoto?.PermitirBloquearDocumentoManualmente ?? false,
                    PermitirMultiplaSelecaoLancamentoLotePagamento = configuracaoCanhoto?.PermitirMultiplaSelecaoLancamentoLotePagamento ?? false,
                    GerarLotePagamentoSomenteParaCTe = configuracaoFinanceiro?.GerarLotePagamentoSomenteParaCTe ?? false,
                };

                ViewBag.ConfiguracoesPagamento = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
                ViewBag.PermissoesPersonalizadasPagamento = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

                return View();
            }
        }

        [CustomAuthorize("Escrituracao/CancelamentoProvisao")]
        public async Task<IActionResult> CancelamentoProvisao()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/CancelamentoProvisao");

            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Escrituracao/CancelamentoPagamento")]
        public async Task<IActionResult> CancelamentoPagamento()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/CancelamentoPagamento");

            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Escrituracao/RegraEscrituracao")]
        public async Task<IActionResult> RegraEscrituracao()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/LancamentosContabeis")]
        public async Task<IActionResult> LancamentosContabeis()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/MotivoCancelamentoPagamento")]
        public async Task<IActionResult> MotivoCancelamentoPagamento()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/ConfiguracaoProvisao")]
        public async Task<IActionResult> ConfiguracaoProvisao()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/RegraPisCofins")]
        public async Task<IActionResult> RegraPisCofins()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga")]
        public async Task<IActionResult> RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/AutorizacaoLiberacaoEscrituracaoPagamentoCarga")]
        public async Task<IActionResult> AutorizacaoLiberacaoEscrituracaoPagamentoCarga()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/ProvisaoManual")]
        public async Task<IActionResult> ProvisaoManual()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/RegraAutorizacaoPagamento")]
        public async Task<IActionResult> RegraAutorizacaoPagamento()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/AutorizacaoPagamento")]
        public async Task<IActionResult> AutorizacaoPagamento()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/LoteEscrituracaoMiro")]
        public async Task<IActionResult> LoteEscrituracaoMiro()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/RegraAutorizacaoProvisaoPendente")]
        public async Task<IActionResult> RegraAutorizacaoProvisaoPendente()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/AutorizacaoEstornoProvisao")]
        public async Task<IActionResult> AutorizacaoEstornoProvisao()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/RegraPagamentoProvedor")]
        public async Task<IActionResult> RegraPagamentoProvedor()
        {
            return View();
        }

        [CustomAuthorize("Escrituracao/ContratoFreteCliente")]
        public async Task<IActionResult> ContratoFreteCliente()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/ContratoFreteCliente");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Escrituracao/SaldoContratoFreteCliente")]
        public ActionResult SaldoContratoFreteCliente()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/SaldoContratoFreteCliente");

            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }
    }
}
