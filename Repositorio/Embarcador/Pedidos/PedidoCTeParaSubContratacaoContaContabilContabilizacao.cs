using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoCTeParaSubContratacaoContaContabilContabilizacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao>
    {
        public PedidoCTeParaSubContratacaoContaContabilContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao>();
            var result = from obj in query where obj.PedidoCTeParaSubContratacao.CargaPedido.Carga.Codigo == carga select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }

        public int DeletarPorCarga(int carga)
        {
            string hql = "DELETE PedidoCTeParaSubContratacaoContaContabilContabilizacao ped where ped.PedidoCTeParaSubContratacao.Codigo in (select pss.Codigo from PedidoCTeParaSubContratacao pss where pss.CargaPedido.Codigo in (select cp.Codigo from CargaPedido cp where cp.Carga.Codigo = :Carga))";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Carga", carga);
            return query.ExecuteUpdate();
        }
    }
}
