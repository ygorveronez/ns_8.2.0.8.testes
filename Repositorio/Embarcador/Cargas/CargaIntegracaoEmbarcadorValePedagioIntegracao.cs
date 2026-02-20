using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoEmbarcadorValePedagioIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao>
    {
        public CargaIntegracaoEmbarcadorValePedagioIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao BuscarPorCodigoCargaIntegracaoEmbarcadorENroComprovante(long codigoCargaIntegracaoEmbarcador, string numeroValePedagio)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao>();

            query = query.Where(o => o.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador && o.NumeroValePedagio == numeroValePedagio);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao BuscarPorCargaEmbarcadorCodigoIntegracaoValePedagioEmbarcador(long codigoCargaIntegracaoEmbarcador, int codigoIntegracaoValePedagioEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao>();

            query = query.Where(o => o.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador && o.CodigoIntegracaoValePedagioEmbarcador == codigoIntegracaoValePedagioEmbarcador);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao> BuscarPorCargaIntegracaoEmbarcador(long codigoCargaIntegracaoEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao>();

            query = query.Where(o => o.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador);

            return query.ToList();
        }
    }
}