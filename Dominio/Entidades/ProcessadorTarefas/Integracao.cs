using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.ProcessadorTarefas
{
    public class Integracao
    {
        #region Propriedades

        public DateTime? DataIntegracao { get; set; }

        public SituacaoIntegracao SituacaoIntegracao { get; set; }

        public TipoIntegracao TipoIntegracao { get; set; }

        public int Tentativas { get; set; }

        public string ProblemaIntegracao { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string SituacaoIntegracaoFormatada
        {
            get { return SituacaoIntegracao.ObterDescricao(); }
        }

        public string TipoIntegracaoFormatada
        {
            get { return TipoIntegracao.ObterDescricao(); }
        }

        public string DataIntegracaoFormatada
        {
            get { return DataIntegracao?.ToLocalTime().ToDateTimeString(showSeconds: false) ?? string.Empty; }
        }

        #endregion Propriedades com Regras
    }
}
