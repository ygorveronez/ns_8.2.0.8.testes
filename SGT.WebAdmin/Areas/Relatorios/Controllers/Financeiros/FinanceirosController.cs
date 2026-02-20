using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	public class FinanceirosController : BaseController
    {
		#region Construtores

		public FinanceirosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Financeiros/PlanoConta")]
        public async Task<IActionResult> PlanoConta()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/ExtratoConta")]
        public async Task<IActionResult> ExtratoConta()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/Titulo")]
        public async Task<IActionResult> Titulo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/ExtratoMotorista")]
        public async Task<IActionResult> ExtratoMotorista()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/BalanceteGerencial")]
        public async Task<IActionResult> BalanceteGerencial()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/PosicaoContasReceber")]
        public async Task<IActionResult> PosicaoContasReceber()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/DescontoAcrescimoCTe")]
        public async Task<IActionResult> DescontoAcrescimoCTe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/TipoMovimento")]
        public async Task<IActionResult> TipoMovimento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/AliquotaICMSCTe")]
        public async Task<IActionResult> AliquotaICMSCTe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/DescontoAcrescimoFatura")]
        public async Task<IActionResult> DescontoAcrescimoFatura()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/Faturamento")]
        public async Task<IActionResult> Faturamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/DocumentoFaturamento")]
        public async Task<IActionResult> DocumentoFaturamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/PosicaoDocumentoReceber")]
        public async Task<IActionResult> PosicaoDocumentoReceber()
        {
            return View();
        }


        [CustomAuthorize("Relatorios/Financeiros/RetornoBoleto")]
        public async Task<IActionResult> RetornoBoleto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/CTeTituloReceber")]
        public async Task<IActionResult> CTeTituloReceber()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/FaturamentoMensal")]
        public async Task<IActionResult> FaturamentoMensal()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/PerfilCliente")]
        public async Task<IActionResult> PerfilCliente()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/TituloAcrescimoDesconto")]
        public async Task<IActionResult> TituloAcrescimoDesconto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/DRE")]
        public async Task<IActionResult> DRE()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/DREGerencial")]
        public async Task<IActionResult> DREGerencial()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/PlanoOrcamentario")]
        public async Task<IActionResult> PlanoOrcamentario()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/FluxoCaixa")]
        public async Task<IActionResult> FluxoCaixa()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/TituloSemMovimento")]
        public async Task<IActionResult> TituloSemMovimento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/RetornoPagamento")]
        public async Task<IActionResult> RetornoPagamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/PagamentoAgregado")]
        public async Task<IActionResult> PagamentoAgregado()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/Cheque")]
        public async Task<IActionResult> Cheque()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/BaixaTitulo")]
        public async Task<IActionResult> BaixaTitulo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/DespesaMensal")]
        public async Task<IActionResult> DespesaMensal()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/NaturezaDaOperacao")]
        public async Task<IActionResult> NaturezaDaOperacao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/CFOP")]
        public async Task<IActionResult> CFOP()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/ExtratoBancario")]
        public async Task<IActionResult> ExtratoBancario()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/ExtratoAcertoViagem")]
        public async Task<IActionResult> ExtratoAcertoViagem()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/PosicaoContasPagar")]
        public async Task<IActionResult> PosicaoContasPagar()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/MovimentoFinanceiro")]
        public async Task<IActionResult> MovimentoFinanceiro()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/RateioDespesaVeiculo")]
        public async Task<IActionResult> RateioDespesaVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/ConciliacaoBancaria")]
        public async Task<IActionResult> ConciliacaoBancaria()
        {
            return View();
        }        

        [CustomAuthorize("Relatorios/Financeiros/ConferenciaFiscal")]
        public async Task<IActionResult> ConferenciaFiscal()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/ContratoFinanceiro")]
        public async Task<IActionResult> ContratoFinanceiro()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/CondicoesPagamentoTransportador")]
        public async Task<IActionResult> CondicoesPagamentoTransportador()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Financeiros/LiberacaoPagamentoProvedor")]
        public async Task<IActionResult> LiberacaoPagamentoProvedor()
        {
            return View();
        }
    }
}
