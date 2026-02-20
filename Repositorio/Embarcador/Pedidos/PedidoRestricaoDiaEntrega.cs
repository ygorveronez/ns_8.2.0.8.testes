using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoRestricaoDiaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoRestricaoDiaEntrega>
    {
        public PedidoRestricaoDiaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoRestricaoDiaEntrega> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoRestricaoDiaEntrega>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;

            return result.ToList();
        }
    }
}
