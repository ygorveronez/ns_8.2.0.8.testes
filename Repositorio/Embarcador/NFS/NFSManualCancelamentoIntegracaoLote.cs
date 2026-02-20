using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class NFSManualCancelamentoIntegracaoLote : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote>
    {
        public NFSManualCancelamentoIntegracaoLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote> BuscarLotesPendentes(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote>();

            query = query.Where(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataRecebimento <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote> BuscarLotesPendentes(int tentativasLimite, double tempoProximaTentativaMinutos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo == tipoIntegracao && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote> BuscarLotesAguardandoRetorno(string propOrdenacao, string dirOrdenacao, int maximoRegistros, int minutosAposEnvio)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote>();

            query = query.Where(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno && obj.DataUltimaConsultaRetorno <= DateTime.Now.AddMinutes(-minutosAposEnvio));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> BuscarCTesPorLote(int codigoLote)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => obj.Lote.Codigo == codigoLote);

            return query.ToList();
        }
    }
}
