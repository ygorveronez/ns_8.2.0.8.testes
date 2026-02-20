namespace Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_PRECO_COMBUSTIVEL_LINHA_COLUNA", EntityName = "ImportacaoPrecoCombustivelLinhaColuna", Name = "Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna", NameType = typeof(ImportacaoPrecoCombustivelLinhaColuna))]
    public class ImportacaoPrecoCombustivelLinhaColuna : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ILC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoPrecoCombustivelLinha", Column = "IPL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha Linha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ILC_NOME_CAMPO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string NomeCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ILC_VALOR", Type = "StringClob", NotNull = true)]
        public virtual string Valor { get; set; }
    }
}
