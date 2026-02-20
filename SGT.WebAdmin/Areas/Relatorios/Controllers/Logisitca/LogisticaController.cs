using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers
{
	[Area("Relatorios")]
	public class LogisticaController : BaseController
    {
		#region Construtores

		public LogisticaController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Logistica/TempoCarregamento")]
        public async Task<IActionResult> TempoCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/GuaritaTMS")]
        public async Task<IActionResult> GuaritaTMS()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/FilaCarregamentoHistorico")]
        public async Task<IActionResult> FilaCarregamentoHistorico()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoNivelServico")]
        public async Task<IActionResult> MonitoramentoNivelServico()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoVeiculoAlvo")]
        public async Task<IActionResult> MonitoramentoVeiculoAlvo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoPosicaoDaFrota")]
        public async Task<IActionResult> MonitoramentoPosicaoDaFrota()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoTratativaAlerta")]
        public async Task<IActionResult> MonitoramentoTratativaAlerta()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoAlerta")]
        public async Task<IActionResult> MonitoramentoAlerta()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoHistoricoTemperatura")]
        public async Task<IActionResult> MonitoramentoHistoricoTemperatura()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/JanelaAgendamento")]
        public async Task<IActionResult> JanelaAgendamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/JanelaDisponivelAgendamento")]
        public async Task<IActionResult> JanelaDisponivelAgendamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/AgendaCancelada")]
        public async Task<IActionResult> AgendaCancelada()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoTempoVeiculo")]
        public async Task<IActionResult> MonitoramentoTempoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoPosicaoFrotaRastreamento")]
        public async Task<IActionResult> MonitoramentoPosicaoFrotaRastreamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/ConsolidacaoGas")]
        public async Task<IActionResult> ConsolidacaoGas()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/ControleTempoViagem")]
        public async Task<IActionResult> ControleTempoViagem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoControleEntrega")]
        public async Task<IActionResult> MonitoramentoControleEntrega()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/PracaPedagio")]
        public async Task<IActionResult> PracaPedagio()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/JanelaCarregamentoIntegracao")]
        public async Task<IActionResult> JanelaCarregamentoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/MonitoramentoVeiculoPosicao")]
        public async Task<IActionResult> MonitoramentoVeiculoPosicao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/Navio")]
        public async Task<IActionResult> Navio()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Logistica/HistoricoJanelaCarregamento")]
        public IActionResult HistoricoJanelaCarregamento()
        {
            return View();
        }
    }
}
