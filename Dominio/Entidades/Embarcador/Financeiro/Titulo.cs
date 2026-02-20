using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO", EntityName = "Titulo", Name = "Dominio.Entidades.Embarcador.Financeiro.Titulo", NameType = typeof(Titulo))]
    public class Titulo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.Titulo>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaDuplicata", Column = "TDD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata DuplicataDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaGuia", Column = "TDG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia DocumentoEntradaGuia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.ContratoFrete ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturaParcela", Column = "FAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.FaturaParcela FaturaParcela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturaCargaDocumento", Column = "FCD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento FaturaCargaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturaDocumento", Column = "FDO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.FaturaDocumento FaturaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixaNegociacao", Column = "TBN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao TituloBaixaNegociacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagamento", Column = "PAG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Pagamento Pagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "TIT_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginal", Column = "TIT_VALOR_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPendente", Column = "TIT_VALOR_PENDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPendente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPago", Column = "TIT_VALOR_PAGO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "TIT_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Acrescimo", Column = "TIT_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Acrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "TIT_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "TIT_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiquidacao", Column = "TIT_DATA_LIQUIDACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiquidacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAutorizacao", Column = "TIT_DATA_AUTORIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_DATA_BASE_LIQUIDACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseLiquidacao { get; set; }

        /// <summary>
        /// Utilizada para controlar a sumarização dos dados para análise de resultados
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TIT_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoInterna", Column = "TIT_OBSERVACAO_INTERNA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ObservacaoInterna { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Historico", Column = "TIT_HISTORICO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Historico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTitulo", Column = "TIT_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo StatusTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTitulo", Column = "TIT_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo TipoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoRemessa", Column = "BRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BoletoRemessa BoletoRemessa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NossoNumero", Column = "TIT_NOSSO_NUMERO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NossoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_LINHA_DIGITAVEL_BOLETO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LinhaDigitavelBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BoletoStatusTitulo", Column = "TIT_STATUS_BOLETO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo BoletoStatusTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoBoleto", Column = "TIT_CAMINHO_BOLETO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CaminhoBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoRetornoGeracaoBoleto", Column = "TIT_ULTIMO_RETORNO_GERACAO_BOLETO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UltimoRetornoGeracaoBoleto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico ConhecimentoDeTransporteEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadoPagamento", Column = "TIT_LIBERADO_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTituloOriginal", Column = "TIT_VALOR_TITULO_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTituloOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoTituloOriginal", Column = "TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TipoDocumentoTituloOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumentoTituloOriginal", Column = "TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string NumeroDocumentoTituloOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "TIT_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeParcela", Column = "CPA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeParcela CTeParcela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bordero", Column = "BOR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Bordero Bordero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CobrancaManual", Column = "CMA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CobrancaManual CobrancaManual { get; set; }

        /// <summary>
        /// Valor a ser cobrado/pago no título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_VALOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Valor { get; set; }

        /// <summary>
        /// Valor do desconto concedido na geração do título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_VALOR_DESCONTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        /// <summary>
        /// Valor do acréscimo concedido na geração do título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_VALOR_ACRESCIMO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAcrescimo { get; set; }

        /// <summary>
        /// Valor + ValorAcrescimo - ValorDesconto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_VALOR_TOTAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        /// <summary>
        /// Valor do desconto concedido na baixa do título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_VALOR_DESCONTO_BAIXA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDescontoBaixa { get; set; }

        /// <summary>
        /// Valor do acréscimo concedido na baixa do título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_VALOR_ACRESCIMO_BAIXA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAcrescimoBaixa { get; set; }

        /// <summary>
        /// Campo utilizado para a integração com a MARFRIG
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFatura", Column = "TIT_NUMERO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPagamento", Column = "TIT_NUMERO_PAGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_MODELO_ANTIGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModeloAntigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_PORTADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Portador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_ADIANTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Adiantado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramacaoPagamento", Column = "TIT_DATA_PROGRAMACAO_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProgramacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TituloDocumento", Column = "TDO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Borderos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BORDERO_TITULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BorderoTitulo", Column = "BOT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo> Borderos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Baixas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_BAIXA_AGRUPADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TituloBaixaAgrupado", Column = "TIA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> Baixas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LancamentosCentroResultado", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_RESULTADO_LANCAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LancamentoCentroResultado", Column = "LCR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> LancamentosCentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Veiculos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TituloAnexo", Column = "TFA_CODIGO")]
        public virtual IList<TituloAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAmbiente", Column = "TIT_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAmbiente), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTitulo", Column = "TIT_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo FormaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Provisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_ADIANTAMENTO_FORNECEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdiantamentoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoReceitaTributo", Column = "TIT_CODIGO_RECEITA_TRIBUTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoReceitaTributo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIdentificacaoTributo", Column = "TIT_CODIGO_IDENTIFICACAO_TRIBUTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoIdentificacaoTributo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_CONTRIBUINTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Contribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoApuracao", Column = "TIT_PERIODO_APURACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PeriodoApuracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TributoTipoDocumento", Column = "TTD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Tributo.TributoTipoDocumento TributoTipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TributoCodigoReceita", Column = "TCR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Tributo.TributoCodigoReceita TributoCodigoReceita { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TributoTipoImposto", Column = "TTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Tributo.TributoTipoImposto TributoTipoImposto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TributoVariacaoImposto", Column = "TVI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Tributo.TributoVariacaoImposto TributoVariacaoImposto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TributoReferencia", Column = "TIT_TRIBUTO_REFERENCIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TributoReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "TIT_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "TIT_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "TIT_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "TIT_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_BOLETO_GERADO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BoletoGeradoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_BOLETO_ENVIADO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BoletoEnviadoPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_ENVIAR_DOCUMENTACAO_FATURAMENTO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDocumentacaoFaturamentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFavorecido", Column = "TIT_CODIGO_FAVORECIDO", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CodigoFavorecido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Abastecimento", Column = "ABA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Abastecimento Abastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotorista.PagamentoMotoristaTMS PagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontoAplicadoNegociacao", Column = "TIT_DESCONTO_APLICADO_NEGOCIACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DescontoAplicadoNegociacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SinistroParcela", Column = "SPA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.SinistroParcela SinistroParcela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "TIT_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoQuitacao", Column = "TIT_INTEGRADO_QUITACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoQuitacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JustificativaCancelamentoFinanceiro", Column = "JCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro JustificativaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIT_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRecebidoIntegracao", Column = "TIT_CODIGO_RECEBIDO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? CodigoRecebidoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoERP", Column = "TIT_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioEmailAvisoVencimento", Column = "TIT_DATA_ENVIO_EMAIL_AVISO_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioEmailAvisoVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioEmailCobranca", Column = "TIT_DATA_ENVIO_EMAIL_COBRANCA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioEmailCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoBoletoIntegracao", Column = "TIT_CAMINHO_BOLETO_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CaminhoBoletoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarrasBoleto", Column = "TIT_CODIGO_BARRAS_BOLETO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoBarrasBoleto { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCargas", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CAR_CODIGO_CARGA_EMBARCADOR AS NVARCHAR(20))
	                                                                                    FROM T_TITULO T
                                                                                        JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO
                                                                                        JOIN T_FATURA_CARGA D ON D.FAT_CODIGO = FP.FAT_CODIGO AND D.FAC_STATUS <> 3
                                                                                        JOIN T_CARGA C ON C.CAR_CODIGO = D.CAR_CODIGO
	                                                                                    WHERE T.TIT_CODIGO = TIT_CODIGO AND T.TIT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Formula = @"SUBSTRING((SELECT ', ' + (CASE TituloDocumento.TDO_TIPO_DOCUMENTO WHEN 1 THEN CONVERT(NVARCHAR(15), CTe.CON_NUM) + '-' + CONVERT(NVARCHAR(15), SerieCTe.ESE_NUMERO) WHEN 2 THEN Carga.CAR_CODIGO_CARGA_EMBARCADOR ELSE '' END)
                                                               FROM T_TITULO_DOCUMENTO TituloDocumento
                                                               LEFT OUTER JOIN T_CARGA Carga ON TituloDocumento.TDO_TIPO_DOCUMENTO = 2 AND Carga.CAR_CODIGO = TituloDocumento.CAR_CODIGO
                                                               LEFT OUTER JOIN T_CTE CTe ON TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND CTe.CON_CODIGO = TituloDocumento.CON_CODIGO
                                                               LEFT OUTER JOIN T_EMPRESA_SERIE SerieCTe ON CTe.CON_SERIE = SerieCTe.ESE_CODIGO WHERE TituloDocumento.TIT_CODIGO = TIT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConhecimentos", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CON_NUM AS NVARCHAR(20))
                                                                                        FROM T_TITULO T
	                                                                                    JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO
                                                                                        JOIN T_FATURA_CARGA_DOCUMENTO D ON D.FAT_CODIGO = FP.FAT_CODIGO AND D.FCD_STATUS_DOCUMENTO = 1
                                                                                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                                                                                        WHERE T.TIT_CODIGO = TIT_CODIGO AND T.TIT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroConhecimentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPagamentoDigital", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(P.PAE_NUMERO AS NVARCHAR(20))
	                                                                                     FROM T_PAGAMENTO_ELETRONICO_TITULO TT
                                                                                         JOIN T_PAGAMENTO_ELETRONICO P ON P.PAE_CODIGO = TT.PAE_CODIGO
                                                                                         WHERE TT.TIT_CODIGO = TIT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroPagamentoDigital { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiPagamentoDigital", Formula = @"CASE WHEN
	                                                                                                ISNULL((SELECT COUNT(*) FROM T_PAGAMENTO_ELETRONICO Pagamento
                                                                                                    JOIN T_PAGAMENTO_ELETRONICO_TITULO Titulo on Titulo.PAE_CODIGO = Pagamento.PAE_CODIGO
                                                                                                    WHERE Titulo.TIT_CODIGO = TIT_CODIGO), 0) > 0
                                                                                                THEN 1 ELSE 0 END", TypeType = typeof(bool), Lazy = true)]
        public virtual bool PossuiPagamentoDigital { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CentrosResultado", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + (CR.CRE_DESCRICAO)
	                                                                                    FROM T_CENTRO_RESULTADO CR
                                                                                        JOIN T_CENTRO_RESULTADO_LANCAMENTO CRT ON CR.CRE_CODIGO = CRT.CRE_CODIGO
                                                                                        JOIN T_TITULO T                        ON T.TIT_CODIGO = CRT.TIT_CODIGO
                                                                                        WHERE T.TIT_CODIGO = TIT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string CentrosResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PortalClienteCodigo", Column = "TIT_PORTAL_CLIENTE_CODIGO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string PortalClienteCodigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaPaga", Column = "TIT_SEQUENCIA_PAGA", TypeType = typeof(int), NotNull = false)]
        public virtual int SequenciaPaga { get; set; }

        public virtual string DescricaoSituacao
        {
            get { return StatusTitulo.ObterDescricao(); }
        }

        public virtual string DescricaoTipo
        {
            get { return TipoTitulo.ObterDescricao(); }
        }

        public virtual string DescricaoTomador
        {
            get
            {
                string descricao = string.Empty;

                if (Pessoa != null)
                    descricao += Pessoa.Nome + " (" + Pessoa.CPF_CNPJ_Formatado + ")";
                else if (GrupoPessoas != null)
                    descricao += GrupoPessoas.Descricao;

                return descricao;
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString() + " (R$ " + this.ValorOriginal.ToString("n2") + ")";
            }
        }

        public virtual decimal Saldo
        {
            get
            {
                if (this.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                    return 0;
                else
                    return this.ValorOriginal - this.Desconto + this.Acrescimo;
            }
        }

        public virtual decimal ValorTotalCalculado
        {
            get
            {
                return this.ValorOriginal - this.ValorPago - this.Desconto + this.Acrescimo;
            }
        }

        public virtual Dominio.Entidades.Cliente ContribuinteTributo
        {
            get
            {
                if (this.Contribuinte != null)
                    return this.Contribuinte;
                else if (this.Portador != null)
                    return this.Portador;
                else if (this.Pessoa != null && this.Pessoa.ClientePortadorConta != null)
                    return this.Pessoa.ClientePortadorConta;
                else if (this.Pessoa != null)
                    return this.Pessoa;
                else
                    return null;
            }
        }

        public virtual Dominio.Entidades.Cliente Fornecedor
        {
            get
            {
                if (this.Portador != null)
                    return this.Portador;
                else if (this.Pessoa != null && this.Pessoa.ClientePortadorConta != null)
                    return this.Pessoa.ClientePortadorConta;
                else if (this.Pessoa != null)
                    return this.Pessoa;
                else
                    return null;
            }
        }

        public virtual bool Equals(Titulo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}
