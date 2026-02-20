using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_APROVACAO_SOLICITACAO_GAS", EntityName = "RegraAprovacaoSolicitacaoGas", Name = "Dominio.Entidades.Embarcador.Logistica.RegraAprovacaoSolicitacaoGas", NameType = typeof(RegraAprovacaoSolicitacaoGas))]
    public class RegraAprovacaoSolicitacaoGas : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTempoExcedido", Column = "RAT_TEMPO_EXCEDIDO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTempoExcedido { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasSolicitacaoGasData", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_SOLICITACAO_GAS_DATA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasSolicitacaoGas.AlcadaSolicitacaoGasData", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaSolicitacaoGasData> AlcadasSolicitacaoGasData { get; set; }
        
        #endregion

        #region Propriedades Sobrescritas
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_SOLICITACAO_GAS_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorTempoExcedido;
        }

        public override void LimparAlcadas()
        {
            AlcadasSolicitacaoGasData?.Clear();
        }

        #endregion
    }
}
