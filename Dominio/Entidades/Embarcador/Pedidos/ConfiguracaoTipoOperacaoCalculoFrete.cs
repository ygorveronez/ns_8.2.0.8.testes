namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CALCULO_FRETE", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoCalculoFrete", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCalculoFrete", NameType = typeof(ConfiguracaoTipoOperacaoCalculoFrete))]
    public class ConfiguracaoTipoOperacaoCalculoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_MESCLAR_VALOR_EMBARCADOR_COM_TABELA_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool MesclarValorEmbarcadorComTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_BLOQUEAR_AJUSTE_CONFIGURACOES_FRETE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAjusteConfiguracoesFreteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_TIPO_COTACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCotacaoFreteInternacional), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCotacaoFreteInternacional? TipoCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 20, NotNull = false)]
        public virtual decimal? ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_PERMITE_INFORMAR_QUANTIDADE_PALETES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarQuantidadePaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_MESCLAR_COMPONENTES_MANUAIS_PEDIDO_COM_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MesclarComponentesManuaisPedidoComTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_EXECUTAR_PRE_CALCULO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExecutarPreCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RatearValorFreteEntrePedidosAposReceberDocumentos", Column = "CTC_RATEAR_VALOR_FRETE_ENTRE_PEDIDOS_APOS_RECEBER_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearValorFreteEntrePedidosAposReceberDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularFretePeloBIDPedidoOrigem", Column = "CTC_CALCULAR_FRETE_PELO_BID_PEDIDO_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFretePeloBIDPedidoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador", Column = "CTC_NAO_INFORMAR_VALOR_ICMS_QUANDO_VALOR_FRETE_INFORMADO_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformarValorFreteTerceiroManualmente", Column = "CTC_INFORMAR_VALOR_FRETE_TERCEIRO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarValorFreteTerceiroManualmente { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarInclusaoIcmsCargaPedidoNaEmissaoCteComplementar", Column = "CTC_CONSIDERAR_INCLUSAO_ICMS_CARGA_PEDIDO_NA_EMISSAO_CTE_COMPLEMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ConsiderarInclusaoIcmsCargaPedidoNaEmissaoCteComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteEscolherDestinacaoDoComplementoDeFrete", Column = "CTC_PERMITE_ESCOLHER_DESTINACAO_DO_COMPLEMENTO_DE_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteEscolherDestinacaoDoComplementoDeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_NECCESSARIO_AGUARDAR_VINCULO_NOTA_REMESSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessarioAguardarVinculoNotadeRemessaIndustrializador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_IMPORTAR_VEICULO_MDFE_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarVeiculoMDFEEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_VALOR_MAXIMO_CALCULO_FRETE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorMaximoCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_NAO_ALTERAR_TIPO_PAGAMENTO_TOMADOR_VALORES_INFORMADOS_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAlterarTipoPagamentoTomadorValoresInformadosManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirComprovantesLiberacaoPagamentoContratoFrete", Column = "CTC_EXIGIR_COMPROVANTE_LIBERACAO_PAGAMENTO_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirComprovantesLiberacaoPagamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarContratoFreteCliente", Column = "CTC_UTILIZAR_CONTRATO_FRETE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarContratoFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RatearValorFreteInformadoEmbarcador", Column = "CTC_RATEAR_VALOR_FRETE_INFORMADO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearValorFreteInformadoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCoberturaDeCarga", Column = "CTC_UTILIZAR_COBERTURA_DE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCoberturaDeCarga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configurações para cálculo de frete";
            }
        }
    }
}
