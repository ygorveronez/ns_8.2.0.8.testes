using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaParcela : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaParcela>
    {
        public FaturaParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.FaturaParcela BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaParcela>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> BuscarPorFatura(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaParcela>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Fetch(o => o.Fatura).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> BuscarPorFaturaSemFetch(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaParcela>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaParcela BuscarPrimeiraPorFatura(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaParcela>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Fetch(o => o.Fatura).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> BuscarPorFatura(List<int> codigosFaturas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaParcela>();
            var result = from obj in query where codigosFaturas.Contains(obj.Fatura.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> BuscarPorFatura(int codigoFatura, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaParcela>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaParcela>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;
            return result.Count();
        }

    }
}
