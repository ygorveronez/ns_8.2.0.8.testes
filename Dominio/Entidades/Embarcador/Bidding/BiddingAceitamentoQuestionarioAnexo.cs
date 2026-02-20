namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_ACEITAMENTO_QUESTIONARIO_ANEXO", EntityName = "BiddingAceitamentoQuestionarioAnexo", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo", NameType = typeof(BiddingAceitamentoQuestionarioAnexo))]
    public class BiddingAceitamentoQuestionarioAnexo : Anexo.Anexo<BiddingAceitamentoQuestaoResposta>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingAceitamentoQuestaoResposta", Column = "TCQ_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override BiddingAceitamentoQuestaoResposta EntidadeAnexo { get; set; }

        #endregion
    }
}
