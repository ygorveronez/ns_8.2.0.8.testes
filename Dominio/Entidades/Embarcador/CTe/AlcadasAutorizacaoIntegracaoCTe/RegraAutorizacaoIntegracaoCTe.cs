using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_INTEGRACAO_CTE", EntityName = "RegraAutorizacaoIntegracaoCTe", Name = "Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe", NameType = typeof(RegraAutorizacaoIntegracaoCTe))]
    public class RegraAutorizacaoIntegracaoCTe : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorFrete", Column = "RAT_VALOR_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_INTEGRACAO_CTE_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAutorizacaoIntegracaoCTe.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_INTEGRACAO_CTE_VALOR_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAutorizacaoIntegracaoCTe.AlcadaValorFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorFrete> AlcadasValorFrete { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AUTORIZACAO_INTEGRACAO_CTE_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (
                RegraPorTipoOperacao ||
                RegraPorValorFrete
            );
        }

        public override void LimparAlcadas()
        {
            AlcadasTipoOperacao?.Clear();
            AlcadasValorFrete?.Clear();
        }

        #endregion
    }
}
