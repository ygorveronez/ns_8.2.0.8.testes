using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_DOCUMENTO_ENTRADA", EntityName = "DocumentoEntradaTMS", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS", NameType = typeof(DocumentoEntradaTMS))]
    public class DocumentoEntradaTMS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EspecieDocumentoFiscal", Column = "EDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EspecieDocumentoFiscal Especie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal Modelo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SituacaoLancamentoDocumentoEntrada", Column = "SLC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SituacaoLancamentoDocumentoEntrada SituacaoLancamentoDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemCompra", Column = "ORC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Compras.OrdemCompra OrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "TDE_CHAVE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLancamento", Column = "TDE_NUMERO_LANCAMENTO", TypeType = typeof(int), NotNull = true, Unique = true)]
        public virtual int NumeroLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrada", Column = "TDE_DATA_ENTRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "TDE_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOLD", Column = "TDE_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroOLD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "TDE_NUMERO_LONG", TypeType = typeof(Int64), NotNull = false)]
        public virtual Int64 Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "TDE_SERIE", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "TDE_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "TDE_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalICMS", Column = "TDE_VALOR_TOTAL_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSST", Column = "TDE_BASE_CALCULO_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalICMSST", Column = "TDE_VALOR_TOTAL_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalIPI", Column = "TDE_VALOR_TOTAL_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalPIS", Column = "TDE_VALOR_TOTAL_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalCOFINS", Column = "TDE_VALOR_TOTAL_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalCreditoPresumido", Column = "TDE_VALOR_TOTAL_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDiferencial", Column = "TDE_VALOR_TOTAL_DIFERENCIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDiferencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDesconto", Column = "TDE_VALOR_TOTAL_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalOutrasDespesas", Column = "TDE_VALOR_TOTAL_OUTRAS_DESPESAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalOutrasDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalFrete", Column = "TDE_VALOR_TOTAL_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalSeguro", Column = "TDE_VALOR_TOTAL_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalFreteFora", Column = "TDE_VALOR_TOTAL_FRETE_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalFreteFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalOutrasDespesasFora", Column = "TDE_VALOR_TOTAL_OUTRAS_DESPESAS_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalOutrasDespesasFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDescontoFora", Column = "TDE_VALOR_TOTAL_DESCONTO_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDescontoFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalImpostosFora", Column = "TDE_VALOR_TOTAL_IMPOSTOS_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalImpostosFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDiferencialFreteFora", Column = "TDE_VALOR_TOTAL_DIFERENCIAL_FRETE_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDiferencialFreteFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalICMSFreteFora", Column = "TDE_VALOR_TOTAL_ICMS_FRETE_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalICMSFreteFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalCusto", Column = "TDE_VALOR_TOTAL_CUSTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_TOTAL_RETENCAO_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRetencaoPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_TOTAL_RETENCAO_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRetencaoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_TOTAL_RETENCAO_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRetencaoINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_TOTAL_RETENCAO_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRetencaoIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_TOTAL_RETENCAO_CSLL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRetencaoCSLL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_TOTAL_RETENCAO_OUTRAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRetencaoOutras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_TOTAL_RETENCAO_IR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRetencaoIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_TOTAL_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_PRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_BRUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorPagamento", Column = "TDE_INDICADOR_PAGAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada IndicadorPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "TDE_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoEmEBS", Column = "TDE_DOCUMENTO_EM_EBS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DocumentoEmEBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAbastecimento", Column = "TDE_DATA_ABASTECIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMAbastecimento", Column = "TDE_KM_ABASTECIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int KMAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_BASE_ST_RETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_ST_RETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacao", Column = "TDE_DATA_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EncerrarOrdemServico", Column = "TDE_ENCERRAR_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EncerrarOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Horimetro", Column = "TDE_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int Horimetro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Equipamento Equipamento { get; set; }

        /// <summary>
        /// Utilizada para controlar a sumarização dos dados para análise de resultados
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "TDE_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "TDE_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "TDE_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNotaAnulacao", Column = "TDE_CHAVE_NOTA_ANULACAO", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveNotaAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TDE_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "TDE_OPERADOR_LANCAMENTO_DOCUMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario OperadorLancamentoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "TDE_OPERADOR_FINALIZA_DOCUMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario OperadorFinalizaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "TDE_EXPEDIDOR_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "TDE_RECEBEDOR_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFinanciamento", Column = "CFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.ContratoFinanciamento ContratoFinanciamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFrete", Column = "TDE_TIPO_FRETE", TypeType = typeof(Dominio.Enumeradores.ModalidadeFrete), NotNull = false)]
        public virtual Dominio.Enumeradores.ModalidadeFrete? TipoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_INICIO_PRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeInicioPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_TERMINO_PRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeTerminoPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoFinalizadoAutomaticamente", Column = "TDE_DOCUMENTO_FINALIZADO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoFinalizadoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "TDE_MOTIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Servico", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal.Servico Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_PRESTACAO_SERVICO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadePrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "TDE_TIPO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoServico), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoServico? TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTServico", Column = "TDE_CST_SERVICO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTServico), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTServico? CSTServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaSimplesNacional", Column = "TDE_ALIQUOTA_SIMPLES_NACIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaSimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoFiscalProvenienteSimplesNacional", Column = "TDE_DOCUMENTO_FISCAL_PROVENIENTE_SIMPLES_NACIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoFiscalProvenienteSimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TributaISSNoMunicipio", Column = "TDE_TRIBUTA_ISS_NO_MUNICIPIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TributaISSNoMunicipio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "TDE_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoImportadoXML", Column = "TDE_XML_DOCUMENTO_IMPORTADO", Type = "StringClob", NotNull = false)]
        public virtual string DocumentoImportadoXML { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Itens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TMS_DOCUMENTO_ENTRADA_ITEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoEntradaItem", Column = "TDI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> Itens { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Duplicatas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TMS_DOCUMENTO_ENTRADA_DUPLICATA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoEntradaDuplicata", Column = "TDD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata> Duplicatas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Guias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TMS_DOCUMENTO_ENTRADA_GUIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoEntradaGuia", Column = "TDG_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia> Guias { get; set; }

        [Obsolete("Utilizar LancamentosCentroResultado.")]
        [NHibernate.Mapping.Attributes.Bag(0, Name = "CentrosResultados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoEntradaCentroResultado", Column = "TDC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultado> CentrosResultados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LancamentosCentroResultado", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_RESULTADO_LANCAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LancamentoCentroResultado", Column = "LCR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> LancamentosCentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Veiculos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TMS_DOCUMENTO_ENTRADA_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Veiculos { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "BasePIS", Formula = @"(SELECT ISNULL(SUM(I.TDI_BASE_CALCULO_PIS), 0)
                                                                                    FROM T_TMS_DOCUMENTO_ENTRADA_ITEM I
                                                                                    WHERE I.TDE_CODIGO = TDE_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal BasePIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCOFINS", Formula = @"(SELECT ISNULL(SUM(I.TDI_BASE_CALCULO_COFINS), 0)
                                                                                    FROM T_TMS_DOCUMENTO_ENTRADA_ITEM I
                                                                                    WHERE I.TDE_CODIGO = TDE_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal BaseCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseIPI", Formula = @"(SELECT ISNULL(SUM(I.TDI_BASE_CALCULO_IPI), 0)
                                                                                    FROM T_TMS_DOCUMENTO_ENTRADA_ITEM I
                                                                                    WHERE I.TDE_CODIGO = TDE_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal BaseIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCSLL", Formula = @"(SELECT ISNULL(SUM(I.TDI_VALOR_TOTAL), 0)
                                                                                    FROM T_TMS_DOCUMENTO_ENTRADA_ITEM I
                                                                                    WHERE I.TDE_VALOR_RETENCAO_CSLL > 0 AND I.TDE_CODIGO = TDE_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal BaseCSLL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseISS", Formula = @"(SELECT ISNULL(SUM(I.TDI_VALOR_TOTAL), 0)
                                                                                    FROM T_TMS_DOCUMENTO_ENTRADA_ITEM I
                                                                                    WHERE I.TDI_VALOR_RETENCAO_INSS > 0 AND I.TDE_CODIGO = TDE_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal BaseISS { get; set; }

        /// <summary>
        /// Consulta baixa de título em até 2 negociações a partir do título original para saber o status
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusFinanceiro", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + 
                                                                                    CASE 
	                                                                                    WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
	                                                                                    WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
		                                                                                    CASE
			                                                                                    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.TDD_CODIGO = T.TDD_CODIGO AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                                                                                                WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
                                                                                                
                                                                                                WHEN ISNULL(
                                                                                                            (SELECT COUNT(1)
                                                                                                            FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                                                            JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                                                            WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4
                                                                                                                AND TBN.TIB_CODIGO IN
                                                                                                                (SELECT tituloBaixa.TIB_CODIGO
                                                                                                                    FROM T_TITULO_BAIXA tituloBaixa
                                                                                                                    JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                                                    WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)), 0) >= 1 THEN 'Renegociado'
                                                                                                WHEN ISNULL(
                                                                                                            (SELECT COUNT(1)
                                                                                                            FROM T_TITULO_BAIXA_NEGOCIACAO TBNN
                                                                                                            JOIN T_TITULO TTT ON TTT.TBN_CODIGO = TBNN.TBN_CODIGO
                                                                                                            WHERE TTT.TIT_STATUS <> 3 AND TTT.TIT_STATUS <> 4
                                                                                                                AND TBNN.TIB_CODIGO IN
                                                                                                                (SELECT tituloBaixa2.TIB_CODIGO
                                                                                                                    FROM T_TITULO_BAIXA tituloBaixa2
                                                                                                                    JOIN T_TITULO_BAIXA_AGRUPADO TBAA ON TBAA.TIB_CODIGO = tituloBaixa2.TIB_CODIGO
                                                                                                                    WHERE tituloBaixa2.TIB_SITUACAO <> 4
                                                                                                                    AND TBAA.TIT_CODIGO IN
                                                                                                                        (SELECT TT.TIT_CODIGO
                                                                                                                        FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                                                                        JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                                                                        WHERE TBN.TIB_CODIGO IN
                                                                                                                            (SELECT tituloBaixa.TIB_CODIGO
                                                                                                                            FROM T_TITULO_BAIXA tituloBaixa
                                                                                                                            JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                                                            WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)))), 0) >= 1 THEN 'Renegociado'

			                                                                                    ELSE 'Pago' 
		                                                                                    END
	                                                                                    ELSE 'Pago' 
                                                                                    END
                                                                                    FROM T_TITULO T
                                                                                    JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = T.TDD_CODIGO
                                                                                    WHERE T.TIT_STATUS <> 4 AND D.TDE_CODIGO = TDE_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string StatusFinanceiro { get; set; }

        public virtual string Descricao
        {
            get { return this.NumeroLancamento.ToString(); }
        }

        #endregion
    }
}
