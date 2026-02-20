using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECK_LIST_CARGA_PERGUNTA_AUDITORIA", EntityName = "CheckListCargaPerguntaAuditoria", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria", NameType = typeof(CheckListCargaPerguntaAuditoria))]
    public class CheckListCargaPerguntaAuditoria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_RESPOSTA_ANTIGA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RespostaAntiga { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_RESPOSTA_NOVA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RespostaNova { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListCargaPergunta", Column = "CLP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListCargaPergunta CheckListCargaPergunta { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Alterado de {RespostaAntiga} para {RespostaNova}";
            }
        }
    }
}