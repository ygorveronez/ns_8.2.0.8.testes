using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Bidding.AlcadasBidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_BIDDING", EntityName = "RegraAutorizacaoBidding", Name = "Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding", NameType = typeof(RegraAutorizacaoBidding))]
    public class RegraAutorizacaoBidding : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoBidding", Column = "RAT_TIPO_BIDDING", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoBidding { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoBidding", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_BIDDING_TIPO_BIDDING")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasBidding.AlcadaTipoBidding", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoBidding> AlcadasTipoBidding { get; set; }

        #endregion Propriedades

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_AUTORIZACAO_BIDDING_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion Propriedades Sobrescritas

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorTipoBidding;
        }

        public override void LimparAlcadas()
        {
            AlcadasTipoBidding?.Clear();
        }

        #endregion Métodos Públicos Sobrescritos
    }
}
