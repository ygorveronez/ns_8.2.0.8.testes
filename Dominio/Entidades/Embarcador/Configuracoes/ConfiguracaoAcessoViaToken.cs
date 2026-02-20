namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ACESSO_VIA_TOKEN", EntityName = "ConfiguracaoAcessoViaToken", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken", NameType = typeof(ConfiguracaoAcessoViaToken))]
    public class ConfiguracaoAcessoViaToken : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Emissor", Column = "CVT_EMISSOR", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Emissor { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Audiencia", Column = "CVT_AUDIENCIA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Audiencia { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveSecreta", Column = "CVT_CHAVE_SECRETA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string ChaveSecreta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVT_GERAR_URL_ACESSO_PORTAL_MULTICLIFOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarUrlAcessoPortalMultiClifor { get; set; }
    }
}