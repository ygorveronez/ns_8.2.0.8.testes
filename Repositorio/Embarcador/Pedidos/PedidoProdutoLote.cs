using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoProdutoLote : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote>
    {
        public PedidoProdutoLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote> BuscarPedidoProdutosLotesPorPedidoProduto(List<int> codigosPedidoProdutoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote>();

            query = query.Where(obj => codigosPedidoProdutoLote.Contains(obj.PedidoProduto.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote BuscarPedidoProdutosLotesPorPedidoProduto(int codigoPedidoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote>();

            query = query.Where(obj => obj.PedidoProduto.Codigo == codigoPedidoProduto);

            return query.FirstOrDefault();
        }
    }
}