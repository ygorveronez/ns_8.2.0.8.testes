using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto>
    {
        public PedidoAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto>();
            var result = query.Where(o => o.Pedido.Codigo == codigoPedido);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto> BuscarPorCarga(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto>();
            var result = query.Where(o => queryCargaPedido.Any(p => o.Pedido.Codigo == p.Pedido.Codigo));

            return result.ToList();
        }
    }
}
