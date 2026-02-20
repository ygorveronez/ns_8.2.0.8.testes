namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SALESFORCE", EntityName = "IntegracaoSalesforce", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSalesforce", NameType = typeof(IntegracaoSalesforce))]
    public class IntegracaoSalesforce : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIS_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLBase", Column = "CIS_URL_BASE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URIToken", Column = "CIS_URI_TOKEN", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URIToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URICasoDevolucao", Column = "CIS_URI_CASO_DEVOLUCAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URICasoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIS_CLIENT_ID", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIS_CLIENT_SECRET", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ClientSecret { get; set; }
    }
}
