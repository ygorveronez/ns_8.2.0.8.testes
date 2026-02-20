namespace Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TABELA_FRETE_DESTINO", EntityName = "AlcadasTabelaFrete.AlcadaDestino", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaDestino", NameType = typeof(AlcadaDestino))]
    public class AlcadaDestino : RegraAutorizacao.Alcada<RegraAutorizacaoTabelaFrete, Localidade>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Localidade PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTabelaFrete", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTabelaFrete RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
