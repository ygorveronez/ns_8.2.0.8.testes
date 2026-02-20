namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_SSO_INTERNO", EntityName = "ConfiguracaoSSOInterno", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno", NameType = typeof(ConfiguracaoSSOInterno))]
    public class ConfiguracaoSSOInterno : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Display", Column = "CSI_DISPLAY", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Display { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientId", Column = "CSI_CLIENT_ID", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string ClientId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlAutenticacao", Column = "CSI_URL_AUTENTICACAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string UrlAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CSI_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlDominio", Column = "CSI_URL_DOMINIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UrlDominio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivoCertificado", Column = "CSI_CAMINHO_ARQUIVO_CERTIFICADO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CaminhoArquivoCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificadorGrupoAdministrador", Column = "CSI_IDENTIFICADOR_GRUPO_ADMINISTRADOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string IdentificadorGrupoAdministrador { get; set; }
    }
}
