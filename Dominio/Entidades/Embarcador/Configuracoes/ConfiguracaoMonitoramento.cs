namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONITORAMENTO", EntityName = "ConfiguracaoMonitoramento", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento", NameType = typeof(ConfiguracaoMonitoramento))]
    public class ConfiguracaoMonitoramento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TelaMonitoramentoFiltroFilialDaCarga", Column = "COM_TELA_MONITORAMENTO_PADRAO_FILTRO_FILIAL_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TelaMonitoramentoFiltroFilialDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgruparVeiculosMapaPosicaoFrota", Column = "COM_AGRUPAR_VEICULOS_MAPA_POSICAO_FROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparVeiculosMapaPosicaoFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarAlertasMonitoramentoEmail", Column = "COM_ENVIAR_ALERTAS_MONITORAMENTO_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarAlertasMonitoramentoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailsAlertaMonitoramento", Column = "COM_EMAILS_ALERTA_MONITORAMENTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailsAlertaMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtualizarMonitoramentoAoGerarMDFeManual", Column = "COM_ATUALIZAR_MONITORAMENTO_GERAR_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarMonitoramentoAoGerarMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtualizarStatusViagemMonitoramentoAoIniciarViagem", Column = "COM_ATUALIZAR_STATUS_VIAGEM_MONITORAMENTO_AO_INICIAR_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarStatusViagemMonitoramentoAoIniciarViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ManterMonitoramentosDeCargasCanceladasAoReceberNovaCarga", Column = "COM_MANTER_MONITORAMENTO_CARGAS_CANCELADAS_AO_RECEBER_NOVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterMonitoramentosDeCargasCanceladasAoReceberNovaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarNovoMonitoramentoCarga", Column = "COM_NAO_GERAR_NOVO_MONITORAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarNovoMonitoramentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoMonitoramentoCritico", Column = "COM_VALOR_MINIMO_MONITORAMENTO_CRITICO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorMinimoMonitoramentoCritico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TelaMonitoramentoAtualizarGridAoReceberAtualizacoesOnTime", Column = "COM_TELA_MONITORAMENTO_ATUALIZAR_GRID_AO_RECEBER_ATUALIZACOES_ONTIME", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TelaMonitoramentoAtualizarGridAoReceberAtualizacoesOnTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IgnorarStatusViagemMonitoramentoAnterioresTransito", Column = "COM_IGNORAR_STATUS_VIAGEM_MONITORAMENTO_ANTERIORES_TRANSITO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarStatusViagemMonitoramentoAnterioresTransito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificarEntradaEmAlvoComPosicaoUnicaIgnorandoTemposDePermanencia", Column = "COM_IDENTIFICAR_ENTRADA_EM_ALVO_COM_POSICAO_UNICA_IGNORANDO_TEMPOS_DE_PERMANENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IdentificarEntradaEmAlvoComPosicaoUnicaIgnorandoTemposDePermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarMonitoramentoAoGerarTransbordoCarga", Column = "COM_FINALIZAR_MONITORAMENTO_GERAR_TRANSBODRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarMonitoramentoAoGerarTransbordoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificarCarregamentoAoIniciarOuFinalizarMonitoramentosConsecutivos", Column = "COM_IDENTIFICAR_CARREGAMENTO_AO_INICIAR_OU_FINALIZAR_MONITORAMENTOS_CONSECUTIVOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IdentificarCarregamentoAoIniciarOuFinalizarMonitoramentosConsecutivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarDadosSumarizadosDasParadasAoFinalizarOMonitoramento", Column = "COM_GERAR_DADOS_SUMARIZADOS_DAS_PARADAS_AO_FINALIZAR_O_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarDadosSumarizadosDasParadasAoFinalizarOMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMonitoramentoAoFecharCarga", Column = "COM_GERAR_MONITORAMENTO_AO_FECHAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMonitoramentoAoFecharCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarDataAuditoriaComoDataFimDoMonitoramento", Column = "COM_CONSIDERAR_DATA_AUDITORIA_COMO_DATA_FIM_DO_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarDataAuditoriaComoDataFimDoMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarCargaEmMonitoramentoAoReceberPosicao", Column = "COM_VALIDAR_CARGA_EM_MONITORAMENTO_RECEBER_POSICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarCargaEmMonitoramentoAoReceberPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FrequenciaCapturaPosicoesAppTrizy", Column = "COM_FREQUENCIA_CAPTURA_POSICOES_APP_TRIZY", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FrequenciaTrackingAppTrizy), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FrequenciaTrackingAppTrizy FrequenciaCapturaPosicoesAppTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarAutomaticamenteAlertasDoMonitoramentoAoFinalizarViagem", Column = "COM_FINALIZAR_AUTOMATICAMENTE_ALERTAS_DO_MONITORAMENTO_AO_FINALIZAR_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarAutomaticamenteAlertasDoMonitoramentoAoFinalizarViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente", Column = "COM_FINALIZAR_AUTOMATICAMENTE_ALERTAS_DO_MONITORAMENTO_PERIODICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente", Column = "COM_DIAS_PARA_FINALIZAR_AUTOMATICAMENTE_ALERTAS_DO_MONITORAMENTO_PERIODICAMENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarAutomaticamenteMonitoramentosEmAndamento", Column = "COM_FINALIZAR_AUTOMATICAMENTE_MONITORAMENTO_ANDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarAutomaticamenteMonitoramentosEmAndamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasFinalizarAutomaticamenteMonitoramentoEmAndamento", Column = "COM_DIAS_PARA_FINALIZAR_AUTOMATICAMENTE_MONITORAMENTO_ANDAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasFinalizarAutomaticamenteMonitoramentoEmAndamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarAutomaticamenteMonitoramentosPrevisaoUltimaEntrega", Column = "COM_FINALIZAR_AUTOMATICAMENTE_MONITORAMENTO_PREVISAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarAutomaticamenteMonitoramentosPrevisaoUltimaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasFinalizarMonitoramentoPrevisaoUltimaEntrega", Column = "COM_DIAS_PARA_FINALIZAR_AUTOMATICAMENTE_MONITORAMENTO_PREVISAO_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasFinalizarMonitoramentoPrevisaoUltimaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaMaximaRotaCurta", Column = "COM_DISTANCIA_MAXIMA_ROTA_CURTA", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaMaximaRotaCurta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoPermitidoPermanenciaEmCarregamento", Column = "COM_TEMPO_PERMITIDO_PERMANENCIA_EM_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPermitidoPermanenciaEmCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoPermitidoPermanenciaNoCliente", Column = "COM_TEMPO_PERMITIDO_PERMANENCIA_NO_CLIENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPermitidoPermanenciaNoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoMonitoramento", Column = "COM_VERSAO_MONITORAMENTO", TypeType = typeof(string), NotNull = false)]
        public virtual string VersaoMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoCompletoMonitoramento", Column = "COM_CAMINHO_COMPLETO_MONITORAMENTO", TypeType = typeof(string), NotNull = false)]
        public virtual string CaminhoCompletoMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarModalAntigoDetalhesMonitoramento", Column = "COM_ULTILIZA_MODAL_ANTIGO_DETALHES_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarModalAntigoDetalhesMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarVinculoPermanenciasComHistoricoStatusViagem", Column = "COM_HABILITAR_VINCULO_PERMANENCIAS_COM_HISTORICO_STATUS_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarVinculoPermanenciasComHistoricoStatusViagem { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Monitoramento"; }
        }
    }
}
