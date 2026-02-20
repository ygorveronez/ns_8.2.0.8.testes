namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM_PERMANENCIA_CLIENTE", EntityName = "MonitoramentoHistoricoStatusViagemPermanenciaCliente", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente", NameType = typeof(MonitoramentoHistoricoStatusViagemPermanenciaCliente))]
    public class MonitoramentoHistoricoStatusViagemPermanenciaCliente : EntidadeBase
    {
        public MonitoramentoHistoricoStatusViagemPermanenciaCliente() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "MonitoramentoHistoricoStatusViagem", Class = "MonitoramentoHistoricoStatusViagem", Column = "MHS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem MonitoramentoHistoricoStatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "PermanenciaCliente", Class = "PermanenciaCliente", Column = "PCL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente PermanenciaCliente { get; set; }
    }
}
