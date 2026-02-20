using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	public class FretesController : BaseController
    {
		#region Construtores

		public FretesController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Fretes/FreteComponentes")]
        public IActionResult FreteComponentes()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/ComissaoGrupoProduto")]
        public IActionResult ComissaoGrupoProduto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/ExtracaoMassivaTabelaFrete")]
        public IActionResult ExtracaoMassivaTabelaFrete()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/ComissaoProduto")]
        public IActionResult ComissaoProduto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/TabelaFreteRota")]
        public IActionResult TabelaFreteRota()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/FreteTerceirizado")]
        public IActionResult FreteTerceirizado()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/ContratoFreteTransportador")]
        public IActionResult ContratoFreteTransportador()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/TransportadoresSemContrato")]
        public IActionResult TransportadoresSemContrato()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/ConfiguracaoSubcontratacaoTabelaFrete")]
        public IActionResult ConfiguracaoSubcontratacaoTabelaFrete()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/RotaFrete")]
        public IActionResult RotaFrete()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/ConfiguracaoTabelaFrete")]
        public IActionResult ConfiguracaoTabelaFrete()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/FreteTerceirizadoAcrescimoDesconto")]
        public IActionResult FreteTerceirizadoAcrescimoDesconto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/FreteTerceirizadoValePedagio")]
        public IActionResult FreteTerceirizadoValePedagio()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/ContratoFreteAcrescimoDesconto")]
        public IActionResult ContratoFreteAcrescimoDesconto()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/ProvisaoVolumetria")]
        public IActionResult ProvisaoVolumetria()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Fretes/FreteTerceirizadoPorCTe")]
        public IActionResult FreteTerceirizadoPorCTe()
        {
            return View();
        }
    }
}
