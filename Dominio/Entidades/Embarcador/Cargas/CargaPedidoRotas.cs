using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_ROTAS", EntityName = "CargaPedidoRotas", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas", NameType = typeof(CargaPedidoRotas))]

    public class CargaPedidoRotas : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "IdenticacaoRota", Column = "CPR_IDENTIFICACAO_ROTA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IdenticacaoRota { get; set; }
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas)this.MemberwiseClone();
        }
        public virtual bool Equals(CargaPedidoRotas other)
        {
            if (other.Codigo == this.Codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}