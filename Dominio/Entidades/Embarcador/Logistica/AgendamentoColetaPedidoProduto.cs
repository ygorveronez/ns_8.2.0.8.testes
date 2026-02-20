using Dominio.Entidades.Embarcador.Pedidos;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_COLETA_PEDIDO_PRODUTO", EntityName = "AgendamentoColetaPedidoProduto", Name = "Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto", NameType = typeof(AgendamentoColetaPedidoProduto))]
    public class AgendamentoColetaPedidoProduto : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AgendamentoColetaPedido", Column = "ACP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AgendamentoColetaPedido AgendamentoColetaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoProduto", Column = "PRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoProduto PedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "APP_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDeCaixas", Column = "APP_QUANTIDADE_DE_CAIXAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeDeCaixas { get; set; }

        #endregion Propriedades

        #region Métodos Públicos

        public virtual string Descricao
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion Métodos Públicos
    }
}
