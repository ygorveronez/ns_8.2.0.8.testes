namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_GHOST", EntityName = "ConfiguracaoIntegracaoGhost", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoGhost", NameType = typeof(ConfiguracaoIntegracaoGhost))]
    public class ConfiguracaoIntegracaoGhost : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_URL_MULE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLMule { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_CLIENT_ID_MULE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ClientIDMule { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_CLIENT_SECRET_MULE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ClientSecretMule { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_URL_FILAH", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLFilaH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_LOGIN_FILAH", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string LoginFilaH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_PASSWORD_FILAH", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string PasswordFilaH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_URL_AUTH_FILAH", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAuthFilaH { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Trizy";
            }
        }

    }
}
