using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_CONTRATO_PRESTACAO_SERVICO", EntityName = "RegraAutorizacaoContratoPrestacaoServico", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico", NameType = typeof(RegraAutorizacaoContratoPrestacaoServico))]
    public class RegraAutorizacaoContratoPrestacaoServico : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CONTRATO_PRESTACAO_SERVICO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasContratoPrestacaoServico.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CONTRATO_PRESTACAO_SERVICO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasContratoPrestacaoServico.AlcadaTransportador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTransportador> AlcadasTransportador { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CONTRATO_PRESTACAO_SERVICO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorFilial || RegraPorTransportador;
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasTransportador?.Clear();
        }

        #endregion
    }
}
