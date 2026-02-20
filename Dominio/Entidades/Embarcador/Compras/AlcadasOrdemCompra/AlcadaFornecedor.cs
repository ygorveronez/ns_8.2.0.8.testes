namespace Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_COMPRA_FORNECEDOR", EntityName = "AlcadasOrdemCompra.AlcadaFornecedor", Name = "Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor", NameType = typeof(AlcadaFornecedor))]
    public class AlcadaFornecedor : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasOrdemCompra", Column = "RRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasOrdemCompra RegrasOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Fornecedor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Fornecedor?.Descricao ?? string.Empty;
            }
        }

        public virtual Dominio.Entidades.Cliente PropriedadeAlcada
        {
            get
            {
                return this.Fornecedor;
            }
            set
            {
                this.Fornecedor = value;
            }
        }
    }
}