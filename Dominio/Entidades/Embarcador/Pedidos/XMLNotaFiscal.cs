using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_XML_NOTA_FISCAL", EntityName = "XMLNotaFiscal", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal", NameType = typeof(XMLNotaFiscal))]
    public class XMLNotaFiscal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>
    {
        public XMLNotaFiscal() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "NF_TIPO_DOCUMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "NF_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "NF_CHAVE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_CHAVE_VENDA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ChaveVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_PROTOCOLO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NF_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOutroDocumento", Column = "NF_NUMERO_OUTRO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroOutroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "NF_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Modelo", Column = "NF_MODELO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string Modelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "NF_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "NF_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "NF_VALOR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoST", Column = "NF_BASE_CALCULO_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorST", Column = "NF_VALOR_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalProdutos", Column = "NF_VALOR_TOTAL_PRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSeguro", Column = "NF_VALOR_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "NF_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorImpostoImportacao", Column = "NF_VALOR_IMPOSTO_IMPORTACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorImpostoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIPI", Column = "NF_VALOR_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "NF_VALOR_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "NF_VALOR_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaImpostoSuspenso", Column = "NF_ALIQUOTA_IMPOSTO_SUSPENSO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaImpostoSuspenso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorImpostoSuspenso", Column = "NF_VALOR_IMPOSTO_SUSPENSO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorImpostoSuspenso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCommodities", Column = "NF_VALOR_COMMODITIES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCommodities { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutros", Column = "NF_VALOR_OUTROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOutros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "NF_PESO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "NF_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "NF_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "NF_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "NF_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "NF_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "NF_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorReducaoIBSEstadual", Column = "NF_VALOR_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "NF_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "NF_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "NF_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorReducaoIBSMunicipal", Column = "NF_VALOR_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "NF_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "NF_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "NF_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorReducaoCBS", Column = "NF_VALOR_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "NF_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        /// <summary>
        /// Valor do frete informado pelo embarcador. Apenas informativo do valor do frete no embarcador, para fins de comparação com o valor do frete calculado pelo sistema.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_VALOR_FRETE_EMBARCADOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteEmbarcador { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteRedespacho", Column = "NF_VALOR_FRETE_REDESPACHO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        //public virtual decimal ValorFreteRedespacho { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "AgValorRedespacho", Column = "NF_AG_VALOR_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool AgValorRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquido", Column = "NF_PESO_LIQUIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_ALTURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Altura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_LARGURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Largura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_COMPRIMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Comprimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_METROS_CUBICOS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetrosCubicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_FATOR_CUBAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal FatorCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_PESO_CUBADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoCubado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_PESO_POR_PALLET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoPorPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_PESO_PALETIZADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoPaletizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallets", Column = "NF_QUANTIDADE_PALLETS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PalletsControle", Column = "NF_PALLETS_CONTROLE", TypeType = typeof(int), NotNull = false)]
        public virtual int PalletsControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_PESO_BASE_CALCULO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoBaseParaCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "NF_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "NF_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamento", Column = "NF_DATA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "NF_XML", Type = "StringClob", NotNull = false, Lazy = true)]
        public virtual string XML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CanceladaPeloEmitente", Column = "NF_CANCELADA_EMITENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CanceladaPeloEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "nfAtiva", Column = "NF_ATIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool nfAtiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SemCarga", Column = "NF_SEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaturezaOP", Column = "NF_NATUREZA_OP", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NaturezaOP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJTranposrtador", Column = "NF_CNPJ_TRANSPORTADOR", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string CNPJTranposrtador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaVeiculoNotaFiscal", Column = "NF_PLACA_VEICULO_NF", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string PlacaVeiculoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalidadeFrete", Column = "NF_MODALIDADE_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete ModalidadeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOperacaoNotaFiscal", Column = "NF_TIPO_OPERACAO_NF", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal TipoOperacaoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoNotaFiscalIntegrada", Column = "NF_TIPO_NOTA_FISCAL_INTEGRADA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada? TipoNotaFiscalIntegrada { get; set; }

        [NHibernate.Mapping.Attributes.OneToOne(0, Name = "Canhoto", Class = "Canhoto", PropertyRef = "XMLNotaFiscal", Access = "property")]
        public virtual Dominio.Entidades.Embarcador.Canhotos.Canhoto Canhoto { get; set; }

        //[NHibernate.Mapping.Attributes.OneToOne(0, Name = "CargaPedidoXMLNotaFiscalParcial", Class = "CargaPedidoXMLNotaFiscalParcial", PropertyRef = "XMLNotaFiscal", Access = "property")]
        //public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial CargaPedidoXMLNotaFiscalParcial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_CODIGO_CLIENTE_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornoNotaIntegrada", Column = "NF_RETORNO_NOTA_INTEGRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornoNotaIntegrada { get; set; }

        [Obsolete("Definição disso está no campo TipoNotaFiscalIntegrada")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaDePallet", Column = "NF_NOTA_DE_PALLET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotaDePallet { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTEs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_XML_NOTAS_FISCAIS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NFX_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO")]
        public virtual IList<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTEs { get; set; }

        /// <summary>
        /// identifica a qual rota a nota pertence, essa informação pode ser utilizada para observação nos CT-es, para calculo de frete entre outras.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Rota", Column = "NF_ROTA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string Rota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SubRota", Column = "NF_SUB_ROTA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string SubRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrauRisco", Column = "NF_GRAU_RISCO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string GrauRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDT", Column = "NF_NUMERO_DT", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroDT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedido", Column = "NF_NUMERO_PEDIDO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_NUMERO_PEDIDO_EMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroPedidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRomaneio", Column = "NF_NUMERO_ROMANEIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroRomaneio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSolicitacao", Column = "NF_NUMERO_SOLICITACAO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NumeroSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_NUMERO_TRANSPORTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RETIRADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteRetirada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisao", Column = "NF_DATA_PREVISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_KM_ROTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal KMRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_OBSERVACAO", Type = "StringClob", NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "NF_NCM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOP", Column = "NF_CFOP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControleCliente", Column = "NF_NUMERO_CONTROLE_CLIENTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroControleCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControlePedido", Column = "NF_NUMERO_CONTROLE_PEDIDO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroControlePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroReferenciaEDI", Column = "NF_NUMERO_REFERENCIA_EDI", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroReferenciaEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCanhoto", Column = "NF_NUMERO_CANHOTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PINSUFRAMA", Column = "NF_PIN_SUFRAMA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PINSUFRAMA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBooking", Column = "NF_NUMERO_BOOKING", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContainer", Column = "NF_NUMERO_CONTAINER", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_OBSERVACAO_NOTA_FISCAL_PARA_CTE", Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoNotaFiscalParaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoRecebidoViaFTP", Column = "NF_DOCUMENTO_RECEBIDO_VIA_FTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoRecebidoViaFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoRecebidoViaEmail", Column = "NF_DOCUMENTO_RECEBIDO_VIA_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoRecebidoViaEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "NF_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoRecebidoViaNOTFIS", Column = "NF_DOCUMENTO_RECEBIDO_VIA_NOTFIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoRecebidoViaNOTFIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_EMBARQUE", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Embarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_MASTER_BL", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string MasterBL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_NUMERO_DI", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDeCarga", Column = "NF_TIPO_DE_CARGA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCarregamento", Column = "NF_NUMERO_CARREGAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Dimensoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_XML_NOTA_FISCAL_DIMENSAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NFX_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "XMLNotaFiscalDimensao", Column = "NXD_CODIGO")]
        public virtual IList<Embarcador.Pedidos.XMLNotaFiscalDimensao> Dimensoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_NOME_DESTINATARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_IE_DESTINATARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IEDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_IE_REMETENTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IERemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_NUMERO_DOCUMENTO_EMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroDocumentoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraCriacaoEmbrcador", Column = "NF_DATA_HORA_CRIACAO_EMBARCADOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraCriacaoEmbrcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_PRODUTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_CLASSIFICACAO_NFE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe? ClassificacaoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouPDF", Column = "NF_GEROU_PDF", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerouPDF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_SEGUNDO_CODIGO_BARRAS", TypeType = typeof(string), Length = 36, NotNull = false)]
        public virtual string SegundoCodigoBarras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissao", Column = "NF_TIPO_EMISSAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal TipoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscalImportacao", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual XMLNotaFiscalImportacao XMLNotaFiscalImportacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscalSituacao", Column = "NFS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal.NotaFiscalSituacao NotaFiscalSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataNotaFiscalSituacao", Column = "NF_DATA_NOTA_FISCAL_SITUACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataNotaFiscalSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_SITUACAO_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal SituacaoEntregaNotaFiscal { get; set; } = ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.AgEntrega;

        /// <summary>
        /// Situação da entrega marcada na última devolução, seja com ou sem chamado/atendimento, pois o campo SituacaoEntregaNotaFiscal fica entregue ao confirmar a entrega
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "NF_ULTIMA_SITUACAO_ENTREGA_DEVOLUCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal? UltimaSituacaoEntregaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFatura", Column = "NF_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturaFake", Column = "NF_FATURA_FAKE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? FaturaFake { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IrrelevanteParaFrete", Column = "NF_IRRELEVANTE_PARA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IrrelevanteParaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOrdemPedidoIntegracaoUnilever", Column = "NF_NUMERO_ORDEM_PEDIDO_INTEGRACAO_UNILEVER", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroOrdemPedidoIntegracaoUnilever { get; set; }

        [Obsolete("Não pode ser um inteiro, esse campo foi movido para NF_NUMERO_DA_FATURA")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFatura", Column = "NF_NUMERO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaFatura", Column = "NF_NUMERO_DA_FATURA", TypeType = typeof(double), NotNull = false)]
        public virtual double NumeroDaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLiquido", Column = "NF_VALOR_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Especie", Column = "NF_ESPECIE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Especie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaIntegracao", Column = "NF_FORMA_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao? FormaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaJaEstavaNaBase", Column = "NF_NOTA_JA_ESTAVA_NA_BASE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotaJaEstavaNaBase { get; set; }

        [Obsolete("Alterado para a entidade GestaoDevolucao")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPermuta", Column = "NF_NUMERO_PERMUTA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroPermuta { get; set; }

        [Obsolete("Alterado para a entidade GestaoDevolucao")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "SeriePermuta", Column = "NF_SERIE_PERMUTA", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string SeriePermuta { get; set; }

        public virtual string DescricaoModalidadeFrete
        {
            get
            {
                string retorno = "";
                switch (this.ModalidadeFrete)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago:
                        retorno = "Pago";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar:
                        retorno = "A Pagar";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros:
                        retorno = "Outros";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido:
                        retorno = "Não Definido";
                        break;
                    default:
                        retorno = "Não Definido";
                        break;
                }
                return retorno;
            }
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal Clonar()
        {
            //problema de proxy load:
            //return (Dominio.Entidades.Embarcador.Pedidos.iscal)this.MemberwiseClone(),

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal copia = new XMLNotaFiscal();
            this.CopyProperties(copia);
            return copia;
        }

        public virtual Dominio.Entidades.Cliente ObterEmitente
        {
            get
            {
                return this.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? this.Emitente : this.Destinatario;
            }
        }

        public virtual bool Equals(XMLNotaFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string SerieOuSerieDaChave
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Serie))
                    return this.Serie;

                if (!string.IsNullOrWhiteSpace(this.Chave) && this.Chave.Length == 44)
                {
                    int serie = 0;

                    if (int.TryParse(this.Chave.Substring(23, 2), out serie))
                        return serie.ToString();
                    else
                        return this.Chave.Substring(23, 2);
                }

                return string.Empty;
            }
        }

        public virtual string EtiquetaCodigoBarrasWMS(int volume)
        {
            string codigoBarrasFormatado = (Emitente?.CPF_CNPJ_SemFormato ?? "0").PadLeft(14, '0');
            codigoBarrasFormatado += Numero > 0 ? Utilidades.String.OnlyNumbers(Numero.ToString()).PadLeft(9, '0') : Utilidades.String.OnlyNumbers("0").PadLeft(9, '0');
            codigoBarrasFormatado += Utilidades.String.OnlyNumbers(volume.ToString("n0")).PadLeft(4, '0');
            codigoBarrasFormatado += Utilidades.String.OnlyNumbers(Serie).PadLeft(2, '0');
            codigoBarrasFormatado += "00";

            return codigoBarrasFormatado;
        }

    }
}
