namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_REDMINE", EntityName = "ConfiguracaoRedMine", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoRedMine", NameType = typeof(ConfiguracaoRedMine))]
    public class ConfiguracaoRedMine : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRM_CLIENTE_REDMINE", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string ClienteRedMine { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRM_CODIGO_USUARIO_DESTINO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoUsuarioDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRM_TOKEN_USER_CADASTRO_REDMINE", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string TokenUsuarioCadastro { get; set; } // 91be8e9a5df189bd4506fcd2359d2d567fcd834e

        public virtual string Descricao
        {
            get { return "Configuração REDMINE"; }
        }
    }
}
