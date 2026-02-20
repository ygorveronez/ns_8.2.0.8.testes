namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_CONTRATANTE", EntityName = "MDFeContratante", Name = "Dominio.Entidades.MDFeContratante", NameType = typeof(MDFeContratante))]
    public class MDFeContratante : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Contratante", Column = "MCO_CONTRATANTE", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string Contratante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeContratante", Column = "MCO_NOME_CONTRATANTE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string NomeContratante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDEstrangeiro", Column = "MCO_ID_ESTRANGEIRO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IDEstrangeiro { get; set; }
    }
}
