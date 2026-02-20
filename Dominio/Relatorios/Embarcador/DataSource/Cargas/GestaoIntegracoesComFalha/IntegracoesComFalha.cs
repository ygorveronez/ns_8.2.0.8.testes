using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoIntegracoesComFalha
{

    public class IntegracoesComFalha
    {
        #region Propriedades

        public string TabelaOrigem { get; set; }

        public long Codigo { get; set; }

        public int CodigoCarga { get; set; }

        public string NumeroCarga { get; set; }

        public SituacaoCarga EtapaCarga { get; set; }

        public SituacaoIntegracao SituacaoIntegracao { get; set; }

        public TipoIntegracao TipoIntegracao { get; set; }

        public DateTime? DataIntegracao { get; set; }

        public string MensagemRetorno { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public string EtapaCargaFormatada
        {
            get { return this.EtapaCarga.ObterDescricao(); }
        }

        public string DataIntegracaoFormatada
        {
            get { return this.DataIntegracao.HasValue ? this.DataIntegracao?.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SituacaoIntegracaoFormatada
        {
            get { return this.SituacaoIntegracao.ObterDescricao(); }
        }

        public string TipoIntegracaoFormatada
        {
            get { return this.TipoIntegracao.ObterDescricao(); }
        }

        #endregion Propriedades Com Regras
    }
}