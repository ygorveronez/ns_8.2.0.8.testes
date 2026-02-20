using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_DESCARREGAMENTO", EntityName = "CargaJanelaDescarregamento", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento", NameType = typeof(CargaJanelaDescarregamento))]
    public class CargaJanelaDescarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CJD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JanelaDescarregamentoSituacao", Column = "JDS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual JanelaDescarregamentoSituacao JanelaDescarregamentoSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDescarregamentoProgramada", Column = "CJD_DATA_DESCARREGAMENTO_PROGRAMADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDescarregamentoProgramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioDescarregamento", Column = "CJD_INICIO_DESCARREGAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime InicioDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConfirmacao", Column = "CJD_DATA_CONFIRMACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TerminoDescarregamento", Column = "CJD_TERMINO_DESCARREGAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime TerminoDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescarregamento", Column = "CJD_TEMPO_DESCARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Excedente", Column = "CJD_EXCEDENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Excedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cancelada", Column = "CJD_CANCELADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cancelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoDescarregamentoExclusivo", Column = "CJD_PERIODO_DESCARREGAMENTO_EXCLUSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PeriodoDescarregamentoExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CJD_SITUACAO", TypeType = typeof(SituacaoCargaJanelaDescarregamento), NotNull = true)]
        public virtual SituacaoCargaJanelaDescarregamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFluxoPatio", Column = "CJD_OBSERVACAO_FLUXO_PATIO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJD_DATA_PREVISAO_CHEGADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoReagendamento", Column = "CJD_MOTIVO_REAGENDAMENTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string MotivoReagendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeNaoComparecimento", Column = "CJD_QUANTIDADE_NAO_COMPARECIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeNaoComparecimento { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (this.CentroDescarregamento != null)
                    return $"{this.Carga.Descricao} - {CentroDescarregamento.Descricao}";

                return $"{this.Carga.Descricao}";
            }
        }
    }
}
