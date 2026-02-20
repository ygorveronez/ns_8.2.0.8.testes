using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_PRODUTO_DIVISAO_CAPACIDADE", EntityName = "CargaPedidoProdutoDivisaoCapacidade", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade", NameType = typeof(CargaPedidoProdutoDivisaoCapacidade))]
    public class CargaPedidoProdutoDivisaoCapacidade : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedidoProduto", Column = "CPP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto CargaPedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCargaDivisaoCapacidade", Column = "MDC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade ModeloVeicularCargaDivisaoCapacidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDV_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDV_QUANTIDADE_PLANEJADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal QuantidadePlanejada { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Divisão Capacidade";
            }
        }

        public virtual bool Equals(CargaPedidoProdutoDivisaoCapacidade other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
