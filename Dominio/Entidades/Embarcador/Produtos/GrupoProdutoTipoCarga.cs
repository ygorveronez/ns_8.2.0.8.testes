namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PRODUTO_TIPO_DE_CARGA", EntityName = "GrupoProdutoTipoCarga", Name = "Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga", NameType = typeof(GrupoProdutoTipoCarga))]
    public class GrupoProdutoTipoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "GTC_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GPR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProduto GrupoProduto { get; set; }
    }
}
