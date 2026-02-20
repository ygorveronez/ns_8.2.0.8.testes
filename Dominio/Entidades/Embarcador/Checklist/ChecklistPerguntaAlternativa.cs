namespace Dominio.Entidades.Embarcador.Checklist
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_PERGUNTA_ALTERNATIVA", EntityName = "ChecklistPerguntaAlternativa", Name = "Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa", NameType = typeof(ChecklistPerguntaAlternativa))]
    public class ChecklistPerguntaAlternativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_RESPOSTA_IMPEDITIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RespostaImpeditiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_VALOR", TypeType = typeof(int), NotNull = true)]
        public virtual int Valor { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "CPA_MARCADO", TypeType = typeof(int), NotNull = true)]
        //public virtual bool Marcado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChecklistPergunta", Column = "CLP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChecklistPergunta ChecklistPergunta { get; set; }
    }
}