using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_DOCUMENTO_ENTRADA_ITEM", EntityName = "DocumentoEntradaItem", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem", NameType = typeof(DocumentoEntradaItem))]
    public class DocumentoEntradaItem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntradaTMS DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemCompraMercadoria", Column = "OCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Compras.OrdemCompraMercadoria OrdemCompraMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoFornecedor", Column = "TDI_CODIGO_PRODUTO_FORNECEDOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoProdutoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoProdutoFornecedor", Column = "TDI_DESCRICAO_PRODUTO_FORNECEDOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoProdutoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarrasEAN", Column = "TDI_CODIGO_BARRAS_EAN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoBarrasEAN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedida", Column = "TDI_UNIDADE_MEDIDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida UnidadeMedida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencial", Column = "TDI_SEQUENCIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int Sequencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "TDI_QUANTIDADE", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "TDI_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "TDI_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "TDI_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTICMS", Column = "TDI_CST", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string CSTICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "TDI_ALIQUOTA_ICMS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "TDI_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "TDI_VALOR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIPI", Column = "TDI_CST_IPI", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBaseCalculoIPI", Column = "TDI_PERCENTUAL_REDUCAO_BC_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualReducaoBaseCalculoIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIPI", Column = "TDI_ALIQUOTA_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIPI", Column = "TDI_BASE_CALCULO_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIPI", Column = "TDI_VALOR_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSST", Column = "TDI_BASE_CALCULO_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSST", Column = "TDI_ALIQUOTA_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSST", Column = "TDI_VALOR_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrasDespesas", Column = "TDI_OUTRAS_DESPESAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal OutrasDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "TDI_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPIS", Column = "TDI_CST_PIS", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBaseCalculoPIS", Column = "TDI_PERCENTUAL_REDUCAO_BC_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualReducaoBaseCalculoPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoPIS", Column = "TDI_BASE_CALCULO_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPIS", Column = "TDI_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "TDI_VALOR_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINS", Column = "TDI_CST_COFINS", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBaseCalculoCOFINS", Column = "TDI_PERCENTUAL_REDUCAO_BC_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualReducaoBaseCalculoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoCOFINS", Column = "TDI_BASE_CALCULO_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINS", Column = "TDI_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "TDI_VALOR_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoCreditoPresumido", Column = "TDI_BASE_CALCULO_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCreditoPresumido", Column = "TDI_ALIQUOTA_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCreditoPresumido", Column = "TDI_VALOR_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoDiferencial", Column = "TDI_BASE_CALCULO_DIFERENCIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoDiferencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaDiferencial", Column = "TDI_ALIQUOTA_DIFERENCIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaDiferencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDiferencial", Column = "TDI_VALOR_DIFERENCIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorDiferencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSeguro", Column = "TDI_VALOR_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteFora", Column = "TDI_VALOR_FRETE_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutrasDespesasFora", Column = "TDI_VALOR_OUTRAS_DESPESAS_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOutrasDespesasFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoFora", Column = "TDI_VALOR_DESCONTO_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDescontoFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorImpostosFora", Column = "TDI_VALOR_IMPOSTOS_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorImpostosFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDiferencialFreteFora", Column = "TDI_VALOR_DIFERENCIAL_FRETE_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDiferencialFreteFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSFreteFora", Column = "TDI_VALOR_ICMS_FRETE_FORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSFreteFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCustoUnitario", Column = "TDI_VALOR_CUSTO_UNITARIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCustoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCustoTotal", Column = "TDI_VALOR_CUSTO_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCustoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalculoCustoProduto", Column = "TDI_CALCULO_CUSTO_PRODUTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CalculoCustoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDI_CEST_PRODUTO_FORNECEDOR", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string CESTProdutoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDI_NCM_PRODUTO_FORNECEDOR", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string NCMProdutoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDI_VALOR_RETENCAO_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDI_VALOR_RETENCAO_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDI_VALOR_RETENCAO_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_RETENCAO_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_RETENCAO_CSLL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoCSLL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_RETENCAO_OUTRAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoOutras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_RETENCAO_IR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_VALOR_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TDE_OBSERVACAO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FatorConversao", Column = "TDE_FATOR_CONVERSAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal FatorConversao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAbastecimento", Column = "TDI_DATA_ABASTECIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMAbastecimento", Column = "TDI_KM_ABASTECIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int KMAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraEntradaDocumento", Column = "RED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraEntradaDocumento RegraEntradaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDI_BASE_ST_RETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDI_VALOR_ST_RETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Horimetro", Column = "TDI_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int Horimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CstIcmsFornecedor", Column = "TDI_CST_FORNECEDOR", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CstIcmsFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CfopFornecedor", Column = "TDI_CFOP_FORNECEDOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CfopFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSFornecedor", Column = "TDI_BASE_ICMS_FORNECEDOR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMSFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSFornecedor", Column = "TDI_ALIQUOTA_ICMS_FORNECEDOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMSFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSFornecedor", Column = "TDI_VALOR_ICMS_FORNECEDOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Equipamento Equipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFogoInicial", Column = "TDI_NUMERO_FOGO_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroFogoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAquisicao", Column = "TDI_TIPO_AQUISICAO_PNEU", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu? TipoAquisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VidaAtual", Column = "TDI_VIDA_ATUAL_PNEU", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.VidaPneu), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.VidaPneu? VidaAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Almoxarifado", Column = "AMX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.Almoxarifado Almoxarifado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO_VINCULADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto ProdutoVinculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeProdutoVinculado", Column = "TDI_QUANTIDADE_PRODUTO_VINCULADO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeProdutoVinculado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.LocalArmazenamentoProduto LocalArmazenamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedidaFornecedor", Column = "TDI_UNIDADE_MEDIDA_FORNECEDOR", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string UnidadeMedidaFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeFornecedor", Column = "TDI_QUANTIDADE_FORNECEDOR", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitarioFornecedor", Column = "TDI_VALOR_UNITARIO_FORNECEDOR", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitarioFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDI_GERA_RATEIO_DESPESA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeraRateioDespesaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemMercadoria", Column = "TDI_ORIGEM_MERCADORIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria? OrigemMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EncerrarOrdemServico", Column = "TDI_ENCERRAR_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAbastecimentoTabelaFornecedor", Column = "TDI_VALOR_ABASTECIMENTO_TABELA_FORNECEDOR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorAbastecimentoTabelaFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAbastecimentoComDivergencia", Column = "TDI_VALOR_ABASTECIMENTO_COM_DIVERGENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorAbastecimentoComDivergencia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Abastecimentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoEntradaItemAbastecimento", Column = "TDA_CODIGO")]
        public virtual IList<DocumentoEntradaItemAbastecimento> Abastecimentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "OrdensServico", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TMS_DOCUMENTO_ENTRADA_ITEM_ORDEM_SERVICO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoEntradaItemOrdemServico", Column = "TDO_CODIGO")]
        public virtual IList<DocumentoEntradaItemOrdemServico> OrdensServico { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (Codigo.ToString() + " - " + (this.Produto != null ? this.Produto.Descricao : string.Empty) + " NF: " + this.DocumentoEntrada.Numero.ToString("n0"));
            }
        }

        public virtual decimal ValorTotalLiquido
        {
            get { return ValorTotal + ValorSeguro + ValorICMSST + ValorFrete + OutrasDespesas + ValorIPI - Desconto; }
        }
    }
}
