namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_AGENDAMENTO_COLETA", EntityName = "ConfiguracaoAgendamentoColeta", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta", NameType = typeof(ConfiguracaoAgendamentoColeta))]
    public class ConfiguracaoAgendamentoColeta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_REMOVER_ETAPA_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverEtapaAgendamentoAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_PERMITIR_TRANSPORTADOR_CADASTRAR_AGENDAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorCadastrarAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_CONSULTAR_SOMENTE_TRANSPORTADORES_PERMITIDOS_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarSomenteTransportadoresPermitidosCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_GERAR_AUTOMATICAMENTE_SENHA_PEDIDOS_AGENDAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAutomaticamenteSenhaPedidosAgendas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_MOSTRAR_TIPO_DE_OPERACAO_NO_PORTAL_MULTI_EMBARCADOR_AGENDAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MostrarTipoDeOperacaoNoPortalMultiEmbarcadorAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_CALCULAR_DATA_DE_ENTREGA_POR_TEMPO_DE_DESCARGA_DA_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularDataDeEntregaPorTempoDeDescargaDaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_TEMPO_PADRAO_DE_DESCARGA_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPadraoDeDescargaMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_UTILIZA_RAZAO_SOCIAL_NA_VISAO_DO_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaRazaoSocialNaVisaoDoAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_ENVIAR_EMAIL_DE_NOTIFICACAO_AUTOMATICAMENTE_AO_TRANSPORTADOR_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailDeNotificacaoAutomaticamenteAoTransportadorDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoModeloEmail", Column = "CME_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail ModeloEmail { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para Agendamento de Coleta"; }
        }
    }
}
