using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO", EntityName = "RegraAutorizacaoContratoFreteAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto", NameType = typeof(RegraAutorizacaoContratoFreteAcrescimoDesconto))]
    public class RegraAutorizacaoContratoFreteAcrescimoDesconto : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorJustificativa", Column = "RAT_JUSTIFICATIVA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValor", Column = "RAT_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasJustificativa", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO_JUSTIFICATIVA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasContratoFreteAcrescimoDesconto.AlcadaJustificativa", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaJustificativa> AlcadasJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasContratoFreteAcrescimoDesconto.AlcadaValor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValor> AlcadasValor { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorJustificativa || RegraPorValor;
        }

        public override void LimparAlcadas()
        {
            AlcadasJustificativa?.Clear();
            AlcadasValor?.Clear();
        }

        #endregion
    }
}
