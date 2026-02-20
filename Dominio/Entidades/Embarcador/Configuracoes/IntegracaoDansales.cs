namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_DANSALES", EntityName = "IntegracaoDansales", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales", NameType = typeof(IntegracaoDansales))]
    public class IntegracaoDansales : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDL_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "IDL_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "IDL_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "IDL_URL_INTEGRACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLToken", Column = "IDL_URL_TOKEN", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URLToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoChat", Column = "IDL_URL_INTEGRACAO_CHAT", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URLIntegracaoChat { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioToken", Column = "IDL_USUARIO_TOKEN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaToken", Column = "IDL_SENHA_TOKEN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaToken { get; set; }

    }
}
