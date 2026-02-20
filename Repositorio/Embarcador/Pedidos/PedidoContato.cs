using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoContato : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoContato>
    {
        public PedidoContato(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoContato> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoContato>();
            var result = query.Where(o => o.Pedido.Codigo == codigoPedido);

            return result.ToList();
        }
    }
}
