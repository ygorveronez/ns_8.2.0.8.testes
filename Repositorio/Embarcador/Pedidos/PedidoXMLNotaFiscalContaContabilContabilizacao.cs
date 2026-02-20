using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoXMLNotaFiscalContaContabilContabilizacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao>
    {
        public PedidoXMLNotaFiscalContaContabilContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == carga select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }

        public int DeletarPorCargaPedido(int cargaPedido)
        {
            string hql = "DELETE PedidoXMLNotaFiscalContaContabilContabilizacao ped where ped.PedidoXMLNotaFiscal in (select pnf.Codigo from PedidoXMLNotaFiscal pnf where pnf.CargaPedido = :CargaPedido)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("CargaPedido", cargaPedido);
            return query.ExecuteUpdate();
        }

        public int DeletarPorCarga(int carga)
        {
            string hql = "DELETE PedidoXMLNotaFiscalContaContabilContabilizacao ped where ped.PedidoXMLNotaFiscal in (select pnf.Codigo from PedidoXMLNotaFiscal pnf where pnf.CargaPedido in (select cp.Codigo from CargaPedido cp where cp.Carga = :Carga))";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Carga", carga);
            return query.ExecuteUpdate();
        }


        public int DeletarPorPedidoNotaFiscal(int pedidoNotaFiscal)
        {
            string hql = "DELETE PedidoXMLNotaFiscalContaContabilContabilizacao ped where ped.PedidoXMLNotaFiscal = :PedidoXMLNotaFiscal";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("PedidoXMLNotaFiscal", pedidoNotaFiscal);
            return query.ExecuteUpdate();
        }

    }
}
