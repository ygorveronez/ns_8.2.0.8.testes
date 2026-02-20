namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_CONFIG_CODIGOS_SERVICO_NFSE", EntityName = "CodigosServicoNFSe", Name = "Dominio.Entidades.CodigosServicoNFSe", NameType = typeof(CodigosServicoNFSe))]
    public class CodigosServicoNFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ECS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoEmpresa ConfiguracaoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTributacao", Column = "ECS_CODIGO_TRIBUTACAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string CodigoTributacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTributacaoPrefeitura", Column = "ECS_CODIGO_TRIBUTACAO_PREFEITURA", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string CodigoTributacaoPrefeitura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTributacaoPrefeitura", Column = "ECS_NUMERO_TRIBUTACAO_PREFEITURA", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string NumeroTributacaoPrefeitura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNAE", Column = "ECS_NUMERO_CNAE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNAE { get; set; }
    }
}
