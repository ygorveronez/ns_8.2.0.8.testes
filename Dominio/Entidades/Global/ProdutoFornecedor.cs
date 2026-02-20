namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_FORNECEDOR", EntityName = "ProdutoFornecedor", Name = "Dominio.Entidades.ProdutoFornecedor", NameType = typeof(ProdutoFornecedor))]
    public class ProdutoFornecedor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProduto", Column = "PFO_CODIGO_PRODUTO", TypeType = typeof(string), Length = 60, NotNull = true)]
        public virtual string CodigoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FatorConversao", Column = "PFO_FATOR_CONVERSAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal FatorConversao { get; set; }
    }
}
