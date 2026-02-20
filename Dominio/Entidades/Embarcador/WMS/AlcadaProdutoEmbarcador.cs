namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_DESCARTE_PRODUTO_EMBARCADOR", EntityName = "AlcadaProdutoEmbarcador", Name = "Dominio.Entidades.Embarcador.WMS.AlcadaProdutoEmbarcador", NameType = typeof(AlcadaProdutoEmbarcador))]
    public class AlcadaProdutoEmbarcador : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraDescarte", Column = "RED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraDescarte RegraDescarte { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ProdutoEmbarcador?.Descricao ?? string.Empty;
            }
        }

        public virtual Produtos.ProdutoEmbarcador PropriedadeAlcada
        {
            get
            {
                return this.ProdutoEmbarcador;
            }
            set
            {
                this.ProdutoEmbarcador = value;
            }
        }
    }
}