namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_SEGURO", EntityName = "MDFeSeguro", Name = "Dominio.Entidades.MDFeSeguro", NameType = typeof(MDFeSeguro))]
    public class MDFeSeguro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MSE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoResponsavel", Column = "MSE_TIPO_RESPONSAVEL", TypeType = typeof(Dominio.Enumeradores.TipoResponsavelSeguroMDFe), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoResponsavelSeguroMDFe TipoResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "MSE_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJSeguradora", Column = "MSE_CNPJ_SEGURADORA", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeSeguradora", Column = "MSE_NOME_SEGURADORA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NomeSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroApolice", Column = "MSE_NUMERO_APOLICE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAverbacao", Column = "MSE_NUMERO_AVERBACAO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string NumeroAverbacao { get; set; }
    }
}
