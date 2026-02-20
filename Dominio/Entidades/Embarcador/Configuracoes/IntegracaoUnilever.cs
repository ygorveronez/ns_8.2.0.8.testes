namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_UNILEVER", EntityName = "IntegracaoUnilever", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever", NameType = typeof(IntegracaoUnilever))]
    public class IntegracaoUnilever : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoUnilever", Column = "CIU_POSSUI_INTEGRACAO_UNILEVER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoUnilever { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarAvancoParaEmissao", Column = "CIU_INTEGRAR_AVANCO_PARA_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarAvancoParaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarValorPreCalculo", Column = "CIU_INTEGRAR_VALOR_PRE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarValorPreCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarDadosValePedagio", Column = "CIU_INTEGRAR_DADOS_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarDadosValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarLeilaoManual", Column = "CIU_INTEGRAR_LEILAO_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarLeilaoManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarLotePagamento", Column = "CIU_INTEGRAR_LOTE_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarLotePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_CANHOTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_RETORNO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoRetornoUnilever { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_CLIENTID_INTEGRACAO_RETORNO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientIDIntegracaoUnilever { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_CLIENTSECRETE_INTEGRACAO_RETORNO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientSecretIntegracaoUnilever { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_AVANCO_PARA_EMISSAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoAvancoParaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_TRAVAMENTO_DT", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoTravamentoDTUnilever { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_PROVISAO_UNILEVER", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoProvisaoUnilever { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_VALOR_PRE_CALCULO_UNILEVER", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoValorPreCalculoUnilever { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_LEILAO_MANUAL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoLeilaoManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_ESCRITURACAO_RETORNO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoEscrituracaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_LOTE_PAGAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoLotePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_CANCELAMENTO_PROVISAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCancelamentoProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarCanhoto", Column = "CIU_INTEGRAR_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarCancelamentoPagamento", Column = "CIU_INTEGRAR_CANCELAMENTO_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarCancelamentoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_CANCELAMENTO_PAGAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCancelamentoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_OCORRENCIAS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoOcorrencia { get; set; }

    }
}