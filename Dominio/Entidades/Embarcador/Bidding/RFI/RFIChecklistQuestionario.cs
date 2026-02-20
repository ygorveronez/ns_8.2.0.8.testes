namespace Dominio.Entidades.Embarcador.Bidding.RFI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RFI_CHECKLIST_QUESTIONARIO", EntityName = "RFIChecklistQuestionario", Name = "Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario", NameType = typeof(RFIChecklistQuestionario))]
    public class RFIChecklistQuestionario : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCQ_DESCRICAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCQ_REQUISITO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRequisitoRFIChecklist), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRequisitoRFIChecklist Requisito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCQ_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRequisitoRFIChecklist), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckListRFI Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RFIChecklist", Column = "RCL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RFIChecklist Checklist { get; set; }
    }
}
