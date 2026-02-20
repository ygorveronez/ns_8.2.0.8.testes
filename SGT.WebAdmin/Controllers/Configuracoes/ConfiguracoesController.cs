using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    public class ConfiguracoesController : BaseController
    {
        #region Construtores

        public ConfiguracoesController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Configuracoes/Integracao")]
        public async Task<IActionResult> Integracao()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ProcessoMovimento")]
        public async Task<IActionResult> ProcessoMovimento()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoMovimento")]
        public async Task<IActionResult> ConfiguracaoMovimento()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ArquivoImportacaoNotaFiscal")]
        public async Task<IActionResult> ArquivoImportacaoNotaFiscal()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoFinanceira")]
        public async Task<IActionResult> ConfiguracaoFinanceira()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/Feriado")]
        public async Task<IActionResult> Feriado()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/Configuracao")]
        public async Task<IActionResult> Configuracao()
        {
            if (this.Usuario.UsuarioAtendimento || this.Usuario.UsuarioMultisoftware || this.Usuario.UsuarioCallCenter)
            {
                ViewBag.ConfiguracoesAdicionais = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    UsuarioInternoAdministrador = this.Usuario.UsuarioInterno?.Administrador ?? false,
                });

                return View();
            }
            
            return View("Home");
        }

        [CustomAuthorize("Configuracoes/ControleAlerta")]
        public async Task<IActionResult> ControleAlerta()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/Impressora")]
        public async Task<IActionResult> Impressora()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/Licenca")]
        public async Task<IActionResult> Licenca()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/Especie")]
        public async Task<IActionResult> Especie()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/CorAnimal")]
        public async Task<IActionResult> CorAnimal()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoCIOT")]
        public async Task<IActionResult> ConfiguracaoCIOT()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoSemParar")]
        public async Task<IActionResult> ConfiguracaoSemParar()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/AcordoFaturamentoCliente")]
        public async Task<IActionResult> AcordoFaturamentoCliente()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Configuracoes/AcordoFaturamentoCliente");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Configuracoes/ContingenciaEstado")]
        public async Task<IActionResult> ContingenciaEstado()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ContingenciaMDFeEstado")]
        public async Task<IActionResult> ContingenciaMDFeEstado()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoControleEntrega")]
        public async Task<IActionResult> ConfiguracaoControleEntrega()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoDiariaAutomatica")]
        public async Task<IActionResult> ConfiguracaoDiariaAutomatica()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoConciliacaoTransportador")]
        public async Task<IActionResult> ConfiguracaoConciliacaoTransportador()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoVtex")]
        public async Task<IActionResult> ConfiguracaoVtex()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoDocumentacaoAFRMM")]
        public async Task<IActionResult> ConfiguracaoDocumentacaoAFRMM()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoEmissaoDocumentoEmbarcador")]
        public async Task<IActionResult> ConfiguracaoEmissaoDocumentoEmbarcador()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/RegraAutomatizacaoEmissoesEmail")]
        public async Task<IActionResult> RegraAutomatizacaoEmissoesEmail()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ControleThread")]
        public async Task<IActionResult> ControleThread()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoPreCarga")]
        public async Task<IActionResult> ConfiguracaoPreCarga()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoTaxaDescarga")]
        public async Task<IActionResult> ConfiguracaoTaxaDescarga()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoTemplateWhatsApp")]
        public async Task<IActionResult> ConfiguracaoTemplateWhatsApp()
        {
            return View();
        }
        [CustomAuthorize("Configuracoes/Palletizacao")]
        public async Task<IActionResult> Palletizacao()
        {
            return View();
        }
        [CustomAuthorize("Configuracoes/NotificacaoMotoristaSMS")]
        public async Task<IActionResult> NotificacaoMotoristaSMS()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/Script")]
        public async Task<IActionResult> Script()
        {
            if (this.Usuario.UsuarioAtendimento || this.Usuario.UsuarioMultisoftware || this.Usuario.UsuarioCallCenter)
                return View();
            else
                return View("Home");
        }

        [CustomAuthorize("Configuracoes/ExecucaoComandos")]
        public async Task<IActionResult> ExecucaoComandos()
        {
            if (this.Usuario.UsuarioAtendimento || this.Usuario.UsuarioMultisoftware || this.Usuario.UsuarioCallCenter)
                return View();
            else
                return View("Home");
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoModeloEmail")]
        public ActionResult ConfiguracaoModeloEmail()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/LiberacaoIntegracao")]
        public async Task<IActionResult> LiberacaoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoEmissorDocumento")]
        public async Task<IActionResult> ConfiguracaoEmissorDocumento()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/Motivo")]
        public async Task<IActionResult> Motivo()
        {
            return View();
        }

        [CustomAuthorize("Configuracoes/ConfiguracaoOrquestradorFila")]
        public async Task<IActionResult> ConfiguracaoOrquestradorFila()
        {
            return View();
        }
    }
}
