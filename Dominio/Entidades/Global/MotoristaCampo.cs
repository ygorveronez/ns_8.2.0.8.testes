namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTORISTA_CAMPO", EntityName = "MotoristaCampo", Name = "Dominio.Entidades.MotoristaCampo", NameType = typeof(MotoristaCampo))]
    public class MotoristaCampo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_CAMPO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Campo { get; set; }
    }
}
