using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoEntregas
{
    public class GestaoEntregasController : BaseController
    {
		#region Construtores

		public GestaoEntregasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("GestaoEntregas/FluxoEntrega")]
        public async Task<IActionResult> FluxoEntrega()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoEntrega = ObterPermissoesPersonalizadas("GestaoEntregas/FluxoEntrega");
            ViewBag.PermissoesPersonalizadasFluxoEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasFluxoEntrega);

            return View();
        }

        [CustomAuthorize("GestaoEntregas/MotivoDevolucaoEntrega")]
        public async Task<IActionResult> MotivoDevolucaoEntrega()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();

                ViewBag.LiberarCamposV3Trizy = configuracaoIntegracaoTrizy?.VersaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoIntegracaoTrizy.Versao3? 1 : 0;

                return View();
            }
        }

        [CustomAuthorize("GestaoEntregas/AcaoDevolucaoMotorista")]
        public async Task<IActionResult> AcaoDevolucaoMotorista()
        {
            return View();
        }

        [CustomAuthorize("GestaoEntregas/ConfiguracaoPortalCliente")]
        public async Task<IActionResult> ConfiguracaoPortalCliente()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                var retorno = new
                {
                    PossuiIntegracaoWhatsApp = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.WhatsApp),
                };


                ViewBag.Integracoes = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                return View();
            }
        }
    }
}
