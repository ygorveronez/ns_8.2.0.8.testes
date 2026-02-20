using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_QUANTIDADES", EntityName = "CargaPedidoQuantidades", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades", NameType = typeof(CargaPedidoQuantidades))]
    public class CargaPedidoQuantidades : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Unidade", Column = "CPQ_UNIDADE", TypeType = typeof(Dominio.Enumeradores.UnidadeMedida), NotNull = true)]
        public virtual Dominio.Enumeradores.UnidadeMedida Unidade { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CPQ_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades)this.MemberwiseClone();
        }
        public virtual bool Equals(CargaPedidoQuantidades other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
