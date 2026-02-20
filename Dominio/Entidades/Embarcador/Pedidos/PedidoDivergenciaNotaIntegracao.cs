using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_DIVERGENCIA_NOTA_INTEGRACAO", EntityName = "PedidoDivergenciaNotaIntegracao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoDivergenciaNotaIntegracao", NameType = typeof(PedidoDivergenciaNotaIntegracao))]
    public class PedidoDivergenciaNotaIntegracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoDivergenciaNotaIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDivergencia", Column = "PDN_DATA_DIVERGENCIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDivergencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Divergencia", Column = "PDN_DIVERGENCIA", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Divergencia { get; set; }
        
        public virtual bool Equals(PedidoDivergenciaNotaIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
