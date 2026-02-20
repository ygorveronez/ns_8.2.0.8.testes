using Microsoft.AspNetCore.Mvc;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    public class IntegracoesController : BaseController
    {
        #region Construtores

        public IntegracoesController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Integracoes/DocumentoTransporteNatura")]
        public async Task<IActionResult> DocumentoTransporteNatura()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/IntegracaoHUB")]
        public async Task<IActionResult> IntegracaoHUB()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/Integradora")]
        public async Task<IActionResult> Integradora()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/PreFaturaNatura")]
        public async Task<IActionResult> PreFaturaNatura()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/ControleIntegracao")]
        public async Task<IActionResult> ControleIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/IndicadorIntegracaoCTe")]
        public async Task<IActionResult> IndicadorIntegracaoCTe()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/IndicadorIntegracaoNFe")]
        public async Task<IActionResult> IndicadorIntegracaoNFe()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/IntegracaoAssincrona")]
        public async Task<IActionResult> IntegracaoAssincrona()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/DocumentoHavan")]
        public async Task<IActionResult> DocumentoHavan()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/IdentificacaoMercadoriaKrona")]
        public async Task<IActionResult> IdentificacaoMercadoriaKrona()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/LoteCliente")]
        public async Task<IActionResult> LoteCliente()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/IntegracaoFTPProcessamentoEDI")]
        public async Task<IActionResult> IntegracaoFTPProcessamentoEDI()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/ExportacaoArquivoIntegracao")]
        public async Task<IActionResult> ExportacaoArquivoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/IntegradoraIntegracaoRetorno")]
        public async Task<IActionResult> IntegradoraIntegracaoRetorno()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/ControleDasIntegracoes")]
        public async Task<IActionResult> ControleDasIntegracoes()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/IntegracaoGhost")]
        public async Task<IActionResult> IntegracaoGhost()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/ConfiguracaoIntegracaoTecnologiaMonitoramento")]
        public async Task<IActionResult> ConfiguracaoIntegracaoTecnologiaMonitoramento()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/IntegracaoEnvioProgramado")]
        public async Task<IActionResult> IntegracaoEnvioProgramado()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeIntegracao> tiposEntidadeIntegracao = repIntegracaoEnvioProgramado.BuscarOrigensIntegracaoDisponiveis();

                ViewBag.TiposEntidadeIntegracao = tiposEntidadeIntegracao.ToJson();
            }

            return View();
        }

        [CustomAuthorize("Integracoes/DocumentoElectrolux")]
        public ActionResult DocumentoElectrolux()
        {
            return View();
        }

        [CustomAuthorize("Integracoes/ConfiguracaoFilialIntegracao")]
        public ActionResult ConfiguracaoFilialIntegracao()
        {
            return View();
        }
    }
}
