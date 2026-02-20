using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaRegistroEncerramentoCargaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>
    {
        public CargaRegistroEncerramentoCargaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao> BuscarIntegracoesPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            query = query.Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
            (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && o.NumeroTentativas < 3 && o.DataIntegracao <= DateTime.Now.AddMinutes(-5)));

            return query.OrderBy(o => o.Codigo).Take(20).ToList();
        }

        public bool PossuiIntegracoesPendentes(int codigoCargaEncerramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            query = query.Where(o => o.CargaRegistroEncerramento.Codigo == codigoCargaEncerramento && o.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

            return query.Count() > 0;
        }

        public bool PossuiIntegracoesIntegrada(int codigoCargaEncerramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            query = query.Where(o => o.CargaRegistroEncerramento.Codigo == codigoCargaEncerramento && o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

            return query.Count() > 0;
        }

        public int ContarBusca(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();
            var result = from obj in query select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.CargaRegistroEncerramento.Codigo == codigo && o.SituacaoIntegracao == situacao);
            else
                result = result.Where(o => o.CargaRegistroEncerramento.Codigo == codigo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao> Consultar(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            var result = from obj in query select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.CargaRegistroEncerramento.Codigo == codigo && o.SituacaoIntegracao == situacao);
            else
                result = result.Where(o => o.CargaRegistroEncerramento.Codigo == codigo);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .Distinct()
                .ToList();
        }

        public int ContarPorCargaCancelamento(int codigoCargaEncerramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            query = query.Where(o => o.CargaRegistroEncerramento.Codigo == codigoCargaEncerramento && o.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao> BuscarIntegracoesPorCargaEncerramento(int codigoCargaEncerramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            return query.Where(o => o.CargaRegistroEncerramento.Codigo == codigoCargaEncerramento).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            return query.Where(o => o.ArquivosTransacao.Any(x => x.Codigo == codigoArquivo)).FirstOrDefault();
        }

    }
}
