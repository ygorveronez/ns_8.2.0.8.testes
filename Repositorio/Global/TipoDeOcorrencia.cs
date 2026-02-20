using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class TipoDeOcorrencia: RepositorioBase<Dominio.Entidades.TipoDeOcorrencia>, Dominio.Interfaces.Repositorios.TipoDeOcorrencia
    {
        public TipoDeOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.TipoDeOcorrencia BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.TipoDeOcorrencia> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.TipoDeOcorrencia>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Status", status));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.TipoDeOcorrencia>();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.TipoDeOcorrencia>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }
    }
}
