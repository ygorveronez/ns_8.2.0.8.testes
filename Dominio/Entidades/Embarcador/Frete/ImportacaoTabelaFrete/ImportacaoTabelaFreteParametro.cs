namespace Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_TABELA_FRETE_PARAMETRO", EntityName = "ImportacaoTabelaFreteParametro", Name = "Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro", NameType = typeof(ImportacaoTabelaFreteParametro))]
    public class ImportacaoTabelaFreteParametro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ITP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoTabelaFrete", Column = "ITF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete ImportacaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITP_ITEM_PARAMETRO_BASE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ItemParametroBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITP_PARAMETRO_BASE", TypeType = typeof(int), NotNull = false)]
        public virtual int ParametroBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITP_TIPO_VALOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITP_COLUNA", TypeType = typeof(int), NotNull = false)]
        public virtual int Coluna { get; set; }
    }
}
