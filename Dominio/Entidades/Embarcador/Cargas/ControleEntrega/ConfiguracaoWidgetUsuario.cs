namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_WIDGET_USUARIO", EntityName = "ConfiguracaoWidgetUsuario", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario", NameType = typeof(ConfiguracaoWidgetUsuario))]
    public class ConfiguracaoWidgetUsuario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CWU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_NOME_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNomeMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_VERSAO_APLICATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirVersaoAplicativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_NIVEL_BATERIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNivelBateria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_SINAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirSinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_NUMERO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_PROXIMO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirProximoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_VALOR_TOTAL_PRODUTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirValorTotalProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_PREVISAO_PROXIMA_PARADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPrevisaoProximaParada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_DISTANCIA_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDistanciaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_TEMPO_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTempoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_ENTREGA_COLETA_REALIZADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirEntregaColetasRealizadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_PESO_RESTANTE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPesoRestanteEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_NUMERO_PEDIDO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroPedidoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_NUMERO_ORDEM_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroOrdemPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_PRIMEIRO_SEGUNDO_TRECHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPrimeiroSegundoTrecho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_ANALISTA_RESPONSAVEL_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAnalistaResponsavelMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_RETORNAR_INFORMACOES_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarInformacoesMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_TELEFONE_CELULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTelefoneCelular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_PREVISAO_RECALCULADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPrevisaoRecalculada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_EXPEDIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_NUMERO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_PESO_BRUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_PESO_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_TENDENCIA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTendenciaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_CANAL_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_MODAL_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirModalTransporte { get; set; }

        public virtual string Descricao { get { return Usuario?.Nome ?? string.Empty; } }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_CANAL_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_CONFIGURACAO_EXIBICAO_DETALHES_ENTREGA", Type = "StringClob", NotNull = false)]
        public virtual string ConfiguracaoExibicaoDetalhesEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_MESORREGIAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirMesorregiao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_REGIAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirRegiao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWU_EXIBIR_TENDENCIA_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool  ExibirTendenciaColeta { get; set; }
    }
}
