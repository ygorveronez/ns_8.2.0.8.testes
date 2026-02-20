using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoEmbarcadorValePedagio : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio>
    {
        public CargaIntegracaoEmbarcadorValePedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio> BuscarPorCargaIntegracaoEmbarcador(long codigoCargaIntegracaoEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio>();

            query = query.Where(o => o.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio BuscarPorCargaIntegracaoEmbarcadorENroComprovante(long codigoCargaIntegracaoEmbarcador, string numeroComprovante)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio>();

            query = query.Where(o => o.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador && o.NumeroComprovante == numeroComprovante);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio BuscarPorCargaEmbarcadorCodigoValePedagioEmbarcador(long codigoCargaIntegracaoEmbarcador, int codigoValePedagioEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio>();

            query = query.Where(o => o.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador && o.CodigoValePedagioEmbarcador == codigoValePedagioEmbarcador);

            return query.FirstOrDefault();
        }
    }
}