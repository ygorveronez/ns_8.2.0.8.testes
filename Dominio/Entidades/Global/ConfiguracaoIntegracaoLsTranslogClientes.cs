namespace Dominio.Entidades
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_LSTRANSLOG_CLIENTES", EntityName = "ConfiguracaoIntegracaoLsTranslogClientes", Name = "Dominio.Entidades.ConfiguracaoIntegracaoLsTranslogClientes", NameType = typeof(ConfiguracaoIntegracaoLsTranslogClientes))]
    public class ConfiguracaoIntegracaoLsTranslogClientes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoIntegracaoLsTranslog", Column = "CLS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoIntegracaoLsTranslog ConfiguracaoIntegracaoLsTranslog { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CIC_CLIENTE", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarRetornoAoSalvarDocumento", Column = "CIC_ENVIAR_RETORNO_AO_SALVAR_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarRetornoAoSalvarDocumento { get; set; }
    }
}
