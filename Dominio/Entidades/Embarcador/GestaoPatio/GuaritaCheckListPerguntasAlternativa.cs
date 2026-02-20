namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUARITA_CHECK_LIST_PERGUNTA_ALTERNATIVA", EntityName = "GuaritaCheckListPerguntasAlternativa", Name = "Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa", NameType = typeof(GuaritaCheckListPerguntasAlternativa))]
    public class GuaritaCheckListPerguntasAlternativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GAL_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GAL_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GAL_MARCADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Marcado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GuaritaCheckListPerguntas", Column = "GPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GuaritaCheckListPerguntas GuaritaCheckListPerguntas { get; set; }
    }
}