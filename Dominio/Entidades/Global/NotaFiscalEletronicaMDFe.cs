namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_NFE", EntityName = "NotaFiscalEletronicaMDFe", Name = "Dominio.Entidades.NotaFiscalEletronicaMDFe", NameType = typeof(NotaFiscalEletronicaMDFe))]
    public class NotaFiscalEletronicaMDFe: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MunicipioDescarregamentoMDFe", Column = "MDD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.MunicipioDescarregamentoMDFe MunicipioDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "MDN_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SegundoCodigoDeBarra", Column = "MDN_SEG_COD_BARRA", TypeType = typeof(string), Length = 36, NotNull = false)]
        public virtual string SegundoCodigoDeBarra { get; set; }
    }
}
