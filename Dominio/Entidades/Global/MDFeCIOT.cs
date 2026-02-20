namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_CIOT", EntityName = "MDFeCIOT", Name = "Dominio.Entidades.MDFeCIOT", NameType = typeof(MDFeCIOT))]
    public class MDFeCIOT : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCIOT", Column = "MCI_NUMERO_CIOT", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string NumeroCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "MCI_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string Responsavel { get; set; }
    }
}
