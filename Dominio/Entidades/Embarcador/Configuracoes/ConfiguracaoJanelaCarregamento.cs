using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_JANELA_CARREGAMENTO", EntityName = "ConfiguracaoJanelaCarregamento", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento", NameType = typeof(ConfiguracaoJanelaCarregamento))]
    public class ConfiguracaoJanelaCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CJC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_EXIBIR_OPCAO_LIBERAR_PARA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirOpcaoLiberarParaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_EXIBIR_OPCAO_MULTIMODAL_AGENDAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirOpcaoMultiModalAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_SUGERIR_DATA_ENTREGA_AGENDAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SugerirDataEntregaAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_GERAR_FLUXO_PATIO_CARGA_COM_EXPEDIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFluxoPatioCargaComExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_DISPONIBILIZAR_CARGA_PARA_TRANSPORTADORES_POR_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_ATUALIZAR_DATA_INICIAL_COLETA_AO_ALTERAR_HORARIO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarDataInicialColetaAoAlterarHorarioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_BLOQUEAR_GERACAO_JANELA_PARA_CARGA_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearGeracaoJanelaParaCargaRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_ATIVAR_PLANEJAMENTO_FROTA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarPlanejamentoFrotaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_ATIVAR_PLANEJAMENTO_FROTA_NO_PLANEJAMENTO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarPlanejamentoFrotaNoPlanejamentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_NAO_PERMITIR_RECALCULAR_VALOR_FRETE_INFORMADO_PELO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirRecalcularValorFreteInformadoPeloTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_NAO_ENVIAR_EMAIL_ALTERACAO_DATA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarEmailAlteracaoDataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_DIAS_PARA_PERMITIR_INFORMAR_HORARIO_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaPermitirInformarHorarioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_ENCAIXAR_HORARIO_RETIRADA_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncaixarHorarioRetiradaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_LIBERAR_CARGA_PARA_COTACAO_AO_LIBERAR_PARA_TRANSPORTADORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCargaParaCotacaoAoLiberarParaTransportadores { get; set; }

        [Obsolete("Migrado para o campo DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada da Entidade CentroDescarregamento")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_DEFINIR_NAO_COMPARECIMENTO_AUTOMATICO_APOS_PRAZO_DATA_AGENDADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PERMITIR_LIBERAR_CARGA_PARA_TRANSPORTADORES_TERCEIROS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirLiberarCargaParaTransportadoresTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_GERAR_JANELA_DE_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarJanelaDeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_UTILIZAR_CENTRO_DESCARREGAMENTO_POR_TIPO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCentroDescarregamentoPorTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_EXIBIR_DETALHES_AGENDAMENTO_JANELA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDetalhesAgendamentoJanelaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_UTILIZAR_PERIODO_DESCARREGAMENTO_EXCLUSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPeriodoDescarregamentoExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_DISPONIBILIZAR_CARGA_PARA_TRANSPORTADORES_POR_PRIORIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarCargaParaTransportadoresPorPrioridade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_EXIBIR_HORA_AGENDADA_PARA_CARGAS_EXCEDENTES_JANELA_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_BLOQUEAR_VEICULO_SEM_TAG_VALE_PEDAGIO_ATIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearVeiculoSemTagValePedagioAtiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_NAO_CANCELAR_CARGA_AO_APLICAR_STATUS_FINALIZADOR_JANELA_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_TEMPO_PERMITIR_REAGENDAMENTO_HORAS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPermitirReagendamentoHoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PERMITIR_TRANSPORTADOR_INFORMAR_PLACAS_MOTORISTA_AO_DECLARAR_INTERESSE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para Janela de Carregamento"; }
        }
    }
}
