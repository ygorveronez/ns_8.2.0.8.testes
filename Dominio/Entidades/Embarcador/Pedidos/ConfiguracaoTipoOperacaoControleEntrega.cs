using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CONTROLE_ENTREGA", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoControleEntrega", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega", NameType = typeof(ConfiguracaoTipoOperacaoControleEntrega))]
    public class ConfiguracaoTipoOperacaoControleEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_LOCAL_PARQUEAMENTO_CLIENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente LocalDeParqueamentoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_EXIGIR_CONFERENCIA_PRODUTOS_AO_CONFIRMAR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirConferenciaProdutosAoConfirmarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_ENVIAR_BOLETIM_DE_VIAGEM_AO_FINALIZAR_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarBoletimViagemAoFinalizarViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_ENVIAR_BOLETIM_DE_VIAGEM_AO_FINALIZAR_VIAGEM_PARA_REMETENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarBoletimViagemAoFinalizarViagemParaRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_ENVIAR_BOLETIM_DE_VIAGEM_AO_FINALIZAR_VIAGEM_PARA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarBoletimViagemAoFinalizarViagemParaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_ALTERAR_SITUACAO_ENTREGA_NFE_PARA_DEVOLVIDA_AO_CONFIRMAR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterarSituacaoEntregaNFeParaDevolvidaAoConfirmarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_GERAR_EVENTO_COLETA_ENTREGA_UNICO_TODOS_TRECHOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarEventoColetaEntregaUnicoParaTodosTrechos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_PERMITIR_INFORMAR_NOTA_FISCAL_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarNotasFiscaisNoControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_EXIGIR_INFORMAR_NUMERO_PACOTES_NA_COLETA_TRIZY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarNumeroPacotesNaColetaTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_FINALIZAR_CONTROLE_ENTREGA_AO_FINALIZAR_MONITORAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarControleEntregaAoFinalizarMonitoramentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_RECRIAR_CONTROLE_DE_ENTREGAS_AO_CONFIRMAR_ENVIO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RecriarControleDeEntregasAoConfirmarEnvioDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_CONFIRMAR_COLETAS_QUANDO_TODAS_AS_ENTREGAS_DA_CARGA_FOREM_CONCLUIDAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_DISPONIBILIZAR_FOTO_DA_COLETA_NA_TELA_DE_APROVACAO_DE_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_NAO_FINALIZAR_ENTREGAS_POR_TRACKING_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarEntregasPorTrackingMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_NAO_FINALIZAR_COLETAS_POR_TRACKING_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarColetasPorTrackingMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_GERAR_CONTROLE_ENTREGA_SEM_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarControleEntregaSemRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_ORDENAR_COLETAS_POR_DATA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OrdenarColetasPorDataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_DATA_REALIZACAO_DO_EVENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataCalculoParadaNoPrazo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataCalculoParadaNoPrazo? DataRealizacaoDoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_DATA_PREVISTA_DO_EVENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataCalculoParadaNoPrazo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataCalculoParadaNoPrazo? DataPrevistaDoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_SOBRESCREVER_DATA_ENTRADA_SAIDA_ALVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SobrescreverDataEntradaSaidaAlvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_BLOQUEAR_INICIO_E_FIM_DE_VIAGEM_PELO_TRANSPORTADOR_EM_CARGA_NAO_EMITIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida { get; set; }

        #region CONFIGURACOES PREVISAO ENTREGA POR TIPO OPERACAO

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_CONSIDERAR_CONFIGURACAO_PREVISAO_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_DESCONSIDERAR_SABADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsiderarSabadosCalculoPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_DESCONSIDERAR_DOMINGOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsiderarDomingosCalculoPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_DESCONSIDERAR_FERIADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsiderarFeriadosCalculoPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_CONSIDERAR_JORNADA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarJornadaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_HORARIO_INICIAL_ALMOCO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan HorarioInicialAlmoco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_MINUTOS_INTERVALO", TypeType = typeof(int), NotNull = false)]
        public virtual int MinutosIntervalo { get; set; }

        //USADO NO RELATORIO DE PARADAS ONDE DESCONSIDERA HORARIOS
        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_DESCONSIDERAR_HORARIO_PRAZO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsiderarHorariosParaPrazoEntrega { get; set; }

        #endregion

        public virtual string Descricao
        {
            get { return "Configurações do Controle de Entrega."; }
        }
    }
}