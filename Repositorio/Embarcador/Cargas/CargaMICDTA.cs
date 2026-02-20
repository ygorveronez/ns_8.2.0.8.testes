using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMICDTA : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA>
    {
        public CargaMICDTA(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ProximoNumeroSequencial(string sigla, string licenca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA>()
                .Where(obj => obj.SiglaPaisOrigem == sigla && obj.NumeroLicencaTNTI == licenca);

            return (query.Max(c => (int?)c.NumeroSequencial) ?? 1) + 1;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA> BuscarPorCargas(List<int> codigosCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA>()
                .Where(obj => codigosCargas.Contains(obj.CargaOrigem.Codigo));

            return query
                .ToList();
        }

        public int ContarConsultaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA>();

            var result = from obj in query select obj;
            if (codigoCarga > 0)
                result = result.Where(obj => obj.Carga.Codigo == codigoCarga);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMICDTA BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA>()
                .Where(obj => obj.CargaOrigem.Codigo == codigoCarga);

            return query
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA> ConsultaPorCarga(int carga, string numero, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultaPorCarga(carga, numero);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorCarga(int carga, string numero)
        {
            var result = _ConsultaPorCarga(carga, numero);

            return result.Count();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA> _ConsultaPorCarga(int carga, string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA>();

            var result = from obj in query
                         where obj.Carga.Codigo == carga
                         select obj;

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(o => o.Numero.Contains(numero));

            return result;
        }
    }
}
