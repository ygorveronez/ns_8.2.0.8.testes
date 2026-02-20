using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoTratativa : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa>
    {
        public PedidoTratativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa>();
            var resut = from obj in query where obj.Pedido.Codigo == codigoPedido orderby obj.Data ascending select obj;
            return resut.ToList();
        }
    }
}
