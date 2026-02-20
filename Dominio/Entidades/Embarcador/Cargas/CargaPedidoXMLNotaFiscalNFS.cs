using System;


namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_XML_NOTA_FISCAL_NFS", EntityName = "CargaPedidoXMLNotaFiscalNFS", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS", NameType = typeof(CargaPedidoXMLNotaFiscalNFS))]
    public class CargaPedidoXMLNotaFiscalNFS : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "XNS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaNFS", Column = "CNS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaNFS CargaNFS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }
        public virtual bool Equals(CargaPedidoXMLNotaFiscalNFS other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
