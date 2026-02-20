using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_CARGA_VIGENCIA", EntityName = "CheckListCargaVigencia", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia", NameType = typeof(CheckListCargaVigencia))]
    public class CheckListCargaVigencia : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCV_DATA_FIM_VIGENCIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFimVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCV_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCV_PREENCHIMENTO_MANUAL_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PreenchimentoManualObrigatorio { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public virtual string Descricao
        {
            get { return $"Vigência para Checklist da Filial: {Filial.Descricao}" + (TipoOperacao != null ? $" e Tipo Operação: {TipoOperacao.Descricao}." : "."); }
        }

        #endregion Propriedades Com Regras
    }
}
