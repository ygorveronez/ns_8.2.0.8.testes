using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_PROCESSAR_EVENTO", EntityName = "MonitoramentoProcessarEvento", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoProcessarEvento", NameType = typeof(MonitoramentoProcessarEvento))]
    public class MonitoramentoProcessarEvento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "MPE_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        //[Obsolete("Coluna removida")]
        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Posicao", Column = "POS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        //public virtual Dominio.Entidades.Embarcador.Logistica.Posicao Posicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Monitoramento", Column = "MON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Monitoramento Monitoramento { get; set; }

    }
}
