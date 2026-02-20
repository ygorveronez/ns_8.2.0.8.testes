using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_AVARIA_PALLET", EntityName = "RegraAutorizacaoAvaria", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria", NameType = typeof(RegraAutorizacaoAvaria))]
    public class RegraAutorizacaoAvaria : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorMotivoAvaria", Column = "RAT_MOTIVO_AVARIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorMotivoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorSetor", Column = "RAT_SETOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorSetor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTransportador", Column = "RAT_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_AVARIA_PALLET_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAvaria.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasMotivoAvaria", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_AVARIA_PALLET_MOTIVO_AVARIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAvaria.AlcadaMotivoAvaria", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaMotivoAvaria> AlcadasMotivoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasSetor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_AVARIA_PALLET_SETOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAvaria.AlcadaSetor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaSetor> AlcadasSetor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_AVARIA_PALLET_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAvaria.AlcadaTransportador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTransportador> AlcadasTransportador { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AVARIA_PALLET_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorFilial || RegraPorMotivoAvaria || RegraPorSetor || RegraPorTransportador;
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasMotivoAvaria?.Clear();
            AlcadasSetor?.Clear();
            AlcadasTransportador?.Clear();
        }

        #endregion
    }
}
