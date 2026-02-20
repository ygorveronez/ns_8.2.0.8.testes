using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class TipoDespesa : RepositorioBase<Dominio.Entidades.TipoDespesa>, Dominio.Interfaces.Repositorios.TipoDespesa
    {
        public TipoDespesa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.TipoDespesa BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDespesa>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.TipoDespesa> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.TipoDespesa>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.TipoDespesa>();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.TipoDespesa>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }
    }
}
