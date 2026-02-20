using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoContaContabilContabilizacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao>
    {
        public CargaPedidoContaContabilContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> BuscarPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }
        
        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao BuscarFirstOrDefaultPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .FirstOrDefault();
        }

        public int DeletarPorCarga(int carga)
        {
            string hql = "DELETE CargaPedidoContaContabilContabilizacao where CargaPedido.Codigo in (select obj.Codigo from CargaPedido obj where obj.Carga.Codigo = :Carga)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Carga", carga);
            return query.ExecuteUpdate();
        }
    }
}
