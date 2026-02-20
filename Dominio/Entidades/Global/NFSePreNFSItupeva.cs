namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_PRE_NFS_ITUPEVA", EntityName = "NFSePreNFSItupeva", Name = "Dominio.Entidades.NFSePreNFSItupeva", NameType = typeof(NFSePreNFSItupeva))]

    public class NFSePreNFSItupeva : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NPI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RPSNFSe", Column = "RPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RPSNFSe RPSNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJ", Column = "NPI_CNPJ", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBloco", Column = "NPI_NUMERO_BLOCO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroBloco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencia", Column = "NPI_NUMERO_SEQUENCIA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroSequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroValidacao", Column = "NPI_NUMERO_VALIDACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroValidacao { get; set; }
    }
}
