
namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONVERSAO_DE_UNIDADE", DynamicUpdate = true, EntityName = "ConversaoDeUnidade", Name = "Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade", NameType = typeof(ConversaoDeUnidade))]
    public class ConversaoDeUnidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeDeMedida", Column = "UNI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeDeMedida UnidadeDeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sigla", Column = "CDU_SIGLA_DE", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Sigla { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CDU_DESCRICAO_DE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Descricao { get; set; }
    }
}
