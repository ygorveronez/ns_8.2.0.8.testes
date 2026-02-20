namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NATUREZA_CARGA_ANTT", EntityName = "NaturezaCargaANTT", Name = "Dominio.Entidades.NaturezaCargaANTT", NameType = typeof(NaturezaCargaANTT))]
    public class NaturezaCargaANTT : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNatureza", Column = "NCA_CODIGO_NATUREZA", TypeType = typeof(string), Length = 4, NotNull = true)]
        public virtual string CodigoNatureza { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "NCA_DESCRICAO", TypeType = typeof(string), Length = 2000, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}
