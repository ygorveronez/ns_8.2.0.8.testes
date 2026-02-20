namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_MARFRIG", EntityName = "IntegracaoMarfrig", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig", NameType = typeof(IntegracaoMarfrig))]
    public class IntegracaoMarfrig : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_API_KEY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ApiKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRACAO_CADASTRA_CARGA_ORDEM_EMBARQUE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCadastraCargaOrdemEmbarque { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRACAO_ALTERA_CARGA_ORDEM_EMBARQUE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoAlteraCargaOrdemEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRACAO_REMOVER_PEDIDO_ORDEM_EMBARQUE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoRemoverPedidoOrdemEmbarque { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRACAO_ADICIONAR_PEDIDO_ORDEM_EMBARQUE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoAdicionarPedidoOrdemEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRACAO_FINALIZA_CARGA_ORDEM_EMBARQUE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoFinalizaCargaOrdemEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRACAO_FINALIZA_CARGA_ORDEM_EMBARQUE_EXPORTACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCargaOrdemEmbarqueExportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_PRE_CALCULO_FRETE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLPreCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_CONSULTA_TITULO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConsultaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_CONSULTA_ACRESCIMO_DECRESCIMO_TITULO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConsultaAcrescimoDecrescimoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_CONSULTA_ADIANTAMENTO_DEVOLUCAO_TITULO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConsultaAdiantamentoDevolucaoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRA_RETORNO_OCORRENCIA_NOTA_DEBITO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegraRetornoOcorrenciaNotaDebito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRA_RETORNO_OCORRENCIA_NOTA_DEBITO_PARCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegraRetornoOcorrenciaNotaDebitoParcelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRA_RETORNO_OCORRENCIA_CTE_COMPLEMENTAR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegraRetornoOcorrenciaCteComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_REQUISICAO_IMPRESSAO_DOCUMENTOS_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLRequisicaoImpressaoDocumentosCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_CANCELAMENTO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLCancelamentoCarga { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_REDESPACHO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLRedespachoCarga { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_TRANSBORDO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLTransbordoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRACAO_EMISSAO_DOCUMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoEmissaoDocumentos{ get; set; }
    }
}