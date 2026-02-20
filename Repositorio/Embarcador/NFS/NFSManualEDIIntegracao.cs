using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;


namespace Repositorio.Embarcador.NFS
{
    public class NFSManualEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>
    {
        public NFSManualEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public NFSManualEDIIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao> Consultar(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoLancamentoNFSManual > 0)
                result = result.Where(obj => obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.TipoIntegracao)
                .Fetch(obj => obj.LayoutEDI)
                .ToList();
        }

        public int ContarConsulta(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoLancamentoNFSManual > 0)
                result = result.Where(obj => obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao> BuscarIntegracoesPendentes(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query
                         where obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                               obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                               !obj.LancamentoNFSManual.GerandoIntegracoes &&
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao BuscarPrimeiroPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao> BuscarPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual select obj;

            return result.ToList();
        }

        public int ContarPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual select obj.Codigo;

            return result.Count();
        }

        public int ContarPorLancamentoNFSManualESituacaoDiff(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public int ContarPorLancamentoNFSManual(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao> BuscarPorLancamentoNFSManual(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public bool VerificarSeExistePorLancamentoNFSManual(int codigoLancamentoNFSManual, int codigoTipoIntegracao, int codigoLayoutEDI, double tomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao>();

            var result = from obj in query
                         where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.LayoutEDI.Codigo == codigoLayoutEDI
                         select obj.Codigo;


            return result.Any();
        }
    }
}
