using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class SeparacaoPedidoPedido : RepositorioBase<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido>
    {
        public SeparacaoPedidoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido> BuscarPorSeparacaoPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido>();
            var result = from obj in query where obj.SeparacaoPedido.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido> BuscarPorPedidos(List<int> codigosPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido>();

            var result = from obj in query where codigosPedido.Contains(obj.Pedido.Codigo) select obj;

            return result
                .Fetch(obj => obj.SeparacaoPedido)
                .Fetch(obj => obj.Pedido)
                .ToList();
        }

        public void RemoverDisponibilidadePedidosSeparacao(int separacaoPedido)
        {
            UnitOfWork.Sessao.CreateSQLQuery("update pedido set pedido.PED_DISPONIVEL_PARA_SEPARACAO = 0 from T_SEPARACAO_PEDIDO_PEDIDO pedidoSeparacao inner join t_pedido pedido on pedidoSeparacao.PED_CODIGO = pedido.PED_CODIGO where pedidoSeparacao.SPE_CODIGO = :SeparacaoPedido")
                .SetInt32("SeparacaoPedido", separacaoPedido).ExecuteUpdate();
        }
    }
}
