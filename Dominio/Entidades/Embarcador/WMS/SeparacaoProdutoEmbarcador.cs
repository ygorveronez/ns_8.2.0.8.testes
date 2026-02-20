namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SEPARACAO_PRODUTO_EMBARCADOR", EntityName = "SeparacaoProdutoEmbarcador", Name = "Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador", NameType = typeof(SeparacaoProdutoEmbarcador))]
    public class SeparacaoProdutoEmbarcador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "SPE_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeSeparada", Column = "SPE_QUANTIDADE_SEPARADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeSeparada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcadorLote", Column = "PEL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcadorLote ProdutoEmbarcadorLote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Separacao", Column = "SEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Separacao Separacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarrasConferido", Column = "SPE_CODIGO_BARRAS_CONFERIDO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string CodigoBarrasConferido { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
