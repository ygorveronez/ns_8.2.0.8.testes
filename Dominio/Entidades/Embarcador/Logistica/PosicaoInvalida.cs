using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_POSICAO_INVALIDA", EntityName = "PosicaoInvalida", Name = "Dominio.Entidades.Embarcador.Logistica.PosicaoInvalida", NameType = typeof(PosicaoInvalida))]
    public class PosicaoInvalida : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "POI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual Int64 Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "POI_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "POI_LATITUDE", TypeType = typeof(double), NotNull = true)]
        public virtual double Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "POI_LONGITUDE", TypeType = typeof(double), NotNull = true)]
         public virtual double Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Velocidade", Column = "POI_VELOCIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Velocidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ignicao", Column = "POI_IGNICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Ignicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDEquipamento", Column = "POI_ID_EQUIPAMENTO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string IDEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "POI_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVeiculo", Column = "POI_DATA_VEICULO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "POI_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "POI_TEMPERATURA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SensorTemperatura", Column = "POI_SENSOR_TEMPERATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SensorTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "POI_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

    }
}
