using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALERTA_MONITOR_NOTIFICACAO", EntityName = "AlertaMonitorNotificacao", Name = "Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao", NameType = typeof(AlertaMonitorNotificacao))]
    public class AlertaMonitorNotificacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AMN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Type = "System.Int32", Column = "AMN_SEQUENCIA", NotNull = true)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "AMN_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AlertaMonitor", Column = "ALE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.AlertaMonitor AlertaMonitor { get; set; }


    }
}
