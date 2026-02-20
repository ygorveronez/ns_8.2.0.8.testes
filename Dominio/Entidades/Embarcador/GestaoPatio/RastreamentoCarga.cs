using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RASTREAMENTO_CARGA", EntityName = "RastreamentoCarga", Name = "Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga", NameType = typeof(RastreamentoCarga))]
    public class RastreamentoCarga : EntidadeBase
    {
        public RastreamentoCarga() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCA_LATITUDE", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCA_LONGITUDE", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal Longitude { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCA_ULTIMA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? UltimaAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCA_PREVISAO_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime PrevisaoInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCA_ETAPA_RASTREAMENTO_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaRastreamentoLiberada { get; set; }

        public virtual void SetLatitude(string latitude)
        {
            decimal.TryParse(latitude.Replace(".", ","), out decimal decLatitude);
            this.Latitude = decLatitude;
        }

        public virtual void SetLongitude(string longitude)
        {
            decimal.TryParse(longitude.Replace(".", ","), out decimal decLongitude);
            this.Longitude = decLongitude;
        }
    }
}