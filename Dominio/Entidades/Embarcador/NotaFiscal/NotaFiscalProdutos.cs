using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_PRODUTOS", EntityName = "NotaFiscalProdutos", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos", NameType = typeof(NotaFiscalProdutos))]
    public class NotaFiscalProdutos : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTICMS", Column = "NFP_CST_CSOSN", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? CSTICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "NFP_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "NFP_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "NFP_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "NFP_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSeguro", Column = "NFP_VALOR_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutrasDespesas", Column = "NFP_VALOR_OUTRAS_DESPESAS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorOutrasDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "NFP_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOrdemCompra", Column = "NFP_NUMERO_ORDEM_COMPRA", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string NumeroOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroItemOrdemCompra", Column = "NFP_NUMERO_ITEM_ORDEM_COMPRA", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string NumeroItemOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCICMS", Column = "NFP_BC_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCICMS", Column = "NFP_REDUCAO_BC_ICMS", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal ReducaoBCICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaRemICMSRet", Column = "NFP_ALIQUOTA_REM_ICMS_RET", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal AliquotaRemICMSRet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSMonoRet", Column = "NFP_VALOR_ICMS_MOTO_RET", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMSMonoRet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "NFP_ALIQUOTA_ICMS", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "NFP_VALOR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCICMSDestino", Column = "NFP_BC_ICMS_DESTINO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCICMSDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaFCP", Column = "NFP_ALIQUOTA_FCP", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? AliquotaFCP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSDestino", Column = "NFP_ALIQUOTA_ICMS_DESTINO", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal AliquotaICMSDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSInterno", Column = "NFP_ALIQUOTA_ICMS_INTERNO", TypeType = typeof(decimal), Scale = 2, Precision = 4, NotNull = false)]
        public virtual decimal AliquotaICMSInterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualPartilha", Column = "NFP_PERCENTUAL_PARTILHA", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal PercentualPartilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFCP", Column = "NFP_VALOR_FCP", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorFCP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSDestino", Column = "NFP_VALOR_ICMS_DESTINO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMSDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSRemetente", Column = "NFP_VALOR_ICMS_REMETENTE", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMSRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCICMSST", Column = "NFP_BC_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MVAICMSST", Column = "NFP_MVA_ICMS_ST", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal MVAICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCICMSST", Column = "NFP_REDUCAO_BC_ICMS_ST", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal ReducaoBCICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSST", Column = "NFP_ALIQUOTA_ICMS_ST", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal AliquotaICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSSTInterestadual", Column = "NFP_ALIQUOTA_ICMS_ST_INTERESTADUAL", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal AliquotaICMSSTInterestadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSST", Column = "NFP_VALOR_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPIS", Column = "NFP_CST_PIS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCPIS", Column = "NFP_BC_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCPIS", Column = "NFP_REDUCAO_BC_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal ReducaoBCPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPIS", Column = "NFP_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "NFP_VALOR_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINS", Column = "NFP_CST_COFINS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCCOFINS", Column = "NFP_BC_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCCOFINS", Column = "NFP_REDUCAO_BC_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal ReducaoBCCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINS", Column = "NFP_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "NFP_VALOR_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIPI", Column = "NFP_CST_IPI", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI? CSTIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCIPI", Column = "NFP_BC_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCIPI", Column = "NFP_REDUCAO_BC_IPI", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal ReducaoBCIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIPI", Column = "NFP_ALIQUOTA_IPI", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal AliquotaIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIPI", Column = "NFP_VALOR_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseISS", Column = "NFP_BASE_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BaseISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaISS", Column = "NFP_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "NFP_VALOR_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCDeducao", Column = "NFP_BC_DEDUCAO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? BCDeducao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrasRetencoes", Column = "NFP_OUTRAS_RETENCOES", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? OutrasRetencoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontoIncondicional", Column = "NFP_DESCONTO_INCONDICIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? DescontoIncondicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontoCondicional", Column = "NFP_DESCONTO_CONDICIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? DescontoCondicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetencaoISS", Column = "NFP_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? RetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigibilidadeISS", Column = "NFP_EXIGIBILIDADE_ISS", TypeType = typeof(Dominio.Enumeradores.ExigibilidadeISS), NotNull = false)]
        public virtual Dominio.Enumeradores.ExigibilidadeISS ExigibilidadeISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncentivoFiscal", Column = "NFP_INCENTIVO_FISCAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IncentivoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProcessoJudicial", Column = "NFP_PROCESSO_JUDICIAL", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ProcessoJudicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseII", Column = "NFP_BASE_II", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BaseII { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDespesaII", Column = "NFP_VALOR_DESPESA_II", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorDespesaII { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorII", Column = "NFP_VALOR_II", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorII { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIOFII", Column = "NFP_VALOR_IOF_II", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorIOFII { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocImportacao", Column = "NFP_NUMERO_DOC_IMPORTACAO", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string NumeroDocImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRegistroImportacao", Column = "NFP_DATA_REGISTRO_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRegistroImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalDesembaraco", Column = "NFP_LOCAL_DESEMBARACO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string LocalDesembaraco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFDesembaraco", Column = "NFP_UF_DESEMBARACO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFDesembaraco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDesembaraco", Column = "NFP_DATA_DESEMBARACO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDesembaraco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJAdquirente", Column = "NFP_CNPJ_ADQUIRENTE", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJAdquirente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ViaTransporteII", Column = "NFP_VIA_TRANSPORTE_II", TypeType = typeof(Dominio.Enumeradores.ViaTransporteInternacional), NotNull = false)]
        public virtual Dominio.Enumeradores.ViaTransporteInternacional ViaTransporteII { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteMarinho", Column = "NFP_VALOR_FRETE_MARINHO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorFreteMarinho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntermediacaoII", Column = "NFP_INTERMEDIACAO_II", TypeType = typeof(Dominio.Enumeradores.IntermediacaoImportacao), NotNull = false)]
        public virtual Dominio.Enumeradores.IntermediacaoImportacao IntermediacaoII { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSDesonerado", Column = "NFP_VALOR_ICMS_DESONERADO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorICMSDesonerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoDesoneracao", Column = "NFP_MOTIVO_DESONERACAO", TypeType = typeof(Dominio.Enumeradores.MotivoDesoneracaoICMS), NotNull = false)]
        public virtual Dominio.Enumeradores.MotivoDesoneracaoICMS MotivoDesoneracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSOperacao", Column = "NFP_VALOR_ICMS_OPERACAO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorICMSOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSOperacao", Column = "NFP_ALIQUOTA_ICMS_OPERACAO", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? AliquotaICMSOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSDiferido", Column = "NFP_VALOR_ICMS_DIFERIDO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorICMSDiferido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorImpostoIBPT", Column = "NFP_VALOR_IMPOSTO_OBPT", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorImpostoIBPT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Servico", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Servico Servico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoItem", Column = "NFP_DESCRICAO_ITEM", TypeType = typeof(string), Length = 120, NotNull = false)]
        public virtual string DescricaoItem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoItem", Column = "NFP_CODIGO_ITEM", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string CodigoItem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencial", Column = "NFP_SEQUENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoANP", Column = "NFP_CODIGO_ANP", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string CodigoANP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSSimples", Column = "NFP_ALIQUOTA_ICMS_SIMPLES", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal AliquotaICMSSimples { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSSimples", Column = "NFP_VALOR_ICMS_SIMPLES", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMSSimples { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCFCPDestino", Column = "NFP_BC_FCP_DESTINO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? BCFCPDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCFCPICMS", Column = "NFP_BC_FCP_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? BCFCPICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualFCPICMS", Column = "NFP_PERCENTUAL_FCP_ICMS", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? PercentualFCPICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFCPICMS", Column = "NFP_VALOR_FCP_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorFCPICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCFCPICMSST", Column = "NFP_BC_FCP_ICMSST", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? BCFCPICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualFCPICMSST", Column = "NFP_PERCENTUAL_FCP_ICMSST", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? PercentualFCPICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFCPICMSST", Column = "NFP_VALOR_FCP_ICMSST", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorFCPICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaFCPICMSST", Column = "NFP_ALIQUOTA_FCP_ICMSST", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? AliquotaFCPICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualIPIDevolvido", Column = "NFP_PERCENTUAL_IPI_DEVOLVIDO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualIPIDevolvido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIPIDevolvido", Column = "NFP_VALOR_IPI_DEVOLVIDO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorIPIDevolvido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacoesAdicionais", Column = "NFP_INFORMACOES_ADICIONAIS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string InformacoesAdicionais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGLP", Column = "NFP_PERCENTUAL_GLP", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? PercentualGLP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGNN", Column = "NFP_PERCENTUAL_GNN", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? PercentualGNN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGNI", Column = "NFP_PERCENTUAL_GNI", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? PercentualGNI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualOrigemComb", Column = "NFP_PERCENTUAL_ORIGEM_COMP", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? PercentualOrigemComb { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualMisturaBiodiesel", Column = "NFP_PERCENTUAL_MISTURA_BIODIESEL", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? PercentualMisturaBiodiesel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPartidaANP", Column = "NFP_VALOR_PARTIDA_ANP", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorPartidaANP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorEscalaRelevante", Column = "NFP_INDICADOR_ESCALA_RELEVANTE", TypeType = typeof(int), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante IndicadorEscalaRelevante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJFabricante", Column = "NFP_CNPJ_FABRICANTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJFabricante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBeneficioFiscal", Column = "NFP_CODIGO_BENEFICIO_FISCAL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoBeneficioFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTributavel", Column = "NFP_QUANTIDADE_TRIBUTAVEL", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal? QuantidadeTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitarioTributavel", Column = "NFP_VALOR_UNITARIO_TRIBUTAVEL", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorUnitarioTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeDeMedidaTributavel", Column = "NFP_UNIDADE_MEDIDA_TRIBUTAVEL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida? UnidadeDeMedidaTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEANTributavel", Column = "NFP_CODIGO_EAN_TRIBUTAVEL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEANTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCICMSSTRetido", Column = "NFP_BC_ICMS_ST_RETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? BCICMSSTRetido { get; set; }

        /// <summary>
        /// Alíquota suportada pelo Consumidor Final
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSSTRetido", Column = "NFP_ALIQUOTA_ICMS_ST_RETIDO", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? AliquotaICMSSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSSTSubstituto", Column = "NFP_VALOR_ICMS_ST_SUBSTITUTO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorICMSSTSubstituto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSSTRetido", Column = "NFP_VALOR_ICMS_ST_RETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorICMSSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCICMSEfetivo", Column = "NFP_BC_ICMS_EFETIVO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? BCICMSEfetivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSEfetivo", Column = "NFP_ALIQUOTA_ICMS_EFETIVO", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? AliquotaICMSEfetivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCICMSEfetivo", Column = "NFP_REDUCAO_BC_ICMS_EFETIVO", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal? ReducaoBCICMSEfetivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSEfetivo", Column = "NFP_VALOR_ICMS_EFETIVO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorICMSEfetivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDrawback", Column = "NFP_NUMERO_DRAWBACK", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string NumeroDrawback { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRegistroExportacao", Column = "NFP_NUMERO_REGISTRO_EXPORTACAO", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string NumeroRegistroExportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveAcessoExportacao", Column = "NFP_CHAVE_ACESSO_EXPORTACAO", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveAcessoExportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNFCI", Column = "NFP_CODIGO_NFCI", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoNFCI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemMercadoria", Column = "NFP_ORIGEM_MERCADORIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria? OrigemMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.LocalArmazenamentoProduto LocalArmazenamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCICMSSTDestino", Column = "NFP_BC_ICMS_ST_DESTINO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? BCICMSSTDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSSTDestino", Column = "NFP_VALOR_ICMS_ST_DESTINO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorICMSSTDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LotesItem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NOTA_FISCAL_PRODUTOS_LOTES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NFP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NotaFiscalProdutosLotes", Column = "NPL_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes> LotesItem { get; set; }

        #region Propriedades Virtuais

        public virtual bool Equals(NotaFiscalProdutos other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoIndicadorEscalaRelevante
        {
            get
            {
                switch (this.IndicadorEscalaRelevante)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.Nenhum:
                        return "Nenhum";
                    case ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.ProduzidoEscalaNaoRelevante:
                        return "N – Produzido em Escala NÃO Relevante";
                    case ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.ProduzidoEscalaRelevante:
                        return "S - Produzido em Escala Relevante";
                    default:
                        return "";
                }
            }
        }

        public virtual string NumeroCSTICMS
        {
            get
            {
                switch (this.CSTICMS)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101:
                        return "101";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102:
                        return "102";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103:
                        return "103";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201:
                        return "201";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202:
                        return "202";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203:
                        return "203";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300:
                        return "300";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400:
                        return "400";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500:
                        return "500";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900:
                        return "900";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00:
                        return "00";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10:
                        return "10";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20:
                        return "20";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30:
                        return "30";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40:
                        return "40";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41:
                        return "41";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50:
                        return "50";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51:
                        return "51";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60:
                        return "60";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST61:
                        return "61";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70:
                        return "70";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90:
                        return "90";
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoCSTICMS
        {
            get
            {
                switch (this.CSTICMS)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101:
                        return "101 - Tributada pelo Simples Nacional com permissão de crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102:
                        return "102 - Tributada pelo Simples Nacional sem permissão de crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103:
                        return "103 - Isenção do ICMS no Simples Nacional para faixa de receita bruta";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201:
                        return "201 - Tributada pelo Simples Nacional com permissão de crédito e com cobrança do ICMS por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202:
                        return "202 - Tributada pelo Simples Nacional sem permissão de crédito e com cobrança do ICMS por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203:
                        return "203 - Isenção do ICMS no Simples Nacional para faixa de receita bruta e com cobrança do ICMS por substituicao tributaria";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300:
                        return "300 - Imune";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400:
                        return "400 - Nao tributada pelo Simples Nacional";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500:
                        return "500 - ICMS cobrado anteriormente por substituicao tributaria (substituido) ou por antecipacao";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900:
                        return "900 - Outros";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00:
                        return "00 - Tributada integralmente";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10:
                        return "10 - Tributada e com cobrança do ICMS por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20:
                        return "20 - Com redução de base de cálculo";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30:
                        return "30 - Isenta ou não tributada e com cobrança do ICMS por substituição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40:
                        return "40 - Isenta";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41:
                        return "41 - Não Tributada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50:
                        return "50 - Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51:
                        return "51 - Diferimento";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60:
                        return "60 - ICMS cobrado anteriormente por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST61:
                        return "61 - Tributação monofásica sobre combustíveis cobrada anteriormente";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70:
                        return "70 - Com redução de base de cálculo e cobrança do ICMS por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90:
                        return "90 - Outras";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCSTPIS
        {
            get
            {
                switch (this.CSTPIS)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01 - Operação Tributável com Alíquota Básica";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02 - Operação Tributável com Alíquota Diferenciada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05 - Operação Tributável por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06 - Operação Tributável a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07 - Operação Isenta da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08 - Operação sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09 - Operação com Suspensão da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49 - Outras Operações de Saída";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55 - Operação com Direito a Crédito - Vinculada a Receitas Não - Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Não - Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67 - Crédito Presumido -Outras Operações";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70 - Operação de Aquisição sem Direito a Crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71 - Operação de Aquisição com Isenção";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72 - Operação de Aquisição com Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73 - Operação de Aquisição a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74 - Operação de Aquisição sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75 - Operação de Aquisição por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98 - Outras Operações de Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99 - Outras Operações";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCSTCOFINS
        {
            get
            {
                switch (this.CSTCOFINS)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01 - Operação Tributável com Alíquota Básica";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02 - Operação Tributável com Alíquota Diferenciada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05 - Operação Tributável por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06 - Operação Tributável a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07 - Operação Isenta da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08 - Operação sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09 - Operação com Suspensão da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49 - Outras Operações de Saída";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55 - Operação com Direito a Crédito - Vinculada a Receitas Não - Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Não - Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67 - Crédito Presumido -Outras Operações";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70 - Operação de Aquisição sem Direito a Crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71 - Operação de Aquisição com Isenção";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72 - Operação de Aquisição com Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73 - Operação de Aquisição a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74 - Operação de Aquisição sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75 - Operação de Aquisição por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98 - Outras Operações de Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99 - Outras Operações";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCSTIPI
        {
            get
            {
                switch (this.CSTIPI)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST00:
                        return "00 - Entrada com Recuperação de Crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST01:
                        return "01 - Entrada Tributável com Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST02:
                        return "02 - Entrada Isenta";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST03:
                        return "03 - Entrada Não-Tributada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST04:
                        return "04 - Entrada Imune";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST05:
                        return "05 - Entrada com Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST49:
                        return "49 - Outras Entradas";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST50:
                        return "50 - Saída Tributada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST51:
                        return "51 - Saída Tributável com Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST52:
                        return "51 - Saída Isenta";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST53:
                        return "53 - Saída Não-Tributada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST54:
                        return "54 - Saída Imune";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST55:
                        return "55 - Saída com Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST99:
                        return "99 - Outras Saídas";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoExigibilidadeISS
        {
            get
            {
                switch (ExigibilidadeISS)
                {
                    case Enumeradores.ExigibilidadeISS.Exigivel:
                        return "Exigível";
                    case Enumeradores.ExigibilidadeISS.NaoInicidencia:
                        return "Não incidência";
                    case Enumeradores.ExigibilidadeISS.Isencao:
                        return "Isenção";
                    case Enumeradores.ExigibilidadeISS.Exportacao:
                        return "Exportação";
                    case Enumeradores.ExigibilidadeISS.Imunidade:
                        return "Imunidade";
                    case Enumeradores.ExigibilidadeISS.SuspensaDecisaoJudicial:
                        return "Suspensa por Decisão Judicial";
                    case Enumeradores.ExigibilidadeISS.SuspensaProcessoAdministrativo:
                        return "Suspensa por Processo Administrativo";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoViaTransporteII
        {
            get
            {
                switch (ViaTransporteII)
                {
                    case Enumeradores.ViaTransporteInternacional.Maritima:
                        return "Marítima";
                    case Enumeradores.ViaTransporteInternacional.Fluvial:
                        return "Fluvial";
                    case Enumeradores.ViaTransporteInternacional.Lacustre:
                        return "Lacustre";
                    case Enumeradores.ViaTransporteInternacional.Aerea:
                        return "Aérea";
                    case Enumeradores.ViaTransporteInternacional.Postal:
                        return "Postal";
                    case Enumeradores.ViaTransporteInternacional.Ferroviaria:
                        return "Ferroviária";
                    case Enumeradores.ViaTransporteInternacional.Rodoviaria:
                        return "Rodoviária";
                    case Enumeradores.ViaTransporteInternacional.CondutoRedeTransmissao:
                        return "Conduto / Rede Transmissão";
                    case Enumeradores.ViaTransporteInternacional.MeiosProprios:
                        return "Meios Próprios";
                    case Enumeradores.ViaTransporteInternacional.EntradaSaidaFicta:
                        return "Entrada / Saída ficta";
                    case Enumeradores.ViaTransporteInternacional.Courier:
                        return "Courier";
                    case Enumeradores.ViaTransporteInternacional.Handcarry:
                        return "Handcarry";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoIntermediacaoII
        {
            get
            {
                switch (IntermediacaoII)
                {
                    case Enumeradores.IntermediacaoImportacao.Propria:
                        return "Importação por conta própria";
                    case Enumeradores.IntermediacaoImportacao.ContaOrdem:
                        return "Importação por conta e ordem";
                    case Enumeradores.IntermediacaoImportacao.Encomenda:
                        return "Importação por encomenda";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoMotivoDesoneracao
        {
            get
            {
                switch (MotivoDesoneracao)
                {
                    case Enumeradores.MotivoDesoneracaoICMS.Taxi:
                        return "Táxi";
                    case Enumeradores.MotivoDesoneracaoICMS.DeficienteFisico:
                        return "Deficiente Físico";
                    case Enumeradores.MotivoDesoneracaoICMS.ProdutorAgropecuario:
                        return "Produtor Agropecuário";
                    case Enumeradores.MotivoDesoneracaoICMS.FrotistaLocadora:
                        return "Frotista/Locadora";
                    case Enumeradores.MotivoDesoneracaoICMS.DiplomaticoConsular:
                        return "Diplomático/Consular";
                    case Enumeradores.MotivoDesoneracaoICMS.UtilitariosMotocicleta:
                        return "Utilitários e Motocicletas da Amazônia Ocidental e Áreas de Livre Comércio";
                    case Enumeradores.MotivoDesoneracaoICMS.SUFRAMA:
                        return "SUFRAMA";
                    case Enumeradores.MotivoDesoneracaoICMS.VendaOrgaoPublico:
                        return "Venda a Órgão Público";
                    case Enumeradores.MotivoDesoneracaoICMS.Outros:
                        return "Outros";
                    case Enumeradores.MotivoDesoneracaoICMS.DeficienteCondutor:
                        return "Deficiente Condutor";
                    case Enumeradores.MotivoDesoneracaoICMS.DeficienteNaoCondutor:
                        return "Deficiente Não Condutor";
                    case Enumeradores.MotivoDesoneracaoICMS.FomentoEDesenvolvimentoAgropecuario:
                        return "Órgão de fomento e desenvolvimento agropecuário";
                    default:
                        return "";
                }
            }
        }

        public virtual Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos Clonar()
        {
            return (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos)this.MemberwiseClone();
        }

        #endregion
    }
}
