namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_SERVICO", EntityName = "ServicoNFSe", Name = "Dominio.Entidades.ServicoNFSe", NameType = typeof(ServicoNFSe))]
    public class ServicoNFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "SER_NUMERO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SER_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NBS", Column = "SER_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aliquota", Column = "SER_ALIQUOTA", TypeType = typeof(decimal), Scale = 4, Precision = 6, NotNull = false)]
        public virtual decimal Aliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNAE", Column = "SER_CNAE", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string CNAE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTributacao", Column = "SER_CODIGO_TRIBUTACAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string CodigoTributacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "SER_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ISSRetido", Column = "SER_ISS_RETIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ISSIncluso", Column = "SER_ISS_INCLUSO", TypeType = typeof(Enumeradores.InclusaoISSNFSe), NotNull = false)]
        public virtual Enumeradores.InclusaoISSNFSe ISSIncluso { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    case "I":
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

    }
}
