
namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_NFSE_MANUAL", EntityName = "ConfiguracaoNFSeManual", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoNFSeManual", NameType = typeof(ConfiguracaoNFSeManual))]
    public class ConfiguracaoNFSeManual : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNM_UTILIZAR_NUMERO_SERIE_INFORMADO_TELA_QUANDO_EMITIDO_MODELO_DOCUMENTO_NAO_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNumeroSerieInformadoTelaQuandoEmitidoModeloDocumentoNaoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNM_VALIDAR_LOCALIDADE_PRESTACAO_TRANSPORTADOR_CONFIGURACAO_NFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarLocalidadePrestacaoTransportadorConfiguracaoNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNM_VALIDAR_EXISTENCIA_PARA_INSERIR_NFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarExistenciaParaInserirNFSe { get; set; }
    }
}
