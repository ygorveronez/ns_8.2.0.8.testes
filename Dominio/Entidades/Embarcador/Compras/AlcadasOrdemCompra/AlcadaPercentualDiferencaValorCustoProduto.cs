namespace Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_COMPRA_PERCENTUAL_DIFERENCA_VALOR_CUSTO_PRODUTO", EntityName = "AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto", Name = "Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto", NameType = typeof(AlcadaPercentualDiferencaValorCustoProduto))]
    public class AlcadaPercentualDiferencaValorCustoProduto : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasOrdemCompra", Column = "RRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasOrdemCompra RegrasOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APD_PERCENTUAL_DIFERENCA_VALOR_CUSTO_PRODUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualDiferencaValorCustoProduto { get; set; }

        public virtual string Descricao
        {
            get { return PercentualDiferencaValorCustoProduto.ToString("n2"); }
        }

        public virtual decimal PropriedadeAlcada
        {
            get
            {
                return PercentualDiferencaValorCustoProduto;
            }
            set
            {
                PercentualDiferencaValorCustoProduto = value;
            }
        }
    }
}
