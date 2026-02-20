namespace Dominio.Entidades.Embarcador.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGIAO_BRASIL", EntityName = "RegiaoBrasil", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Localidades.RegiaoBrasil", NameType = typeof(RegiaoBrasil))]
    public class RegiaoBrasil : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RBR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RBR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

    }
}
