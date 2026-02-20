namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_WEBERCHILE", EntityName = "IntegracaoWeberChile", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWeberChile", NameType = typeof(IntegracaoWeberChile))]
    public class IntegracaoWeberChile : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIW_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIW_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "CIW_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIW_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIW_CLIENT_ID", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIW_CLIENT_SECRET", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApiKey", Column = "CIW_API_KEY", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ApiKey { get; set; }
    }
}
