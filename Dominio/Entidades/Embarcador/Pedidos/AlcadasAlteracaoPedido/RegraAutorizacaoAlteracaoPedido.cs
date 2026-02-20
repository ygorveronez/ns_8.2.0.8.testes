using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_ALTERACAO_PEDIDO", EntityName = "RegraAutorizacaoAlteracaoPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido", NameType = typeof(RegraAutorizacaoAlteracaoPedido))]
    public class RegraAutorizacaoAlteracaoPedido : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoCarga", Column = "RAT_TIPO_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTransportador", Column = "RAT_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ALTERACAO_PEDIDO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAlteracaoPedido.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ALTERACAO_PEDIDO_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAlteracaoPedido.AlcadaTipoCarga", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoCarga> AlcadasTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ALTERACAO_PEDIDO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAlteracaoPedido.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ALTERACAO_PEDIDO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasAlteracaoPedido.AlcadaTransportador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTransportador> AlcadasTransportador { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ALTERACAO_PEDIDO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (
                RegraPorFilial ||
                RegraPorTipoCarga ||
                RegraPorTipoOperacao ||
                RegraPorTransportador
            );
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasTipoCarga?.Clear();
            AlcadasTipoOperacao?.Clear();
            AlcadasTransportador?.Clear();
        }

        #endregion
    }
}
