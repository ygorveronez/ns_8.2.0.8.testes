namespace Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_COMPRA_GRUPO_PRODUTO", EntityName = "AlcadasOrdemCompra.AlcadaGrupoProduto", Name = "Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto", NameType = typeof(AlcadaGrupoProduto))]
    public class AlcadaGrupoProduto : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasOrdemCompra", Column = "RRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasOrdemCompra RegrasOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProdutoTMS", Column = "GPR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS GrupoProdutoTMS { get; set; }

        public virtual string Descricao
        {
            get { return GrupoProdutoTMS?.Descricao ?? string.Empty; }
        }

        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS PropriedadeAlcada
        {
            get { return GrupoProdutoTMS; }
            set { GrupoProdutoTMS = value; }
        }
    }
}
