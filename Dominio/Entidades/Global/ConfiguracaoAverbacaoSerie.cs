namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIG_AVERBACAO_SERIE", EntityName = "ConfiguracaoAverbacaoSerie", Name = "Dominio.Entidades.ConfiguracaoAverbacaoSerie", NameType = typeof(ConfiguracaoAverbacaoSerie))]
    public class ConfiguracaoAverbacaoSerie : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "save-update")]
        public virtual ConfiguracaoEmpresa Configuracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "ESE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie EmpresaSerie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoAverbar", Column = "CAS_NAO_AVERBAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAverbar { get; set; }

    }
}
