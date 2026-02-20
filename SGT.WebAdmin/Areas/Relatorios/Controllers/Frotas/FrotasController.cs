using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frotas
{
	[Area("Relatorios")]
	public class FrotasController : BaseController
    {
		#region Construtores

		public FrotasController(Conexao conexao) : base(conexao) { }

		#endregion

        public IActionResult Abastecimento()
        {
            return View();
        }

        public IActionResult Motorista()
        {
            return View();
        }

        public IActionResult Pedagio()
        {
            return View();
        }

        public IActionResult Multa()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/MultaParcela")]
        public IActionResult MultaParcela()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/DespesaVeiculo")]
        public IActionResult DespesaVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/DespesaOrdemServico")]
        public IActionResult DespesaOrdemServico()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/AbastecimentoNotaEntrada")]
        public IActionResult AbastecimentoNotaEntrada()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/ManutencaoVeiculo")]
        public IActionResult ManutencaoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/MotoristaExtratoSaldo")]
        public IActionResult MotoristaExtratoSaldo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/DespesaDetalhadaOrdemServico")]
        public IActionResult DespesaDetalhadaOrdemServico()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/RetornoAbastecimentoAngellira")]
        public IActionResult RetornoAbastecimentoAngellira()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/OrdemServicoPorMecanico")]
        public IActionResult OrdemServicoPorMecanico()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Frotas/DespesaOrdemServicoProduto")]
        public IActionResult DespesaOrdemServicoProduto()
        {
            return View();
        }        
        [CustomAuthorize("Relatorios/Frotas/AbastecimentoTicketLog")]
        public IActionResult AbastecimentoTicketLog()
        {
            return View();
        }
    }
}
