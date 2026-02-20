namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ITEM_NAO_CONFORMIDADE_CFOP", EntityName = "ItemNaoConformidadeCFOP", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP", NameType = typeof(ItemNaoConformidadeCFOP))]
    public class ItemNaoConformidadeCFOP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ItemNaoConformidade", Column = "INC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade ItemNaoConformidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.CFOP CFOP { get; set; }

    }
}
