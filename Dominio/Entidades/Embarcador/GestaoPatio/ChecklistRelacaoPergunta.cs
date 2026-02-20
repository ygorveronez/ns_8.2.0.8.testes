namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_RELACAO_PERGUNTA", EntityName = "ChecklistRelacaoPergunta", Name = "Dominio.Entidades.Embarcador.GestaoPatio.ChecklistRelacaoPergunta", NameType = typeof(ChecklistRelacaoPergunta))]
    public class ChecklistRelacaoPergunta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CRP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoChecklistImpressao", Column = "TCI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoChecklistImpressao TipoChecklistImpressao { get; set; }
    }
}
