namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORGANIZACAO", EntityName = "Organizacao", Name = "Dominio.Entidades.Embarcador.Embarcador.Organizacao", NameType = typeof(Organizacao))]
    public class Organizacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ORG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "ORG_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ORG_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Canal", Column = "ORG_CANAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Canal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Setor", Column = "ORG_SETOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Setor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nivel", Column = "ORG_NIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Nivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoHierarquia", Column = "ORG_CODIGO_HIERARQUIA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoHierarquia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoHierarquia", Column = "ORG_DESCRICAO_HIERARQUIA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoHierarquia { get; set; }

    }
}
