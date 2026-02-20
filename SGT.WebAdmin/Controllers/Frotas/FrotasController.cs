using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frotas
{
    public class FrotasController : BaseController
    {
		#region Construtores

		public FrotasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Frotas/Abastecimento")]
        public async Task<IActionResult> Abastecimento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Frotas/Abastecimento");
                
                var configuracoesAbastecimento = new
                {
                    LimitarOperacaoPorEmpresa = this.Usuario?.LimitarOperacaoPorEmpresa ?? false,
                };

                ViewBag.ConfiguracoesAbastecimento = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesAbastecimento);
                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            }

            return View();
        }

        [CustomAuthorize("Frotas/ImportacaoDePedagio")]
        public async Task<IActionResult> ImportacaoDePedagio()
        {
            return View();
        }
        
        [CustomAuthorize("Frotas/Pedagio")]
        public async Task<IActionResult> Pedagio()
        {
            return View();
        }

        [CustomAuthorize("Frotas/MovimentacaoDePlacas")]
        public async Task<IActionResult> MovimentacaoDePlacas()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

                var enabledAlterarGestor = configuracaoVeiculo.UtilizarMesmoGestorParaTodaComposicao;

                ViewBag.AtivarBotaoAlterarGestor = enabledAlterarGestor;
            }

            return View();
        }

        [CustomAuthorize("Frotas/ConfiguracaoAbastecimento")]
        public async Task<IActionResult> ConfiguracaoAbastecimento()
        {
            return View();
        }

        [CustomAuthorize("Frotas/FechamentoPedagio")]
        public async Task<IActionResult> FechamentoPedagio()
        {
            return View();
        }

        [CustomAuthorize("Frotas/FechamentoAbastecimento")]
        public async Task<IActionResult> FechamentoAbastecimento()
        {
            var configuracoesFechamentoAbastecimento = new
            {
                LimitarOperacaoPorEmpresa = this.Usuario?.LimitarOperacaoPorEmpresa ?? false,
            };

            ViewBag.ConfiguracoesFechamentoAbastecimento = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesFechamentoAbastecimento);

            return View();
        }

        [CustomAuthorize("Frotas/ConfiguracaoValePedagio")]
        public async Task<IActionResult> ConfiguracaoValePedagio()
        {
            return View();
        }

        [CustomAuthorize("Frotas/ReprocessarAbastecimento")]
        public async Task<IActionResult> ReprocessarAbastecimento()
        {
            return View();
        }
        
        [CustomAuthorize("Frotas/TipoDestinoOleo")]
        public async Task<IActionResult> TipoDestinoOleo()
        {
            return View();
        }
        
        [CustomAuthorize("Frotas/TipoOleo")]
        public ActionResult TipoOleo()
        {
            return View();
        }

        [CustomAuthorize("Frotas/BombaAbastecimento")]
        public ActionResult BombaAbastecimento()
        {
            return View();
        }

        [CustomAuthorize("Frotas/TabelaPrecoCombustivel")]
        public ActionResult TabelaPrecoCombustivel()
        {
            return View();
        }

        [CustomAuthorize("Frotas/ControleKmReboque")]
        public async Task<IActionResult> ControleKmReboque()
        {
            return View();
        }
    }
}
