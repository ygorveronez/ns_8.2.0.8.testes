using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TEMPO_BALSA", EntityName = "TempoBalsa", Name = "Dominio.Entidades.Embarcador.Logistica.TempoBalsa", NameType = typeof(TempoBalsa))]
    public class TempoBalsa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TrechoBalsa", Column = "TCB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TrechoBalsa TrechoBalsa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoGeral", Column = "TTB_TEMPO_GERAL", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoGeral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "TTB_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "TTB_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }

    }
}
