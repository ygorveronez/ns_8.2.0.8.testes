using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>
    {
        public CargaMDFeManualIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorCargaMDFeManual(int codigoCargaCTeAgrupada, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query where obj.CargaMDFeManual.Codigo == codigoCargaCTeAgrupada && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query
                         where
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> BuscarCargaMDFeManualIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query
                         where
                         !obj.CargaMDFeManual.GerandoIntegracoes
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

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorCargaMDFeManual(int codigoCargaCTeAgrupada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query where obj.CargaMDFeManual.Codigo == codigoCargaCTeAgrupada select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> BuscarPedidosPorCargaETipoIntegracao(int codigoMDFeManual, int codigoTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoMDFeManual && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> Consultar(int codigoCargaMDFeManual, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query select obj;

            if (codigoCargaMDFeManual > 0)
                result = result.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoCargaMDFeManual, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query select obj;

            if (codigoCargaMDFeManual > 0)
                result = result.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }



        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> BuscarPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query where obj.CargaMDFeManual.Codigo == codigoCargaMDFeManual select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> BuscarPorCargaMDFeManual(int codigoCargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            if (codigoCargaMDFeManual > 0)
                query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            return query.ToList();
        }
        public int ContarPorCargaMDFeManualESituacaoDiff(int codigoCargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

            var result = from obj in query where obj.CargaMDFeManual.Codigo == codigoCargaMDFeManual && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public bool ExistePorCargaMDFeManualETipo(int codigoCargaMDFeManual, int codigoMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var consultaCargaMDFeManualIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>()
                .Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual && o.MDFe.Codigo == codigoMDFe && o.TipoIntegracao.Tipo == tipo);

            return consultaCargaMDFeManualIntegracao.Count() > 0;
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> BuscarPorCargaMDFeManualETipo(int codigoCargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var consultaCargaMDFeManualIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>()
                .Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual && o.TipoIntegracao.Tipo == tipo );

            return consultaCargaMDFeManualIntegracao.ToList();
        }
    }
}
