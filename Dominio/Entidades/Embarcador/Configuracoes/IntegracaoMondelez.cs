namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_MONDELEZ", EntityName = "IntegracaoMondelez", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMondelez", NameType = typeof(IntegracaoMondelez))]
    public class IntegracaoMondelez : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIM_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLDrivin", Column = "CIM_URL_DRIVIN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLDrivin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApiKeyDrivin", Column = "CIM_API_KEY_DRIVIN", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string ApiKeyDrivin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApiKeyDrivinLegado", Column = "CIM_API_KEY_DRIVIN_LEGADO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string ApiKeyDrivinLegado { get; set; }
    }
}