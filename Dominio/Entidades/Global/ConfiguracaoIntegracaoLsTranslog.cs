using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_LSTRANSLOG", EntityName = "ConfiguracaoIntegracaoLsTranslog", Name = "Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog", NameType = typeof(ConfiguracaoIntegracaoLsTranslog))]
    public class ConfiguracaoIntegracaoLsTranslog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Login", Column = "CLS_LOGIN", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Login { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CLS_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_INTEGRACAO_LSTRANSLOG_CLIENTES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoIntegracaoLsTranslogClientes", Column = "CIC_CODIGO")]
        public virtual IList<Dominio.Entidades.ConfiguracaoIntegracaoLsTranslogClientes> Clientes { get; set; }

    }
}
