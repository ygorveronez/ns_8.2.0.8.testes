namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_PRODUTO", EntityName = "GestaoDevolucaoProduto", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto", NameType = typeof(GestaoDevolucaoProduto))]

    public class GestaoDevolucaoProduto : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GDP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucao", Column = "GDV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDevolucao GestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_DEVOLUCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscalDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "GDP_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "GDP_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoUnitario", Column = "GDP_PESO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoDescricao", Column = "GDP_PRODUTO_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string ProdutoDescricao { get; set; }

        #endregion

        #region Atributos Virtuais
        public virtual string Descricao
        {
            get
            {
                return string.Empty;
            }
        }
        public virtual decimal ValorTotal
        {
            get
            {
                return Quantidade * ValorUnitario;
            }
        }
        #endregion
    }
}
