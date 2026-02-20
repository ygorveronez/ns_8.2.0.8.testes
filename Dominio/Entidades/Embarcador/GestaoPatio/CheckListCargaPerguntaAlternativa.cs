namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECK_LIST_CARGA_PERGUNTA_ALTERNATIVA", EntityName = "CheckListCargaPerguntaAlternativa", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa", NameType = typeof(CheckListCargaPerguntaAlternativa))]
    public class CheckListCargaPerguntaAlternativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_MARCADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Marcado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_OPCAO_IMPEDITIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OpcaoImpeditiva { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListCargaPergunta", Column = "CLP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListCargaPergunta CheckListCargaPergunta { get; set; }
    }
}
