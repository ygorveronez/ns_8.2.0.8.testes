using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Checklist
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_PERGUNTA", EntityName = "ChecklistPergunta", Name = "Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta", NameType = typeof(ChecklistPergunta))]
    public class ChecklistPergunta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_TIPO_RESPOSTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList TipoResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_OBRIGATORIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Obrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_PERMITE_OPCAO_NAO_SE_APLICA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteOpcaoNaoSeAplica { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Checklist", Column = "CKL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Checklist Checklist { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Alternativas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHECKLIST_PERGUNTA_ALTERNATIVA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChecklistPerguntaAlternativa", Column = "CPA_CODIGO")]
        public virtual ICollection<ChecklistPerguntaAlternativa> Alternativas { get; set; }
    }
}
