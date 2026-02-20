namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_COMPRA_RECEBIMENTO_PRODUTO", EntityName = "FluxoCompraRecebimentoProduto", Name = "Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto", NameType = typeof(FluxoCompraRecebimentoProduto))]
    public class FluxoCompraRecebimentoProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoCompra", Column = "FCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoCompra FluxoCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemCompraMercadoria", Column = "OCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemCompraMercadoria OrdemCompraMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FRP_QUANTIDADE_RECEBIDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeRecebida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FRP_QUANTIDADE_DOCUMENTO_FISCAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeDocumentoFiscal { get; set; }
    }
}
