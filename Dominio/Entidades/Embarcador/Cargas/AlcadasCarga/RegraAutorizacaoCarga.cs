using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_CARGA", EntityName = "RegraAutorizacaoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga", NameType = typeof(RegraAutorizacaoCarga))]
    public class RegraAutorizacaoCarga : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarAcrescimoValorTabelaFretePorComponenteFrete", Column = "RAT_VALIDAR_ACRESCIMO_VALOR_TABELA_FRETE_POR_COMPONENTE_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarAcrescimoValorTabelaFretePorComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorComponenteFrete", Column = "RAT_COMPONENTE_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorModeloVeicularCarga", Column = "RAT_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPercentualAcrescimoValorTabelaFrete", Column = "RAT_PERCENTUAL_ACRESCIMO_VALOR_TABELA_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorPercentualAcrescimoValorTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPercentualDescontoValorTabelaFrete", Column = "RAT_PERCENTUAL_DESCONTO_VALOR_TABELA_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorPercentualDescontoValorTabelaFrete { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPercentualDiferencaFreteLiquidoFreteTerceiro", Column = "RAT_PERCENTUAL_DIFERENCA_VALOR_FRETE_LIQUIDO_PARA_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorPercentualDiferencaFreteLiquidoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro", Column = "RAT_PERCENTUAL_DIFERENCA_VALOR_FRETE_LIQUIDO_TOTAL_PARA_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoCarga", Column = "RAT_TIPO_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTomador", Column = "RAT_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPortoOrigem", Column = "RAT_PORTO_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorPortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPortoDestino", Column = "RAT_PORTO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorPortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorFrete", Column = "RAT_VALOR_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDiferencaValorFrete", Column = "RAT_DIFERENCA_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorDiferencaValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPesoContainer", Column = "RAT_PESO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorPesoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TIPO_REGRA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga TipoRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorMotivoSolicitacaoFrete", Column = "RAT_MOTIVO_SOLICITACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorMotivoSolicitacaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorAcrescimoValorTabelaFrete", Column = "RAT_VALOR_ACRESCIMO_VALOR_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorValorAcrescimoValorTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPercentualFreteSobreNota", Column = "RAT_PERCENTUAL_FRETE_SOBRE_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorPercentualFreteSobreNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarLinkParaAprovacaoPorEmail", Column = "RAT_ENVIAR_LINK_PARA_APROVACAO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarLinkParaAprovacaoPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirInformarJustificativaCustoExtraCadastrado", Column = "RAT_EXIGIR_INFORMAR_JUSTIFICATIVA_CUSTO_EXTRA_CADASTRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarJustificativaCustoExtraCadastrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorAutorizacaoTipoTerceiro", Column = "RAT_REGRA_AUTORIZACAO_POR_TIPO_TERCEIRO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorAutorizacaoTipoTerceiro { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasComponenteFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_COMPONENTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaComponenteFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaComponenteFrete> AlcadasComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasModeloVeicularCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_MODELO_VEICULAR_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaModeloVeicularCarga", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaModeloVeicularCarga> AlcadasModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPercentualAcrescimoValorTabelaFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_PERCENTUAL_ACRESCIMO_VALOR_TABELA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaPercentualAcrescimoValorTabelaFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPercentualAcrescimoValorTabelaFrete> AlcadasPercentualAcrescimoValorTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPercentualDescontoValorTabelaFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_PERCENTUAL_DESCONTO_VALOR_TABELA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaPercentualDescontoValorTabelaFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPercentualDescontoValorTabelaFrete> AlcadasPercentualDescontoValorTabelaFrete { get; set; }
        
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPercentualDiferencaFreteLiquidoFreteTerceiro", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_PERCENTUAL_DIFERENCA_VALOR_FRETE_LIQUIDO_PARA_FRETE_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro> AlcadasPercentualDiferencaFreteLiquidoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPercentualDiferencaFreteLiquidoTotalFreteTerceiro", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_PERCENTUAL_DIFERENCA_VALOR_FRETE_LIQUIDO_TOTAL_PARA_FRETE_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoTotalFreteTerceiro", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPercentualDiferencaFreteLiquidoTotalFreteTerceiro> AlcadasPercentualDiferencaFreteLiquidoTotalFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaTipoCarga", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoCarga> AlcadasTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTomador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_TOMADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaTomador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTomador> AlcadasTomador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPortoOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_PORTO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaPortoOrigem", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPortoOrigem> AlcadasPortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPortoDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_PORTO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaPortoDestino", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPortoDestino> AlcadasPortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_VALOR_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaValorFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorFrete> AlcadasValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasDiferencaValorFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_DIFERENCA_VALOR_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaDiferencaValorFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaDiferencaValorFrete> AlcadasDiferencaValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPesoContainer", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_PESO_CONTAINER")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaPesoContainer", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPesoContainer> AlcadasPesoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasMotivoSolicitacaoFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_MOTIVO_SOLICITACAO_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaMotivoSolicitacaoFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaMotivoSolicitacaoFrete> AlcadasMotivoSolicitacaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorAcrescimoValorTabelaFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_VALOR_ACRESCIMO_VALOR_TABELA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaValorAcrescimoValorTabelaFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorAcrescimoValorTabelaFrete> AlcadasValorAcrescimoValorTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPercentualFreteSobreNota", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_PERCENTUAL_FRETE_SOBRE_NOTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaPercentualFreteSobreNota", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPercentualFreteSobreNota> AlcadasPercentualFreteSobreNota { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasAutorizacaoTipoTerceiro", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_AUTORIZACAO_TIPO_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCarga.AlcadaAutorizacaoTipoTerceiro", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaAutorizacaoTipoTerceiro> AlcadasAutorizacaoTipoTerceiro { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CARGA_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (
                RegraPorComponenteFrete ||
                RegraPorFilial ||
                RegraPorModeloVeicularCarga ||
                RegraPorPercentualAcrescimoValorTabelaFrete ||
                RegraPorPercentualDescontoValorTabelaFrete ||
                RegraPorTipoCarga ||
                RegraPorTomador ||
                RegraPorPortoDestino ||
                RegraPorPortoOrigem ||
                RegraPorTipoOperacao ||
                RegraPorValorFrete ||
                RegraPorPesoContainer ||
                RegraPorMotivoSolicitacaoFrete ||
                RegraPorValorAcrescimoValorTabelaFrete ||
                RegraPorPercentualFreteSobreNota ||
                RegraPorAutorizacaoTipoTerceiro
            );
        }

        public override void LimparAlcadas()
        {
            AlcadasComponenteFrete?.Clear();
            AlcadasFilial?.Clear();
            AlcadasModeloVeicularCarga?.Clear();
            AlcadasPercentualAcrescimoValorTabelaFrete?.Clear();
            AlcadasPercentualDescontoValorTabelaFrete?.Clear();
            AlcadasTipoCarga?.Clear();
            AlcadasTomador?.Clear();
            AlcadasPortoDestino?.Clear();
            AlcadasPortoOrigem?.Clear();
            AlcadasTipoOperacao?.Clear();
            AlcadasValorFrete?.Clear();
            AlcadasPesoContainer?.Clear();
            AlcadasMotivoSolicitacaoFrete?.Clear();
            AlcadasValorAcrescimoValorTabelaFrete?.Clear();
            AlcadasPercentualFreteSobreNota?.Clear();
            AlcadasAutorizacaoTipoTerceiro?.Clear();
        }

        #endregion
    }
}
