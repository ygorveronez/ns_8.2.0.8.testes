using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_VEDACIT", EntityName = "IntegracaoVedacit", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVedacit", NameType = typeof(IntegracaoVedacit))]
    public class IntegracaoVedacit : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIV_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "CIV_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [Obsolete("Não será usado no momento", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIV_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoCarga", Column = "CIV_URL_INTEGRACAO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIV_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIV_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [Obsolete("Não será usado no momento", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIV_CLIENT_ID", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ClientID { get; set; }

        [Obsolete("Não será usado no momento", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIV_CLIENT_SECRET", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [Obsolete("Não será usado no momento", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "TenantID", Column = "CIV_TENANT_ID", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TenantID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioIntegracaoCarga", Column = "CIV_USUARIO_INTEGRACAO_CARGA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaIntegracaoCarga", Column = "CIV_SENHA_INTEGRACAO_CARGA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaIntegracaoCarga { get; set; }
    }
}
