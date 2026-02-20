namespace Dominio.Entidades.Embarcador.PreCargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CARGA_ESTADO_DESTINO", EntityName = "PreCargaEstadoDestino", Name = "Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino", NameType = typeof(PreCargaEstadoDestino))]
    public class PreCargaEstadoDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }
    }
}
