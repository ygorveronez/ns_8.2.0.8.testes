using System;

namespace Dominio.Entidades.Embarcador.CotacaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_PEDIDO_COMPONENTE", EntityName = "CotacaoPedidoComponente", Name = "Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente", NameType = typeof(CotacaoPedidoComponente))]
    public class CotacaoPedidoComponente : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoPedido", Column = "CTP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido CotacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CPC_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "CPC_PERCENTUAL_NOTA_FISCAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAcrescimo", Column = "CPC_PERCENTUAL_ACRESCIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDesconto", Column = "CPC_PERCENTUAL_DESCONTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PercentualDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "CPC_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente Clonar()
        {
            return (Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente)this.MemberwiseClone();
        }

        public virtual bool Equals(CotacaoPedidoComponente other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
