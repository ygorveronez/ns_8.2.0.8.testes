using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_CHECKLIST_QUESTIONARIO_PADRAO", EntityName = "BiddingChecklistQuestionarioPadrao", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao", NameType = typeof(BiddingChecklistQuestionarioPadrao))]
    public class BiddingChecklistQuestionarioPadrao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCP_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCP_REQUISITO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRequisitoBiddingChecklist), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRequisitoBiddingChecklist Requisito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoBidding", Column = "TBI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoBidding TipoBidding { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_CHECKLIST_QUESTIONARIO_ANEXO_PADRAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BiddingChecklistQuestionarioAnexoPadrao", Column = "ANX_CODIGO")]
        public virtual IList<BiddingChecklistQuestionarioAnexoPadrao> Anexos { get; set; }
    }
}
