using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPreChekinIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao>
    {
        #region Construtor
        public CargaPreChekinIntegracao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> consulta = Consultar(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigoCarga });
            return consulta.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao BuscarRegistroIntegrado(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> consulta = Consultar(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigoCarga, Situacao = SituacaoIntegracao.Integrado });
            return consulta.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> consulta = Consultar(filtroPesquisa);
            return ObterLista(consulta, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> consulta = Consultar(filtroPesquisa);
            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> RegistrosPendentesDeIntegracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao>();

            query = from obj in query where obj.Carga.AguardandoIntegracaoFrete == true && (obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao || (obj.NumeroTentativas < 3 && obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)) select obj;

            return query.ToList();
        }

        #endregion

        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao>();

            if (filtroPesquisa.Codigo > 0)
                query = query.Where(r => r.Carga.Codigo == filtroPesquisa.Codigo);

            if (filtroPesquisa.Situacao.HasValue)
                query = query.Where(s => s.SituacaoIntegracao == filtroPesquisa.Situacao.Value);

            return query;

        }

        #endregion
    }
}
