using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_FRIMESA", EntityName = "IntegracaoFrimesa", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa", NameType = typeof(IntegracaoFrimesa))]
    public class IntegracaoFrimesa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLContabilizacao", Column = "CIH_URL_CONTABILIZACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIH_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIH_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoOAuth", Column = "CIH_TIPO_INTEGRACAO_OAUTH", TypeType = typeof(TipoIntegracaoOAuth), NotNull = false)]
        public virtual TipoIntegracaoOAuth TipoIntegracaoOAuth { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CIH_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIH_CLIENT_ID", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIH_CLIENT_SECRET", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AccessToken", Column = "CIH_ACESS_TOKEN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string AccessToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Scope", Column = "CIH_SCOPE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Scope { get; set; }
    }
}
