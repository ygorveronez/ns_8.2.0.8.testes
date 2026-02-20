using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoEmbarcadorPedido : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido>
    {
        public CargaIntegracaoEmbarcadorPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido> BuscarPorCargaIntegracaoEmbarcador(long codigoCargaIntegracaoEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido>();

            query = query.Where(o => o.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido BuscarPrimeiroPorCargaIntegracaoEmbarcador(long codigoCargaIntegracaoEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido>();

            query = query.Where(o => o.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido BuscarPorCargaIntegracaoEmbarcadorEProtocolo(long codigoCargaIntegracaoEmbarcador, int codigoProtocolo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido>();

            query = query.Where(o => o.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador && o.Protocolo == codigoProtocolo);

            return query.FirstOrDefault();
        }
    }
}
