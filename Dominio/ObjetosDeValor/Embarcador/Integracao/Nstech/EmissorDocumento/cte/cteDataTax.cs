using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataTax
    {
        /// <summary>
        /// Objetos: cteDataTaxFiscalCode00, cteDataTaxFiscalCode20, cteDataTaxFiscalCode40_41_51
        ///          cteDataTaxFiscalCode60, cteDataTaxFiscalCode90, cteDataTaxFiscalCode90
        ///          cteDataTaxFiscalCodeOtherState, cteDataTaxFiscalCodeSimplesNacional
        /// </summary>
        public object tax { get; set; }

        public cteDataTaxDestinationState destinationState { get; set; }

        /// <summary>
        /// Total Tax Amount(tag vTotTrib)
        /// </summary>
        public decimal? totalTaxAmount { get; set; }

        /// <summary>
        /// Additional Information of Interest to the Tax Authority(tag infAdFisco)
        /// </summary>
        public string observation { get; set; }

        public cteIBSCBS IBSCBS { get; set; }

        /// <summary>
        /// Document Total Amount(tag vTotDFe)
        /// </summary>
        [JsonProperty("documentTotalAmount")]
        public decimal? ValorTotalDocumento { get; set; }
    }

    public class cteDataTaxFiscalCode
    {
        /// <summary>
        /// Tax Classification of the Service(tag CST)
        /// <para>00</para>
        /// <para>20</para>
        /// <para>40, 41, 51</para>
        /// <para>60</para>
        /// <para>90</para>
        /// <para>other_state</para>
        /// <para>simples_nacional</para>
        /// </summary>
        public string fiscalCode { get; set; }

        /// <summary>
        /// <para>00 - ICMS Taxable Base Value(tag vBC)</para>
        /// <para>20 - ICMS Taxable Base Value(tag vBC)</para>
        /// <para>90 - ICMS Taxable Base Value(tag vBC)</para>
        /// <para>other_state - ICMS Taxable Base Value(tag vBCOutraUF)</para>
        /// </summary>
        public decimal taxBaseAmount { get; set; }

        /// <summary>
        /// <para>20 - Taxable Base Reduction Percentage(tag pRedBC)</para>
        /// <para>90 - Taxable Base Reduction Percentage(tag pRedBC)</para>
        /// <para>other_state - Taxable Base Reduction Percentage(tag pRedBCOutraUF)</para>
        /// </summary>
        public decimal taxBaseAmountReduction { get; set; }

        /// <summary>
        /// <para>00 - ICMS Rate(tag pICMS)</para>
        /// <para>20 - ICMS Rate(tag pICMS)</para>
        /// <para>90 - ICMS Rate(tag pICMS)</para>
        /// <para>other_state - ICMS Rate(tag pICMSOutraUF)</para>
        /// </summary>
        public decimal taxRate { get; set; }

        /// <summary>
        /// <para>00 - ICMS Amount(tag vICMS)</para>
        /// <para>20 - ICMS Amount(tag vICMS)</para>
        /// <para>90 - ICMS Amount(tag vICMS)</para>
        /// <para>other_state - ICMS Amount(tag vICMSOutraUF)</para>
        /// </summary>
        public decimal taxAmount { get; set; }
    }

    public class cteDataTaxDestinationState
    {
        /// <summary>
        /// ICMS Taxable Base Value in the State of Service Delivery Completion(tag vBCUFFim)
        /// </summary>
        public decimal taxSateDestinationBase { get; set; }

        /// <summary>
        /// ICMS Percentage Related to the Poverty Combat Fund (FCP) in the State of Service Completion(tag pFCPUFFim)
        /// </summary>
        public decimal fcpStateDestinationRate { get; set; }

        /// <summary>
        /// Internal ICMS Rate in the State of Service Delivery Completion(tag pICMSUFFim)
        /// </summary>
        public decimal taxSateDestinationRate { get; set; }

        /// <summary>
        /// Interstate ICMS Rate of the Involved States(tag pICMSInter)
        /// </summary>
        public decimal interstateTaxtRate { get; set; }

        /// <summary>
        /// ICMS Amount Related to the Poverty Combat Fund (FCP) in the State of Service Completion(tag vFCPUFFim)
        /// </summary>
        public decimal fcpStateDestinationAmount { get; set; }

        /// <summary>
        /// ICMS Share Value for the State of Service Completion(tag vICMSUFFim)
        /// </summary>
        public decimal taxStateDestinationAmount { get; set; }

        /// <summary>
        /// ICMS Share Value for the State of Service Start(tag vICMSUFIni)
        /// </summary>
        public decimal taxStateSenderAmount { get; set; }
    }

    public class cteIBSCBS
    {
        /// <summary>
        /// Tax Situation Code(tag CST)
        /// </summary>
        [JsonProperty("fiscalCode")]
        public string CodigoFiscal { get; set; }

        /// <summary>
        /// Tax Classification Code(tag cClassTrib)
        /// </summary>
        [JsonProperty("TaxClassification")]
        public string ClassificacaoTributaria { get; set; }

        /// <summary>
        /// (tag gIBSCBS)
        /// </summary>
        [JsonProperty("IBSCBSGroup")]
        public GrupoIBSCBS GrupoIBSCBS { get; set; }
    }

    public class GrupoIBSCBS
    {
        /// <summary>
        /// Tax Base Amount(tag vBC)
        /// </summary>
        [JsonProperty("TaxBaseAmount")]
        public decimal BaseCalculo { get; set; }

        /// <summary>
        /// IBS State(tag gIBSUF)
        /// </summary>
        [JsonProperty("IBSState")]
        public IBSEstado IBSEstado { get; set; }

        /// <summary>
        /// IBS Municipality(tag gIBSMun)
        /// </summary>
        [JsonProperty("IBSMunicipality")]
        public IBSMunicipal IBSMunicipal { get; set; }

        /// <summary>
        /// IBS Tax Amount(tag vIBS)
        /// </summary>
        [JsonProperty("taxAmountIBS")]
        public decimal ValorIBS { get; set; }

        /// <summary>
        /// CBS(tag gCBS)
        /// </summary>
        [JsonProperty("CBS")]
        public CBS CBS { get; set; }

        /// <summary>
        /// Regular Tax(tag gTribRegular)
        /// </summary>
        [JsonProperty("regularTax")]
        public TributoRegular TributoRegular { get; set; }

        /// <summary>
        /// IBS Presumed Credit(tag gIBSCredPres)
        /// </summary>
        [JsonProperty("IBSPresumedCredit")]
        public CreditoPresumido IBSCreditoPresumido { get; set; }

        /// <summary>
        /// CBS Presumed Credit(tag gCBSCredPres)
        /// </summary>
        [JsonProperty("CBSPresumedCredit")]
        public CreditoPresumido CBSCreditoPresumido { get; set; }

        /// <summary>
        /// Government Procurement Tax(tag gTribCompraGov)
        /// </summary>
        [JsonProperty("governmentProcurementTax")]
        public TributoAquisicaoGoverno TributoAquisicaoGoverno { get; set; }
    }

    public class CBS
    {
        /// <summary>
        /// CBS Tax Rate(tag pCBS)
        /// </summary>
        [JsonProperty("taxRateCBS")]
        public decimal AliquotaCBS { get; set; }

        /// <summary>
        /// CBS Deferral(tag gDif)
        /// </summary>
        [JsonProperty("deferral")]
        public Diferimento Diferimento { get; set; }

        /// <summary>
        /// CBS Tax Refund(tag gDevTrib)
        /// </summary>
        [JsonProperty("taxRefund")]
        public RestituicaoTributaria Restituicao { get; set; }

        /// <summary>
        /// CBS Tax Rate Reduction(tag gRed)
        /// </summary>
        [JsonProperty("taxRateReduction")]
        public ReducaoAliquota ReducaoAliquota { get; set; }

        /// <summary>
        /// CBS Tax Amount(tag vCBS)
        /// </summary>
        [JsonProperty("taxAmountCBS")]
        public decimal ValorCBS { get; set; }
    }

    public class Diferimento
    {
        /// <summary>
        /// CBS Deferral Rate(tag pDif)
        /// </summary>
        [JsonProperty("deferralRate")]
        public decimal PercentualDiferimento { get; set; }

        /// <summary>
        /// CBS Deferral Amount(tag vDif)
        /// </summary>
        [JsonProperty("deferralAmount")]
        public decimal ValorDiferido { get; set; }
    }

    public class ReducaoAliquota
    {
        /// <summary>
        /// IBS Municipality Tax Rate Reduction(tag pRedAliq)
        /// </summary>
        [JsonProperty("rateReduction")]
        public decimal PercentualReducao { get; set; }

        /// <summary>
        /// IBS Municipality Effective Tax Rate(tag pAliqEfet)
        /// </summary>
        [JsonProperty("effectiveTaxRate")]
        public decimal AliquotaEfetiva { get; set; }
    }

    public class RestituicaoTributaria
    {
        /// <summary>
        /// IBS State Tax Refund Amount(tag vDevTrib)
        /// </summary>
        [JsonProperty("refundAmount")]
        public decimal ValorRestituicao { get; set; }
    }

    public class CreditoPresumido
    {
        /// <summary>
        /// Presumed Credit Code(tag cCredPres)
        /// </summary>
        [JsonProperty("PresumedCreditCode")]
        public string CodigoCreditoPresumido { get; set; }

        /// <summary>
        /// Presumed Credit Rate(tag pCredPres)
        /// </summary>
        [JsonProperty("PresumedCreditRate")]
        public decimal AliquotaCreditoPresumido { get; set; }

        [JsonProperty("presumedCredit")]
        public DetalheCreditoPresumido DetalheCreditoPresumido { get; set; }
    }

    public class DetalheCreditoPresumido
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        /// <summary>
        /// Presumed Credit Amount(tag vCredPres)
        /// </summary>
        [JsonProperty("creditAmount")]
        public decimal ValorCredito { get; set; }
    }

    public class TributoAquisicaoGoverno
    {
        /// <summary>
        /// IBS State Tax Rate(tag pAliqIBSUF)
        /// </summary>
        [JsonProperty("taxRateIBSState")]
        public decimal AliquotaIBSEstadual { get; set; }

        /// <summary>
        /// IBS State Tax Amount(tag vTribIBSUF)
        /// </summary>
        [JsonProperty("taxAmountIBSState")]
        public decimal ValorIBSEstadual { get; set; }

        /// <summary>
        /// IBS Municipality Tax Rate(tag pAliqIBSMun)
        /// </summary>
        [JsonProperty("taxRateIBSMunicipality")]
        public decimal AliquotaIBSMunicipal { get; set; }

        /// <summary>
        /// IBS Municipality Tax Amount(tag vTribIBSMun)
        /// </summary>
        [JsonProperty("taxAmountIBSMunicipality")]
        public decimal ValorIBSMunicipal { get; set; }

        /// <summary>
        /// CBS Tax Rate(tag pAliqCBS)
        /// </summary>
        [JsonProperty("taxRateCBS")]
        public decimal AliquotaCBS { get; set; }

        /// <summary>
        /// CBS Tax Amount(tag vTribCBS)
        /// </summary>
        [JsonProperty("taxAmountCBS")]
        public decimal ValorCBS { get; set; }
    }

    public class IBSMunicipal
    {
        /// <summary>
        /// IBS Municipality Tax Rate(tag pIBSMun)
        /// </summary>
        [JsonProperty("taxRateMunicipality")]
        public decimal AliquotaIBSMunicipal { get; set; }

        /// <summary>
        /// IBS Municipality Deferral(tag gDif)
        /// </summary>
        [JsonProperty("deferral")]
        public Diferimento Diferimento { get; set; }

        /// <summary>
        /// IBS Municipality Tax Refund(tag gDevTrib)
        /// </summary>
        [JsonProperty("taxRefund")]
        public RestituicaoTributaria Restituicao { get; set; }

        /// <summary>
        /// IBS Municipality Tax Rate Reduction(tag gRed)
        /// </summary>
        [JsonProperty("taxRateReduction")]
        public ReducaoAliquota ReducaoAliquota { get; set; }

        /// <summary>
        /// IBS Municipality Tax Amount(tag vIBSMun)
        /// </summary>
        [JsonProperty("taxAmountMunicipality")]
        public decimal ValorIBSMunicipal { get; set; }
    }

    public class IBSEstado
    {
        /// <summary>
        /// IBS State Tax Rate(tag pIBSUF)
        /// </summary>
        [JsonProperty("taxRateState")]
        public decimal AliquotaIBSEstadual { get; set; }

        /// <summary>
        /// IBS State Deferral(tag gDif)
        /// </summary>
        [JsonProperty("deferral")]
        public Diferimento Diferimento { get; set; }

        /// <summary>
        /// IBS State Tax Refund(tag gDevTrib)
        /// </summary>
        [JsonProperty("taxRefund")]
        public RestituicaoTributaria Restituicao { get; set; }

        /// <summary>
        /// IBS State Tax Rate Reduction(tag gRed)
        /// </summary>
        [JsonProperty("taxRateReduction")]
        public ReducaoAliquota ReducaoAliquota { get; set; }

        /// <summary>
        /// IBS State Tax Amount(tag vIBSUF)
        /// </summary>
        [JsonProperty("taxAmountState")]
        public decimal ValorIBSEstadual { get; set; }
    }

    public class TributoRegular
    {
        /// <summary>
        /// Regular Tax Situation Code(tag CSTReg)
        /// </summary>
        [JsonProperty("fiscalCode")]
        public string CodigoFiscal { get; set; }

        /// <summary>
        /// Regular Tax Classification Code(tag cClassTribReg)
        /// </summary>
        [JsonProperty("TaxClassification")]
        public string ClassificacaoTributaria { get; set; }

        /// <summary>
        /// Effective Tax Rate of IBS State(tag pAliqEfetRegIBSUF)
        /// </summary>
        [JsonProperty("effectiveTaxRateIBSState")]
        public decimal AliquotaIBSEstadual { get; set; }

        /// <summary>
        /// Tax Amount of IBS State(tag vTribRegIBSUF)
        /// </summary>
        [JsonProperty("taxAmountIBSState")]
        public decimal ValorIBSEstadual { get; set; }

        /// <summary>
        /// Effective Tax Rate of IBS Municipality(tag pAliqEfetRegIBSMun)
        /// </summary>
        [JsonProperty("effectiveTaxRateIBSMunicipality")]
        public decimal AliquotaIBSMunicipal { get; set; }

        /// <summary>
        /// Tax Amount of IBS Municipality(tag vTribRegIBSMun)
        /// </summary>
        [JsonProperty("taxAmountIBSMunicipality")]
        public decimal ValorIBSMunicipal { get; set; }

        /// <summary>
        /// Effective Tax Rate of CBS(tag pAliqEfetRegCBS)
        /// </summary>
        [JsonProperty("effectiveTaxRateCBS")]
        public decimal AliquotaCBS { get; set; }

        /// <summary>
        /// Tax Amount of CBS(tag vTribRegICBS)
        /// </summary>
        [JsonProperty("taxAmountCBS")]
        public decimal ValorCBS { get; set; }
    }
}