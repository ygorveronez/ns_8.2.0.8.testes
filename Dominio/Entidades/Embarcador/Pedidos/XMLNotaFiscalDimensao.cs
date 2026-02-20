namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_XML_NOTA_FISCAL_DIMENSAO", EntityName = "XMLNotaFiscalDimensao", Name = "Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao", NameType = typeof(XMLNotaFiscalDimensao))]
    public class XMLNotaFiscalDimensao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NXD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NXD_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NXD_ALTURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Altura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NXD_LARGURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Largura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NXD_COMPRIMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Comprimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NXD_METROS_CUBICOS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetrosCubicos { get; set; }
    }
}
