using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE", EntityName = "CargaPedidoXMLNotaFiscalCTe", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe", NameType = typeof(CargaPedidoXMLNotaFiscalCTe))]
    public class CargaPedidoXMLNotaFiscalCTe : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComplemento", Column = "CAR_VALOR_COMPLEMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorComplemento { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe)this.MemberwiseClone();
        }

        public virtual bool Equals(CargaPedidoXMLNotaFiscalCTe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
