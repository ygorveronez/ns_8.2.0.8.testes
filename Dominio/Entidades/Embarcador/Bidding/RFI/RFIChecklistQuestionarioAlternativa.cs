namespace Dominio.Entidades.Embarcador.Bidding.RFI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RFI_CHECKLIST_QUESTIONARIO_ALTERNATIVA", EntityName = "RFIChecklistQuestionarioAlternativa", Name = "Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa", NameType = typeof(RFIChecklistQuestionarioAlternativa))]
    public class RFIChecklistQuestionarioAlternativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RLA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RLA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RLA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RFIChecklistQuestionario", Column = "RCQ_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RFIChecklistQuestionario RFIChecklistQuestionario { get; set; }
    }
}