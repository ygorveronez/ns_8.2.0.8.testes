namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_PORTAL_CABOTAGEM", EntityName = "IntegracaoPortalCabotagem", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPortalCabotagem", NameType = typeof(IntegracaoPortalCabotagem))]
    public class IntegracaoPortalCabotagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoPortalAzureStorage", Column = "CIC_ATIVAR_INTEGRACAO_PORTAL_AZURE_STORAGE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoPortalAzureStorage { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Container", Column = "CIC_CONTAINER", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Container { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StorageAccount", Column = "CIC_STORAGE_ACCOUNT", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string StorageAccount { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIC_URL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClienteID", Column = "CIC_CLIENTE_ID", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClienteID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Secret", Column = "CIC_SECRET", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Secret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioPDFFatura", Column = "CIC_ATIVAR_ENVIO_PDF_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioPDFFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioPDFCTE", Column = "CIC_ATIVAR_ENVIO_PDF_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioPDFCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioPDFBoleto", Column = "CIC_ATIVAR_ENVIO_PDF_BOLETO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioPDFBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioXMLCTE", Column = "CIC_ATIVAR_ENVIO_XML_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioXMLCTE { get; set; }
    }
}
