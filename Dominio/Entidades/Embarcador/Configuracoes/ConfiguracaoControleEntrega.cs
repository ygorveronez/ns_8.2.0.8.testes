using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CONTROLE_ENTREGA", EntityName = "ConfiguracaoControleEntrega", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega", NameType = typeof(ConfiguracaoControleEntrega))]
    public class ConfiguracaoControleEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_MENSAGEM_CHAT_ASSUMIR_MONITORAMENTO_CARGA", TypeType = typeof(string), Length = 4096, NotNull = true)]
        public virtual string MensagemChatAssumirMonitoramentoCarga { get; set; }

        /// <summary>
        /// Após o tempo configurado (em minutos) da emissão dos Documentos (data finalização emissão) o sistema inicia automaticamente a viagem caso não esteja iniciada.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_TEMPO_INICIO_VIAGEM_APOS_EMISSAO_DOC", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoInicioViagemAposEmissaoDoc { get; set; }

        /// <summary>
        /// Após o tempo configurado (em minutos) a finalização do Fluxo do pátio  o sistema inicia automaticamente a viagem caso não esteja iniciada.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_TEMPO_INICIO_VIAGEM_APOS_FINALIZACAO_FLUXO_PATIO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoInicioViagemAposFinalizacaoFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_UTILIZAR_PREVISAO_ENTREGA_PEDIDO_COMO_DATA_PREVISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPrevisaoEntregaPedidoComoDataPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_UTILIZAR_MAIOR_DATA_COLETA_PREVISTA_COMO_DATA_PREVISTA_PARA_ENTREGA_UNICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarMaiorDataColetaPrevistaComoDataPrevistaParaEntregaUnica { get; set; }

        /// <summary>
        /// Obrigatório informar Freetime no cadastro de rotas (Coleta/Entrega/Fronteiras)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_OBRIGATORIO_INFORMAR_FREETIME", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarFreetime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_EXIBIR_OPCAO_AJUSTAR_ENTREGA_ON_TIME", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirOpcaoAjustarEntregaOnTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_EXIBIR_PACOTES_OCORRENCIA_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPacotesOcorrenciaControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITIR_REORDENAR_ENTREGAS_ADD_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirReordenarEntregasAoAddPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_HORA_CORTE_RECALCULAR_PRAZO_ENTREGA_APOS_EMISSAO_DOCUMENTOS", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_HORA_FIM_PADRAO_ENTREGA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraFimPadraoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITE_ALTERAR_AGENDAMENTO_DA_ENTREGA_NO_ACOMPANHAMENTO_DE_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarAgendamentoDaEntregaNoAcompanhamentoDeCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITIR_ABRIR_ATENDIMENTO_VIA_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAbrirAtendimentoViaControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_RECALCULAR_PREVISAO_AO_INICIAR_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RecalcularPrevisaoAoIniciarViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_EXIBIR_DATA_ENTREGA_NOTA_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDataEntregaNotaControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITE_EXIBIR_CARGA_CANCELADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteExibirCargaCancelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_NAO_PERMITIR_CONFIRMACAO_ENTREGA_PORTAL_TRANSPORTADOR_SEM_DIGITALIZACAO_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirConfirmacaoEntregaPortalTransportadorSemDigitalizacaoCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_CONSIDERAR_CARGA_ORIGEM_PARA_ENTREGAS_TRANSBORDADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarCargaOrigemParaEntregasTransbordadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITIR_ALTERAR_DATA_AGENDAMENTO_ENTREGA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarDataAgendamentoEntregaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_REJEITAR_ENTREGA_NOTA_FISCAL_REJEITAR_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RejeitarEntregaNotaFiscalAoRejeitarCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_CONSIDERAR_MEDIA_DE_VELOCIDADE_DAS_ULTIMAS_CINCO_POSICOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarMediaDeVelocidadeDasUltimasCincoPosicoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_BLOQUEAR_INICIO_E_FIM_DE_VIAGEM_PELO_TRANSPORTADOR_EM_CARGA_NAO_EMITIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_UTILIZAR_LEAD_TIME_DA_TABELA_DE_FRETE_PARA_CALCULO_DA_PREVISAO_DE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarLeadTimeDaTabelaDeFreteParaCalculoDaPrevisaoDeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_CALCULAR_DATA_AGENDAMENTO_AUTOMATICAMENTE_DATA_FATURAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularDataAgendamentoAutomaticamenteDataFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITIR_ENVIO_CANHOTOS_PELO_PORTAL_TRANSPORTADOR_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_TORNAR_FINALIZACAO_DE_ENTREGAS_ASSINCRONA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TornarFinalizacaoDeEntregasAssincrona { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITIR_ENVIO_NOVAS_OCORRENCIAS_COM_MESMO_CADASTRO_TIPO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEnvioNovasOcorrenciasComMesmoCadastroTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITIR_BUSCAR_CARGAS_AGRUPADAS_AO_PESQUISAR_NUMERO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirBuscarCargasAgrupadasAoPesquisarNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_ENCERRA_MDFE_AUTOMATICAMENTE_CONFORME_ENTREGAS_FINALIZADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarMDFeAutomaticamenteAoFinalizarEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITIR_AJUSTAR_ENTREGAS_ETAPAS_ANTERIORES_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAjustarEntregasEtapasAnterioresIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_PERMITIR_BLOQUEIO_FINALIZACAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirBloqueioFinalizacaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_POSSUI_NOTA_COBERTURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNotaCobertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoReprocessarCargaEntregasSemNotas", Column = "CCE_TEMPO_REPROCESSAR_CARGA_ENTREGAS_SEM_NOTAS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoReprocessarCargaEntregasSemNotas { get; set; }
        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}