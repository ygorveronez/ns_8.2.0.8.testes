using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCancelamentoCargaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>
    {
        public CargaCancelamentoCargaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorCargaCancelamento(int codigoCargaCancelamento, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query
                         where
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorCargaCancelamento(int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> BuscarPedidosPorCargaETipoIntegracao(int codigoCargaCancelamento, int codigoTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            query = query.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> Consultar(int codigoCargaCancelamento, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query select obj;

            if (codigoCargaCancelamento > 0)
                result = result.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoCargaCancelamento, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query select obj;

            if (codigoCargaCancelamento > 0)
                result = result.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }

        public int ContarPorSituacaoConsulta(int codigoCargaCancelamento, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query select obj;

            if (codigoCargaCancelamento > 0)
                result = result.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento);

            result = result.Where(o => o.SituacaoIntegracao == situacao);


            return result.Count();
        }

        public int ContarPorCargaCancelamentoETipo(int codigoCargaCancelamento, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento && obj.TipoIntegracao.Codigo == codigoTipoIntegracao select obj;

            return result.Count();
        }

        public int ContarPorCargaCancelamentoESituacaoDiff(int codigoCargaCancelamento, SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento && obj.SituacaoIntegracao != situacaoDiff select obj;

            return result.Count();
        }

        public int ContarPorCargaCancelamento(int codigoCargaCancelamento, SituacaoIntegracao[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento && situacao.Contains(obj.SituacaoIntegracao) select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> BuscarPorCargaCancelamento(int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> BuscarPorCargaCancelamento(int codigoCargaCancelamento, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao BuscarPorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var consultaCargaCancelamentoCargaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>()
                .Where( o => o.CargaCancelamento.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipo);

            return consultaCargaCancelamentoCargaIntegracao.FirstOrDefault();
        }

        public int removerStageCargaCancelamento(int CodigoStage)
        {
            string hql = "update CargaCancelamentoCargaIntegracao set Stage = null where Stage = :stage";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("stage", CodigoStage);
            return query.ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return result.FirstOrDefault();
        }
    }
}
