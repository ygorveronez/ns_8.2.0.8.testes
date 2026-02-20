using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    public class ProdutosController : BaseController
    {
        #region Construtores

        public ProdutosController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Produtos/GrupoProduto")]
        public async Task<IActionResult> GrupoProduto()
        {
            return View();
        }

        [CustomAuthorize("Produtos/Produto")]
        public async Task<IActionResult> Produto()
        {
            return View();
        }

        [CustomAuthorize("Produtos/ProdutoEmbarcador")]
        public async Task<IActionResult> ProdutoEmbarcador()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

                var configuracoes = new
                {
                    UtilizarPesoProdutoParaCalcularPesoCarga = configuracaoGeralCarga?.UtilizarPesoProdutoParaCalcularPesoCarga ?? false,
                    ControlarOrganizacaoProdutos = configuracaoGeral?.ControlarOrganizacaoProdutos ?? false,
                };

                ViewBag.Configuracoes = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoes);

                return View();
            }
        }

        [CustomAuthorize("Produtos/AlteracaoProduto")]
        public async Task<IActionResult> AlteracaoProduto()
        {
            return View();
        }

        [CustomAuthorize("Produtos/UnidadeMedidaFornecedor")]
        public async Task<IActionResult> UnidadeMedidaFornecedor()
        {
            return View();
        }

        [CustomAuthorize("Produtos/GrupoProdutoTMS")]
        public async Task<IActionResult> GrupoProdutoTMS()
        {
            return View();
        }

        [CustomAuthorize("Produtos/ProdutoNCMAbastecimento")]
        public async Task<IActionResult> ProdutoNCMAbastecimento()
        {
            return View();
        }

        [CustomAuthorize("Produtos/LocalArmazenamentoProduto")]
        public async Task<IActionResult> LocalArmazenamentoProduto()
        {
            return View();
        }

        [CustomAuthorize("Produtos/MarcaProduto")]
        public async Task<IActionResult> MarcaProduto()
        {
            return View();
        }

        [CustomAuthorize("Produtos/ProdutoOpentech")]
        public async Task<IActionResult> ProdutoOpentech()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralOpenTech configuracaoIntegracaoOpenTech = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralOpenTech(unitOfWork).Buscar();

                var configuracoes = new
                {
                    ConsiderarLocalidadeProdutoIntegracaoEntrega = configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false,
                };

                ViewBag.Configuracoes = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoes);

                return View();
            }
        }

        [CustomAuthorize("Produtos/TipoEmbalagem")]
        public async Task<IActionResult> TipoEmbalagem()
        {
            return View();
        }

        [CustomAuthorize("Produtos/OrdemDeCompra")]
        public async Task<IActionResult> OrdemDeCompra()
        {
            return View();
        }

        [CustomAuthorize("Produtos/MotivoFalhaGTA")]
        public async Task<IActionResult> MotivoFalhaGTA()
        {
            return View();

        }

        [CustomAuthorize("Produtos/ConversaoDeUnidades")]
        public async Task<IActionResult> ConversaoDeUnidades()
        {
            return View();
        }

        [CustomAuthorize("Produtos/VincularProdutosFornecedorEmbarcadorPorNFe")]
        public async Task<IActionResult> VincularProdutosFornecedorEmbarcadorPorNFe()
        {
            return View();
        }

    }
}
