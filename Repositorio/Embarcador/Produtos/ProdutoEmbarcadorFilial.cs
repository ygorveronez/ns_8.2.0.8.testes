using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class ProdutoEmbarcadorFilial : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial>
    {
        #region Construtores

        public ProdutoEmbarcadorFilial(UnitOfWork unitOfWork) : base (unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial> BuscarFiliaisPorProdutoEmbarcador(int codigoProdutoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial>()
                .Where(o => o.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial BuscarPorProdutoEmbarcadorEFilial(int codigoProdutoEmbarcador, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial>()
                .Where(o => o.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador && o.Filial.Codigo == codigoFilial);

            return query.FirstOrDefault();
        }

        public UsoMaterial? BuscarUsoMaterialPorCargaPedido(int codigoCargaPedido)
        {
            (int CodigoProduto, int CodigoFilial) dadosCargaPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>()
                .Where(cargaPedidoProduto => cargaPedidoProduto.CargaPedido.Codigo == codigoCargaPedido)
                .OrderByDescending(cargaPedidoProduto => cargaPedidoProduto.Quantidade * cargaPedidoProduto.ValorUnitarioProduto)
                .Select(cargaPedidoProduto => ValueTuple.Create(cargaPedidoProduto.Produto.Codigo, (cargaPedidoProduto.CargaPedido.Pedido.Filial == null ? 0 : cargaPedidoProduto.CargaPedido.Pedido.Filial.Codigo)))
                .FirstOrDefault();

            var consultaProdutoEmbarcadorFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial>()
                .Where(produtoFilial =>
                    produtoFilial.Ativo == true &&
                    produtoFilial.ProdutoEmbarcador.Codigo == dadosCargaPedidoProduto.CodigoProduto &&
                    produtoFilial.Filial.Codigo == dadosCargaPedidoProduto.CodigoFilial
                );

            return consultaProdutoEmbarcadorFilial
                .Select(o => (UsoMaterial?)o.UsoMaterial)
                .FirstOrDefault();
        }

        public bool ProdutoEmbarcadorFilialExistente(int codigoProdutoEmbarcador, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial>()
                .Where(o => o.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador && o.Filial.Codigo == codigoFilial);

            return query.Any();
        }

        #endregion Métodos Públicos
    }
}
