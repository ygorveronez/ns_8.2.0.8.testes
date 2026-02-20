using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class StageAgrupamentoPedidoRemovido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoPedidoRemovido>
    {
        public StageAgrupamentoPedidoRemovido(UnitOfWork unitOfWork) : base(unitOfWork) { }


        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoPedidoRemovido BuscarPorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoPedidoRemovido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoPedidoRemovido>();
            query = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return query.FirstOrDefault();
        }

        #endregion
    }
}
