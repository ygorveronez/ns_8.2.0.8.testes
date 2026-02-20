namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROPOSTA_ITEM", EntityName = "PropostaItem", Name = "Dominio.Entidades.PropostaItem", NameType = typeof(PropostaItem))]
    public class PropostaItem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Proposta", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Proposta Proposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PRI_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PRI_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PRI_TIPO", TypeType = typeof(Enumeradores.TipoItemProposta), NotNull = false)]
        public virtual Enumeradores.TipoItemProposta Tipo { get; set; }
    }
}