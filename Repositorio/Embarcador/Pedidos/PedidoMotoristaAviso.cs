using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoMotoristaAviso : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoMotoristaAviso>
    {
        public PedidoMotoristaAviso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public int BuscarProximaSequencia(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoMotoristaAviso>();
            query = query.Where(o => o.Pedido.Codigo == codigoPedido);

            int? ultimoNumero = query.Max(o => (int?)o.NumeroSequencia);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoMotoristaAviso> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoMotoristaAviso>();
            var resut = from obj in query where obj.Pedido.Codigo == codigoPedido orderby obj.NumeroSequencia ascending select obj;
            return resut.ToList();
        }

        #endregion
    }
}
