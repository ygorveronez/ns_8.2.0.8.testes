namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONITORAMENTO_EVENTO_TRATATIVA_AUTOMATICA", EntityName = "MonitoramentoEventoTratativaAutomatica", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativaAutomatica", NameType = typeof(MonitoramentoEventoTratativaAutomatica))]
    public class MonitoramentoEventoTratativaAutomatica : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MonitoramentoEvento", Column = "MEV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento MonitoramentoEvento { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Gatilho", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TratativaAutomaticaMonitoramentoEvento), Column = "MTA_GATILHO", NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TratativaAutomaticaMonitoramentoEvento Gatilho { get; set; }
    }
}
