namespace Dominio.Entidades.Embarcador.PreCargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CARGA_DESTINO", EntityName = "PreCargaDestino", Name = "Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino", NameType = typeof(PreCargaDestino))]
    public class PreCargaDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }
    }
}
