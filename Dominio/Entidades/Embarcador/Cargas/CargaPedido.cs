using Dominio.Entidades.Embarcador.Pedidos;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO", EntityName = "CargaPedido", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedido", NameType = typeof(CargaPedido))]
    public class CargaPedido : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedido>
    {
        public CargaPedido()
        {
            this.DataCriacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_CONTROLE_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalVenda", Column = "CNV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalVenda CanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Redespacho", Column = "RED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Redespacho CargaRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorCTeGlobalizadoDestinatario", Column = "PED_INDICADOR_CTE_GLOBALIZADO_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorCTeGlobalizadoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorNFSGlobalizado", Column = "PED_INDICADOR_NFS_GLOBALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorNFSGlobalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorCTeSimplificado", Column = "CPE_INDICADOR_CTE_SIMPLIFICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorCTeSimplificado { get; set; }

        /// <summary>
        /// Valor do frete somando os componentes de frete (Exemplo: Pedágio, Descarga, entre outros) 
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteAPagar", Column = "PED_VALOR_FRETE_PAGAR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteAPagar { get; set; }

        /// <summary>
        /// Valor de frete que será utilizado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PED_VALOR_FRETE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteAntesAlteracaoManual", Column = "PED_VALOR_FRETE_ANTES_ALTERACAO_MANUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteAntesAlteracaoManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteFilialEmissoraAntesAlteracaoManual", Column = "PED_VALOR_FRETE_FILIAL_EMISSORA_ANTES_ALTERACAO_MANUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteFilialEmissoraAntesAlteracaoManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteResidual", Column = "PED_VALOR_FRETE_RESIDUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteResidual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseFrete", Column = "PED_VALOR_BASE_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTabelaFrete", Column = "PED_VALOR_TABELA_FRETE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTabelaFreteFilialEmissora", Column = "PED_VALOR_TABELA_FRETE_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTabelaFreteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "PED_TIPO_TOMADOR", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_PONTO_PARTIDA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente PontoPartida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiColetaEquipamentoPontoPartida", Column = "PED_POSSUI_COLETA_EQUIPAMENTO_PONTO_PARTIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiColetaEquipamentoPontoPartida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraTomador", Column = "RTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.RegraTomador RegraTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoCTeParticipantes", Column = "PED_TIPO_EMISSA_CTE_PARTICIPANTES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes TipoEmissaoCTeParticipantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRateio", Column = "PED_TIPO_RATEIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos TipoRateio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula FormulaRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContratacaoCarga", Column = "PED_CONTRATACAO_CARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga TipoContratacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContratacaoCargaSubContratacaoFilialEmissora", Column = "PED_CONTRATACAO_CARGA_SUB_CONTRATACAO_FILIAL_EMISSORA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga TipoContratacaoCargaSubContratacaoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroReboque", Column = "PED_NUMERO_REBOQUE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque NumeroReboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCarregamentoPedido", Column = "PED_TIPO_CARREGAMENTO_PEDIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCarregamentoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCarregamentoPedido TipoCarregamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "PED_VALOR_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoISS", Column = "PED_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaISS", Column = "PED_PERCENTUAL_ALICOTA_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 6, NotNull = false)]
        public virtual decimal PercentualAliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRetencaoISS", Column = "PED_PERCENTUAL_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgInformarRecebedor", Column = "PED_AG_INFORMAR_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgInformarRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirISSBaseCalculo", Column = "PED_INCLUIR_ISS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirISSBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetencaoISS", Column = "PED_VALOR_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReterIR", Column = "PED_RETER_IR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReterIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIR", Column = "PED_ALIQUOTA_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIR", Column = "PED_BASE_CALCULO_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIR", Column = "PED_VALOR_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "PED_VALOR_ICMS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCofins", Column = "PED_VALOR_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPis", Column = "PED_VALOR_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSIncluso", Column = "PED_VALOR_ICMS_INCLUSO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSIncluso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PERCENTUAL_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "PED_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBC", Column = "PED_PERCENTUAL_REDUCAO_BC", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoBC { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "PED_CST", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSBaseCalculo", Column = "PED_INCLUIR_ICMS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMSBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualIncluirBaseCalculo", Column = "PED_PERCENTUAL_INCLUIR_BASE_CALCULO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualIncluirBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRegraICMSCTe", Column = "PED_OBSERVACAO_REGRA_ICMS_CTE", TypeType = typeof(string), NotNull = false, Length = 400)]
        public virtual string ObservacaoRegraICMSCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescarga", Column = "PED_VALOR_DESCARGA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "PED_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoDescargaCliente", Column = "CDC_CONFIGURACAO_DESCARGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente ConfiguracaoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdValorem", Column = "PED_VALOR_AD_VALOREM", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ICMSPagoPorST", Column = "PED_ICMS_PAGO_POR_ST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ICMSPagoPorST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImpostoInformadoPeloEmbarcador", Column = "PED_IMPOSTO_INFORMADO_PELO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImpostoInformadoPeloEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoPallet", Column = "PED_PEDIDO_PALLET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquota", Column = "PED_PERCENTUAL_ALICOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCofins", Column = "PED_ALICOTA_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPis", Column = "PED_ALICOTA_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaInternaDifal", Column = "PED_PERCENTUAL_ALICOTA_INTERNA_DIFAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaInternaDifal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaFilialEmissoraInternaDifal", Column = "PED_PERCENTUAL_ALICOTA_FILIAL_EMISSORA_INTERNA_DIFAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaFilialEmissoraInternaDifal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PED_PESO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pallet", Column = "PED_PALLET", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Pallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquido", Column = "PED_PESO_LIQUIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioDaCarga", Column = "PED_INICIO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CTesEmitidos", Column = "PED_CTES_EMITIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CTesEmitidos { get; set; }

        //Quando for Filial emissora este campo indica que o CTe de subcontratacao da filial emissora foi emitido
        [NHibernate.Mapping.Attributes.Property(0, Name = "CTesFilialEmissoraEmitidos", Column = "PED_CTES_FILIAL_EMISSORA_EMITIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CTesFilialEmissoraEmitidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoEmissao", Column = "PED_SITUACAO_EMISSAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF), Scale = 2, NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF SituacaoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PED_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CienciaDoEnvioDaNotaInformado", Column = "PED_CIENCIA_ENVIO_NOTA_INFORMADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CienciaDoEnvioDaNotaInformado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_INTRAMUNICIPAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalIntramunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiCTe", Column = "PED_POSSUI_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiNFS", Column = "PED_POSSUI_NFS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteGerarCargaDistribuidor", Column = "PED_PENDENTE_GERAR_CARGA_DISTRIBUIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteGerarCargaDistribuidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PERCENTUAL_PAGAMENTO_AGREGADO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemColeta", Column = "PED_ORDEM_COLETA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemEntrega", Column = "PED_ORDEM_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemEntrega { get; set; }

        #region Valores Filial Emissora

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaPedidoFilialEmissora", Column = "PED_CARGA_PEDIDO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPedidoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirComplementarFilialEmissora", Column = "PED_EMITIR_COMPLEMENTAR_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirComplementarFilialEmissora { get; set; }

        /// <summary>
        /// quando a carga tem recebedor e filial emissora indica que no próximo trecho a carga será gerado um complemeto do filial emissora e não será necessário um novo cte, ou seja a próxima carga será um redespacho e não uma subcontratação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ProximoTrechoComplementaFilialEmissora", Column = "PED_PROXIMO_TRECHO_COMPLEMENTA_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProximoTrechoComplementaFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteAPagarFilialEmissora", Column = "PED_VALOR_FRETE_A_PAGAR_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteAPagarFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteFilialEmissora", Column = "PED_VALOR_FRETE_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSFilialEmissora", Column = "PED_VALOR_ICMS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PERCENTUAL_CREDITO_PRESUMIDO_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualCreditoPresumidoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_CREDITO_PRESUMIDO_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCreditoPresumidoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSFilialEmissora", Column = "PED_BASE_CALCULO_ICMS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBCFilialEmissora", Column = "PED_PERCENTUAL_REDUCAO_BC_FILIAL_EMISSOR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoBCFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO_FILIAL_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOPFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTFilialEmissora", Column = "PED_CST_FILIAL_EMISSORA", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSBaseCalculoFilialEmissora", Column = "PED_INCLUIR_ICMS_BASE_CALCULO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMSBaseCalculoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualIncluirBaseCalculoFilialEmissora", Column = "PED_PERCENTUAL_INCLUIR_BASE_CALCULO_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualIncluirBaseCalculoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRegraICMSCTeFilialEmissora", Column = "PED_OBSERVACAO_REGRA_ICMS_CTE_FILIAL_EMISSORA", TypeType = typeof(string), NotNull = false, Length = 400)]
        public virtual string ObservacaoRegraICMSCTeFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaFilialEmissora", Column = "PED_PERCENTUAL_ALICOTA_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ICMSPagoPorSTFilialEmissora", Column = "PED_ICMS_PAGO_POR_ST_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ICMSPagoPorSTFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_ESCRITURACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_ICMS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_PIS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoPIS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_COFINS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_MAXIMO_CENTRO_CONTABILIZACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximoCentroContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta ContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ELEMENTO_PEP", TypeType = typeof(string), NotNull = false, Length = 24)]
        public virtual string ElementoPEP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemServico", Column = "PED_ITEM_SERVICO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ItemServico { get; set; }

        #endregion


        /// <summary>
        /// As notas de serviços serão emitidas e controladas separadas da carga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiNFSManual", Column = "PED_POSSUI_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoSemNFe", Column = "PED_PEDIDO_SEM_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoSemNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CTeEmitidoNoEmbarcador", Column = "PED_CTE_EMITIDO_NO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CTeEmitidoNoEmbarcador { get; set; }

        /// <summary>
        /// Indica com qual pedido anterior estava vinculado.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "PED_CODIGO_PEDIDO_ENCAIXADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedidoEncaixe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoEncaixado", Column = "PED_PEDIDO_ENXAIXADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoEncaixado { get; set; }

        /// <summary>
        /// Indica com qual pedido anterior estava vinculado.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "PED_CODIGO_TRECHO_ANTERIOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedidoTrechoAnterior { get; set; }

        /// <summary>
        /// Indica com qual é o próximo trecho vinculado ao pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "PED_CODIGO_PROXIMO_TRECHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedidoProximoTrecho { get; set; }

        /// <summary>
        /// É um redespacho de um pedido anterior seta essa flag.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Redespacho", Column = "PED_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Redespacho { get; set; }

        /// <summary>
        /// É um redespacho de um pedido anterior seta essa flag.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "AgValorRedespacho", Column = "PED_AG_VALOR_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgValorRedespacho { get; set; }

        /// <summary>
        /// Determina se a próxima etapa do fluxo de pátio está liberada
        /// Quando todos os pedidos estiverem liberados, avança o fluxo de pátio para a etapa seguinte
        /// Controle utilizado somente para o método InformarEtapaFluxoPatioPorPedido do web service JanelaCarregamento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ProximaEtapaFluxoGestaoPatioLiberada", Column = "PED_PROXIMA_ETAPA_FLUXO_GESTAO_PATIO_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProximaEtapaFluxoGestaoPatioLiberada { get; set; }


        /// <summary>
        /// indica que é um pedido de reentrega
        /// </summary>

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReentregaSolicitada", Column = "PED_REENTREGA_SOLICITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReentregaSolicitada { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarICMSDoValorAReceber", Column = "PED_DESCONTAR_ICMS_ST_QUANDO_ICMS_NAO_INCLUSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarICMSDoValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoReduzirRetencaoICMSDoValorDaPrestacao", Column = "PED_NAO_REDUZIR_RETENCAO_ICMS_DO_VALOR_DA_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoReduzirRetencaoICMSDoValorDaPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaPedidoRotas", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_ROTAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoRotas", Column = "CPR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas> CargaPedidoRotas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ApoliceSeguroAverbacao", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ApoliceSeguroAverbacao", Column = "CPA_CODIGO")]
        public virtual ICollection<Seguros.ApoliceSeguroAverbacao> ApoliceSeguroAverbacao { get; set; }

        [Obsolete("Utilizar a lista RotasFretes.")]
        [NHibernate.Mapping.Attributes.Set(0, Name = "RotaFretes", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_ROTAS_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFrete> RotaFretes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidoCTesParaSubContratacao", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_CTE_PARA_SUB_CONTRATACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoCTeParaSubContratacao", Column = "PSC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> PedidoCTesParaSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscais", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_XML_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> NotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaPedidoXMLNotasFiscaisParcial", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_XML_NOTA_FISCAL_PARCIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoXMLNotaFiscalParcial", Column = "CFP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> CargaPedidoXMLNotasFiscaisParcial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaPedidoDocumentosCTe", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_DOCUMENTO_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoDocumentoCTe", Column = "CDC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> CargaPedidoDocumentosCTe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaPedidoDocumentosMDFe", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_DOCUMENTO_MDFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoDocumentoMDFe", Column = "CPM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> CargaPedidoDocumentosMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RotasFretes", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_ROTA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoRotaFrete", Column = "CPF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> RotasFretes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Produtos", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoProduto", Column = "CPP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> Produtos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataChegada", Column = "PED_DATA_CHEGADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSaida", Column = "PED_DATA_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaida { get; set; }

        /// <summary>
        /// NO MOMENTO USADO PELA INTEGRACAO ORTEC.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioJanelaDescarga", Column = "PED_DATA_INICIO_JANELA_DESCARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? InicioJanelaDescarga { get; set; }

        /// <summary>
        /// NO MOMENTO USADO PELA INTEGRACAO ORTEC.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "FimJanelaDescarga", Column = "PED_DATA_FIM_JANELA_DESCARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? FimJanelaDescarga { get; set; }

        /// <summary>
        /// Armazena a data de previsão de entrega pré definida para a carga pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoEntrega", Column = "PED_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NAO_IMPRIMIR_IMPOSTOS_DACTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImprimirImpostosDACTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NAO_ENVIAR_IMPOSTO_ICMS_NA_EMISSAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarImpostoICMSNaEmissaoCte { get; set; }

        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoMercadoriaDescontar", Column = "PED_PESO_MERCADORIA_DESCONTAR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PesoMercadoriaDescontar { get; set; }

        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMercadoriaDescontar", Column = "PED_VALOR_MERCADORIA_DESCONTAR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorMercadoriaDescontar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO_FILIAL_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFreteFilialEmissora { get; set; }

        //Multimodal
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCobrancaMultimodal", Column = "TBF_TIPO_COBRANCA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal TipoCobrancaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalPropostaMultimodal", Column = "TBF_MODAL_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal ModalPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultimodal", Column = "TBF_TIPO_SERVICO_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal TipoServicoMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropostaMultimodal", Column = "TBF_TIPO_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal TipoPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoDosDestinatario", Column = "TBF_BLOQUEAR_EMISSAO_DESTINATARIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEmissaoDosDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoDeEntidadeSemCadastro", Column = "TBF_BLOQUEAR_EMISSAO_ENTIDADES_SEM_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEmissaoDeEntidadeSemCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaPedidoIntegrada", Column = "PED_CARGA_PEDIDO_INTEGRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPedidoIntegrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_TOTAL_MOEDA_PAGAR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoedaPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtVolumes", Column = "PED_QUANTIDADE_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int QtVolumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloDocumentoEmpresaPropria", Column = "PED_MODELO_DOCUMENTO_EMPRESA_PROPRIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModeloDocumentoEmpresaPropria { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesBloquearEmissaoDosDestinatario", Cascade = "none", Inverse = true, Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_CLIENTE_BLOQUEAR_EMISSAO_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO")]
        public virtual ICollection<Cliente> ClientesBloquearEmissaoDosDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraICMS", Column = "RIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ICMS.RegraICMS RegraICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImpostoValorAgregado", Column = "IVA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Contabeis.ImpostoValorAgregado ImpostoValorAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DISPONIBILIZAR_DOCUMENTO_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_RECEBEU_DADOS_PRE_CALCULO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RecebeuDadosPreCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_FRETE_COM_ICMS_INCLUSO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteComICMSIncluso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Stage", Column = "STA_CODIGO_RELEVANTE_CUSTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Stage StageRelevanteCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoFrete", Column = "PED_CUSTO_FRETE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CustoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorRemessaVenda", Column = "PED_INDICADOR_REMESSA_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorRemessaVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PROTOCOLO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ProtocoloIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPaleteCliente", Column = "CPE_TIPO_PALETE_CLIENTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente? TipoPaleteCliente { get; set; }

        /// <summary>
        /// Contem o peso dos pallets (cadastro em TipoDetalhe.Valor * qtdePallet).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoPallet", Column = "CRP_PESO_PALLET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? PesoPallet { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoPedido", Column = "CTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoPedido.CotacaoPedido CotacaoPedido { get; set; }

        /// <summary>
        /// Flag utitizada para ignorar o Recebedor na emissão de documentos e também para calcular o valor do frete pela origem e destino (Logvett).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NAO_CONSIDERAR_RECEBEDOR_PARA_EMITIR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoConsiderarRecebedorParaEmitirDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSBCFreteProprio", Column = "CPE_INCLUIR_ICMS_BC_FRETE_PROPRIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IncluirICMSBCFreteProprio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPE_FATOR_CUBAGEM_RATEIO_FORMULA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal? FatorCubagemRateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPE_TIPO_USO_FATOR_CUBAGEM_RATEIO_FORMULA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem? TipoUsoFatorCubagemRateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Incoterm", Column = "CPE_INCOTERM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EnumIncotermPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EnumIncotermPedido? Incoterm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TransitoAduaneiro", Column = "CPE_TRANSITO_ADUANEIRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TransitoAduaneiro), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TransitoAduaneiro? TransitoAduaneiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_NOTIFICACAO_CRT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente NotificacaoCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DtaRotaPrazoTransporte", Column = "CPE_DTA_ROTA_PRAZO_TRANSPORTE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string DtaRotaPrazoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoEmbalagem", Column = "MRC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem TipoEmbalagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DetalheMercadoria", Column = "CPE_DETALHE_MERCADORIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string DetalheMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeVolumesNF", Column = "CPE_QUANTIDADE_VOLUMES_NF", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeVolumesNF { get; set; }

        #region Imposto IBS/CBS

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPE_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPE_CODIGO_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string CodigoIndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "CPE_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "CPE_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "CPE_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "CPE_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "CPE_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "CPE_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "CPE_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "CPE_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "CPE_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "CPE_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "CPE_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "CPE_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        #region Imposto para Filial Emissora

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBSFilialEmissora", Column = "CPE_CST_IBSCBS_FILIAL_EMISSORA", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBSFilialEmissora", Column = "CPE_CLASSIFICACAO_TRIBUTARIA_IBSCBS_FILIAL_EMISSORA", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBSFilialEmissora", Column = "CPE_BASE_CALCULO_IBSCBS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadualFilialEmissora", Column = "CPE_ALIQUOTA_IBS_ESTADUAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadualFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadualFilialEmissora", Column = "CPE_PERCENTUAL_REDUCAO_IBS_ESTADUAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadualFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadualFilialEmissora", Column = "CPE_VALOR_IBS_ESTADUAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadualFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipalFilialEmissora", Column = "CPE_ALIQUOTA_IBS_MUNICIPAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipalFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipalFilialEmissora", Column = "CPE_PERCENTUAL_REDUCAO_IBS_MUNICIPAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipalFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipalFilialEmissora", Column = "CPE_VALOR_IBS_MUNICIPAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipalFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBSFilialEmissora", Column = "CPE_ALIQUOTA_CBS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBSFilialEmissora", Column = "CPE_PERCENTUAL_REDUCAO_CBS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBSFilialEmissora", Column = "CPE_VALOR_CBS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBSFilialEmissora { get; set; }

        #endregion Imposto para Filial Emissora

        #endregion Imposto CBS/IBS

        #region Propriedades com Regras

        public virtual void SetarRegraICMS(int codigoRegraICMS)
        {
            if (codigoRegraICMS > 0)
                RegraICMS = new Dominio.Entidades.Embarcador.ICMS.RegraICMS() { Codigo = codigoRegraICMS };
            else
                RegraICMS = null;
        }

        public virtual void SetarRegraOutraAliquota(int codigoOutraAliquota)
        {
            OutrasAliquotas = codigoOutraAliquota > 0 ? new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas() { Codigo = codigoOutraAliquota } : null;
        }

        public virtual Dominio.Entidades.Cliente ClienteEntrega
        {
            get
            {
                var isRecebedor = (Recebedor != null) && ((TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor) ||
                                                          (TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor));
                if (isRecebedor)
                    return Recebedor;

                return Pedido?.Destinatario;
            }
        }

        public virtual Dominio.Entidades.Cliente ClienteColeta
        {
            get
            {
                var isExpedidor = (Expedidor != null) && ((TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor) ||
                                                          (TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor));
                if (isExpedidor)
                    return Expedidor;

                return Pedido?.Remetente;
            }
        }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido Clonar(bool setarPropriedadesPadrao = true)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = (Dominio.Entidades.Embarcador.Cargas.CargaPedido)this.MemberwiseClone();

            if (setarPropriedadesPadrao)
            {
                cargaPedido.CTesEmitidos = false;
                cargaPedido.CTesFilialEmissoraEmitidos = false;
                cargaPedido.IndicadorCTeGlobalizadoDestinatario = false;
                cargaPedido.IndicadorNFSGlobalizado = false;
                cargaPedido.OrdemColeta = 0;
                cargaPedido.OrdemEntrega = 0;
            }

            return cargaPedido;
        }

        public virtual decimal ValorTotalAReceberComICMSeISSFilialEmissora
        {
            get
            {
                if (this.CST != "60")
                    return this.ValorFreteAPagarFilialEmissora; //+ this.ValorISS + (this.IncluirICMSBaseCalculo ? this.ValorICMS * (this.PercentualIncluirBaseCalculo / 100) : this.ValorICMS);
                else
                    return this.ValorFreteAPagarFilialEmissora;// + this.ValorISS;
            }
        }

        public virtual decimal ValorTotalAReceberComICMSeISS
        {
            get
            {
                if (this.CST != "60")
                    return this.ValorFreteAPagar; //+ this.ValorISS + (this.IncluirICMSBaseCalculo ? this.ValorICMS * (this.PercentualIncluirBaseCalculo / 100) : this.ValorICMS);
                else
                    return this.ValorFreteAPagar;// + this.ValorISS;
            }
        }

        public virtual decimal ValorPrestacaoServico
        {
            get
            {
                if (this.CST == "60")
                    return this.ValorFreteAPagar + this.ValorISS + (this.IncluirICMSBaseCalculo ? this.ValorICMS * (this.PercentualIncluirBaseCalculo / 100) : this.ValorICMS);
                else
                    return this.ValorFreteAPagar;

            }
        }

        public virtual string DescricaoTipoPagamentoCIFFOB
        {
            get
            {
                if (TipoTomador == Enumeradores.TipoTomador.Remetente)
                    return "CIF";

                if (TipoTomador == Enumeradores.TipoTomador.Destinatario)
                    return "FOB";

                return "Outro";
            }
        }

        public virtual Dominio.Entidades.Cliente ObterDestinatario()
        {
            if (Recebedor != null)
                return Recebedor;

            return Pedido.Destinatario;
        }

        public virtual Dominio.Entidades.Cliente ObterTomador()
        {
            if (TipoTomador == Enumeradores.TipoTomador.Remetente)
                return Pedido.Remetente;
            else if (TipoTomador == Enumeradores.TipoTomador.Destinatario)
                return Pedido.Destinatario;
            else if (TipoTomador == Enumeradores.TipoTomador.Outros || TipoTomador == Enumeradores.TipoTomador.Tomador)
                return Tomador;
            else if (TipoTomador == Enumeradores.TipoTomador.Recebedor)
                return Recebedor;
            else if (TipoTomador == Enumeradores.TipoTomador.Expedidor)
                return Expedidor;
            else
                return null;
        }

        public virtual string Descricao
        {
            get
            {
                return this.Carga.CodigoCargaEmbarcador + " - " + this.Pedido.Numero.ToString();
            }
        }

        public virtual bool Equals(CargaPedido other)
        {
            if (other.Codigo == this.Codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual Dominio.Entidades.Cliente ObterRecebedorParaEmissao()
        {
            if (NaoConsiderarRecebedorParaEmitirDocumentos)
                return null;

            return Recebedor;
        }

        public virtual Dominio.Entidades.Localidade ObterDestinoParaEmissao()
        {
            if (NaoConsiderarRecebedorParaEmitirDocumentos)
                return Pedido.Destinatario.Localidade;

            return Destino;
        }

        #endregion
    }
}

