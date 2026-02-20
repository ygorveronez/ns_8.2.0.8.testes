namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_VINCULADA", EntityName = "CargaVinculada", Name = "Dominio.Entidades.Embarcador.Cargas.CargaVinculada", NameType = typeof(CargaVinculada))]
    public class CargaVinculada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CARGA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_VINCULO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Vinculo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Carga.CodigoCargaEmbarcador + " -> " + this.Vinculo.CodigoCargaEmbarcador;
            }
        }
    }
}
