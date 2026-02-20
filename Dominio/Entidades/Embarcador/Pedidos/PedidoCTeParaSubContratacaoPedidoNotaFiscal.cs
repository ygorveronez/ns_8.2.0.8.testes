using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_CTE_PARA_SUBCONTRATACAO_PEDIDO_NOTA_FISCAL", EntityName = "PedidoCTeParaSubContratacaoPedidoNotaFiscal", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal", NameType = typeof(PedidoCTeParaSubContratacaoPedidoNotaFiscal))]
    public class PedidoCTeParaSubContratacaoPedidoNotaFiscal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PSN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoCTeParaSubContratacao", Column = "PSC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoCTeParaSubContratacao PedidoCTeParaSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }
        
        public virtual bool Equals(PedidoCTeParaSubContratacaoPedidoNotaFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal)this.MemberwiseClone();
        }

    }
}
