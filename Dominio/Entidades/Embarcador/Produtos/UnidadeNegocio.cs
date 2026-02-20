namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_UNIDADE_NEGOCIO", DynamicUpdate = true, EntityName = "UnidadeNegocio", Name = "Dominio.Entidades.Embarcador.Produtos.UnidadeNegocio", NameType = typeof(UnidadeNegocio))]
    public class UnidadeNegocio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "UNE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "UNE_DESCRICAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "UNE_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

    }
}
