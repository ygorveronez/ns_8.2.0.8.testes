using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_CARREGAMENTO", EntityName = "RegraAutorizacaoCarregamento", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento", NameType = typeof(RegraAutorizacaoCarregamento))]
    public class RegraAutorizacaoCarregamento : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDiferencaValorApoliceTransportador", Column = "RAT_DIFERENCA_VALOR_APOLICE_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorDiferencaValorApoliceTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorModeloVeicularCarga", Column = "RAT_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPercentualOcupacaoCubagem", Column = "RAT_PERCENTUAL_OCUPACAO_CUBAGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorPercentualOcupacaoCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPercentualOcupacaoPallet", Column = "RAT_PERCENTUAL_OCUPACAO_PALLET", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorPercentualOcupacaoPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPercentualOcupacaoPeso", Column = "RAT_PERCENTUAL_OCUPACAO_PESO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorPercentualOcupacaoPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoCarga", Column = "RAT_TIPO_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarLinkParaAprovacaoPorEmail", Column = "RAT_ENVIAR_LINK_PARA_APROVACAO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarLinkParaAprovacaoPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasDiferencaValorApoliceTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARREGAMENTO_DIFERENCA_VALOR_APOLICE_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasMontagemCarga.AlcadaDiferencaValorApoliceTransportador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaDiferencaValorApoliceTransportador> AlcadasDiferencaValorApoliceTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARREGAMENTO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasMontagemCarga.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasModeloVeicularCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARREGAMENTO_MODELO_VEICULAR_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasMontagemCarga.AlcadaModeloVeicularCarga", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaModeloVeicularCarga> AlcadasModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPercentualOcupacaoCubagem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARREGAMENTO_PERCENTUAL_OCUPACAO_CUBAGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasMontagemCarga.AlcadaPercentualOcupacaoCubagem", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPercentualOcupacaoCubagem> AlcadasPercentualOcupacaoCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPercentualOcupacaoPallet", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARREGAMENTO_PERCENTUAL_OCUPACAO_PALLET")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasMontagemCarga.AlcadaPercentualOcupacaoPallet", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPercentualOcupacaoPallet> AlcadasPercentualOcupacaoPallet { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPercentualOcupacaoPeso", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARREGAMENTO_PERCENTUAL_OCUPACAO_PESO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasMontagemCarga.AlcadaPercentualOcupacaoPeso", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPercentualOcupacaoPeso> AlcadasPercentualOcupacaoPeso { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARREGAMENTO_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasMontagemCarga.AlcadaTipoCarga", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoCarga> AlcadasTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARREGAMENTO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasMontagemCarga.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CARREGAMENTO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (
                RegraPorFilial ||
                RegraPorModeloVeicularCarga ||
                RegraPorPercentualOcupacaoCubagem ||
                RegraPorPercentualOcupacaoPallet ||
                RegraPorPercentualOcupacaoPeso ||
                RegraPorTipoCarga ||
                RegraPorTipoOperacao ||
                RegraPorDiferencaValorApoliceTransportador
            );
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasModeloVeicularCarga?.Clear();
            AlcadasPercentualOcupacaoCubagem?.Clear();
            AlcadasPercentualOcupacaoPallet?.Clear();
            AlcadasPercentualOcupacaoPeso?.Clear();
            AlcadasTipoCarga?.Clear();
            AlcadasTipoOperacao?.Clear();
            AlcadasDiferencaValorApoliceTransportador?.Clear();
        }

        #endregion
    }
}
