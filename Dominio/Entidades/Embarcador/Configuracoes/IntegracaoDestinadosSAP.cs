namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_DESTINADOS_SAP", EntityName = "IntegracaoDestinadosSAP", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP", NameType = typeof(IntegracaoDestinadosSAP))]
    public class IntegracaoDestinadosSAP: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CID_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_URL_INTEGRACAO_XML", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_URL_INTEGRACAO_STATUS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_CLIENTID_INTEGRACAO_RETORNO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientIDIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_CLIENTSECRET_INTEGRACAO_RETORNO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientSecretIntegracao { get; set; }
    }
}