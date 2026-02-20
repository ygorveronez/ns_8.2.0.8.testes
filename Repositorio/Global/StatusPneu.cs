using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class StatusPneu : RepositorioBase<Dominio.Entidades.StatusPneu>, Dominio.Interfaces.Repositorios.StatusPneu
    {
        public StatusPneu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.StatusPneu BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.StatusPneu>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.StatusPneu> Consultar(int codigoEmpresa, string descricao, string tipo, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.StatusPneu>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(tipo))
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Tipo", tipo));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.AddOrder(NHibernate.Criterion.Order.Asc("Descricao"));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.StatusPneu>();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string tipo, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.StatusPneu>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(tipo))
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Tipo", tipo));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.StatusPneu> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.StatusPneu>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.ToList();
        }
    }
}
