using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoTransbordo : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>
    {
        public PedidoTransbordo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> BuscarPorPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            var result = from obj in query where obj.Pedido.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo BuscarTransbordoExistente(int sequencia, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Sequencia == sequencia select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> BuscarPorCarga(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultCargaPedido = queryCargaPedido.Where(c => c.Carga.Codigo == codigoCarga).Select(o => o.Pedido.Codigo);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            query = query.Where(p => resultCargaPedido.Contains(p.Pedido.Codigo));

            return query.ToList();
        }
    }
}
