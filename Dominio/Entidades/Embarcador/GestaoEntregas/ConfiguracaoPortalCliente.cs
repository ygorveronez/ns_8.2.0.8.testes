

namespace Dominio.Entidades.Embarcador.GestaoEntregas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_PORTAL_CLIENTE", EntityName = "ConfiguracaoPortalCliente", Name = "Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente", NameType = typeof(ConfiguracaoPortalCliente))]
    public class ConfiguracaoPortalCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirMapa", Column = "CPL_EXIBIR_MAPA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExibirMapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDetalhesPedido", Column = "CPL_EXIBIR_DETALHES_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDetalhesPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirHistoricoPedido", Column = "CPL_EXIBIR_HISTORICO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHistoricoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDetalhesMotorista", Column = "CPL_EXIBIR_DETALHES_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDetalhesMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDetalhesProduto", Column = "CPL_EXIBIR_DETALHES_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDetalhesProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirProduto", Column = "CPL_EXIBIR_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarAvaliacao", Column = "CPL_HABILITAR_AVALIACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool HabilitarAvaliacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarPrevisaoEntrega", Column = "CPL_HABILITAR_PREVISAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarObservacao", Column = "CPL_HABILITAR_OBSERVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarNumeroPedidoCliente", Column = "CPL_HABILITAR_NUMERO_PEDIDO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarNumeroPedidoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarNumeroOrdemCompra", Column = "CPL_HABILITAR_NUMERO_ORDEM_COMPRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarNumeroOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquido", Column = "CPL_PESO_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "CPL_PESO_BRUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeVolumes", Column = "CPL_QUANTIDADE_VOLUMES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool QuantidadeVolumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarSMS", Column = "CPL_ENVIAR_SMS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EnviarSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarEmail", Column = "CPL_ENVIAR_EMAIL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EnviarEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarMensagemWhatsApp", Column = "CPL_ENVIAR_MENSAGEM_WHATSAPP", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EnviarMensagemWhatsApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_LINK_AVALIACAO_EXTERNA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string LinkAvaliacaoExterna { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_TIPO_AVALIACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAvaliacaoPortalCliente), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAvaliacaoPortalCliente TipoAvaliacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAdicionarAnexos", Column = "CPL_PERMITIR_ADICIONAR_ANEXOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAdicionarAnexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarVisualizacaoFotosPortal", Column = "CPL_HABILITAR_VISUALIZACAO_FOTOS_PORTAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarVisualizacaoFotosPortal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarAcessoPortalMultiCliFor", Column = "CPL_HABILITAR_ACESSO_PORTAL_MULTICLIFOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarAcessoPortalMultiCliFor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_LINK_ACESSO_PORTAL_MULTICLIFOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string LinkAcessoPortalMultiCliFor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Portal do Cliente";
            }
        }
    }
}
