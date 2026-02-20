using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_TIPO_PAGAMENTO", EntityName = "PedidoTipoPagamento", Name = "Dominio.Entidades.Embarcador.Financeiro.Despesa.PedidoTipoPagamento", NameType = typeof(PedidoTipoPagamento))]
    public class PedidoTipoPagamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "PTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTP_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTP_OBSERVACAO_PEDIDO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ObservacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaPagamento", Column = "PTP_FORMA_PAGAMENTO", TypeType = typeof(FormaPagamento), NotNull = false)]
        public virtual FormaPagamento? FormaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual bool Equals(PedidoTipoPagamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
