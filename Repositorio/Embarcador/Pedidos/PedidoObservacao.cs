using System;
using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminMultisoftware.Dominio.Entidades.Pessoas;
using Dominio.Entidades;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoObservacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoObservacao>
    {
        #region Construtores
        public PedidoObservacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoObservacao> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoObservacao>()
                .Where(obj => obj.Pedido != null && obj.Pedido.Codigo == codigoPedido)
                .OrderByDescending(obj => obj.DataHoraInclusao);
            return query.ToList();
        }

        #endregion
    }
}
