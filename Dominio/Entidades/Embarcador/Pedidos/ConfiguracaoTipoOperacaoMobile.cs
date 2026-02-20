using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_MOBILE", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoMobile", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMobile", NameType = typeof(ConfiguracaoTipoOperacaoMobile))]
    public class ConfiguracaoTipoOperacaoMobile : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_FOTOS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteFotosEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_QUANTIDADE_MINIMAS_FOTOS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeMinimasFotosEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_CONFIRMAR_CHEGADA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteConfirmarChegadaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_CONFIRMAR_CHEGADA_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteConfirmarChegadaColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_CONTROLAR_TEMPO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarTempoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_NAO_UTILIZAR_PRODUTOS_NA_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarProdutosNaColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITIR_ESCANEAR_CHAVES_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEscanearChavesNfe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_OBRIGAR_ESCANEAR_CHAVES_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarEscanearChavesNfe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITIR_VISUALIZAR_PROGRAMACAO_ANTES_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirVisualizarProgramacaoAntesViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_EXIBIR_ENTREGA_ANTES_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirEntregaAntesEtapaTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_EXIBIR_ENTREGA_ETAPA_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirEntregaEtapaEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_SOLICITAR_JUSTIFICATIVA_REGISTRO_FORA_RAIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarJustificativaRegistroForaRaio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_EVENTOS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteEventos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_CHAT", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteChat { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_SAC", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteSAC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_CANHOTO_MODO_MANUAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteCanhotoModoManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_CONFIRMAR_ENTREGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteConfirmarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_BLOQUEAR_RASTREAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool BloquearRastreamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_ENTREGA_PARCIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteEntregaParcial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_CONTROLAR_TEMPO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarTempoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_EXIBIR_RELATORIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExibirRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_NAO_RETORNAR_COLETAS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoRetornarColetas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_OBRIGAR_ASSINATURA_PRODUTOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ObrigarAssinaturaProdutor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_FOTOS_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteFotosColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_QUANTIDADE_MINIMAS_FOTOS_COLETA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMinimasFotosColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_FORCAR_PREENCHIMENTO_SEQUENCIAL_MOBILE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ForcarPreenchimentoSequencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_OBRIGAR_FOTO_CANHOTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ObrigarFotoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_OBRIGAR_ASSINATURA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ObrigarAssinaturaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_OBRIGAR_DADOS_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarDadosRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_SOLICITAR_RECONHECIMENTO_FACIAL_DO_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarReconhecimentoFacialDoRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_EXIBIR_AVALIACAO_NA_ASSINATURA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExibirAvaliacaoNaAssintura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_PERMITE_BAIXAR_OS_DOCUMENTOS_DE_TRANSPORTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteBaixarOsDocumentosDeTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_NECESSARIO_CONFIRMACAO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessarioConfirmacaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_TEMPO_LIMITE_CONFIRMACAO_MOTORISTA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan TempoLimiteConfirmacaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_NAO_LISTAR_PRODUTOS_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoListarProdutosColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_AGUARDAR_ANALISE_NAO_CONFORMIDADE_NFS_CHECKIN", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardarAnaliseNaoConformidadesNFsCheckin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_NAO_APRESENTAR_DATA_INICIO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoApresentarDataInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_INICIAR_VIAGEM_NO_CONTROLE_DE_PATIO_AO_INICIAR_VIAGEM_NO_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciarViagemNoControleDePatioAoIniciarViagemNoApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COM_REPLICAR_DATA_DIGITALIZACAO_CANHOTO_DATA_ENTREGA_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReplicarDataDigitalizacaoCanhotoDataEntregaCliente { get; set; }

        public virtual string Descricao => "Configuração Tipo Operação Mobile";
    }
}
