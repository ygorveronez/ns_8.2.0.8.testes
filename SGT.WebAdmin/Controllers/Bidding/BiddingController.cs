using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Bidding
{
    public class BiddingController : BaseController
    {
		#region Construtores

		public BiddingController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Bidding/BiddingConvite")]
        public async Task<IActionResult> BiddingConvite()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {

                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = repositorioConfiguracaoBidding.BuscarConfiguracaoPadrao();

                ViewBag.ConfiguracaoBidding = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoBidding.PermiteSelecionarMaisDeUmaOfertaPorBidding,
                    configuracaoBidding.PermiteRemoverObrigatoriedadeDatas,
                    configuracaoBidding.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding,
                });

                return View();
            }
        }

        [CustomAuthorize("Bidding/BiddingAceitacao")]
        public async Task<IActionResult> BiddingAceitacao()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = repositorioConfiguracaoBidding.BuscarConfiguracaoPadrao();

                ViewBag.ConfiguracaoBidding = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoBidding.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding
                });
            }

            return View();
        }

        [CustomAuthorize("Bidding/BiddingAvaliacao")]
        public async Task<IActionResult> BiddingAvaliacao(string tokenAcesso)
        {
            ViewBag.TokenAcessoAvaliacao = !string.IsNullOrEmpty(tokenAcesso) ? Servicos.Criptografia.Descriptografar(tokenAcesso, "BIDDING-AVALIACAO") : Newtonsoft.Json.JsonConvert.SerializeObject("NaoInformado");

            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = repositorioConfiguracaoBidding.BuscarConfiguracaoPadrao();

                ViewBag.ConfiguracaoBidding = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoBidding.PermiteRemoverObrigatoriedadeDatas
                });
            }

            return View();
        }

        [CustomAuthorize("Bidding/TipoBidding")]
        public async Task<IActionResult> TipoBidding()
        {
            return View();
        }

        [CustomAuthorize("Bidding/RegrasAutorizacaoBidding")]
        public async Task<IActionResult> RegrasAutorizacaoBidding()
        {
            return View();
        }

        [CustomAuthorize("Bidding/AutorizacaoBidding")]
        public async Task<IActionResult> AutorizacaoBidding()
        {
            return View();
        }

        [CustomAuthorize("Bidding/TipoBaseline")]
        public async Task<IActionResult> TipoBaseline()
        {
            return View();
        }

        [CustomAuthorize("Bidding/RFIConvite")]
        public async Task<IActionResult> RFIConvite()
        {
            return View();
        }

        [CustomAuthorize("Bidding/BiddingChecklistQuestionarioPadrao")]
        public ActionResult BiddingChecklistQuestionarioPadrao()
        {
            return View();
        }
    }
}
