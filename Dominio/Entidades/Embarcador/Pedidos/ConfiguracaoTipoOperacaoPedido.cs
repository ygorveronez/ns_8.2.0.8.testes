namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_PEDIDO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPedido", NameType = typeof(ConfiguracaoTipoOperacaoPedido))]
    public class ConfiguracaoTipoOperacaoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTD_BLOQUEAR_INCLUSAO_ALTERACAO_PEDIDOS_NAO_TENHAM_TABELA_FRETE_CONFIGURADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearInclusaoAlteracaoPedidosNaoTenhamTabelaFreteConfigurada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiltrarPedidosPorRemetenteRetiradaProduto", Column = "CTD_FILTRAR_PEDIDOS_POR_REMETENTE_RETIRADA_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarPedidosPorRemetenteRetiradaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarPedidoReentregaAutomaticamenteRoteirizar", Column = "CTD_ENVIAR_PEDIDO_REENTREGA_AUTOMATICAMENTE_ROTEIRIZAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPedidoReentregaAutomaticamenteRoteirizar { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Pedido"; }
        }
    }
}
