using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Veiculos
{
    public class TecnologiaRastreadorCodigoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao>
    {
        public TecnologiaRastreadorCodigoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao> BuscarPorTecnologiaRastreador(Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao>();

            query = query.Where(o => o.TecnologiaRastreador == tecnologiaRastreador);

            return query.Fetch(o => o.TipoIntegracao).ToList();
        }

        public Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao BuscarPorTecnologiaRastreadorETipoIntegracao(Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao>();

            query = query.Where(o => o.TecnologiaRastreador == tecnologiaRastreador && o.TipoIntegracao == tipoIntegracao);

            return query.FirstOrDefault();
        }
    }
}
