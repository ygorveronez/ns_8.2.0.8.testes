using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.NFS
{
    public class NFSManualIntegracaoLote : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote>
    {
        public NFSManualIntegracaoLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote> BuscarLotesPendentes(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote>();

            var result = from obj in query where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataRecebimento <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)) select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote> BuscarLotesPendentes(int tentativasLimite, double tempoProximaTentativaMinutos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote>();

            var result = from obj in query where obj.TipoIntegracao.Tipo == tipoIntegracao && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote> BuscarLotesAguardandoRetorno(string propOrdenacao, string dirOrdenacao, int maximoRegistros, int minutosAposEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote>();

            var result = from obj in query where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno && obj.DataUltimaConsultaRetorno <= DateTime.Now.AddMinutes(-minutosAposEnvio) select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> BuscarCTesPorLote(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;

            return result.ToList();
        }
    }
}
