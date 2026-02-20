using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class EixoVeiculo : RepositorioBase<Dominio.Entidades.EixoVeiculo>, Dominio.Interfaces.Repositorios.EixoVeiculo
    {
        public EixoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        new public int ContarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EixoVeiculo>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj.Codigo;
            return result.Count();
        }

        public List<Dominio.Entidades.EixoVeiculo> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EixoVeiculo>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public Dominio.Entidades.EixoVeiculo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EixoVeiculo>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.EixoVeiculo> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.EixoVeiculo>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.AddOrder(NHibernate.Criterion.Order.Asc("Descricao"));
            criteria.AddOrder(NHibernate.Criterion.Order.Asc("OrdemEixo"));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.EixoVeiculo>();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.EixoVeiculo>();
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
