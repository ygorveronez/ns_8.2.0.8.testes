using Newtonsoft.Json;
using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
	[Area("Relatorios")]
	public class CargasController : BaseController
    {
		#region Construtores

		public CargasController(Conexao conexao) : base(conexao) { }

		#endregion


        [CustomAuthorize("Relatorios/Cargas/Carga")]
        public IActionResult Carga()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/DirecionamentoOperador")]
        public IActionResult DirecionamentoOperador()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/Quantidade")]
        public IActionResult Quantidade()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/TaxaOcupacaoVeiculo")]
        public IActionResult TaxaOcupacaoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/TaxaIncidenciaFrete")]
        public IActionResult TaxaIncidenciaFrete()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/Pedido")]
        public IActionResult Pedido()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/Encaixe")]
        public IActionResult Encaixe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/DocumentoEmissaoNFSManual")]
        public IActionResult DocumentoEmissaoNFSManual()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/CargaProduto")]
        public IActionResult CargaProduto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/CargaProdutoTransportador")]
        public IActionResult CargaProdutoTransportador()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/PedidoProduto")]
        public IActionResult PedidoProduto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/AlteracaoFrete")]
        public IActionResult AlteracaoFrete()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/PreCarga")]
        public async Task<IActionResult> PreCarga(CancellationToken cancellationToken)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorio.BuscarPrimeiroRegistroAsync();

                ViewBag.ConfiguracoesPreCarga = JsonConvert.SerializeObject(new
                {
                    configuracaoGeralCarga.UtilizarProgramacaoCarga
                });

                return View();
            }
        }

        [CustomAuthorize("Relatorios/Cargas/Paradas")]
        public IActionResult Paradas()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/AvaliacaoEntregaPedido")]
        public IActionResult AvaliacaoEntregaPedido()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/CargaIntegracao")]
        public IActionResult CargaIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/CargaPedidoEmbarcador")]
        public IActionResult CargaPedidoEmbarcador()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/CargaEntregaPedido")]
        public IActionResult CargaEntregaPedido()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/CargaViagemEventos")]
        public IActionResult CargaViagemEventos()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/QuantidadeDescarga")]
        public IActionResult QuantidadeDescarga()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/RotaControleEntrega")]
        public IActionResult RotaControleEntrega()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/AgendamentoEntregaPedido")]
        public IActionResult AgendamentoEntregaPedido()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/GestaoCarga")]
        public IActionResult GestaoCarga()
        {
            return View();
        }        

        [CustomAuthorize("Relatorios/Cargas/ControleEntrega")]
        public IActionResult ControleEntrega()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/CargaEntregaChecklist")]
        public IActionResult CargaEntregaChecklist()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/CargaIntegracaoDadosTransportes")]
        public IActionResult CargaIntegracaoDadosTransportes()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/Pacotes")]
        public IActionResult Pacotes()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/HistoricoVinculo")]
        public IActionResult HistoricoVinculo()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/CargasComInteresseTransportadorTerceiro")]
        public IActionResult CargasComInteresseTransportadorTerceiro()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Cargas/ModeloVeicularCarga")]
        public IActionResult ModeloVeicularCarga()
        {
            return View();
        }
    }
}
