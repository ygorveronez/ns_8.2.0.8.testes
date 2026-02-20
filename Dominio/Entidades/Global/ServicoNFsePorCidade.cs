namespace Dominio.Entidades
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_CONFIG_SERVICO_CIDADE", EntityName = "ServicoNFsePorCidade", Name = "Dominio.Entidades.ServicoNFsePorCidade", NameType = typeof(ServicoNFsePorCidade))]
    public class ServicoNFsePorCidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ESC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoEmpresa ConfiguracaoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaNFSe", Column = "NAN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaNFSe NaturezaNFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoNFSe", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ServicoNFSe ServicoNFSe { get; set; }
    }
}
