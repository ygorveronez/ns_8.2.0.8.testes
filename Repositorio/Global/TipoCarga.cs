using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class TipoCarga : RepositorioBase<Dominio.Entidades.TipoCarga>, Dominio.Interfaces.Repositorios.TipoCarga
    {
        public TipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken ) { }

        public Dominio.Entidades.TipoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        
        public Dominio.Entidades.TipoCarga BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoCarga>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.TipoCarga> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.TipoCarga>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.TipoCarga>();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.TipoCarga>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.TipoCarga> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoCarga>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.TipoCarga>> BuscarPorCodigosAsync(List<int> codigos)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.TipoCarga>()
                .Where(x => codigos.Contains(x.Codigo)).ToListAsync();
        }

        public List<Dominio.Entidades.TipoCarga> Consultar(string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoCarga>();

            var result = from obj in query where obj.Status == "A" select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoCarga>();

            var result = from obj in query where obj.Status == "A" select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.Count();
        }
    }
}
