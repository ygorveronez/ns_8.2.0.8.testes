namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PRODUTO_OPENTECH", EntityName = "GrupoProdutoOpenTech", Name = "Dominio.Entidades.Embarcador.Embarcador.GrupoProdutoOpenTech", NameType = typeof(GrupoProdutoOpenTech))]
    public class GrupoProdutoOpenTech : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GPR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProduto GrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoValorMaior", Column = "POP_CODIGO_PRODUTO_VALOR_MAIOR", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoProdutoValorMaior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoValorMenor", Column = "POP_CODIGO_PRODUTO_VALOR_MENOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProdutoValorMenor { get; set; }
    }
}
