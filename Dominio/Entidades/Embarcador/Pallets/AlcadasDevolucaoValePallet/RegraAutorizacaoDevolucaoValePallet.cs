using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_DEVOLUCAO_VALE_PALLET", EntityName = "RegraAutorizacaoDevolucaoValePallet", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet", NameType = typeof(RegraAutorizacaoDevolucaoValePallet))]
    public class RegraAutorizacaoDevolucaoValePallet : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDiasDevolucao", Column = "RAT_DIAS_DEVOLUCAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorDiasDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasDiasDevolucao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DEVOLUCAO_VALE_PALLET_DIAS_DEVOLUCAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasDevolucaoValePallet.AlcadaDiasDevolucao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaDiasDevolucao> AlcadasDiasDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DEVOLUCAO_VALE_PALLET_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasDevolucaoValePallet.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_DEVOLUCAO_VALE_PALLET_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorDiasDevolucao || RegraPorFilial;
        }

        public override void LimparAlcadas()
        {
            AlcadasDiasDevolucao?.Clear();
            AlcadasFilial?.Clear();
        }

        #endregion
    }
}
