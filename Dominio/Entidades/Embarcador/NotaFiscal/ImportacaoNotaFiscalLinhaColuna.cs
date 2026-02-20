namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_NOTA_FISCAL_LINHA_COLUNA", EntityName = "ImportacaoNotaFiscalLinhaColuna", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna", NameType = typeof(ImportacaoNotaFiscalLinhaColuna))]
    public class ImportacaoNotaFiscalLinhaColuna : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoNotaFiscalLinha", Column = "IML_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ImportacaoNotaFiscalLinha Linha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "IMC_NOME_CAMPO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string NomeCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IMC_VALOR", Type = "StringClob", NotNull = true)]
        public virtual string Valor { get; set; }
    }
}
