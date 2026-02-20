using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class ItemPreFaturaNatura : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura>
    {
        public ItemPreFaturaNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura BuscarPorCargaCTe(int codigoPreFatura, int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura>();

            query = query.Where(o => o.CargaCTe.Codigo == codigoCargaCTe && o.PreFatura.Codigo == codigoPreFatura);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura> Consultar(int codigoPreFatura, int numero, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura>();

            if (codigoPreFatura > 0)
                query = query.Where(o => o.PreFatura.Codigo == codigoPreFatura);

            if (numero > 0)
                query = query.Where(o => o.CargaCTe.CTe.Numero == numero);

            return query.Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoPreFatura, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura>();

            if (codigoPreFatura > 0)
                query = query.Where(o => o.PreFatura.Codigo == codigoPreFatura);

            if (numero > 0)
                query = query.Where(o => o.CargaCTe.CTe.Numero == numero);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura BuscarPorCodigo(int codigoItemPreFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura>();

            query = query.Where(o => o.Codigo == codigoItemPreFatura);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura> BuscarCanceladosPorPreFatura(int codigoPreFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura>();

            query = query.Where(o => o.PreFatura.Codigo == codigoPreFatura && (o.CargaCTe.CTe.Status == "C" || o.CargaCTe.CTe.Status == "Z"));

            return query.ToList();
        }
    }
}
