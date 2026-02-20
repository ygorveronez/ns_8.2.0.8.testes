namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_AVIPED", EntityName = "IntegracaoAVIPED", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED", NameType = typeof(IntegracaoAVIPED))]
    public class IntegracaoAVIPED : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "AVI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVI_NUM_AVISO_RECEBIMENTO", Type = "StringClob", NotNull = false)]
        public virtual string NumeroAvisoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVI_NUM_PEDIDO_COMPRA", Type = "StringClob", NotNull = false)]
        public virtual string NumeroPedidoCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVI_MENSAGEM", Type = "StringClob", NotNull = false)]
        public virtual string Mensagem { get; set; }
    }
}
