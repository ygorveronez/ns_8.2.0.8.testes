namespace Dominio.Entidades.Embarcador.Bidding.AlcadasBidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_BIDDING_TIPO_BIDDING", EntityName = "AlcadasBidding.AlcadaTipoBidding", Name = "Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AlcadaTipoBidding", NameType = typeof(AlcadaTipoBidding))]
    public class AlcadaTipoBidding : RegraAutorizacao.Alcada<RegraAutorizacaoBidding, Bidding.TipoBidding>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoBidding", Column = "TBI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Bidding.TipoBidding PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoBidding", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoBidding RegrasAutorizacao { get; set; }

        #endregion Propriedades Sobrescritas

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion Métodos Públicos Sobrescritos
    }
}
