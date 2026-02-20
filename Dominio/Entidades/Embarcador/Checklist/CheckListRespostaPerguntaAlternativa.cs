namespace Dominio.Entidades.Embarcador.Checklist
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_RESPOSTA_PERGUNTA_ALTERNATIVA", EntityName = "CheckListRespostaPerguntaAlternativa", Name = "Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa", NameType = typeof(CheckListRespostaPerguntaAlternativa))]
    public class CheckListRespostaPerguntaAlternativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRA_MARCADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Marcado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRA_OPCAO_IMPEDITIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OpcaoImpeditiva { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListRespostaPergunta", Column = "CRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListRespostaPergunta CheckListRespostaPergunta { get; set; }
    }
}
