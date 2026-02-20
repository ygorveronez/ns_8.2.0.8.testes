namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ESPECIE_DOCUMENTO_FISCAL", EntityName = "EspecieDocumentoFiscal", Name = "Dominio.Entidades.EspecieDocumentoFiscal", NameType = typeof(EspecieDocumentoFiscal))]
    public class EspecieDocumentoFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EDF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sigla", Column = "EDF_SIGLA", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string Sigla { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "EDF_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}
