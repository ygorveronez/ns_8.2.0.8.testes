namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_MARISA", EntityName = "IntegracaoMarisa", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarisa", NameType = typeof(IntegracaoMarisa))]
    public class IntegracaoMarisa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoMarisa", Column = "CIM_POSSUI_INTEGRACAO_MARISA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoMarisa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIM_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIM_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Url", Column = "CIM_URL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Url { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoIntegracaoTabelaMarisa", Column = "CIM_ENDERECO_INTEGRACAO_TABELA_MARISA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string EnderecoIntegracaoTabelaMarisa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioIntegracaoTabelaMarisa", Column = "CIM_USUARIO_INTEGRACAO_TABELA_MARISA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioIntegracaoTabelaMarisa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaIntegracaoTabelaMarisa", Column = "CIM_SENHA_INTEGRACAO_TABELA_MARISA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaIntegracaoTabelaMarisa { get; set; }
    }
}
