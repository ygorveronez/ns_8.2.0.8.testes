using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_XML_NOTA_FISCAL", DynamicUpdate = true, EntityName = "PedidoXMLNotaFiscal", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal", NameType = typeof(PedidoXMLNotaFiscal))]
    public class PedidoXMLNotaFiscal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_VALOR_FRETE_TABELA_FRETE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PNF_VALOR_FRETE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteAjusteManual", Column = "PNF_VALOR_FRETE_AJUSTE_MANUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteAjusteManual { get; set; }

        /// <summary>
        /// Armazena o valor dos compoentes exceto impostos e frete liquido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalComponentes", Column = "PNF_TOTAL_COMPONENTES", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "PNF_VALOR_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoISS", Column = "PNF_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaISS", Column = "PNF_PERCENTUAL_ALICOTA_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRetencaoISS", Column = "PNF_PERCENTUAL_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirISSBaseCalculo", Column = "PNF_INCLUIR_ISS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirISSBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetencaoISS", Column = "PNF_VALOR_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReterIR", Column = "PNF_RETER_IR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReterIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIR", Column = "PNF_ALIQUOTA_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIR", Column = "PNF_BASE_CALCULO_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIR", Column = "PNF_VALOR_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIR { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "PNF_VALOR_ICMS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCofins", Column = "PNF_VALOR_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPis", Column = "PNF_VALOR_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSIncluso", Column = "PNF_VALOR_ICMS_INCLUSO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSIncluso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_PERCENTUAL_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_VALOR_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiCTe", Column = "PNF_POSSUI_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiNFS", Column = "PNF_POSSUI_NFS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiNFSManual", Column = "PNF_POSSUI_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ICMSPagoPorST", Column = "PNF_ICMS_PAGO_POR_ST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ICMSPagoPorST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquota", Column = "PNF_PERCENTUAL_ALICOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCofins", Column = "PNF_ALICOTA_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPis", Column = "PNF_ALICOTA_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaInternaDifal", Column = "PNF_PERCENTUAL_ALICOTA_INTERNA_DIFAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaInternaDifal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaFilialEmissoraInternaDifal", Column = "PNF_PERCENTUAL_ALICOTA_FILIAL_EMISSORA_INTERNA_DIFAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaFilialEmissoraInternaDifal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "PNF_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "PNF_CST", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSBaseCalculo", Column = "PNF_INCLUIR_ICMS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMSBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualIncluirBaseCalculo", Column = "PNF_PERCENTUAL_INCLUIR_BASE_CALCULO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualIncluirBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBC", Column = "PNF_PERCENTUAL_REDUCAO_BC", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRegraICMSCTe", Column = "PNF_OBSERVACAO_REGRA_ICMS_CTE", TypeType = typeof(string), NotNull = false, Length = 400)]
        public virtual string ObservacaoRegraICMSCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoNotaFiscal", Column = "PNF_OBSERVACAO_NOTA_FISCAL", Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_PERCENTUAL_PAGAMENTO_AGREGADO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_NAO_IMPRIMIR_IMPOSTOS_DACTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImprimirImpostosDACTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_NAO_ENVIAR_IMPOSTO_ICMS_NA_EMISSAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarImpostoICMSNaEmissaoCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoNotaFiscal", Column = "PNF_TIPO_NOTA_FISCAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal TipoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemColeta", Column = "PNF_ORDEM_COLETA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemEntrega", Column = "PNF_ORDEM_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemEntrega { get; set; }

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

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_VALOR_MAXIMO_CENTRO_CONTABILIZACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximoCentroContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemServico", Column = "PNF_ITEM_SERVICO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ItemServico { get; set; }

        #region Valores Filial Emissora

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_VALOR_FRETE_TABELA_FRETE_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTabelaFreteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteFilialEmissora", Column = "PNF_VALOR_FRETE_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSFilialEmissora", Column = "PNF_VALOR_ICMS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_PERCENTUAL_CREDITO_PRESUMIDO_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualCreditoPresumidoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_VALOR_CREDITO_PRESUMIDO_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCreditoPresumidoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ICMSPagoPorSTFilialEmissora", Column = "PNF_ICMS_PAGO_POR_ST_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ICMSPagoPorSTFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaFilialEmissora", Column = "PNF_PERCENTUAL_ALICOTA_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSFilialEmissora", Column = "PNF_BASE_CALCULO_ICMS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO_FILIAL_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOPFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTFilialEmissora", Column = "PNF_CST_FILIAL_EMISSORA", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSBaseCalculoFilialEmissora", Column = "PNF_INCLUIR_ICMS_BASE_CALCULO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMSBaseCalculoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualIncluirBaseCalculoFilialEmissora", Column = "PNF_PERCENTUAL_INCLUIR_BASE_CALCULO_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualIncluirBaseCalculoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBCFilialEmissora", Column = "PNF_PERCENTUAL_REDUCAO_BC_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoBCFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRegraICMSCTeFilialEmissora", Column = "PNF_OBSERVACAO_REGRA_ICMS_CTE_FILIAL_EMISSORA", TypeType = typeof(string), NotNull = false, Length = 400)]
        public virtual string ObservacaoRegraICMSCTeFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarICMSDoValorAReceber", Column = "PNF_DESCONTAR_ICMS_ST_QUANDO_ICMS_NAO_INCLUSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarICMSDoValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoReduzirRetencaoICMSDoValorDaPrestacao", Column = "PNF_NAO_REDUZIR_RETENCAO_ICMS_DO_VALOR_DA_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoReduzirRetencaoICMSDoValorDaPrestacao { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PNF_PESO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [Obsolete("Migrada a situação para o XMLNotaFiscal")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal SituacaoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_TOTAL_MOEDA_COMPONENTES", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMoedaComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaFiscalEmOutraCarga", Column = "PNF_NOTA_FISCAL_EM_OUTRA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotaFiscalEmOutraCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PNF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoXMLNotaFiscalCTe", Column = "CAR_CODIGO")]
        public virtual ICollection<Cargas.CargaPedidoXMLNotaFiscalCTe> CTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocsParaEmissaoNFSManual", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PNF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO")]
        public virtual ICollection<Cargas.CargaDocumentoParaEmissaoNFSManual> DocsParaEmissaoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NFSs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_XML_NOTA_FISCAL_NFS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PNF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoXMLNotaFiscalNFS", Column = "XNS_CODIGO")]
        public virtual ICollection<Cargas.CargaPedidoXMLNotaFiscalNFS> NFSs { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraICMS", Column = "RIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ICMS.RegraICMS RegraICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_VALOR_FRETE_COM_ICMS_INCLUSO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteComICMSIncluso { get; set; }

        #region Imposto IBS/CBS

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNF_CODIGO_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string CodigoIndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "PNF_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "PNF_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "PNF_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "PNF_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "PNF_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "PNF_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "PNF_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "PNF_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "PNF_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "PNF_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "PNF_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "PNF_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        #region Imposto para Filial Emissora

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBSFilialEmissora", Column = "PNF_CST_IBSCBS_FILIAL_EMISSORA", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBSFilialEmissora", Column = "PNF_CLASSIFICACAO_TRIBUTARIA_IBSCBS_FILIAL_EMISSORA", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBSFilialEmissora", Column = "PNF_BASE_CALCULO_IBSCBS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadualFilialEmissora", Column = "PNF_ALIQUOTA_IBS_ESTADUAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadualFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadualFilialEmissora", Column = "PNF_PERCENTUAL_REDUCAO_IBS_ESTADUAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadualFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadualFilialEmissora", Column = "PNF_VALOR_IBS_ESTADUAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadualFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipalFilialEmissora", Column = "PNF_ALIQUOTA_IBS_MUNICIPAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipalFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipalFilialEmissora", Column = "PNF_PERCENTUAL_REDUCAO_IBS_MUNICIPAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipalFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipalFilialEmissora", Column = "PNF_VALOR_IBS_MUNICIPAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipalFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBSFilialEmissora", Column = "PNF_ALIQUOTA_CBS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBSFilialEmissora", Column = "PNF_PERCENTUAL_REDUCAO_CBS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBSFilialEmissora", Column = "PNF_VALOR_CBS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBSFilialEmissora { get; set; }

        #endregion Imposto para Filial Emissora

        #endregion Imposto CBS/IBS

        #region Propriedades Virtuais

        public virtual decimal ValorTotalAReceberComICMSeISS
        {
            get
            {
                decimal valor = this.ValorFrete + this.ValorTotalComponentes;
                if (this.CST != "60")
                    return valor + (this.IncluirICMSBaseCalculo ? this.ValorICMS : 0) + (this.IncluirISSBaseCalculo ? this.ValorISS : 0);
                else
                    return valor;
            }
        }

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

        public virtual bool Equals(PedidoXMLNotaFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal)this.MemberwiseClone();
        }

        #endregion
    }
}
