using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers
{
    public class GuiasController : BaseController
    {
		#region Construtores

		public GuiasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Guias/GuiasRecolhimento")]
        public async Task<IActionResult> GuiasRecolhimento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfigGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                bool VisualizarGNRESemValidacaoDocumentos = repConfigGeral.BuscarConfiguracaoPadrao().VisualizarGNRESemValidacaoDocumentos;

                var configuracoesGuiasRecolhimento = new
                {
                    VisualizarGNRESemValidacaoDocumentos
                };
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Guias/GuiasRecolhimento");

                ViewBag.ConfiguracoesGuiasRecolhimento = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesGuiasRecolhimento);
                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                return View();
            }
        }

        [CustomAuthorize("Guias/VincularGuia")]
        public async Task<IActionResult> VincularGuia()
        {
            return View();
        }

        [AllowAuthenticate]
        [CustomAuthorize("Guias/RenderizarPDF", "Guias/VincularGuia", "Guias/GuiasRecolhimento")]
        public async Task<IActionResult> RenderizarPDF()
        {
            ViewBag.Codigo = Request.GetIntParam("Codigo");
            ViewBag.Guia = Request.GetIntParam("Guia");

            return View();
        }
    }
}
