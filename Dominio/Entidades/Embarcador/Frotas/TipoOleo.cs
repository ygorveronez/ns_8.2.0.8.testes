namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ABASTECIMENTO_TIPO_OLEO", EntityName = "TipoOleo", Name = "Dominio.Entidades.Embarcador.Frotas.TipoOleo", NameType = typeof(TipoOleo))]
    public class TipoOleo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "TOL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOL_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOL_TIPO_DE_OLEO", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string TipoDeOleo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOL_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }
    }
}