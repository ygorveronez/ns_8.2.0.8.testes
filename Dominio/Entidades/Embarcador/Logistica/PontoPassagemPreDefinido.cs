namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_PONTO_PRE_DEFINIDO", EntityName = "PontoPassagemPreDefinido", Name = "Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido", NameType = typeof(PontoPassagemPreDefinido))]
    public class PontoPassagemPreDefinido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RPD_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "RPD_LATITUDE", TypeType = typeof(decimal), NotNull = true, Scale = 10, Precision = 18)]
        public virtual decimal Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "RPD_LONGITUDE", TypeType = typeof(decimal), NotNull = true, Scale = 10, Precision = 18)]
        public virtual decimal Longitude { get; set; }

        /// <summary>
        /// Tempo estimado de permanência do motorista nesse ponto, em minutos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEstimadoPermanencia", Column = "RPD_TEMPO_ESTIMADO_PERMANENCIA_MINUTOS", Type = "int", NotNull = false)]
        public virtual int TempoEstimadoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalDeParqueamento", Column = "RPD_LOCAL_DE_PARQUEAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LocalDeParqueamento { get; set; }

        public virtual string TempoEstimadoPermanenciaFormatado
        {
            get { return $"{TempoEstimadoPermanencia / 60:D2}:{TempoEstimadoPermanencia % 60:D2}"; }
        }

        public virtual Dominio.Entidades.Localidade ObterLocalidade()
        {
            return Cliente?.Localidade ?? Localidade;
        }

        public virtual string ObterDescricao()
        {
            return Cliente?.Descricao ?? Localidade?.Descricao ?? Descricao;
        }

        public virtual string ObterLatitude()
        {
            return Cliente?.Latitude ?? Localidade?.Latitude.ToString() ?? Latitude.ToString();
        }

        public virtual string ObterLongitude()
        {
            return Cliente?.Longitude ?? Localidade?.Longitude.ToString() ?? Longitude.ToString();
        }
    }
}
