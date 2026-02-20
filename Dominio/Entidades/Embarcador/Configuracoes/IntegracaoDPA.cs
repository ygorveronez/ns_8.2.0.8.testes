namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_DPA", EntityName = "IntegracaoDPA", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA", NameType = typeof(IntegracaoDPA))]
    public class IntegracaoDPA : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_URL_INTEGRACAO_DPA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoDPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_URL_AUTENTICACAO_DPA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLAutenticacaoDPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_USUARIO_AUTENTICACAO_DPA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string UsuarioAutenticacaoDPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_SENHA_AUTENTICACAO_DPA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string SenhaAutenticacaoDPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_MATERIAL_DPA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Material { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_MATERIAL_GROUP_DPA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MaterialGroup { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_DESCRIPTION_DPA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Description { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_URL_INTEGRACAO_DPA_CIOT", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoDPACiot { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_URL_AUTENTICACAO_DPA_CIOT", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLAutenticacaoDPACiot { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_USUARIO_AUTENTICACAO_DPA_CIOT", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string UsuarioAutenticacaoDPACiot { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_SENHA_AUTENTICACAO_DPA_CIOT", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string SenhaAutenticacaoDPACiot { get; set; }
    }
}
