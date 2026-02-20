using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Pedidos
{

    public class PedidoInformacoesBancarias : RepositorioBase<Dominio.Entidades.Global.PedidoInformacoesBancarias>
    {
        #region Construtores

        public PedidoInformacoesBancarias(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PedidoInformacoesBancarias(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Método Públicos

        public Dominio.Entidades.Global.PedidoInformacoesBancarias BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.PedidoInformacoesBancarias>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.FirstOrDefault();
        }

        #endregion

    }
}