using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_COMPONENTE_FRETE_TEMPO", EntityName = "ComponenteFreteTabelaFreteTempo", Name = "Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFreteTempo", NameType = typeof(ComponenteFreteTabelaFreteTempo))]
    public class ComponenteFreteTabelaFreteTempo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFreteTabelaFrete", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ComponenteFreteTabelaFrete ComponenteFreteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFT_HORA_INICIAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFT_HORA_FINAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFT_PERIODO_INICIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PeriodoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFT_HORA_INICIAL_COBRANCA_MINIMA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicialCobrancaMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFT_HORA_FINAL_COBRANCA_MINIMA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraFinalCobrancaMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFT_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Das " + string.Format("{0:00}:{1:00}", HoraInicial.Hours, HoraInicial.Minutes) + "h às " + string.Format("{0:00}:{1:00}", HoraFinal.Hours, HoraFinal.Minutes) + "h";
            }
        }
    }
}
