using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    public class FrotaController : BaseController
    {
		#region Construtores

		public FrotaController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Frota/ServicoVeiculo")]
        public async Task<IActionResult> ServicoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Frota/OrdemServico")]
        public async Task<IActionResult> OrdemServico()
        {
            ViewBag.CodigoOrdemServico = Request.GetIntParam("ordem");

            var ConfiguracoesOrdemServico = new
            {
                LimitarOperacaoPorEmpresa = this.Usuario?.LimitarOperacaoPorEmpresa ?? false,
            };
            ViewBag.ConfiguracoesOrdemServico = Newtonsoft.Json.JsonConvert.SerializeObject(ConfiguracoesOrdemServico);

            return View();
        }

        [CustomAuthorize("Frota/Adiantamento")]
        public async Task<IActionResult> Adiantamento()
        {
            return View();
        }

        [CustomAuthorize("Frota/FinalidadeProdutoOrdemServico")]
        public async Task<IActionResult> FinalidadeProdutoOrdemServico()
        {
            return View();
        }

        [CustomAuthorize("Frota/TipoMovimentoMotorista")]
        public async Task<IActionResult> TipoMovimentoMotorista()
        {
            return View();
        }

        [CustomAuthorize("Frota/TipoInfracao")]
        public async Task<IActionResult> TipoInfracao()
        {
            return View();
        }

        [CustomAuthorize("Frota/Infracao")]
        public async Task<IActionResult> Infracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");
            ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);
            ViewBag.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoOcorrencia.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos);

            return View();
        }

        [CustomAuthorize("Frota/RegraAutorizacaoInfracao")]
        public async Task<IActionResult> RegraAutorizacaoInfracao()
        {
            return View();
        }

        [CustomAuthorize("Frota/AutorizacaoInfracao")]
        public async Task<IActionResult> AutorizacaoInfracao()
        {
            return View();
        }

        [CustomAuthorize("Frota/InfracaoHistoricoAnexo")]
        public async Task<IActionResult> InfracaoHistoricoAnexo()
        {
            return View();
        }

        [CustomAuthorize("Frota/Almoxarifado")]
        public async Task<IActionResult> Almoxarifado()
        {
            return View();
        }

        [CustomAuthorize("Frota/DimensaoPneu")]
        public async Task<IActionResult> DimensaoPneu()
        {
            return View();
        }

        [CustomAuthorize("Frota/MarcaPneu")]
        public async Task<IActionResult> MarcaPneu()
        {
            return View();
        }

        [CustomAuthorize("Frota/ModeloPneu")]
        public async Task<IActionResult> ModeloPneu()
        {
            return View();
        }

        [CustomAuthorize("Frota/BandaRodagemPneu")]
        public async Task<IActionResult> BandaRodagemPneu()
        {
            return View();
        }

        [CustomAuthorize("Frota/MotivoSucateamentoPneu")]
        public async Task<IActionResult> MotivoSucateamentoPneu()
        {
            return View();
        }

        [CustomAuthorize("Frota/Pneu")]
        public async Task<IActionResult> Pneu()
        {
            return View();
        }

        [CustomAuthorize("Frota/MovimentacaoPneu")]
        public async Task<IActionResult> MovimentacaoPneu()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Frota/MovimentacaoPneu");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Frota/TipoOrdemServico")]
        public async Task<IActionResult> TipoOrdemServico()
        {
            return View();
        }

        [CustomAuthorize("Frota/ProgramacaoAlocacao")]
        public async Task<IActionResult> ProgramacaoAlocacao()
        {
            return View();
        }

        [CustomAuthorize("Frota/ProgramacaoEspecialidade")]
        public async Task<IActionResult> ProgramacaoEspecialidade()
        {
            return View();
        }

        [CustomAuthorize("Frota/ProgramacaoLicenciamento")]
        public async Task<IActionResult> ProgramacaoLicenciamento()
        {
            return View();
        }

        [CustomAuthorize("Frota/ProgramacaoSituacao")]
        public async Task<IActionResult> ProgramacaoSituacao()
        {
            return View();
        }

        [CustomAuthorize("Frota/ProgramacaoSituacaoTMS")]
        public async Task<IActionResult> ProgramacaoSituacaoTMS()
        {
            return View();
        }

        [CustomAuthorize("Frota/ProgramacaoMotorista")]
        public async Task<IActionResult> ProgramacaoMotorista()
        {
            return View();
        }

        [CustomAuthorize("Frota/ProgramacaoVeiculo")]
        public async Task<IActionResult> ProgramacaoVeiculo()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Frota/ProgramacaoVeiculo");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Frota/ProgramacaoVeiculoTMS")]
        public async Task<IActionResult> ProgramacaoVeiculoTMS()
        {
            return View();
        }

        [CustomAuthorize("Frota/ProgramacaoLogistica")]
        public async Task<IActionResult> ProgramacaoLogistica()
        {
            return View();
        }

        [CustomAuthorize("Frota/ControleTacografo")]
        public async Task<IActionResult> ControleTacografo()
        {
            return View();
        }

        [CustomAuthorize("Frota/GrupoServico")]
        public async Task<IActionResult> GrupoServico()
        {
            return View();
        }

        [CustomAuthorize("Frota/RegraAutorizacaoOrdemServico")]
        public async Task<IActionResult> RegraAutorizacaoOrdemServico()
        {
            return View();
        }

        [CustomAuthorize("Frota/AutorizacaoOrdemServico")]
        public async Task<IActionResult> AutorizacaoOrdemServico()
        {
            ViewBag.CodigoOrdemServico = Request.GetIntParam("ordem");
            
            return View();
        }

        [CustomAuthorize("Frota/DespesaFrotaPropria")]
        public async Task<IActionResult> DespesaFrotaPropria()
        {
            return View();
        }

        [CustomAuthorize("Frota/ImportacaoPrecoCombustivel")]
        public async Task<IActionResult> ImportacaoPrecoCombustivel()
        {
            return View();
        }

        [CustomAuthorize("Frota/Sinistro")]
        public async Task<IActionResult> Sinistro()
        {
            return View();
        }

        [CustomAuthorize("Frota/ImportacaoPedagio")]
        public async Task<IActionResult> ImportacaoPedagio()
        {
            return View();
        }

        [CustomAuthorize("Frota/GeracaoFrotaAutomatizada")]
        public async Task<IActionResult> GeracaoFrotaAutomatizada()
        {
            return View();
        }

        [CustomAuthorize("Frota/SugestaoMensal")]
        public async Task<IActionResult> SugestaoMensal()
        {
            return View();
        }

        [CustomAuthorize("Frota/JustificativaDeIndisponibilidade")]
        public async Task<IActionResult> JustificativaDeIndisponibilidade()
        {
            return View();
        }

        [CustomAuthorize("Frota/ListaDiaria")]
        public async Task<IActionResult> ListaDiaria()
        {
            return View();
        }

        [CustomAuthorize("Frota/RodizioPlacas")]
        public async Task<IActionResult> RodizioPlacas()
        {
            return View();
        }

        [CustomAuthorize("Frota/IndicadoresManutencao")]
        public async Task<IActionResult> IndicadoresManutencao()
        {
            return View();
        }    
        
        [CustomAuthorize("Frota/TipoSinistro")]
        public async Task<IActionResult> TipoSinistro()
        {
            return View();
        }

        [CustomAuthorize("Frota/TipoLocalManutencao")]
        public async Task<IActionResult> TipoLocalManutencao()
        {
            return View();
        }

        [CustomAuthorize("Frota/GravidadeSinistro")]
        public ActionResult GravidadeSinistro()
        {
            return View();
        }
    }
}
