using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Escrituracao
{
    public class LoteEscrituracaoCancelamentoEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>
    {
        public LoteEscrituracaoCancelamentoEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LoteEscrituracaoCancelamentoEDIIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> Consultar(int codigoLoteEscrituracaoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            if (codigoLoteEscrituracaoCancelamento > 0)
                query = query.Where(obj => obj.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento);

            if (situacao != null)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros)
                        .Fetch(obj => obj.TipoIntegracao)
                        .Fetch(obj => obj.LayoutEDI)
                        .ToList();
        }

        public int ContarConsulta(int codigoLoteEscrituracaoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            if (codigoLoteEscrituracaoCancelamento > 0)
                query = query.Where(obj => obj.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public Task<List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>> BuscarIntegracoesPendentesAsync(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                                       obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                                       (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                        (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                         obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao &&
                                         obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))));

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao)
                        .Skip(0)
                        .Take(numeroRegistrosPorVez)
                        .ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> BuscarPorLoteEscrituracao(int codigoLoteEscrituracaoCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao BuscarPrimeiroPorLoteEscrituracao(int codigoLoteEscrituracaoCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento);

            return query.FirstOrDefault();
        }

        public int ContarPorLoteEscrituracaoESituacaoDiff(int codigoLoteEscrituracaoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento && o.SituacaoIntegracao != situacaoDiff);

            return query.Select(o => o.Codigo).Count();
        }

        public int ContarPorLoteEscrituracao(int codigoLoteEscrituracaoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento && o.SituacaoIntegracao == situacao);

            return query.Select(o => o.Codigo).Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> BuscarPorLoteEscrituracao(int codigoLoteEscrituracaoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao.Value);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorLoteEscrituracao(int codigoLoteEscrituracaoCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento);

            return query.Select(o => o.TipoIntegracao.Tipo).Distinct().ToList();
        }

        public bool VerificarSeExistePorLoteEscrituracao(int codigoLoteEscrituracaoCancelamento, int codigoTipoIntegracao, int codigoLayoutEDI)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento && o.TipoIntegracao.Codigo == codigoTipoIntegracao && o.LayoutEDI.Codigo == codigoLayoutEDI);

            return query.Select(o => o.Codigo).Any();
        }
    }
}
