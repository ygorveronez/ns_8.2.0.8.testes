using System;

namespace Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACOMPANHAMENTO_ENTREGA_TEMPO_CONFIGURACAO", EntityName = "AcompanhamentoEntregaTempoConfiguracao", Name = "Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao", NameType = typeof(AcompanhamentoEntregaTempoConfiguracao))]
    public class AcompanhamentoEntregaTempoConfiguracao : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaidaEmTempo", Column = "AEC_SAIDA_EM_TEMPO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan SaidaEmTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaidaAtraso1", Column = "AEC_SAIDA_ATRASO1", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan SaidaAtraso1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaidaAtraso2", Column = "AEC_SAIDA_ATRASO2", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan SaidaAtraso2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaidaAtraso3", Column = "AEC_SAIDA_ATRASO3", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan SaidaAtraso3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmtransitoEmTempo", Column = "AEC_EM_TRANSITO_TEMPO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan EmtransitoEmTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmTrasitoAtraso1", Column = "AEC_EM_TRANSITO_ATRASO1", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan EmTrasitoAtraso1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmTrasitoAtraso2", Column = "AEC_EM_TRANSITO_ATRASO2", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan EmTrasitoAtraso2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmTrasitoAtraso3", Column = "AEC_EM_TRANSITO_ATRASO3", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan EmTrasitoAtraso3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestinoEmTempo", Column = "AEC_DESTINO_EM_TEMPO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan DestinoEmTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestinoAtraso1", Column = "AEC_DESTINO_ATRASO1", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan DestinoAtraso1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestinoAtraso2", Column = "AEC_DESTINO_ATRASO2", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan DestinoAtraso2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestinoAtraso3", Column = "AEC_DESTINO_ATRASO3", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan DestinoAtraso3 { get; set; }

        //configurações salvas na tela ConfiguracaoAlertaCarga/ConfiguracaoAlertaCargaController
        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaDestinoEmTempo", Column = "AEC_DESTINO_EM_TEMPO_ALERTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaDestinoEmTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaDestinoAtraso1", Column = "AEC_DESTINO_ATRASO1_ALERTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaDestinoAtraso1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaDestinoAtraso2", Column = "AEC_DESTINO_ATRASO2_ALERTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaDestinoAtraso2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaDestinoAtraso3", Column = "AEC_DESTINO_ATRASO3_ALERTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaDestinoAtraso3 { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaSaidaEmTempo", Column = "AEC_SAIDA_EM_TEMPO_ALERTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaSaidaEmTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaSaidaAtraso1", Column = "AEC_SAIDA_ATRASO1_ALERTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaSaidaAtraso1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaSaidaAtraso2", Column = "AEC_SAIDA_ATRASO2_ALERTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaSaidaAtraso2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaSaidaAtraso3", Column = "AEC_SAIDA_ATRASO3_ALERTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaSaidaAtraso3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataReferencia", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DataBaseAlerta), Column = "AEC_DATA_REFERENCIA", NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DataBaseAlerta? DataReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarDataEntradaComoDestino", Column = "AEC_CONSIDERAR_DATA_ENTRADA_COMO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarDataEntradaComoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTendenciaAdiantamento", Column = "AEC_HABILITAR_TENDENCIA_ADIANTAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTendenciaAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTendenciaAtraso", Column = "AEC_HABILITAR_TENDENCIA_ATRASO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTendenciaAtraso { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
