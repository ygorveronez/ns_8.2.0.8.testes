namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_CHECKLIST_QUESTIONARIO_ANEXO_PADRAO", EntityName = "BiddingChecklistQuestionarioAnexoPadrao", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioAnexoPadrao", NameType = typeof(BiddingChecklistQuestionarioAnexoPadrao))]
    public class BiddingChecklistQuestionarioAnexoPadrao : Anexo.Anexo<BiddingChecklistQuestionarioPadrao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingChecklistQuestionarioPadrao", Column = "TCP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override BiddingChecklistQuestionarioPadrao EntidadeAnexo { get; set; }

        #endregion
    }
}
