using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet
{
    public class DadosPesquisaAgendamentoPallet
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string Carga { get; set; }

        public string Senha { get; set; }

        public DateTime DataAgendamento { get; set; }

        public DateTime? DataCriacao { get; set; }

        public DateTime? DataConfirmada { get; set; }

        public string Remetente { get; set; }

        public string Destinatario { get; set; }

        public string TipoCarga { get; set; }

        public EtapaAgendamentoPallet EtapaAgendamentoPallet { get; set; }

        public SituacaoAgendamentoPallet Situacao { get; set; }

        public SituacaoCarga SituacaoCarga { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string EtapaAgendamentoPalletFormatada
        {
            get { return EtapaAgendamentoPallet.ObterDescricao(); }
        }

        public string SituacaoFormatada
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string SituacaoCargaFormatada
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }

        public string DataAgendamentoFormatada
        {
            get { return DataAgendamento.ToDateTimeString(); }
        }

        public string DataCriacaoFormatada
        {
            get { return DataCriacao.HasValue ? DataCriacao.ToDateTimeString() : string.Empty; }
        }

        public string DataConfirmadaFormatada
        {
            get { return DataConfirmada.HasValue ? DataConfirmada.ToDateTimeString() : string.Empty; }
        }

        #endregion Propriedades com Regras
    }
}