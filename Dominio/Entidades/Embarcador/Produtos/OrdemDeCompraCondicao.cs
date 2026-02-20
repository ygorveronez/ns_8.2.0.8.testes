namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_DE_COMPRA_CONDICAO", DynamicUpdate = true, EntityName = "OrdemDeCompraCondicao", Name = "Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao", NameType = typeof(OrdemDeCompraCondicao))]
    public class OrdemDeCompraCondicao : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCondicao", Column = "ODC_NUMERO_CONDICAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroCondicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemCondicao", Column = "ODC_ITEM_CONDICAO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ItemCondicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCondicao", Column = "ODC_TIPO_CONDICAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string TipoCondicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeTipoCondicao", Column = "ODC_NOME_TIPO_CONDICAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NomeTipoCondicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBase", Column = "ODC_VALOR_BASE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Taxa", Column = "ODC_TAXA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Taxa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCondicao", Column = "ODC_VALOR_CONDICAO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorCondicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlteradoManualmente", Column = "ODC_ALTERADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlteradoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemDeCompraItem", Column = "OCI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemDeCompraItem OrdemDeCompraItem { get; set; }

    }
}
