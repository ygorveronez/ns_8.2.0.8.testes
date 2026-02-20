namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ITALAC", EntityName = "IntegracaoItalac", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalac", NameType = typeof(IntegracaoItalac))]
    public class IntegracaoItalac : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CII_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoItalac", Column = "CII_POSSUI_INTEGRACAO_ITALAC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoItalac { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLItalac", Column = "CII_URL_ITALAC", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLItalac { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioItalac", Column = "CII_USUARIO_ITALAC", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioItalac { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaItalac", Column = "CII_SENHA_ITALAC", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaItalac { get; set; }
    }
}