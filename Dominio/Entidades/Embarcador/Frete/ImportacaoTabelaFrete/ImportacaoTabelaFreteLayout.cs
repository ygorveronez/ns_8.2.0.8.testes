namespace Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPORTACAO_TABELA_FRETE_LAYOUT", EntityName = "ImportacaoTabelaFreteLayout", Name = "Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout", NameType = typeof(ImportacaoTabelaFreteLayout))]
    public class ImportacaoTabelaFreteLayout : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ITF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JSONLayout", Column = "ITF_JSON_LAYOUT", Type = "StringClob", NotNull = true)]
        public virtual string JSONLayout { get; set; }
    }
}
