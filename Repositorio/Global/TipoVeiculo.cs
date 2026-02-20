using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class TipoVeiculo : RepositorioBase<Dominio.Entidades.TipoVeiculo>, Dominio.Interfaces.Repositorios.TipoVeiculo
    {
        public TipoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.TipoVeiculo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoVeiculo>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.TipoVeiculo BuscarPorDescricao(int codigoEmpresa, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoVeiculo>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Descricao.Equals(descricao) select obj;

            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.TipoVeiculo> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.TipoVeiculo>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.AddOrder(NHibernate.Criterion.Order.Asc("Descricao"));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.TipoVeiculo>();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.TipoVeiculo>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.TipoVeiculo> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoVeiculo>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" select obj;
            return result.ToList();
        }

        public int ContarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoVeiculo>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" select obj;
            return result.Count();
        }
    }
}
