using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    public class TorreControleController : BaseController
    {
        #region Construtores

        public TorreControleController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("TorreControle/ConsultaPorEntrega")]
        public async Task<IActionResult> ConsultaPorEntrega()
        {
            return View();
        }

        [CustomAuthorize("TorreControle/ConsultaPorNotaFiscal")]
        public async Task<IActionResult> ConsultaPorNotaFiscal()
        {
            return View();
        }

        [CustomAuthorize("TorreControle/AcompanhamentoCarga")]
        public async Task<IActionResult> AcompanhamentoCarga()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasChamado = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");

                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.ConfiguracoesJanelaCarregamento = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    ExibirOpcaoLiberarParaTransportador = configuracaoJanelaCarregamento?.ExibirOpcaoLiberarParaTransportador ?? false,
                    PermiteSelecionarHorarioEncaixe = operadorLogistica?.PermiteSelecionarHorarioEncaixe ?? false
                });

                bool permissaoDelegarChamado = false;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    permissaoDelegarChamado = UsuarioPossuiPermissao("Chamados/RegrasAnaliseChamados");

                ViewBag.NomeUsuario = this.Usuario.Nome ?? string.Empty;
                ViewBag.ControleEntregaVisaoPrevisao = ConfiguracaoEmbarcador.ControleEntregaVisaoPrevisao;
                ViewBag.PermissaoDelegar = permissaoDelegarChamado ? "true" : "false";
                ViewBag.PermitirContatoWhatsApp = ConfiguracaoEmbarcador.PermitirContatoWhatsApp ? "true" : "false";
                ViewBag.HabilitarWidgetAtendimento = ConfiguracaoEmbarcador.HabilitarWidgetAtendimento ? "true" : "false";
                ViewBag.HabilitarIconeEntregaAtrasada = ConfiguracaoEmbarcador.HabilitarIconeEntregaAtrasada ? "true" : "false";
                ViewBag.FiltrarWidgetAtendimentoProFiltro = ConfiguracaoEmbarcador.FiltrarWidgetAtendimentoProFiltro ? "true" : "false";
                ViewBag.PermitirAbrirAtendimento = (configuracaoControleEntrega?.PermitirAbrirAtendimentoViaControleEntrega ?? false) ? "true" : "false";
                ViewBag.TempoSemPosicaoParaVeiculoPerderSinal = ConfiguracaoEmbarcador.TempoSemPosicaoParaVeiculoPerderSinal > 0 ? "true" : "false";

                ViewBag.PermissoesPersonalizadasChamado = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasChamado);
                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);
                ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);
                ViewBag.PermissoesPersonalizadasControleEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

                return View();
            }

        }

        [CustomAuthorize("TorreControle/ConsultaEntregasAtrasadas")]
        public async Task<IActionResult> ConsultaEntregasAtrasadas()
        {

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasMonitoramento = ObterPermissoesPersonalizadas("Logistica/Monitoramento");
            string permissoesPersonalizadasMonitoramentoJson = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasMonitoramento);
            ViewBag.PermissoesPersonalizadasControleEntrega = permissoesPersonalizadasMonitoramentoJson;
            ViewBag.PermissoesPersonalizadasMonitoramento = permissoesPersonalizadasMonitoramentoJson;

            return View();
        }

        [CustomAuthorize("TorreControle/FinalizacaoColetaEntregaEmLote")]
        public async Task<IActionResult> FinalizacaoColetaEntregaEmLote()
        {
            return View();
        }

        [CustomAuthorize("TorreControle/MonitorNotificacoesApp")]
        public async Task<IActionResult> MonitorNotificacoesApp()
        {
            return View();
        }

        [CustomAuthorize("TorreControle/MonitorIntegracoesSuperApp")]
        public async Task<IActionResult> MonitorIntegracoesSuperApp()
        {
            return View();
        }

        [CustomAuthorize("TorreControle/PlanejamentoVolume")]
        public async Task<IActionResult> PlanejamentoVolume()
        {
            return View();
        }

        [CustomAuthorize("TorreControle/RegraQualidadeMonitoramento")]
        public ActionResult RegraQualidadeMonitoramento()
        {
            return View();
        }

        [CustomAuthorize("TorreControle/MonitorFinalizacaoEntregaAssincrona")]
        public ActionResult MonitorFinalizacaoEntregaAssincrona()
        {
            return View();
        }

        [CustomAuthorize("TorreControle/QualidadeEntrega")]
        public ActionResult QualidadeEntrega()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasQualidadeEntrega = ObterPermissoesPersonalizadas("TorreControle/QualidadeEntrega");
            string permissoesPersonalizadasQualidadeEntregaJson = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasQualidadeEntrega);
            ViewBag.PermissoesPersonalizadasQualidadeEntrega = permissoesPersonalizadasQualidadeEntregaJson;

            return View();
        }

        [CustomAuthorize("TorreControle/ChecklistSuperApp")]
        public ActionResult ChecklistSuperApp()
        {
            return View();
        }

        [CustomAuthorize("TorreControle/TendenciaParadas")]
        public ActionResult TendenciaParadas()
        {
            return View();
        }

        [AllowAuthenticate]
        [CustomAuthorize("TorreControle/RenderizarPDF")]
        public async Task<IActionResult> RenderizarPDF()
        {
            ViewBag.Codigo = Request.GetIntParam("Codigo");

            return View();
        }
    }
}
