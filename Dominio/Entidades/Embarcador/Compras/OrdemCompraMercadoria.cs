namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_COMPRA_MERCADORIA", EntityName = "OrdemCompraMercadoria", Name = "Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria", NameType = typeof(OrdemCompraMercadoria))]
    public class OrdemCompraMercadoria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemCompra", Column = "ORC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemCompra OrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCM_QUANTIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCM_QUANTIDADE_PENDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePendente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCM_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo VeiculoMercadoria { get; set; }

        public virtual decimal ValorTotal
        {
            get { return ValorUnitario * Quantidade; }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Produto?.Descricao ?? string.Empty;
            }
        }
    }
}
