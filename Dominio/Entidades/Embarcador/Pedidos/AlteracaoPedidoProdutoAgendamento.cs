namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALTERACAO_PEDIDO_PRODUTO_AGENDAMENTO", EntityName = "AlteracaoPedidoProdutoAgendamento", Name = "Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento", NameType = typeof(AlteracaoPedidoProdutoAgendamento))]

    public class AlteracaoPedidoProdutoAgendamento : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoProduto", Column = "PRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoProduto PedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovaQuantidadeProduto", Column = "APA_NOVA_QUANTIDADE_PRODUTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal NovaQuantidadeProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AgendamentoColeta", Column = "ACO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta AgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportadoPorPlanilha", Column = "APA_IMPORTADO_POR_PLANILHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportadoPorPlanilha { get; set; }

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