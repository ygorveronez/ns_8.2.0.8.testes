namespace Dominio.Entidades.Embarcador.Tanques
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TANQUE", EntityName = "Tanque", Name = "Dominio.Entidades.Embarcador.Tanques.Tanque", NameType = typeof(Tanque))]
    public class Tanque : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ID", Column = "TAN_ID", NotNull = true, TypeType = typeof(string), Length = 100)]
        public virtual string ID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TAN_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

    }
}