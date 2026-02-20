namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM_PERMANENCIA_SUBAREA", EntityName = "MonitoramentoHistoricoStatusViagemPermanenciaSubArea", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea", NameType = typeof(MonitoramentoHistoricoStatusViagemPermanenciaSubArea))]
    public class MonitoramentoHistoricoStatusViagemPermanenciaSubArea : EntidadeBase
    {
        public MonitoramentoHistoricoStatusViagemPermanenciaSubArea() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "MonitoramentoHistoricoStatusViagem", Class = "MonitoramentoHistoricoStatusViagem", Column = "MHS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem MonitoramentoHistoricoStatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "PermanenciaSubarea", Class = "PermanenciaSubarea", Column = "PSA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea PermanenciaSubarea { get; set; }
    }
}
