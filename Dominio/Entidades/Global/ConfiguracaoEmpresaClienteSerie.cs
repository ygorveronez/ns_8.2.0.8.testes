namespace Dominio.Entidades
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_CONFIG_CLIENTE_SERIE", EntityName = "ConfiguracaoEmpresaClienteSerie", Name = "Dominio.Entidades.ConfiguracaoEmpresaClienteSerie", NameType = typeof(ConfiguracaoEmpresaClienteSerie))]
    public class ConfiguracaoEmpresaClienteSerie : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoEmpresa ConfiguracaoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "ESE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCliente", Column = "EES_TIPO_CLIENTE", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoTomador TipoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RaizCNPJ", Column = "EES_RAIZ_CNPJ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RaizCNPJ { get; set; }
    }
}
