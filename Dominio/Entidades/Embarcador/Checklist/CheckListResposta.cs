using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Checklist
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_RESPOSTA", EntityName = "CheckListResposta", Name = "Dominio.Entidades.Embarcador.Checklist.CheckListResposta", NameType = typeof(CheckListResposta))]
    public class CheckListResposta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLR_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataResposta { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "CLC_DATA_LIBERACAO", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? DataLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLR_REAVALIADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Reavaliada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLR_SITUACAO", TypeType = typeof(SituacaoCheckList), NotNull = false)]
        public virtual SituacaoCheckList Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLR_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Checklist", Column = "CKL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Checklist Checklist { get; set; }

        //perguntas do checklist respondido
        [NHibernate.Mapping.Attributes.Bag(0, Name = "Perguntas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHECKLIST_RESPOSTA_PERGUNTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CheckListRespostaPergunta", Column = "CRP_CODIGO")]
        public virtual ICollection<CheckListRespostaPergunta> Perguntas { get; set; }


        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual string DescricaoSituacao
        {
            get { return this.Situacao.ObterDescricao(); }
        }

    }
}