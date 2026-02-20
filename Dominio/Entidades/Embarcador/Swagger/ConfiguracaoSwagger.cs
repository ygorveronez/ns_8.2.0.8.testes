namespace Dominio.Entidades.Embarcador.Swagger
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_SWAGGER", EntityName = "ConfiguracaoSwagger", Name = "Dominio.Entidades.Configuracao.ConfiguracaoSwagger", NameType = typeof(ConfiguracaoSwagger))]
    public class ConfiguracaoSwagger : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SWA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "SWA_USUARIO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "SWA_SENHA", TypeType = typeof(string), Length = 256, NotNull = true)]
        public virtual string Senha { get; set; }
    }
}
