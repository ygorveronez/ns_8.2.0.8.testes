namespace Dominio.Entidades.Embarcador.Checklist
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_RESPOSTA_PERGUNTA", EntityName = "CheckListRespostaPergunta", Name = "Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta", NameType = typeof(CheckListRespostaPergunta))]
    public class CheckListRespostaPergunta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Obrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_RESPOSTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CheckListResposta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CheckListResposta? Resposta { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "CLP_TIPO_INFORMATIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoInformativo), NotNull = false)]
        //public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoInformativo TipoInformativo { get; set; }

        //checklist respondido
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListResposta", Column = "CLR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListResposta CheckListResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_RESPOSTA_IMPEDITIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CheckListResposta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CheckListResposta? RespostaImpeditiva { get; set; }

        public virtual string DescricaoComObrigatoriedade
        {
            get
            {
                return this.Obrigatorio ? $"*{this.Descricao}" : this.Descricao;
            }
        }
    }
}
