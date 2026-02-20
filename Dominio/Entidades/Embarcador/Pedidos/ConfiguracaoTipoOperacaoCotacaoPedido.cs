namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_COTACAO_PEDIDO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoCotacaoPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCotacaoPedido", NameType = typeof(ConfiguracaoTipoOperacaoCotacaoPedido))]
    public class ConfiguracaoTipoOperacaoCotacaoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_HABILITA_INFORMAR_DADOS_DOS_PEDIDOS_NA_COTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitaInformarDadosDosPedidosNaCotacao { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações Tipo Operação para Cotação de Pedido."; }
        }
    }
}