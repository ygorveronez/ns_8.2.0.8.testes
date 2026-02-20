namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_BIDDING_ANEXO", EntityName = "TipoBiddingAnexo", Name = "Dominio.Entidades.Embarcador.Bidding.TipoBiddingAnexo", NameType = typeof(TipoBiddingAnexo))]
    public class TipoBiddingAnexo : Anexo.Anexo<TipoBidding>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoBidding", Column = "TBI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TipoBidding EntidadeAnexo { get; set; }

        #endregion
    }
}
