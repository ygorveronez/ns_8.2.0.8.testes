using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao>
    {
        public GestaoDevolucaoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao BuscarPorCodigo(long codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> BuscarIntegracoesPendentes()
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao>();
            query = query.Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                    (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && o.NumeroTentativas < 3));
            return query.Take(10).ToList();
        }

        public bool ExistePendenteParaGestaoDevolucaoETipo(long codigoGestaoDevolucao, int codigoTipoIntegracao, TipoIntegracaoGestaoDevolucao tipoIntegracaoGestaoDevolucao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao>();
            query = query.Where(o => (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                      (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && o.NumeroTentativas < 3))
                                    && o.GestaoDevolucao.Codigo == codigoGestaoDevolucao
                                    && o.TipoIntegracao.Codigo == codigoTipoIntegracao
                                    && o.TipoIntegracaoGestaoDevolucao == tipoIntegracaoGestaoDevolucao);
            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao BuscarPorGestaoDevolucaoETipo(long codigoGestaoDevolucao, int codigoTipoIntegracao, TipoIntegracaoGestaoDevolucao tipoIntegracaoGestaoDevolucao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao>();
            query = query.Where(o => (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                      (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && o.NumeroTentativas < 3))
                                    && o.GestaoDevolucao.Codigo == codigoGestaoDevolucao
                                    && o.TipoIntegracao.Codigo == codigoTipoIntegracao
                                    && o.TipoIntegracaoGestaoDevolucao == tipoIntegracaoGestaoDevolucao);
            return query.FirstOrDefault();
        }

        public int ContarPorGestaoDevolucao(long codigoGestaoDevolucao, SituacaoIntegracao situacao, List<TipoIntegracaoGestaoDevolucao> listaTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao>();
            query = query.Where(obj => obj.GestaoDevolucao.Codigo == codigoGestaoDevolucao && obj.SituacaoIntegracao == situacao && listaTipoIntegracao.Contains(obj.TipoIntegracaoGestaoDevolucao));
            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> consultaCargaMonitoramentoLogisticoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return consultaCargaMonitoramentoLogisticoIntegracao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> Consultar(int codigoGestaoDevolucao, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGestaoDevolucao> tipoIntegracaoGestaoDevolucao)
        {
            return ObterQueryConsulta(codigoGestaoDevolucao, situacao, tipo, tipoIntegracaoGestaoDevolucao)
                .OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoGestaoDevolucao, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGestaoDevolucao> tipoIntegracaoGestaoDevolucao)
        {
            return ObterQueryConsulta(codigoGestaoDevolucao, situacao, tipo, tipoIntegracaoGestaoDevolucao).Count();
        }

        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> ObterQueryConsulta(int codigoGestaoDevolucao, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGestaoDevolucao> tipoIntegracaoGestaoDevolucao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao>();

            if (codigoGestaoDevolucao > 0)
                query = query.Where(o => o.GestaoDevolucao.Codigo == codigoGestaoDevolucao);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo);

            if (tipoIntegracaoGestaoDevolucao != null)
                query = query.Where(o => tipoIntegracaoGestaoDevolucao.Contains(o.TipoIntegracaoGestaoDevolucao));

            return query;
        }
        #endregion
    }
}
