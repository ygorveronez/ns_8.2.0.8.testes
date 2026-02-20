using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_PAGAMENTO", EntityName = "RegraAutorizacaoPagamento", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento", NameType = typeof(RegraAutorizacaoPagamento))]
    public class RegraAutorizacaoPagamento : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTransportador", Column = "RAT_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorPagamento", Column = "RAT_VALOR_PAGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PAGAMENTO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasPagamento.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PAGAMENTO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasPagamento.AlcadaTransportador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTransportador> AlcadasTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorPagamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PAGAMENTO_VALOR_PAGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasPagamento.AlcadaValorPagamento", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorPagamento> AlcadasValorPagamento { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PAGAMENTO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (
                RegraPorFilial ||
                RegraPorTransportador ||
                RegraPorValorPagamento
            );
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasTransportador?.Clear();
            AlcadasValorPagamento?.Clear();
        }

        #endregion
    }
}
