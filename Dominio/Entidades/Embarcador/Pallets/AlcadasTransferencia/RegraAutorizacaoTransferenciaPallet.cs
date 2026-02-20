using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_TRANSFERENCIA_PALLET", EntityName = "RegraAutorizacaoTransferenciaPallet", Name = "Dominio.Entidades.Embarcador.Pallets.RegraAutorizacaoTransferenciaPallet", NameType = typeof(RegraAutorizacaoTransferenciaPallet))]
    public class RegraAutorizacaoTransferenciaPallet : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorQuantidade", Column = "RAT_QUANTIDADE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorQuantidade { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TRANSFERENCIA_PALLET_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTransferencia.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasQuantidade", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TRANSFERENCIA_PALLET_QUANTIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTransferencia.AlcadaQuantidade", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaQuantidade> AlcadasQuantidade { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_TRANSFERENCIA_PALLET_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorFilial || RegraPorQuantidade;
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasQuantidade?.Clear();
        }

        #endregion
    }
}
