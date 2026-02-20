using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_ROTA_FRETE", EntityName = "CargaPedidoRotaFrete", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete", NameType = typeof(CargaPedidoRotaFrete))]
    public class CargaPedidoRotaFrete : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_VALOR_TABELA_FRETE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorTabelaFrete { get; set; }

        public virtual bool Equals(CargaPedidoRotaFrete other)
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

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete)this.MemberwiseClone();
        }
    }
}
