namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_DEXCO", EntityName = "IntegracaoDexco", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDexco", NameType = typeof(IntegracaoDexco))]
    public class IntegracaoDexco : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AccessKeyDexco", Column = "CID_ACCESS_KEY", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string AccessKeyDexco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FoType", Column = "CID_FOTYPE", TypeType = typeof(string), Length = 4, NotNull = true)]
        public virtual string FoType { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlDexco", Column = "CID_URL", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string UrlDexco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CID_USUARIO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CID_SENHA", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Senha { get; set; }
    }
}
