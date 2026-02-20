using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Canhotos
{

    public class CanhotosController : BaseController
    {
        #region Construtores

        public CanhotosController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Canhotos/Canhoto")]
        public async Task<IActionResult> Canhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarPrimeiroRegistro();

                ViewBag.ObrigatorioInformarDataEnvioCanhoto = configuracaoEmbarcador?.ObrigatorioInformarDataEnvioCanhoto ?? false ? "true" : "false";
                ViewBag.PrazoParaReverterDigitalizacaoAposAprovacao = configuracaoCanhoto.PrazoParaReverterDigitalizacaoAposAprovacao;
                ViewBag.PermitirAtualizarCanhotosPorImportacao = (configuracaoCanhoto.PermitirAtualizarSituacaoCanhotoPorImportacao || configuracaoCanhoto.PermitirAtualizarSituacaoCanhotoAvulsoPorImportacao) ? "true" : "false";
                ViewBag.VisualizarPermitirAlterarDataEntregaNaConfirmacaoCanhoto = configuracaoGeral.VisualizarPermitirAlterarDataEntregaNaConfirmacaoCanhoto ? "true" : "false";
                ViewBag.ExigirDataEntregaNotaClienteCanhotosReceberFisicamente = configuracaoCanhoto.ExigirDataEntregaNotaClienteCanhotosReceberFisicamente ? "true" : "false";
                ViewBag.PermitirImportarCanhotoNFFaturada = configuracaoCanhoto.PermitirImportarCanhotoNFFaturada ? "true" : "false";
                ViewBag.PermitirEnviarImagemParaMultiplosCanhotos = configuracaoCanhoto.PermitirEnviarImagemParaMultiplosCanhotos ? "true" : "false";
                ViewBag.IntegrarCanhotosComValidadorIAComprovei = configuracaoCanhoto.IntegrarCanhotosComValidadorIAComprovei ? "true" : "false";
            }
            catch (Exception excecao)
            {
                ViewBag.ObrigatorioInformarDataEnvioCanhoto = "false";
                Servicos.Log.TratarErro(excecao);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

            ViewBag.PermissoesPersonalizadasCanhotos = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            bool portalCliente = IsLayoutClienteAtivo(unitOfWork);
            string caminhoBaseViews = "~/Views/Canhotos";
            string caminhosViewMasterLayout = portalCliente ? $"{caminhoBaseViews}/CanhotoCliente.cshtml" : $"{caminhoBaseViews}/Canhoto.cshtml";

            return View(caminhosViewMasterLayout);
        }

        [CustomAuthorize("Canhotos/LocalArmazenamentoCanhoto")]
        public async Task<IActionResult> LocalArmazenamentoCanhoto()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = new Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS();

            try
            {
                config = ConfiguracaoEmbarcador;
            }
            catch (Exception)
            {

            }

            ViewBag.ConfiguracoesLocalArmazenamentoCanhoto = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                ArmazenamentoCanhotoComFilial = config?.ArmazenamentoCanhotoComFilial ?? false
            });

            return View();
        }

        [CustomAuthorize("Canhotos/BaixarCanhoto")]
        public async Task<IActionResult> BaixarCanhoto()
        {
            return View();
        }

        [CustomAuthorize("Canhotos/VincularCanhotoManual")]
        public async Task<IActionResult> VincularCanhotoManual()
        {
            return View();
        }

        [CustomAuthorize("Canhotos/CanhotoMalote")]
        public async Task<IActionResult> CanhotoMalote()
        {
            return View();
        }

        [CustomAuthorize("Canhotos/Malote")]
        public async Task<IActionResult> Malote()
        {
            return View();
        }

        [CustomAuthorize("Canhotos/MotivoInconsistenciaDigitacao")]
        public async Task<IActionResult> MotivoInconsistenciaDigitacao()
        {
            return View();
        }

        [CustomAuthorize("Canhotos/CanhotoIntegracao")]
        public async Task<IActionResult> CanhotoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Canhotos/ConfiguracaoReconhecimentoCanhoto")]
        public async Task<IActionResult> ConfiguracaoReconhecimentoCanhoto()
        {
            return View();
        }

        [AllowAuthenticate]
        [CustomAuthorize("Canhotos/RenderizarPDF", "Canhotos/VincularCanhotoManual")]
        public async Task<IActionResult> RenderizarPDF()
        {
            ViewBag.Codigo = Request.GetIntParam("Codigo");
            ViewBag.Canhoto = Request.GetIntParam("Canhoto");

            return View();
        }

        [CustomAuthorize("Canhotos/MotivoRejeicaoAuditoria")]
        public async Task<IActionResult> MotivoRejeicaoAuditoria()
        {
            return View();
        }

        [CustomAuthorize("Canhotos/EnviarCanhotos")]
        public async Task<IActionResult> EnviarCanhotos()
        {
            ViewBag.AntiForgery = Guid.NewGuid().ToString();
            return View();
        }

        [CustomAuthorize("Canhotos/CanhotoIntegracaoPorCTe")]
        public async Task<IActionResult> CanhotoIntegracaoPorCTe()
        {
            return View();
        }
    }
}
