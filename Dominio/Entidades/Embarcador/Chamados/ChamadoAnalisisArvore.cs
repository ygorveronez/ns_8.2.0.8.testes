using Dominio.ObjetosDeValor.Embarcador.Enumeradores;



namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_ANALISIS_ARVORE", EntityName = "ChamadoOcorrenciaArvore", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrenciaArvore", NameType = typeof(ChamadoAnalisisArvore))]
    public class ChamadoAnalisisArvore : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_PAI", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Pai { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_KEY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Key { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_PERGUNTA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Pergunta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_RESPOSTA", TypeType = typeof(TipoResposta), Length = 250, NotNull = false)]
        public virtual TipoResposta Resposta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoChamado MotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_SITUACAO", TypeType = typeof(SituacaoPerguntaArvore), NotNull = false)]
        public virtual SituacaoPerguntaArvore Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_STATUS_FINALIZACAO_ATENDIMENTO", TypeType = typeof(StatusFinalizacaoAtendimento), NotNull = false)]
        public virtual StatusFinalizacaoAtendimento StatusFinalizacaoAtendimento { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoAnalise", Column = "ANC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual ChamadoAnalise ChamadoAnalise { get; set; }  
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamadoCausas", Column = "MCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoChamadoCausas MotivoChamadoCausa { get; set; }

    }
}
