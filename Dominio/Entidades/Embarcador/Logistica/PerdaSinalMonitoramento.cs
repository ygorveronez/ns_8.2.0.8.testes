using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERDA_SINAL_MONITORAMENTO", EntityName = "PerdaSinalMonitoramento", Name = "Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento", NameType = typeof(PerdaSinalMonitoramento))]
    public class PerdaSinalMonitoramento : EntidadeBase
    {
        public PerdaSinalMonitoramento()
        {
            DataInicio = DateTime.Now;
            TempoSegundos = 0;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "PPS_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "PPS_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoSegundos", Column = "PPS_TEMPO_SEGUNDOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? TempoSegundos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Monitoramento", Class = "Monitoramento", Column = "MON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Monitoramento Monitoramento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Veiculo", Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LatitudeInicio", Column = "PPS_LATITUDE_INICIO_PERDA", TypeType = typeof(double), NotNull = false)]
        public virtual double LatitudeInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongitudeInicio", Column = "PPS_LONGITUDE_INICIO_PERDA", TypeType = typeof(double), NotNull = false)]
        public virtual double LongitudeInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LatitudeFim", Column = "PPS_LATITUDE_FIM_PERDA", TypeType = typeof(double), NotNull = false)]
        public virtual double LatitudeFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongitudeFim", Column = "PPS_LONGITUDE_FIM_PERDA", TypeType = typeof(double), NotNull = false)]
        public virtual double LongitudeFim { get; set; }

        /// <summary>
        /// Em metros
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PPS_DISTANCIA_PERDA_SINAL", TypeType = typeof(double), NotNull = false)]
        public virtual double DistanciaPerdaSinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertaPossuiPosicaoRetroativa", Column = "PPS_POSICAO_RETROATIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertaPossuiPosicaoRetroativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AlertaMonitor", Column = "ALE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.AlertaMonitor AlertaMonitor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MonitoramentoFinalizadoComPerdaSinalAberto", Column = "PPS_PERDA_SINAL_ABERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MonitoramentoFinalizadoComPerdaSinalAberto { get; set; }


        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

    }
}