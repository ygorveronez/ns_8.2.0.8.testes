using Dominio.Entidades.Embarcador.Frete;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_EMP", EntityName = "IntegracaoEMP", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP", NameType = typeof(IntegracaoEMP))]
    public class IntegracaoEMP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoEMP", Column = "CIE_POSSUI_INTEGRACAO_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BoostrapServersEMP", Column = "CIE_BOOSTRAP_SERVERS_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string BoostrapServersEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GroupIdEMP", Column = "CIE_GROUP_ID_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string GroupIdEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioEMP", Column = "CIE_USUARIO_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaEMP", Column = "CIE_SENHA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioCTesAnterioresEMP", Column = "CIE_ATIVAR_ENVIO_CTES_ANTERIORES_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioCTesAnterioresEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicCTesAnterioresEMP", Column = "CIE_TOPIC_CTES_ANTERIORES_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicCTesAnterioresEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicCTesAnterioresEMP", Column = "EMP_STATUS_TOPIC_CTES_ANTERIORES_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicCTesAnterioresEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicBuscarCTesEMP", Column = "CIE_TOPIC_BUSCAR_CTES_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicBuscarCTesEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicBuscarCTesEMP", Column = "EMP_STATUS_TOPIC_BUSCAR_CTES_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicBuscarCTesEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicBuscarFaturaCTeEMP", Column = "CIE_TOPIC_BUSCAR_FATURA_CTE_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicBuscarFaturaCTeEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicBuscarFaturaCTeEMP", Column = "EMP_STATUS_TOPIC_BUSCAR_FATURA_CTE_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicBuscarFaturaCTeEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicBuscarCargaEMP", Column = "CIE_TOPIC_BUSCAR_CARGA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicBuscarCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicBuscarCargaEMP", Column = "EMP_STATUS_TOPIC_BUSCAR_CARGA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicBuscarCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCargaEMP", Column = "CIE_ATIVAR_INTEGRACAO_CARGA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioSerializaçãoEMP", Column = "CIE_ATIVAR_ENVIO_SERIALIZACAO_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioSerializaçãoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoCargaEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_CARGA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoCargaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CARGA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoDadosCargaEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_DADOS_CARGA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoDadosCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoDadosCargaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_DADOS_CARGA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoDadosCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCancelamentoCargaEMP", Column = "CIE_ATIVAR_INTEGRACAO_CANCELAMENTO_CARGA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCancelamentoCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoCancelamentoCargaEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_CARGA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoCancelamentoCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoCancelamentoCargaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_CARGA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoCancelamentoCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoOcorrenciaEMP", Column = "CIE_ATIVAR_INTEGRACAO_OCORRENCIA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoOcorrenciaEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_OCORRENCIA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoOcorrenciaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_OCORRENCIA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCancelamentoOcorrenciaEMP", Column = "CIE_ATIVAR_INTEGRACAO_CANCELAMENTO_OCORRENCIA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCancelamentoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoCancelamentoOcorrenciaEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_OCORRENCIA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoCancelamentoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoCancelamentoOcorrenciaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_OCORRENCIA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoCancelamentoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCTeManualEMP", Column = "CIE_ATIVAR_INTEGRACAO_CTE_MANUAL_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCTeManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoCTeManualEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_CTE_MANUAL_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoCTeManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoCTeManualEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CTE_MANUAL_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoCTeManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoCancelamentoCTeManualEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_CTE_MANUAL_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoCancelamentoCTeManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoCancelamentoCTeManualEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_CTE_MANUAL_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoCancelamentoCTeManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoFaturaEMP", Column = "CIE_ATIVAR_INTEGRACAO_FATURA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoFaturaEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_FATURA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoFaturaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_FATURA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCancelamentoFaturaEMP", Column = "CIE_ATIVAR_INTEGRACAO_CANCELAMENTO_FATURA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCancelamentoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoCancelamentoFaturaEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_FATURA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoCancelamentoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoCancelamentoFaturaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_FATURA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoCancelamentoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCartaCorrecaoEMP", Column = "CIE_ATIVAR_INTEGRACAO_CARTA_CORRECAO_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCartaCorrecaoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoCartaCorrecaoEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_CARTA_CORRECAO_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoCartaCorrecaoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoCartaCorrecaoEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CARTA_CORRECAO_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoCartaCorrecaoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoContainerEMP", Column = "CIE_ATIVAR_INTEGRACAO_CONTAINER_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoContainerEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoContainerEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_CONTAINER_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoContainerEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoContainerEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CONTAINER_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoContainerEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoNFTPEMP", Column = "CIE_ATIVAR_INTEGRACAO_NFTP_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoNFTPEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoNFTPEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_NFTP_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoNFTPEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoNFTPEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_NFTP_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoNFTPEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoRecebimentoNavioEMP", Column = "CIE_ATIVAR_INTEGRACAO_RECEBIMENTO_NAVIO_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoRecebimentoNavioEMP { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CIE_COMPONENTE_FRETE_NFTP_EMP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ComponenteFrete ComponenteFreteNFTPEMP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CIE_COMPONENTE_IMPOSTO_NFTP_EMP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ComponenteFrete ComponenteImpostoNFTPEMP { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CIE_COMPONENTE_VALOR_TOTAL_PRESTACAO_NFTP_EMP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ComponenteFrete ComponenteValorTotalPrestacaoNFTPEMP { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoComObjetoUnitoParaTodosTopics", Column = "CIE_ATIVAR_INTEGRACAO_COM_OBJETO_UNICO_PARA_TODOS_TOPICS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoComObjetoUnitoParaTodosTopics { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicRecebimentoIntegracaoVesselEMP", Column = "CIE_TOPIC_RECEBIMENTO_INTEGRACAO_VESSEL_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicRecebimentoIntegracaoVesselEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicRecebimentoIntegracaoVesselEMP", Column = "EMP_STATUS_TOPIC_RECEBIMENTO_INTEGRACAO_VESSEL_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicRecebimentoIntegracaoVesselEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoRecebimentoPessoaEMP", Column = "CIE_ATIVAR_INTEGRACAO_RECEBIMENTO_PESSOA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoRecebimentoPessoaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicRecebimentoIntegracaoCustomerEMP", Column = "CIE_TOPIC_RECEBIMENTO_INTEGRACAO_CUSTOMER_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicRecebimentoIntegracaoCustomerEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicRecebimentoIntegracaoCustomerEMP", Column = "EMP_STATUS_TOPIC_RECEBIMENTO_INTEGRACAO_CUSTOMER_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicRecebimentoIntegracaoCustomerEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoRecebimentoScheduleEMP", Column = "CIE_ATIVAR_INTEGRACAO_RECEBIMENTO_SCHEDULE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoRecebimentoScheduleEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarLeituraHeaderSchedule", Column = "CIE_ATIVAR_LEITURA_HEADER_SCHEDULE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarLeituraHeaderSchedule { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicRecebimentoIntegracaoScheduleEMP", Column = "CIE_TOPIC_RECEBIMENTO_INTEGRACAO_SCHEDULE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicRecebimentoIntegracaoScheduleEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicRecebimentoIntegracaoScheduleEMP", Column = "EMP_STATUS_TOPIC_RECEBIMENTO_INTEGRACAO_SCHEDULE", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicRecebimentoIntegracaoScheduleEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsumerGroupSchedule", Column = "CIE_CONSUMER_GROUP_SCHEDULE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ConsumerGroupSchedule { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoRecebimentoRolagemEMP", Column = "CIE_ATIVAR_INTEGRACAO_RECEBIMENTO_ROLAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoRecebimentoRolagemEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarLeituraHeaderRolagem", Column = "CIE_ATIVAR_LEITURA_HEADER_ROLAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarLeituraHeaderRolagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicRecebimentoIntegracaoRolagemEMP", Column = "CIE_TOPIC_RECEBIMENTO_INTEGRACAO_ROLAGEM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicRecebimentoIntegracaoRolagemEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicRecebimentoIntegracaoRolagemEMP", Column = "EMP_STATUS_TOPIC_RECEBIMENTO_INTEGRACAO_ROLAGEM", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicRecebimentoIntegracaoRolagemEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsumerGroupRolagem", Column = "CIE_CONSUMER_GROUP_ROLAGEM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ConsumerGroupRolagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoParaSILEMP", Column = "CIE_ATIVAR_INTEGRACAO_PARA_SIL_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoParaSILEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoParaSILEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_PARA_SIL_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoParaSILEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoParaSILEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_PARA_SIL_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoParaSILEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlSchemaRegistry", Column = "CIE_URL_SCHEMA_REGISTRY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UrlSchemaRegistry { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioSchemaRegistry", Column = "CIE_USUARIO_SCHEMA_REGISTRY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioSchemaRegistry { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaSchemaRegistry", Column = "CIE_SENHA_SCHEMA_REGISTRY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaSchemaRegistry { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModificarConexaoParaRetina", Column = "CIE_MODIFICAR_CONEXAO_PARA_RETINA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModificarConexaoParaRetina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModificarConexaoParaEnvioRetina", Column = "CIE_MODIFICAR_CONEXAO_PARA_ENVIO_RETINA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModificarConexaoParaEnvioRetina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GroupIDRetina", Column = "CIE_GROUP_ID_RETINA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string GroupIDRetina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BootstrapServerRetina", Column = "CIE_BOOTSTRAP_SERVER_RETINA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string BootstrapServerRetina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLSchemaRegistryRetina", Column = "CIE_URL_SCHEMA_REGISTRY_RETINA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string URLSchemaRegistryRetina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioServerRetina", Column = "CIE_USUARIO_SERVER_RETINA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioServerRetina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioSchemaRegistryRetina", Column = "CIE_USUARIO_SCHEMA_REGISTRY_RETINA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioSchemaRegistryRetina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaServerRetina", Column = "CIE_SENHA_SERVER_RETINA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaServerRetina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaSchemaRegistryRetina", Column = "CIE_SENHA_SCHEMA_REGISTRY_RETINA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaSchemaRegistryRetina { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "CIE_CERTIFICADO_CRT_SERVER_RETINA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao CertificadoCRTServerRetina { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "CIE_CERTIFICADO_SCHEMA_REGISTRY_RETINA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao CertificadoShemaRegistryRetina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoBooking", Column = "CIE_ATIVAR_INTEGRACAO_BOOKING", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicBooking", Column = "CIE_TOPIC_BOOKING", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string TopicBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicRecebimentoIntegracaoBooking", Column = "EMP_STATUS_TOPIC_RECEBIMENTO_INTEGRACAO_BOOKING", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicRecebimentoIntegracaoBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAVRO", Column = "CIE_TIPO_AVRO", TypeType = typeof(TipoAVRO), NotNull = false)]
        public virtual TipoAVRO TipoAVRO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarLeituraHeadersConsumoEMP", Column = "CIE_ATIVAR_LEITURA_HEADERS_CONSUMO_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarLeituraHeadersConsumoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarLeituraHeadersConsumoKeyEMP", Column = "CIE_ATIVAR_LEITURA_HEADERS_CONSUMO_KEY_EMP", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string AtivarLeituraHeadersConsumoKeyEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarLeituraHeadersConsumoValueEMP", Column = "CIE_ATIVAR_LEITURA_HEADERS_CONSUMO_VALUE_EMP", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string AtivarLeituraHeadersConsumoValueEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarLeituraHeaderBooking", Column = "CIE_ATIVAR_LEITURA_HEADER_BOOKING", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarLeituraHeaderBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarLeituraHeaderCustomer", Column = "CIE_ATIVAR_LEITURA_HEADER_CUSTOMER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarLeituraHeaderCustomer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarLeituraHeaderVessel", Column = "CIE_ATIVAR_LEITURA_HEADER_VESSEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarLeituraHeaderVessel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsumerGroupBooking", Column = "CIE_CONSUMER_GROUP_BOOKING", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ConsumerGroupBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsumerGroupCustomer", Column = "CIE_CONSUMER_GROUP_CUSTOMER", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ConsumerGroupCustomer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsumerGroupVessel", Column = "CIE_CONSUMER_GROUP_VESSEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ConsumerGroupVessel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCEMercanteEMP", Column = "CIE_ATIVAR_INTEGRACAO_CE_MERCANTE_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCEMercanteEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoCEMercanteEMP", Column = "CIE_TOPIC_ENVIO_INTEGRACAO_CE_MERCANTE_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoCEMercanteEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicEnvioIntegracaoCEMercanteEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CE_MERCANTE_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicEnvioIntegracaoCEMercanteEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoCTEDaCargaEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_CTE_DA_CARGA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoCTEDaCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoCTEDaCargaEMP", Column = "CIE_TOPIC_INTEGRACAO_CTE_DA_CARGA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoCTEDaCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoCTEDaCargaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CET_DA_CARGA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoCTEDaCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCancelamentoDaCargaEMP", Column = "CIE_ATIVAR_INTEGRACAO_CANCELAMENTO_DA_CARGA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCancelamentoDaCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoCancelamentoDaCargaEMP", Column = "CIE_TOPIC_INTEGRACAO_CANCELAMENTO_DA_CARGA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoCancelamentoDaCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoCancelamentoDaCargaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_DA_CARGA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoCancelamentoDaCargaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoCTEManualEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_CTE_MANUAL_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoCTEManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoCTEManualEMP", Column = "CIE_TOPIC_INTEGRACAO_CTE_MANUAL_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoCTEManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoCTEManualEMP", Column = "EMP_STATUS_TOPIC_ENVIO_DA_INTEGRACAO_CTE_MANUAL_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoCTEManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoCancelamentoCTEManualEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_CANCELAMENTO_CTE_MANUAL_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoCancelamentoCTEManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoCancelamentoCTEManualEMP", Column = "CIE_TOPIC_INTEGRACAO_CANCELAMENTO_CTE_MANUAL_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoCancelamentoCTEManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoCancelamentoCTEManualEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_DO_CANCELAMENTO_CTE_MANUAL_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoCancelamentoCTEManualEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoOcorrenciaEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_OCORRENCIA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoOcorrenciaEMP", Column = "CIE_TOPIC_INTEGRACAO_OCORRENCIA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoOcorrenciaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_DA_OCORRENCIA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoCancelamentoOcorrenciaEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_CANCELAMENTO_OCORRENCIA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoCancelamentoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoCancelamentoOcorrenciaEMP", Column = "CIE_TOPIC_INTEGRACAO_CANCELAMENTO_OCORRENCIA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoCancelamentoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoCancelamentoOcorrenciaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_DO_CANCELAMENTO_OCORRENCIA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoCancelamentoOcorrenciaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoFaturaEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_FATURA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoFaturaEMP", Column = "CIE_TOPIC_INTEGRACAO_FATURA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoFaturaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_DA_FATURA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoCancelamentoFaturaEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_CANCELAMENTO_FATURA_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoCancelamentoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoCancelamentoFaturaEMP", Column = "CIE_TOPIC_INTEGRACAO_CANCELAMENTO_FATURA_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoCancelamentoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoCancelamentoFaturaEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_DA_FATURA_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoCancelamentoFaturaEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoCartaCorrecaoEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_CARTA_CORRECAO_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoCartaCorrecaoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoCartaCorrecaoEMP", Column = "CIE_TOPIC_INTEGRACAO_CARTA_CORRECAO_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoCartaCorrecaoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoCartaCorrecaoEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_DA_CARTA_CORRECAO_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoCartaCorrecaoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoBoletoEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_BOLETO_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoBoletoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoBoletoEMP", Column = "CIE_TOPIC_INTEGRACAO_BOLETO_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoBoletoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoBoletoEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_BOLETO_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoBoletoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioIntegracaoCancelamentoBoletoEMP", Column = "CIE_ATIVAR_ENVIO_INTEGRACAO_CANCELAMENTO_BOLETO_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioIntegracaoCancelamentoBoletoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicIntegracaoCancelamentoBoletoEMP", Column = "CIE_TOPIC_INTEGRACAO_CANCELAMENTO_BOLETO_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicIntegracaoCancelamentoBoletoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnviarTopicIntegracaoCancelamentoBoletoEMP", Column = "EMP_STATUS_TOPIC_ENVIO_INTEGRACAO_CANCELAMENTO_BOLETO_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEnviarTopicIntegracaoCancelamentoBoletoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarNoLayoutAvroDoPortalEMP", Column = "CIE_ENVIAR_NO_LAYOUT_AVRO_PORTAL_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNoLayoutAvroDoPortalEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCancelamentoBoletoEMP", Column = "CIE_ATIVAR_INTEGRACAO_CANCELAMENTO_DO_BOLETO_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCancelamentoBoletoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TopicEnvioIntegracaoCancelamentoBoletoEMP", Column = "CIE_TOPIC_INTEGRACAO_CANCELAMENTO_DO_BOLETO_EMP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TopicEnvioIntegracaoCancelamentoBoletoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTopicEnvioIntegracaoCancelamentoBoletoEMP", Column = "EMP_STATUS_TOPIC_ENVIO_DA_INTEGRACAO_CANCELAMENTO_BOLETO_EMP", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusTopicEnvioIntegracaoCancelamentoBoletoEMP { get; set; }
    }
}
