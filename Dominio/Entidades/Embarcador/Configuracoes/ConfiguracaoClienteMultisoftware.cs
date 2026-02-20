namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CLIENTE_MULTISOFTWARE", EntityName = "ConfiguracaoClienteMultisoftware", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware", NameType = typeof(ConfiguracaoClienteMultisoftware))]
    public class ConfiguracaoClienteMultisoftware : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RazaoSocial", Column = "CCM_RAZAO_SOCIAL", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string RazaoSocial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLPrincipal", Column = "CCM_URL_PRINCIPAL", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLTransportador", Column = "CCM_URL_TRANSPORTADOR", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLFornecedor", Column = "CCM_URL_FORNECEDOR", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLFornecedor { get; set; }
    }
}