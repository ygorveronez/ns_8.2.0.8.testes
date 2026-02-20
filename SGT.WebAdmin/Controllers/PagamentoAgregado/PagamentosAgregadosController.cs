using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.PagamentoAgregado
{
    public class PagamentosAgregadosController : BaseController
    {
		#region Construtores

		public PagamentosAgregadosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("PagamentosAgregados/RegrasPagamentoAgregado")]
        public async Task<IActionResult> RegrasPagamentoAgregado()
        {
            return View();
        }

        [CustomAuthorize("PagamentosAgregados/AutorizacaoPagamentoAgregado")]
        public async Task<IActionResult> AutorizacaoPagamentoAgregado()
        {
            return View();
        }

        [CustomAuthorize("PagamentosAgregados/PagamentoAgregado")]
        public async Task<IActionResult> PagamentoAgregado()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repositorioConfiguracaoContratoFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFrete = repositorioConfiguracaoContratoFrete.BuscarConfiguracaoPadrao();

                var configuracoesPagamentoAoAgregado = new
                {
                    configuracaoContratoFrete.HabilitarLayoutFaturaPagamentoAgregado,
                    LimitarOperacaoPorEmpresa = this.Usuario?.LimitarOperacaoPorEmpresa ?? false,
                };

                ViewBag.ConfiguracoesPagamentoAoAgregado = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesPagamentoAoAgregado);
            }

            return View();
        }

        [CustomAuthorize("PagamentosAgregados/AjudanteCarga")]
        public async Task<IActionResult> AjudanteCarga()
        {
            return View();
        }

    }

}
