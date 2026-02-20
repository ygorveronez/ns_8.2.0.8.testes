using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
	[Area("Relatorios")]
	public class NFeController : BaseController
    {
		#region Construtores

		public NFeController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/NFe/DANFE")]
        public async Task<IActionResult> DANFE()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/NotasEmitidas")]
        public async Task<IActionResult> NotasEmitidas()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/EstoqueProdutos")]
        public async Task<IActionResult> EstoqueProdutos()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/HistoricoEstoque")]
        public async Task<IActionResult> HistoricoEstoque()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/CCeNFe")]
        public async Task<IActionResult> CCeNFe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/NotasEmitidasAdministrativo")]
        public async Task<IActionResult> NotasEmitidasAdministrativo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/GiroEstoque")]
        public async Task<IActionResult> GiroEstoque()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/CurvaABCProduto")]
        public async Task<IActionResult> CurvaABCProduto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/CurvaABCPessoa")]
        public async Task<IActionResult> CurvaABCPessoa()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/HistoricoProduto")]
        public async Task<IActionResult> HistoricoProduto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/CompraVendaNCM")]
        public async Task<IActionResult> CompraVendaNCM()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/ProdutoSemMovimentacao")]
        public async Task<IActionResult> ProdutoSemMovimentacao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/VendasReduzidas")]
        public async Task<IActionResult> VendasReduzidas()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/NotasDetalhadas")]
        public async Task<IActionResult> NotasDetalhadas()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/PedidoNota")]
        public async Task<IActionResult> PedidoNota()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/NFe/NFes")]
        public async Task<IActionResult> NFes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                bool portalCliente = IsLayoutClienteAtivo(unitOfWork);

                string pagina = portalCliente ? "NFesCliente.cshtml" : "NFes.cshtml";

                return View($"~/Areas/Relatorios/Views/NFe/{pagina}");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [CustomAuthorize("Relatorios/NFe/Notas")]
        public async Task<IActionResult> Notas()
        {
            return View();
        }
    }
}
