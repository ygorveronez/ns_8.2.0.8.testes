namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_SSO", EntityName = "ConfiguracaoSSO", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO", NameType = typeof(ConfiguracaoSSO))]
    public class ConfiguracaoSSO : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSSo", Column = "COS_TIPO_SSO", TypeType = typeof(int), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoSso TipoSSo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Display", Column = "COS_DISPLAY", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Display { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientId", Column = "COS_CLIENT_ID", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string ClientId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "COS_CLIENT_SECRET", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlAutenticacao", Column = "COS_URL_AUTENTICACAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string UrlAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlAccessToken", Column = "COS_URL_ACCESS_TOKEN", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string UrlAccessToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlRefreshToken", Column = "COS_URL_REFRESH_TOKEN", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string UrlRefreshToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlRevokeToken", Column = "COS_URL_REVOKE_TOKEN", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string UrlRevokeToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "COS_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlDominio", Column = "COS_URL_DOMINIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UrlDominio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivoCertificado", Column = "COS_CAMINHO_ARQUIVO_CERTIFICADO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CaminhoArquivoCertificado { get; set; }
    }
}
