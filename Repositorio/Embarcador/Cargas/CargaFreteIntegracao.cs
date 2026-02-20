using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaFreteIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao>
    {
        #region Construtores
        public CargaFreteIntegracao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaFreteIntegracao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> ExisteIntegracoesCargaFreteParaEstaCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao>();
            query = from obj in query
                    where obj.Carga.Codigo == codigoCarga && (obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                    obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                    select obj;
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> consulta = Consultar(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigoCarga });
            return consulta.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public bool ExistePorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> consulta = Consultar(filtroPesquisa);
            return ObterLista(consulta, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> consulta = Consultar(filtroPesquisa);
            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> RegistrosPendentesDeIntegracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao>();

            query = from obj in query 
                    where obj.Carga.ValorFrete > 0m 
                    && 
                    ((  obj.Carga.AguardandoIntegracaoFrete == true ||
                        obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.RejeicaoCte ||
                        obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster) ||
                        obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace
                    )
                    && 
                    (obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao || (obj.NumeroTentativas < 3 && obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao))

                    select obj;

            return query.ToList();
        }

        public bool ExisteIntegracaoParaEstaStage(int codigoStage, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao>();

            query = from obj in query where obj.Stage.Codigo == codigoStage && tipoIntegracao == obj.TipoIntegracao.Tipo select obj;

            return query.Any();
        }

        public int removerStageIntegraca(int CodigoStage)
        {
            string hql = "update CargaFreteIntegracao set Stage = null where Stage = :stage";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("stage", CodigoStage);
            return query.ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao BuscarPorStageECarga(int codigoStage, int codCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao>();

            query = from obj in query where obj.Carga.Codigo == codCarga && obj.Stage.Codigo == codigoStage && tipoIntegracao == obj.TipoIntegracao.Tipo select obj;

            return query.FirstOrDefault();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao>();

            if (filtroPesquisa.Codigo > 0)
                query = query.Where(r => r.Carga.Codigo == filtroPesquisa.Codigo);

            if (filtroPesquisa.Situacao.HasValue)
                query = query.Where(s => s.SituacaoIntegracao == filtroPesquisa.Situacao.Value);

            return query;

        }

        #endregion Métodos Privados
    }
}
