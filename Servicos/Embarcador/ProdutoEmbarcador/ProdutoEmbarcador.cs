using Dominio.Excecoes.Embarcador;

namespace Servicos.Embarcador.ProdutoEmbarcador
{
    public class ProdutoEmbarcador
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;

        public ProdutoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador CriarProdutoEmbarcador(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto objetoProduto)
        {
            Repositorio.Embarcador.Produtos.GrupoProduto repositorioGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador()
            {
                CodigoProdutoEmbarcador = objetoProduto.CodigoProduto,
                Descricao = objetoProduto.DescricaoProduto,
                GrupoProduto = !string.IsNullOrEmpty(objetoProduto.CodigoGrupoProduto) ? repositorioGrupoProduto.BuscarPorCodigoEmbarcador(objetoProduto.CodigoGrupoProduto) : null,
                CodigoCEAN = objetoProduto.CodigocEAN,
                PesoUnitario = objetoProduto.PesoUnitario,
                MetroCubito = objetoProduto.MetroCubito,
                QtdPalet = objetoProduto.QuantidadePallet,
                QuantidadeCaixaPorPallet = objetoProduto.QuantidadeCaixaPorPallet,
                Ativo = true,
                Integrado = false
            };

            repositorioProdutoEmbarcador.Inserir(produtoEmbarcador);

            return produtoEmbarcador;
        }


        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador CriarProdutoEmbarcadorComRestricaoGrupoProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto objetoProduto)
        {
            Repositorio.Embarcador.Produtos.GrupoProduto repositorioGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = null;

            grupoProduto = repositorioGrupoProduto.BuscarPorCodigoEmbarcador(objetoProduto.CodigoGrupoProduto);

            if (grupoProduto == null && string.IsNullOrEmpty(objetoProduto.DescricaoGrupoProduto))
                throw new ServicoException("Produto não cadastrado, descrição do grupo de produto não informada, cadastro não realizado");

            if (grupoProduto == null && !string.IsNullOrEmpty(objetoProduto.DescricaoGrupoProduto))
            {
                grupoProduto = new Dominio.Entidades.Embarcador.Produtos.GrupoProduto()
                {
                    Ativo = true,
                    CodigoGrupoProdutoEmbarcador = objetoProduto.CodigoGrupoProduto,
                    Descricao = objetoProduto.DescricaoGrupoProduto
                };
                repositorioGrupoProduto.Inserir(grupoProduto);
            }

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador()
            {
                CodigoProdutoEmbarcador = objetoProduto.CodigoProduto,
                Descricao = objetoProduto.DescricaoProduto,
                GrupoProduto = grupoProduto,
                CodigoCEAN = objetoProduto.CodigocEAN,
                PesoUnitario = objetoProduto.PesoUnitario,
                MetroCubito = objetoProduto.MetroCubito,
                QtdPalet = objetoProduto.QuantidadePallet,
                QuantidadeCaixaPorPallet = objetoProduto.QuantidadeCaixaPorPallet,
                Ativo = true,
                Integrado = false
            };

            repositorioProdutoEmbarcador.Inserir(produtoEmbarcador);

            return produtoEmbarcador;
        }

    }
}
