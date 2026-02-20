namespace Dominio.Entidades.Embarcador.Bidding.AlcadasBidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_BIDDING_CONVITE", EntityName = "AprovacaoAlcadaBiddingConvite", Name = "Dominio.Entidades.Embarcador.Bidding.AlcadasBiddingConvite.AprovacaoAlcadaBiddingConvite", NameType = typeof(AprovacaoAlcadaBiddingConvite))]
    public class AprovacaoAlcadaBiddingConvite : RegraAutorizacao.AprovacaoAlcada<BiddingConvite, RegraAutorizacaoBidding>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingConvite", Column = "TBC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override BiddingConvite OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoBidding", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoBidding RegraAutorizacao { get; set; }

        #endregion Propriedades Sobrescritas
    }
}