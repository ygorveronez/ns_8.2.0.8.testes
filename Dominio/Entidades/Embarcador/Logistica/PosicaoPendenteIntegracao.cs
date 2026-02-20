using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_POSICAO_PENDENTE_INTEGRACAO", EntityName = "PosicaoPendenteIntegracao", Name = "Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao", NameType = typeof(PosicaoPendenteIntegracao))]
    public class PosicaoPendenteIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "PPI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual Int64 Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ID", Type = "System.Int64", Column = "PPI_ID", NotNull = true)]
        public virtual Int64 ID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PPI_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDEquipamento", Column = "PPI_ID_EQUIPAMENTO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string IDEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PPI_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVeiculo", Column = "PPI_DATA_VEICULO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "PPI_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "PPI_LATITUDE", TypeType = typeof(double), NotNull = true)]
        public virtual double Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "PPI_LONGITUDE", TypeType = typeof(double), NotNull = true)]
         public virtual double Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Velocidade", Column = "PPI_VELOCIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Velocidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ignicao", Column = "PPI_IGNICAO", TypeType = typeof(int), NotNull = false)]
        public virtual int Ignicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "PPI_TEMPERATURA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelBateria", Column = "PPI_NIVEL_BATERIA", TypeType = typeof(decimal), Scale = 2, Precision = 10, NotNull = false)]
        public virtual decimal NivelBateria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelSinalGPS", Column = "PPI_NIVEL_SINAL_GPS", TypeType = typeof(decimal), Scale = 2, Precision = 10, NotNull = false)]
        public virtual decimal NivelSinalGPS { get; set; }

        public virtual bool Equals(PosicaoPendenteIntegracao other)
        {
            if (other == null)
                return false;

            return other.Latitude == Latitude && other.Longitude == Longitude;
        }
    }
}
