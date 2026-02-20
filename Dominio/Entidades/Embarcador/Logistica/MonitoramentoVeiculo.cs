using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONITORAMENTO_VEICULO", EntityName = "MonitoramentoVeiculo", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo", NameType = typeof(MonitoramentoVeiculo))]
    public class MonitoramentoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Monitoramento", Column = "MON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Monitoramento Monitoramento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "MOV_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "MOV_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOV_POLILINHA", Type = "StringClob", NotNull = false, Lazy = true)]
        public virtual string Polilinha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOV_DISTANCIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Distancia { get; set; }
    }
}
