using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_CARGA_EMISSAO_DOCUMENTO", EntityName = "ConfiguracaoCargaEmissaoDocumento", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento", NameType = typeof(ConfiguracaoCargaEmissaoDocumento))]
    public class ConfiguracaoCargaEmissaoDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_VALIDAR_DATA_PREVISAO_SAIDA_PEDIDO_MENOR_DATA_ATUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarDataPrevisaoSaidaPedidoMenorDataAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_VALIDAR_DATA_PREVISAO_ENTREGA_PEDIDO_MENOR_DATA_ATUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarDataPrevisaoEntregaPedidoMenorDataAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_NAO_ALTERAR_CENTRO_RESULTADO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAlterarCentroResultadoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_BLOQUEAR_EMISSAO_TOMADOR_SEM_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEmissaoTomadorSemEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_CONSULTAR_DOCUMENTOS_DESTINADOS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarDocumentosDestinadosCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_UTILIZAR_NUMERO_OUTRO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNumeroOutroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_NAO_PERMITIR_NFS_MANUAL_COM_MULTIPLOS_CENTROS_RESULTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirNFSComMultiplosCentrosResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_BLOQUEAR_EMISSAO_CARGA_TERCEIROS_SEM_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEmissaoCargaTerceirosSemValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_ATIVAR_ENVIO_DOCUMANTACAO_FINALIZACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioDocumentacaoFinalizacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_NAO_COMPRAR_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoComprarValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_NAO_PERMITIR_ACESSAR_DOCUMENTOS_ANTES_CARGA_EM_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAcessarDocumentosAntesCargaEmTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_CONTROLAR_VALORES_COMPONENTES_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarValoresComponentesCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_NAO_ENVIAR_EMAIL_DOCUMENTO_EMITIDO_PARA_PROPRIETARIO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarEmailDocumentoEmitidoProprietarioVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_NAO_PERMITIR_EMISSAO_COM_MESMA_ORIGEM_E_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirEmissaoComMesmaOrigemEDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_CRIAR_CONTROLE_DE_EMISSAO_DE_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CriarControleDeEmissaoDeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_GERAR_REGISTRO_POR_PEDIDO_NA_NFS_MANUAL_POR_CTE_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarRegistroPorPedidoNaNFSManualPorCTeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_USA_FLUXO_SUBSTITUICAO_FASEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsaFluxoSubstituicaoFaseada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAjusteSiniefNro8", Column = "CCE_DATA_AJUSTE_SINIEF_NRO8", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAjusteSiniefNro8 { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração para emissão dos documentos da carga";
            }
        }
    }
}
