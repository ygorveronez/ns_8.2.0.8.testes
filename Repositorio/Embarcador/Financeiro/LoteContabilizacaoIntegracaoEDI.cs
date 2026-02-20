using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Financeiro
{
    public class LoteContabilizacaoIntegracaoEDI : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>
    {
        public LoteContabilizacaoIntegracaoEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LoteContabilizacaoIntegracaoEDI(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public int ContarPorLoteContabilizacaoESituacaoDiff(int codigoLoteContabilizacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

            query = query.Where(o => o.LoteContabilizacao.Codigo == codigoLoteContabilizacao && o.SituacaoIntegracao != situacaoDiff);

            return query.Count();
        }

        public Task<int> ContarPorLoteContabilizacaoAsync(int codigoLoteContabilizacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

            query = query.Where(o => o.LoteContabilizacao.Codigo == codigoLoteContabilizacao && o.SituacaoIntegracao == situacao);

            return query.CountAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> BuscarPorLoteContabilizacao(int codigoLoteContabilizacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

            query = query.Where(o => o.LoteContabilizacao.Codigo == codigoLoteContabilizacao);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao.Value);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorLoteContabilizacao(int codigoLoteContabilizacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

            query = query.Where(o => o.LoteContabilizacao.Codigo == codigoLoteContabilizacao);

            return query.Select(o => o.TipoIntegracao.Tipo).Distinct().ToList();
        }

        public bool VerificarSeExistePorLoteContabilizacao(int codigoLoteContabilizacao, int codigoTipoIntegracao, int codigoLayoutEDI)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

            query = query.Where(o => o.LoteContabilizacao.Codigo == codigoLoteContabilizacao && o.TipoIntegracao.Codigo == codigoTipoIntegracao && o.LayoutEDI.Codigo == codigoLayoutEDI);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> Consultar(int codigoLoteContabilizacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

            if (codigoLoteContabilizacao > 0)
                query = query.Where(obj => obj.LoteContabilizacao.Codigo == codigoLoteContabilizacao);

            if (situacao != null)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros)
                        .Fetch(obj => obj.TipoIntegracao)
                        .Fetch(obj => obj.LayoutEDI)
                        .ToList();
        }

        public int ContarConsulta(int codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

            if (codigoLoteEscrituracao > 0)
                query = query.Where(obj => obj.LoteContabilizacao.Codigo == codigoLoteEscrituracao);

            if (situacao != null)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> BuscarIntegracoesPendentes(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                                       obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                                       //!obj.LoteEscrituracao.GerandoIntegracoes &&
                                       obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                       (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                        obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> BuscarPorLoteContabilizacao(int codigoLoteContabilizacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

            query = query.Where(o => o.LoteContabilizacao.Codigo == codigoLoteContabilizacao);

            return query.ToList();
        }
    }
}
