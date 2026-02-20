using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_POSICAO", EntityName = "Posicao", Name = "Dominio.Entidades.Embarcador.Logistica.Posicao", NameType = typeof(Posicao))]
    public class Posicao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "POS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual Int64 Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "POS_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "POS_LATITUDE", TypeType = typeof(double), NotNull = true)]
        public virtual double Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "POS_LONGITUDE", TypeType = typeof(double), NotNull = true)]
        public virtual double Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Velocidade", Column = "POS_VELOCIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Velocidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ignicao", Column = "POS_IGNICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Ignicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDEquipamento", Column = "POS_ID_EQUIPAMENTO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string IDEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "POS_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVeiculo", Column = "POS_DATA_VEICULO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "POS_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "POS_TEMPERATURA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SensorTemperatura", Column = "POS_SENSOR_TEMPERATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SensorTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmAlvo", Column = "POS_EM_ALVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EmAlvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmLocal", Column = "POS_EM_LOCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EmLocal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelBateria", Column = "POS_NIVEL_BATERIA", TypeType = typeof(decimal), Scale = 2, Precision = 10, NotNull = false)]
        public virtual decimal NivelBateria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelSinalGPS", Column = "POS_NIVEL_SINAL_GPS", TypeType = typeof(decimal), Scale = 2, Precision = 10, NotNull = false)]
        public virtual decimal NivelSinalGPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Processar", Column = "POS_PROCESSAR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao Processar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quilometros", Column = "POS_KM", TypeType = typeof(double), NotNull = false)]
        public virtual double Quilometros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rastreador", Column = "POS_RASTREADOR", TypeType = typeof(EnumTecnologiaRastreador), NotNull = true)]
        public virtual EnumTecnologiaRastreador Rastreador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Gerenciadora", Column = "POS_GERENCIADORA", TypeType = typeof(EnumTecnologiaGerenciadora), NotNull = true)]
        public virtual EnumTecnologiaGerenciadora Gerenciadora { get; set; }

    }
}
