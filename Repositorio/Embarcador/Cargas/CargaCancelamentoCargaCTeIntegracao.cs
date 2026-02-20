using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCancelamentoCargaCTeIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>
    {
        public CargaCancelamentoCargaCTeIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public bool ExistePorCargaCTeETipoIntegracao(int codigoCargaCTe, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            query = query.Where(o => o.CargaCTe.Codigo == codigoCargaCTe && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.Any();
        }

        public int ContarPorCargaCancelamento(int codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            query = query.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento && o.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public int ContarPorCargaCancelamento(int codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] situacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            query = query.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento && situacoes.Contains(o.SituacaoIntegracao));

            return query.Count();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorCargaCancelamento(int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public List<int> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                                       obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                                       (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                       (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                        obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))));

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Select(o => o.Codigo).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao> Consultar(int codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            var result = from obj in query select obj;

            if (codigoCargaCancelamento > 0)
                result = result.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).Fetch(obj => obj.TipoIntegracao).ToList();
        }

        public int ContarConsulta(int codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            var result = from obj in query select obj;

            if (codigoCargaCancelamento > 0)
                result = result.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao> BuscarPorCargaCancelamento(int codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            if (codigoCargaCancelamento > 0)
                query = query.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao>();

            query = query.Where(o => o.CargaCTe.Codigo == codigoCargaCTe);

            return query.ToList();
        }
    }
}
