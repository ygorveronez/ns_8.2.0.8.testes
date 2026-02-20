using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_CARGA_HISTORICO", EntityName = "CheckListCargaHistorico", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaHistorico", NameType = typeof(CheckListCargaHistorico))]
    public class CheckListCargaHistorico : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_SITUACAO", TypeType = typeof(SituacaoCheckList), NotNull = true)]
        public virtual SituacaoCheckList Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListCarga", Column = "CLC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListCarga ChecklistCarga { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public virtual string DescricaoSituacao
        {
            get { return this.Situacao.ObterDescricao(); }
        }

        #endregion Propriedades Com Regras
    }
}
