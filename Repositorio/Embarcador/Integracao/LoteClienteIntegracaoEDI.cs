using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Integracao
{
    public class LoteClienteIntegracaoEDI : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>
    {
        #region Construtores

        public LoteClienteIntegracaoEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LoteClienteIntegracaoEDI(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #endregion

        #region Métodos Públicos

        public int ContarPorLoteClienteESituacaoDiff(int codigoLoteCliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>();

            query = query.Where(o => o.LoteCliente.Codigo == codigoLoteCliente && o.SituacaoIntegracao != situacaoDiff);

            return query.Count();
        }

        public int ContarPorLoteCliente(int codigoLoteCliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>();

            query = query.Where(o => o.LoteCliente.Codigo == codigoLoteCliente && o.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> BuscarPorLoteCliente(int codigoLoteCliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>();

            query = query.Where(o => o.LoteCliente.Codigo == codigoLoteCliente);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao.Value);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorLoteCliente(int codigoLoteCliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>();

            query = query.Where(o => o.LoteCliente.Codigo == codigoLoteCliente);

            return query.Select(o => o.TipoIntegracao.Tipo).Distinct().ToList();
        }

        public bool VerificarSeExistePorLoteCliente(int codigoLoteCliente, int codigoTipoIntegracao, int codigoLayoutEDI)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>();

            query = query.Where(o => o.LoteCliente.Codigo == codigoLoteCliente && o.TipoIntegracao.Codigo == codigoTipoIntegracao && o.LayoutEDI.Codigo == codigoLayoutEDI);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> Consultar(int codigoLoteCliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>();

            if (codigoLoteCliente > 0)
                query = query.Where(obj => obj.LoteCliente.Codigo == codigoLoteCliente);

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
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>();

            if (codigoLoteEscrituracao > 0)
                query = query.Where(obj => obj.LoteCliente.Codigo == codigoLoteEscrituracao);

            if (situacao != null)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public Task<List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>> BuscarIntegracoesPendentesAsync(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                                       obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                                       //!obj.LoteEscrituracao.GerandoIntegracoes &&
                                       obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                       (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                        obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToListAsync(cancellationToken);
        }

        #endregion
    }
}
