namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BANCO", EntityName = "Banco", Name = "Dominio.Entidades.Banco", NameType = typeof(Banco))]
    public class Banco : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "BCO_NUMERO", TypeType = typeof(int), NotNull = true, Unique = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "BCO_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "BCO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }
    }
}
