using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PARADAS_MONITORAMENTOS_FINALIZADOS", EntityName = "ParadasMonitoramentosFinalizados", Name = "Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados", NameType = typeof(ParadasMonitoramentosFinalizados))]
    public class ParadasMonitoramentosFinalizados : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Posicao", Column = "POS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Posicao Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Alerta", Column = "PMF_ALERTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Alerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PMF_TIPO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PMF_DESCRICAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "PMF_PLACA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "PMF_LATITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "PMF_LONGITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "PMF_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "PMF_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tempo", Column = "PMF_TEMPO", TypeType = typeof(TimeSpan), NotNull = false)]
        public virtual TimeSpan Tempo { get; set; }

        public virtual string TempoFormatado
        {
            get
            {
                string formato = String.Empty;
                if (Tempo.Days > 0)
                {
                    formato = $"{Tempo.Days} dias ";
                }
                return formato + Tempo.ToString(@"hh\:mm\:ss");
            }
        }
    }
}