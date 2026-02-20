using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.CTeAgrupado
{
    public class CargaCTeAgrupadoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga>
    {
        public CargaCTeAgrupadoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga> BuscarPorCargaCTeAgrupado(int codigoCargaCTeAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado);

            return query.Fetch(o => o.Carga).ToList();
        }

        public List<int> BuscarCodigosPorCargaCTeAgrupadoSemCTeGerado(int codigoCargaCTeAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado && o.CargaCTeAgrupadoCTe == null);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga> BuscarPorCodigo(List<int> codigosCargaCTeAgrupadoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga>();

            query = query.Where(o => codigosCargaCTeAgrupadoCarga.Contains(o.Codigo));

            return query.Fetch(o => o.Carga).ToList();
        }

    }
}
