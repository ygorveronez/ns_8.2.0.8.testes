namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIG_EMAIL", EntityName = "ConfiguracaoEmail", Name = "Dominio.Entidades.ConfiguracaoEmail", NameType = typeof(ConfiguracaoEmail))]
    public class ConfiguracaoEmail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "COE_EMAIL", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "COE_SENHA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Smtp", Column = "COE_SMTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Smtp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PortaSmtp", Column = "COE_PORTA_SMTP", TypeType = typeof(int), NotNull = false)]
        public virtual int PortaSmtp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RequerAutenticacaoSmtp", Column = "COE_REQUER_AUTENTICACAO_SMTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RequerAutenticacaoSmtp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "User", Column = "COE_USER", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string User { get; set; }


    }
}
