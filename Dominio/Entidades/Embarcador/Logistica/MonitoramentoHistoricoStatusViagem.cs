using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM", EntityName = "MonitoramentoHistoricoStatusViagem", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem", NameType = typeof(MonitoramentoHistoricoStatusViagem))]
    public class MonitoramentoHistoricoStatusViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MHS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "MHS_LATITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double? Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "MHS_LONGITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double? Longitude { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Monitoramento", Column = "MON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Monitoramento Monitoramento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MonitoramentoStatusViagem", Column = "MSV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem StatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SubareaCliente", Column = "SAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.SubareaCliente SubareaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "MHS_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "MHS_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoSegundos", Column = "MHS_TEMPO_SEGUNDOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? TempoSegundos { get; set; }

        public virtual TimeSpan Tempo
        {
            get
            {
                return TimeSpan.FromSeconds(TempoSegundos ?? 0);
            }
        }
    }
}
