namespace Dominio.Entidades.Embarcador.Bidding.RFI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RFI_CHECKLIST_QUESTIONARIO_ANEXO", EntityName = "RFIChecklistQuestionarioAnexo", Name = "Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo", NameType = typeof(RFIChecklistQuestionarioAnexo))]
    public class RFIChecklistQuestionarioAnexo : Anexo.Anexo<RFIChecklistQuestionario>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RFIChecklistQuestionario", Column = "RCQ_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RFIChecklistQuestionario EntidadeAnexo { get; set; }

        #endregion
    }
}
