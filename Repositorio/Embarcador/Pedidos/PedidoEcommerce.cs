using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoEcommerce : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoEcommerce>
    {
        public PedidoEcommerce(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoEcommerce BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEcommerce>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;

            return result.FirstOrDefault();
        }
    }
}
