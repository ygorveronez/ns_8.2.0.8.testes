using System.Drawing;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_CHAMADO", EntityName = "ConfiguracaoChamado", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoChamado", NameType = typeof(ConfiguracaoChamado))]
    public class ConfiguracaoChamado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_BLOQUEAR_ABERTURA_CHAMADO_RETENCAO_QUANDO_POSSUIR_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAberturaChamadoRetencaoQuandoPossuirReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_OBRIGATORIO_INFORMAR_NOTA_NA_DEVOLUCAO_PARCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarNotaNaDevolucaoParcialChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_HABILITAR_ARVORE_DECISAO_ESCALATION_LIST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarArvoreDecisaoEscalationList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_ESCALAR_AUTOMATICAMENTE_NIVEL_EXCEDER_TEMPO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EscalarAutomaticamenteNivelExcederTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_FINALIZAR_ENTREGA_QUANDO_DEVOLUCAO_PARCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarEntregaQuandoDevolucaoParcial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_OBRIGATORIO_INFORMAR_NOTA_FISCAL_PARA_ABERTURA_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarNotaFiscalParaAberturaChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_CALCULAR_VALOR_DAS_DEVOLUCOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularValorDasDevolucoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_PERMITIR_REGISTRAR_OBSERVACOES_SEM_VISUALIZACAO_TRANSPORTADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRegistrarObservacoesSemVisualizacaoTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_ATIVAR_ALERTA_CHAMADOS_MAIS_48H_ABERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarAlertaChamadosMais48hAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_PERMITIR_ABRIR_CHAMADO_PARA_ENTREGA_JA_REALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAbrirChamadoParaEntregaJaRealizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_PERMITE_FINALIZAR_ATENDIMENTO_COM_OCORRENCIA_REJEITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteFinalizarAtendimentoComOcorrenciaRejeitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_VINCULAR_PRIMEIRO_PEDIDO_DO_CLIENTE_AO_ABRIR_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularPrimeiroPedidoDoClienteAoAbrirChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_PERMITIR_SELECIONAR_CTE_APENAS_COM_NFE_VINCULADA_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSelecionarCteApenasComNfeVinculadaOcorrencia { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_PERMITIR_GERAR_ATENDIMENTO_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirGerarAtendimentoPorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_FAZER_GESTAO_CRITICIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FazerGestaoCriticidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_PERMITIR_ATUALIZAR_CHAMADO_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAtualizarChamadoStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_OCULTAR_TOMADOR_NO_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarTomadorNoAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_BLOQUEAR_ESTORNO_ATENDIMENTOS_FINALIZADOS_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEstornoAtendimentosFinalizadosPortalTransportador { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para o chamado"; }
        }
    }
}
