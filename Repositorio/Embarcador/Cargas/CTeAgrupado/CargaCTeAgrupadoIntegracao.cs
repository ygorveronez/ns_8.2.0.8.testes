using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.CTeAgrupado
{
    public class CargaCTeAgrupadoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>
    {
        public CargaCTeAgrupadoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorCargaCTeAgrupado(int codigoCargaCTeAgrupada, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query where obj.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query
                         where
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> BuscarCargaCTeAgrupadoIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query
                         where
                         !obj.CargaCTeAgrupado.GerandoIntegracoes
                         && (
                            obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                            || (
                                obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                && (obj.NumeroTentativas < tentativasLimite)
                                && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)
                            )
                         )
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorCargaCTeAgrupado(int codigoCargaCTeAgrupada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query where obj.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> BuscarPedidosPorCargaETipoIntegracao(int codigoCargaCTeAgrupada, int codigoTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> Consultar(int codigoCargaCTeAgrupada, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query select obj;

            if (codigoCargaCTeAgrupada > 0)
                result = result.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoCargaCTeAgrupada, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query select obj;

            if (codigoCargaCTeAgrupada > 0)
                result = result.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }



        public Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> BuscarPorCargaCTeAgrupada(int codigoCargaCTeAgrupada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query where obj.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> BuscarPorCargaCTeAgrupada(int codigoCargaCTeAgrupada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            if (codigoCargaCTeAgrupada > 0)
                query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo);

            return query.ToList();
        }
        public int ContarPorCargaCTeAgrupadoESituacaoDiff(int codigoCargaCTeAgrupada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>();

            var result = from obj in query where obj.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public bool ExistePorCargaCTeAgrupadoETipo(int codigoCargaCTeAgrupada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoDocumento)
        {
            var consultaCargaCTeAgrupadoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>()
                .Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada && o.TipoIntegracao.Tipo == tipo && o.TipoAcaoIntegracao == tipoAcaoDocumento);

            return consultaCargaCTeAgrupadoIntegracao.Count() > 0;
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> BuscarPorCargaCTeAgrupadoETipo(int codigoCargaCTeAgrupada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoDocumento)
        {
            var consultaCargaCTeAgrupadoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>()
                .Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupada && o.TipoIntegracao.Tipo == tipo && o.TipoAcaoIntegracao == tipoAcaoDocumento);

            return consultaCargaCTeAgrupadoIntegracao.ToList();
        }
    }
}
