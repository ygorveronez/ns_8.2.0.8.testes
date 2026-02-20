using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.NFS
{
    public class NFSManualCancelamentoIntegracaoEDI : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>
    {
        public NFSManualCancelamentoIntegracaoEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> Consultar(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            if (codigoNFSManualCancelamento > 0)
                query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            if (situacao != null)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                        .Fetch(obj => obj.TipoIntegracao)
                        .Fetch(obj => obj.LayoutEDI)
                        .ToList();
        }

        public int ContarConsulta(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            if (codigoNFSManualCancelamento > 0)
                query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            if (situacao != null)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public Task<List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>> BuscarIntegracoesPendentesAsync(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            query = query.Where(obj => !obj.NFSManualCancelamento.GerandoIntegracoes &&
                                       obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                       (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                        obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> BuscarPorNFSManualCancelamento(int codigoLancamentoNFSManual)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoLancamentoNFSManual);

            return query.ToList();
        }

        public int ContarPorNFSManualCancelamento(int codigoNFSManualCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            return query.Count();
        }

        public int ContarPorNFSManualCancelamentoESituacaoDiff(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && obj.SituacaoIntegracao != situacaoDiff);

            return query.Count();
        }

        public int ContarPorNFSManualCancelamento(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && obj.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> BuscarPorNFSManualCancelamento(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao.Value);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorNFSManualCancelamento(int codigoNFSManualCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            return query.Select(obj => obj.TipoIntegracao.Tipo).Distinct().ToList();
        }

        public bool VerificarSeExistePorNFSManualCancelamento(int codigoNFSManualCancelamento, int codigoTipoIntegracao, int codigoLayoutEDI, double tomador)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.LayoutEDI.Codigo == codigoLayoutEDI);
                        
            return query.Any();
        }
    }
}
