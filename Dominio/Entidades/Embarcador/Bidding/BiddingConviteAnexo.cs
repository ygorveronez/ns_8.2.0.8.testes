namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_CONVITE_ANEXO", EntityName = "BiddingConviteAnexo", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingConviteAnexo", NameType = typeof(BiddingConviteAnexo))]
    public class BiddingConviteAnexo : Anexo.Anexo<BiddingConvite>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingConvite", Column = "TBC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override BiddingConvite EntidadeAnexo { get; set; }

        #endregion
    }
}
