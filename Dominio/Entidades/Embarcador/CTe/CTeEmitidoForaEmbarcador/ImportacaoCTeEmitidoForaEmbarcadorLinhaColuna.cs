namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_CTE_EMITIDO_FORA_EMBARCADOR_LINHA_COLUNA", EntityName = "ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna", Name = "Dominio.Entidades.Embarcador.CTe.CTeEmitidoForaEmbarcadorLinhaColuna", NameType = typeof(ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna))]
    public class ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ILC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoCTeEmitidoForaEmbarcadorLinha", Column = "ICL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha Linha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ILC_NOME_CAMPO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string NomeCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ILC_VALOR", Type = "StringClob", NotNull = true)]
        public virtual string Valor { get; set; }
    }
}
