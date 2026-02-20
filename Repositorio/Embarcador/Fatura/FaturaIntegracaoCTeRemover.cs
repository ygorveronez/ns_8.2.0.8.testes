using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaIntegracaoCTeRemover : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover>
    {
        public FaturaIntegracaoCTeRemover(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover> BuscarPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover BuscarPorCTeEFatura(int codigoCTe, int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.CTe.Codigo == codigoCTe);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover> Consultar(int codigoFatura, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Fetch(o => o.CTe).OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Count();
        }
    }
}
