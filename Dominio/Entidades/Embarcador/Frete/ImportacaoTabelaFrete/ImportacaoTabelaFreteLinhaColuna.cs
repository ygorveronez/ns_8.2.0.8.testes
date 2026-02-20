using System;

namespace Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete
{
    [ObsoleteAttribute("Essa classe nao é mais usada.", false)]
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_TABELA_FRETE_LINHA_COLUNA", EntityName = "ImportacaoTabelaFreteLinhaColuna", Name = "Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna", NameType = typeof(ImportacaoTabelaFreteLinhaColuna))]
    public class ImportacaoTabelaFreteLinhaColuna : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ITC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoTabelaFreteLinha", Column = "ITL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha Linha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITC_NOME_CAMPO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string NomeCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITC_VALOR", Type = "StringClob", NotNull = true)]
        public virtual string Valor { get; set; }
    }
}
