namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CATEGORIA_PESSOA", EntityName = "CategoriaPessoa", Name = "Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa", NameType = typeof(CategoriaPessoa))]
    public class CategoriaPessoa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CTP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CTP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CTP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "CTP_COR", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string Cor { get; set; }
    }
}
