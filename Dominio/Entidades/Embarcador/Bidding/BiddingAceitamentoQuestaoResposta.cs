namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_ACEITAMENTO_QUESTAO_RESPOSTA", EntityName = "BiddingAceitamentoQuestaoResposta", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta", NameType = typeof(BiddingAceitamentoQuestaoResposta))]
    public class BiddingAceitamentoQuestaoResposta : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BQR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingChecklistQuestionario", Column = "BCQ_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BiddingChecklistQuestionario Pergunta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BQR_OBSERVACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BQR_RESPOSTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Resposta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingChecklistBiddingTransportador", Column = "CBT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BiddingChecklistBiddingTransportador ChecklistBiddingTransportador { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Resposta para a pergunta: " + this.Pergunta.Descricao;
            }
        } 
    }
}
