using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Fatura.AlcadasFatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_FATURA", EntityName = "RegraAutorizacaoFatura", Name = "Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura", NameType = typeof(RegraAutorizacaoFatura))]
    public class RegraAutorizacaoFatura : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTomador", Column = "RAT_TOMADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValor", Column = "RAT_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTomador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_FATURA_TOMADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasFatura.AlcadaTomador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTomador> AlcadasTomador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_FATURA_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasFatura.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_FATURA_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasFatura.AlcadaValor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValor> AlcadasValor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_FATURA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasFatura.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_AUTORIZACAO_FATURA_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorTomador || RegraPorFilial || RegraPorValor || RegraPorTipoOperacao;
        }

        public override void LimparAlcadas()
        {
            AlcadasTomador?.Clear();
            AlcadasFilial?.Clear();
            AlcadasValor?.Clear();
            AlcadasTipoOperacao?.Clear();
        }

        #endregion
    }
}
