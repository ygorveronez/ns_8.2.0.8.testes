using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Logistica
{
    public class HistoricoAgendamentoEntregaPedido : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido>
    {
        #region Construtores
        public HistoricoAgendamentoEntregaPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido> BuscarPorPedido(int codigoPedido)
        {
            IOrderedQueryable<Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido>()
                .Where(obj => obj.Pedido != null && obj.Pedido.Codigo == codigoPedido)
                .OrderBy(obj => obj.DataHoraRegistro);
            return query.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido> BuscarPorPedidos(List<int> codigosPedidos)
        {
            IOrderedQueryable<Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido>()
                .Where(obj => obj.Pedido != null && codigosPedidos.Contains(obj.Pedido.Codigo))
                .OrderBy(obj => obj.DataHoraRegistro).ThenBy(obj => obj.Pedido.Codigo);
            return query.ToList();
        }
        #endregion
    }
}
