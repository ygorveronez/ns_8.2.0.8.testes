namespace Dominio.Entidades.Embarcador.Bidding.RFI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RFI_CONVITE_ANEXO", EntityName = "RFIConviteAnexo", Name = "Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteAnexo", NameType = typeof(RFIConviteAnexo))]
    public class RFIConviteAnexo : Anexo.Anexo<RFIConvite>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RFIConvite", Column = "TRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RFIConvite EntidadeAnexo { get; set; }

        #endregion
    }
}
