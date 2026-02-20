using System.Collections.Generic;
using NHibernate.Mapping.Attributes;

namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga
{
    [Class(0, Table = "T_REGRAS_AUTORIZACAO_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA", EntityName = "RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga", NameType = typeof(RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga))]
    public class RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [Property(0, Name = "RegraPorModeloVeicularCarga", Column = "RAT_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorModeloVeicularCarga { get; set; }

        [Property(0, Name = "RegraPorTipoCarga", Column = "RAT_TIPO_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoCarga { get; set; }

        [Property(0, Name = "RegraPorTipoOperacao", Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [Property(0, Name = "RegraPorValorFrete", Column = "RAT_VALOR_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorFrete { get; set; }

        [Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = CollectionLazy.True, Table = "T_ALCADA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA_FILIAL")]
        [Key(1, Column = "RAT_CODIGO")]
        [ManyToMany(2, Class = "AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [Bag(0, Name = "AlcadasModeloVeicularCarga", Cascade = "none", Lazy = CollectionLazy.True, Table = "T_ALCADA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA_MODELO_VEICULAR_CARGA")]
        [Key(1, Column = "RAT_CODIGO")]
        [ManyToMany(2, Class = "AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaModeloVeicularCarga", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaModeloVeicularCarga> AlcadasModeloVeicularCarga { get; set; }

        [Bag(0, Name = "AlcadasTipoCarga", Cascade = "none", Lazy = CollectionLazy.True, Table = "T_ALCADA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA_TIPO_CARGA")]
        [Key(1, Column = "RAT_CODIGO")]
        [ManyToMany(2, Class = "AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaTipoCarga", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoCarga> AlcadasTipoCarga { get; set; }

        [Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = CollectionLazy.True, Table = "T_ALCADA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA_TIPO_OPERACAO")]
        [Key(1, Column = "RAT_CODIGO")]
        [ManyToMany(2, Class = "AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        [Bag(0, Name = "AlcadasValorFrete", Cascade = "none", Lazy = CollectionLazy.True, Table = "T_ALCADA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA_VALOR_FRETE")]
        [Key(1, Column = "RAT_CODIGO")]
        [ManyToMany(2, Class = "AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaValorFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorFrete> AlcadasValorFrete { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [Set(0, Name = "Aprovadores", Cascade = "none", Lazy = CollectionLazy.True, Table = "T_REGRA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA_FUNCIONARIOS")]
        [Key(1, Column = "RAT_CODIGO")]
        [ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (
                RegraPorFilial ||
                RegraPorModeloVeicularCarga ||
                RegraPorTipoCarga ||
                RegraPorTipoOperacao ||
                RegraPorValorFrete
            );
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasModeloVeicularCarga?.Clear();
            AlcadasTipoCarga?.Clear();
            AlcadasTipoOperacao?.Clear();
            AlcadasValorFrete?.Clear();
        }

        #endregion
    }
}
