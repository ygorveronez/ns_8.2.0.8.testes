
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.Cargas
{
    public class PacoteWebHook : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook>
    {
        public PacoteWebHook(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.PacoteWebHook BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook> BuscarPacotesPendentesIntegracaoPasso1WebHook()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook>()
                .Fetch(p => p.CTeTerceiroXML)
                .Where(p => p.CTeTerceiroXML == null &&
                            //p.TipoIntegracao != null implementar sobre demanda observar que clientes irão rodar inicialmente como null 
                            (p.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                             p.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao));
            return query.Take(1000).ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook> BuscarPacotesPendentesIntegracaoPasso2WebHook()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook>()
                .Fetch(p => p.CTeTerceiroXML)
                .Where(p => p.CTeTerceiroXML != null &&
                (p.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                 p.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao));
            return query.Take(1000).ToList();
        }
    }
}


