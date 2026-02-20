using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TERCEIRO", EntityName = "ContratoFrete", Name = "Dominio.Entidades.Embarcador.Terceiros.ContratoFrete", NameType = typeof(ContratoFrete))]
    public class ContratoFrete : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>
    {
        public ContratoFrete() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoAgregado", Column = "PAA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado PagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Transbordo", Column = "TRB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Transbordo Transbordo { get; set; }

        /// <summary>
        /// Valor de frete há ser pago pela subcontratação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteSubcontratacao", Column = "CFT_VALOR_FRETE_SUB_CONTRATACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContrato", Column = "CFT_NUMERO_CONTRATO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroContrato { get; set; }

        /// <summary>
        /// Valor de frete de subContratação Calculado Pela Tabela De Frete
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteSubContratacaoTabelaFrete", Column = "CFT_VALOR_FRETE_SUB_CONTRATACAO_TABELA_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteSubContratacaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        /// <summary>
        /// Valor de frete incluindo o Pedágio na subcontratação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "CFT_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO_VALE_PEDAGIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCobradoDoTerceiro", Column = "CFT_PERCENTUAL_COBRADO_DO_TERCEIRO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PercentualCobradoDoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAdiantamento", Column = "CFT_PERCENTUAL_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "CFT_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_PERCENTUAL_ABASTECIMENTO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ABASTECIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAbastecimento { get; set; }

        /// <summary>
        /// Outro valor adiantamento ele é descontado do contrato do frete.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutrosAdiantamento", Column = "CFT_OUTROS_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOutrosAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descontos", Column = "CFT_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Descontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFreteEscolhido", Column = "CFT_TIPO_FRETE_ESCOLHIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido TipoFreteEscolhido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoContratoFrete", Column = "CFT_CONTRATO_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete SituacaoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CFT_USUARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoContrato", Column = "CFT_DATA_EMISSAO_CONTRATO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissaoContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DATA_ENCERRAMENTO_CONTRATO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEncerramentoContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CFT_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente TransportadorTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bloqueado", Column = "CFT_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmEncerramentoCIOT", Column = "CFT_EM_ENCERRAMENTO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmEncerramentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaBloqueio", Column = "CFT_JUSTIFICATIVA_BLOQUEIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string JustificativaBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TEXTO_ADICIONAL_CONTRATO_FRETE", TypeType = typeof(string), Length = 100000, NotNull = false)]
        public virtual string TextoAdicionalContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_RETER_IMPOSTOS_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReterImpostosContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DIAS_VENCIMENTO_ADIANTAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasVencimentoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DIAS_VENCIMENTO_SALDO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasVencimentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_BASE_CALCULO_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ALIQUOTA_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_BASE_CALCULO_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ALIQUOTA_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_BASE_CALCULO_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ALIQUOTA_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_BASE_CALCULO_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_BASE_CALCULO_IRRF_SEM_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIRRFSemDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_BASE_CALCULO_IRRF_SEM_ACUMULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIRRFSemAcumulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ALIQUOTA_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_IRRF_SEM_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIRRFSemDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TIPO_GERACAO_TITULO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTituloContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTituloContratoFrete TipoGeracaoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DATA_AUTORIZACAO_PAGAMENTO_ADIANTAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacaoPagamentoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DATA_AUTORIZACAO_PAGAMENTO_SALDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacaoPagamentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DATA_AUTORIZACAO_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DATA_LIBERACAO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TARIFA_TRANSFERENCIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TarifaTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TARIFA_SAQUE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TarifaSaque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ALIQUOTA_INSS_PATRONAL", Scale = 2, Precision = 5, TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal AliquotaINSSPatronal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_INSS_PATRONAL", Scale = 2, Precision = 18, TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorINSSPatronal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_UTILIZAR_DATA_EMISSAO_PARA_MOVIMENTO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataEmissaoParaMovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContratoCIOT", Column = "CFT_TIPO_CONTRATO_CIOT", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContratoCIOT), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContratoCIOT TipoContratoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_NUMERO_CONTROLE", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_CALCULAR_ADIANTAMENTO_COM_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularAdiantamentoComPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ENVIOU_CONTRATO_AX_COM_SUCESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouContratoAXComSucesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ENVIOU_ACERTO_CONTAS_CONTRATO_AX_COM_SUCESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouAcertoContasContratoAXComSucesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_POSSUI_INTEGRACAO_AX", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoAX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "CFT_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoTributaria", Column = "CFT_CODIGO_INTEGRACAO_TRIBUTARIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoIntegracaoTributaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TIPO_PAGAMENTO_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete? TipoPagamentoContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrouValoresAcrescimoDesconto", Column = "CFT_INTEGROU_VALORES_ACRESCIMO_DESCONTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IntegrouValoresAcrescimoDesconto { get; set; }

        /// <summary>
        /// Sumarizador para o valor total de acréscimos no saldo. Não popular este campo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_TOTAL_ACRESCIMO_SALDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalAcrescimoSaldo { get; set; }

        /// <summary>
        /// Sumarizador para o valor total de descontos no saldo. Não popular este campo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_TOTAL_DESCONTO_SALDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDescontoSaldo { get; set; }

        /// <summary>
        /// Sumarizador para o valor total de acréscimos no adiantamento. Não popular este campo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_TOTAL_ACRESCIMO_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalAcrescimoAdiantamento { get; set; }

        /// <summary>
        /// Sumarizador para o valor total de descontos no adiantamento. Não popular este campo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_TOTAL_DESCONTO_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDescontoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_IRRF_PERIODO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIRRFPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_NAO_SOMAR_VALOR_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorPedagio { get; set; }

        /// <summary>
        /// Não deve considerar o desconto no cálculo de impostos do contrato de frete.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_NAO_CONSIDERAR_ACRESCIMO_DESCONTO_CALCULO_IMPOSTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoConsiderarDescontoCalculoImpostos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ALTEROU_OBSERVACAO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterouObservacaoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDependentes", Column = "CFT_QUANTIDADE_DEPENDENTES", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeDependentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_POR_DEPENDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPorDependente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_TOTAL_DEPENDENTES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDependentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCompanhia", Column = "CFT_CODIGO_COMPANHIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCompanhia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CFT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteIntegracaoArquivo", Column = "CFI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ValoresAdicionais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TERCEIRO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteValor", Column = "CFV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> ValoresAdicionais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TERCEIRO_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteCTe", Column = "CTC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe> CTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ContratoAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AprovacaoAlcadaContratoFrete", Column = "AAC_CODIGO")]
        public virtual ICollection<AprovacaoAlcadaContratoFrete> ContratoAutorizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamentoCIOT", Column = "CFT_TIPO_PAGAMENTO_CIOT", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? TipoPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerceiro", Column = "TPT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro TipoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DATA_FIXA_VENCIMENTO_SALDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DataFixaVencimentoSaldo { get; set; }

        public virtual ContratoFrete Clonar()
        {
            return (ContratoFrete)this.MemberwiseClone();
        }

        public virtual bool Equals(ContratoFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                return this.Carga?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoSituacaoContratoFrete
        {
            get
            {
                switch (this.SituacaoContratoFrete)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao:
                        return "Ag Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado:
                        return "Aprovado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Rejeitado:
                        return "Rejeitado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.SemRegra:
                        return "Sem Regra";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoFreteEscolhido
        {
            get
            {
                switch (this.TipoFreteEscolhido)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Leilao:
                        return "Por Leilão";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador:
                        return "Pelo Operador";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela:
                        return "Pela Tabela de Frete do Terceiro";
                    default:
                        return "";
                }


            }
        }

        public virtual decimal ValorSaldoComAdiantamento
        {
            get
            {
                decimal saldo = this.ValorFreteSubcontratacao;

                if (ReterImpostosContratoFrete)
                    saldo = saldo - ValorImpostosReter - Descontos;
                else
                    saldo -= Descontos;

                return saldo;
            }
        }

        public virtual decimal ValorSaldo
        {
            get
            {
                decimal saldo = ValorFreteSubcontratacao;

                if (ReterImpostosContratoFrete)
                    saldo = saldo - ValorImpostosReter - Descontos;

                return saldo - ValorAdiantamento + ValorTotalAcrescimoSaldo - ValorTotalDescontoSaldo;
            }
        }

        public virtual decimal ValorImpostosReter
        {
            get
            {
                return ValorINSS + ValorSENAT + ValorSEST + ValorIRRFReter;
            }
        }

        /// <summary>
        /// #32389
        /// Para os casos em que incide IR e o valor calculado é menor que R$ 10,00, não deve descontar (reter) o valor do terceiro.
        /// Esse valor deverá ficar acumulado para ser descontado (retido) junto com outro contrato.
        /// </summary>
        public virtual decimal ValorIRRFReter
        {
            get
            {
                if (ValorIRRFPeriodo >= 10m)
                    return ValorIRRF;
                else if (ValorIRRFPeriodo < 10m && (ValorIRRFPeriodo + ValorIRRF) >= 10m)
                    return ValorIRRF + ValorIRRFPeriodo;

                return 0m;
            }
        }

        public virtual decimal SaldoAReceber
        {
            get
            {
                return ValorLiquidoSemAdiantamento - ValorAdiantamento + ValorTotalAcrescimoAdiantamento - ValorTotalDescontoAdiantamento - ValorAbastecimento + ValorTotalAcrescimoSaldo - ValorTotalDescontoSaldo;
            }
        }

        public virtual decimal ValorLiquidoSemAdiantamento
        {
            get
            {
                decimal saldo = ValorFreteSubcontratacao - ValorOutrosAdiantamento - TarifaSaque - TarifaTransferencia - Descontos;

                if (TipoIntegracaoValePedagio != null)
                {
                    if (!TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato && !(Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false))
                        saldo -= ValorPedagio;
                }
                else
                {
                    if (!NaoSomarValorPedagio)
                        saldo += ValorPedagio;
                }

                if (ReterImpostosContratoFrete && TransportadorTerceiro != null && TransportadorTerceiro.Tipo == "F")
                    saldo -= ValorImpostosReter;

                return saldo;
            }
        }

        public virtual decimal ValorLiquidoSemAdiantamentoEImpostos
        {
            get
            {
                decimal saldo = ValorFreteSubcontratacao - ValorOutrosAdiantamento - TarifaSaque - TarifaTransferencia - Descontos;

                if (TipoIntegracaoValePedagio != null)
                {
                    if (!TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato && !(Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false))
                        saldo -= ValorPedagio;
                }
                else
                {
                    if (!NaoSomarValorPedagio)
                        saldo += ValorPedagio;
                }

                return saldo;
            }
        }

        public virtual decimal ValorBruto
        {
            get
            {
                decimal valorBruto = ValorFreteSubcontratacao - ValorAbastecimento - TarifaSaque - TarifaTransferencia - Descontos + ValorTotalAcrescimoSaldo - ValorTotalDescontoSaldo + ValorTotalAcrescimoAdiantamento - ValorTotalDescontoAdiantamento;

                if (TipoIntegracaoValePedagio != null)
                {
                    if (!TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato && !(Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false))
                        valorBruto -= ValorPedagio;
                }
                else
                {
                    if (!NaoSomarValorPedagio)
                        valorBruto += ValorPedagio;
                }

                if (ReterImpostosContratoFrete && TransportadorTerceiro != null && TransportadorTerceiro.Tipo == "F")
                    valorBruto -= ValorImpostosReter + TarifaSaque + TarifaTransferencia;
                else
                    valorBruto += ValorINSS + ValorIRRF + ValorSEST + ValorSENAT;

                return valorBruto;
            }
        }

        public virtual decimal ValorBrutoComAcrescimoDescontoSaldo
        {
            get
            {
                decimal valorBruto = ValorFreteSubcontratacao - ValorAbastecimento - TarifaSaque - TarifaTransferencia - Descontos + ValorTotalAcrescimoSaldo - ValorTotalDescontoSaldo;

                if (TipoIntegracaoValePedagio != null)
                {
                    if (!TipoIntegracaoValePedagio.NaoSubtrairValePedagioDoContrato && !(Carga?.TipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false))
                        valorBruto -= ValorPedagio;
                }
                else
                {
                    if (!NaoSomarValorPedagio)
                        valorBruto += ValorPedagio;
                }

                if (ReterImpostosContratoFrete && TransportadorTerceiro != null && TransportadorTerceiro.Tipo == "F")
                    valorBruto -= ValorImpostosReter + TarifaSaque + TarifaTransferencia;
                else
                    valorBruto += ValorINSS + ValorIRRF + ValorSEST + ValorSENAT;

                return valorBruto;
            }
        }
    }
}
