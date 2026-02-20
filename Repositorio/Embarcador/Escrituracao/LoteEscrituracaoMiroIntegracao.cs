using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Escrituracao
{
    public class LoteEscrituracaoMiroIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao>
    {
        #region Contructores
        public LoteEscrituracaoMiroIntegracao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao> BuscarIntegracoesPendentes(int quantidadeRegistro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao>()
                .Where(x => x.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno || (x.NumeroTentativas <= 3 && x.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) && x.DataIntegracao <= DateTime.Now.AddMinutes(x.TipoIntegracao.TempoConsultaIntegracao));
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao> BuscarIntegracoesPorLote(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao>()
                .Where(x => x.LoteEscrituracaoMiroDocumento.LoteEscrituracaoMiro.Codigo == codigoLote);
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao BuscarIntegracoesPorNumeroMiro(string chaveDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao>()
                .Where(x => x.LoteEscrituracaoMiroDocumento.ChaveDocumento == chaveDocumento);
            return query.FirstOrDefault();
        }

        #endregion
    }
}
