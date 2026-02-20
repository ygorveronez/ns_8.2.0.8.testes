namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_EMBARCADOR_FORNECEDOR", DynamicUpdate = true, EntityName = "ProdutoEmbarcadorFornecedor", Name = "Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor", NameType = typeof(ProdutoEmbarcadorFornecedor))]
    public class ProdutoEmbarcadorFornecedor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoInterno", Column = "PEF_CODIGO_INTERNO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoInterno { get; set; }

        public virtual string Descricao
        {
            get
            {
                return !string.IsNullOrEmpty(this.CodigoInterno) ? this.CodigoInterno : string.Empty;
            }
        }
    }
}
