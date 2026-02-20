using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class Atividade : RepositorioBase<Dominio.Entidades.Atividade>, Dominio.Interfaces.Repositorios.Atividade
    {
        public Atividade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Atividade> ConsultarTodas(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Atividade>();
            var result = from obj in query select obj;
            
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsultaTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Atividade>();
            var result = from obj in query select obj;
            
            return result.Count();
        }


        public IList<Dominio.Entidades.Atividade> Consulta(string descricao, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Atividade>();
            var codigo = 0;
            if (int.TryParse(descricao, out codigo))
                criteria.Add(new NHibernate.Criterion.OrExpression(NHibernate.Criterion.Expression.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere), NHibernate.Criterion.Expression.Eq("Codigo", codigo)));
            else
                criteria.Add(NHibernate.Criterion.Expression.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            criteria.AddOrder(NHibernate.Criterion.Order.Asc("Descricao"));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.Atividade>();
        }

        public int ContarConsulta(string descricao)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Atividade>();
            var codigo = 0;
            if (int.TryParse(descricao, out codigo))
                criteria.Add(new NHibernate.Criterion.OrExpression(NHibernate.Criterion.Expression.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere), NHibernate.Criterion.Expression.Eq("Codigo", codigo)));
            else
                criteria.Add(NHibernate.Criterion.Expression.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public Dominio.Entidades.Atividade BuscarPorCodigo(int codigo, List<Dominio.Entidades.Atividade> lstAtividade = null)
        {
            if (lstAtividade != null)
                return lstAtividade.Where(obj => obj.Codigo == codigo).FirstOrDefault();

            return this.SessionNHiBernate.Query<Dominio.Entidades.Atividade>().Where(obj => obj.Codigo == codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Atividade BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Atividade>().Where(obj => obj.Codigo == codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Atividade BuscarPrimeiraAtividade()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Atividade>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Atividade> Consultar(string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Atividade>();

            var result = from obj in query select obj;

            var codigo = 0;
            if (int.TryParse(descricao, out codigo))
                result = result.Where(obj => obj.Codigo == codigo);
            else if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }
    }
}
