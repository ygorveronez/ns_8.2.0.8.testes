using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_PEDIDO", EntityName = "CTePedido", Name = "Dominio.Entidades.Embarcador.CTe.CTePedido", NameType = typeof(CTePedido))]
    public class CTePedido : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CTe.CTePedido>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico ConhecimentoDeTransporteEletronico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoVenda", Column = "PEV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda PedidoVenda { get; set; }

        public virtual bool Equals(CTePedido other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
