namespace Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_COMPRA_PRODUTO", EntityName = "AlcadasOrdemCompra.AlcadaProduto", Name = "Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto", NameType = typeof(AlcadaProduto))]
    public class AlcadaProduto : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasOrdemCompra", Column = "RRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasOrdemCompra RegrasOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Produto Produto { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Produto?.Descricao ?? string.Empty;
            }
        }

        public virtual Dominio.Entidades.Produto PropriedadeAlcada
        {
            get
            {
                return this.Produto;
            }
            set
            {
                this.Produto = value;
            }
        }
    }
}