using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaCTeIntegracaoLote : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote>
    {
        public OcorrenciaCTeIntegracaoLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote> BuscarLotesPendentes(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote>();

            var result = from obj in query where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataRecebimento <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)) select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote> BuscarLotesAguardandoRetorno(string propOrdenacao, string dirOrdenacao, int maximoRegistros, int minutosAposEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote>();

            var result = from obj in query where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno && obj.DataUltimaConsultaRetorno <= DateTime.Now.AddMinutes(-minutosAposEnvio) select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> BuscarCTesPorLote(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;

            return result.ToList();
        }
    }
}
