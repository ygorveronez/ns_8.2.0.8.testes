namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DE_CARGA_SENDOR_OPENTECH", EntityName = "TipoDeCargaSensorOpentech", Name = "Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech", NameType = typeof(TipoDeCargaSensorOpentech))]
    public class TipoDeCargaSensorOpentech : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TSO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTipoSensorOpentech", Column = "TSO_CODIGO_TIPO_SENSOR_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoTipoSensorOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeSensores", Column = "TSO_QUANTIDADE_SENSORES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeSensores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaTemperaturaSuperior", Column = "TSO_TOLERANCIA_TEMPERATURA_SUPERIOR", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaTemperaturaSuperior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaTemperaturaInferior", Column = "TSO_TOLERANCIA_TEMPERATURA_INFERIOR", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaTemperaturaInferior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TemperaturaIdealSuperior", Column = "TSO_TEMPERATURA_IDEAL_SUPERIOR", TypeType = typeof(int), NotNull = false)]
        public virtual int TemperaturaIdealSuperior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TemperaturaIdealInferior", Column = "TSO_TEMPERATURA_IDEAL_INFERIOR", TypeType = typeof(int), NotNull = false)]
        public virtual int TemperaturaIdealInferior { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoDeCarga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TipoDeCarga?.Descricao + " - " + this.CodigoTipoSensorOpentech.ToString();
            }
        }
    }
}
