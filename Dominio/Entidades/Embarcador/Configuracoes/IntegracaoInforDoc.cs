namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_INFORDOC", EntityName = "IntegracaoInforDoc", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoInforDoc", NameType = typeof(IntegracaoInforDoc))]
    public class IntegracaoInforDoc : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CID_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "APIKey", Column = "CID_API_KEY", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string APIKey { get; set; }
    }
}
