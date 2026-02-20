using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ServicoVeiculo : RepositorioBase<Dominio.Entidades.ServicoVeiculo>, Dominio.Interfaces.Repositorios.ServicoVeiculo
    {
        public ServicoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ServicoVeiculo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoVeiculo>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.ServicoVeiculo> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ServicoVeiculo>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.ServicoVeiculo>();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ServicoVeiculo>();
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
