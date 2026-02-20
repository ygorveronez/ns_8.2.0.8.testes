using System;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public class Line
    {
        public string accounting_combination { get; set; }
        //public string classification_code { get; set; }
        //public string utilization_code { get; set; }
        public string cfo_code { get; set; }
        public string uom { get; set; }
        public int quantity { get; set; }
        // public decimal unit_price { get; set; }
        [IgnoreDataMember]
        public decimal unit_price { get; set; }
        [DataMember(Name = "unit_price")]
        public decimal round_unit_price { get { return Math.Round(unit_price, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        //public string operation_fiscal_type { get; set; }
        //public decimal icms_base { get; set; }
        [IgnoreDataMember]
        public decimal icms_base { get; set; }
        [DataMember(Name = "icms_base")]
        public decimal round_icms_base { get { return Math.Round(icms_base, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        public int icms_tax { get; set; }
        //public decimal icms_amount { get; set; }
        [IgnoreDataMember]
        public decimal icms_amount { get; set; }
        [DataMember(Name = "icms_amount")]
        public decimal round_icms_amount { get { return Math.Round(icms_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal icms_amount_recover { get; set; }
        [IgnoreDataMember]
        public decimal icms_amount_recover { get; set; }
        [DataMember(Name = "icms_amount_recover")]
        public decimal round_icms_amount_recover { get { return Math.Round(icms_amount_recover, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        public int icms_tax_code { get; set; }
        public int diff_icms_tax { get; set; }
        //public decimal diff_icms_amount { get; set; }
        [IgnoreDataMember]
        public decimal diff_icms_amount { get; set; }
        [DataMember(Name = "diff_icms_amount")]
        public decimal round_diff_icms_amount { get { return Math.Round(diff_icms_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal icms_st_base { get; set; }
        [IgnoreDataMember]
        public decimal icms_st_base { get; set; }
        [DataMember(Name = "icms_st_base")]
        public decimal round_icms_st_base { get { return Math.Round(icms_st_base, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal icms_st_amount { get; set; }
        [IgnoreDataMember]
        public decimal icms_st_amount { get; set; }
        [DataMember(Name = "icms_st_amount")]
        public decimal round_icms_st_amount { get { return Math.Round(icms_st_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal icms_st_amount_recover { get; set; }
        [IgnoreDataMember]
        public decimal icms_st_amount_recover { get; set; }
        [DataMember(Name = "icms_st_amount_recover")]
        public decimal round_icms_st_amount_recover { get { return Math.Round(icms_st_amount_recover, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal ipi_base_amount { get; set; }
        [IgnoreDataMember]
        public decimal ipi_base_amount { get; set; }
        [DataMember(Name = "ipi_base_amount")]
        public decimal round_ipi_base_amount { get { return Math.Round(ipi_base_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        public int ipi_tax { get; set; }
        //public decimal ipi_amount { get; set; }
        [IgnoreDataMember]
        public decimal ipi_amount { get; set; }
        [DataMember(Name = "ipi_amount")]
        public decimal round_ipi_amount { get { return Math.Round(ipi_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal ipi_amount_recover { get; set; }
        [IgnoreDataMember]
        public decimal ipi_amount_recover { get; set; }
        [DataMember(Name = "ipi_amount_recover")]
        public decimal round_ipi_amount_recover { get { return Math.Round(ipi_amount_recover, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        public int ipi_tax_code { get; set; }
        //public decimal total_amount { get; set; }
        [IgnoreDataMember]
        public decimal total_amount { get; set; }
        [DataMember(Name = "total_amount")]
        public decimal round_total_amount { get { return Math.Round(total_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal diff_icms_amount_recover { get; set; }
        [IgnoreDataMember]
        public decimal diff_icms_amount_recover { get; set; }
        [DataMember(Name = "diff_icms_amount_recover")]
        public decimal round_diff_icms_amount_recover { get { return Math.Round(diff_icms_amount_recover, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        public string tributary_status_code { get; set; }
        public int ipi_tributary_code { get; set; }
        public string item { get; set; }

        //public decimal iss_base_amount { get; set; }
        [IgnoreDataMember]
        public decimal iss_base_amount { get; set; }
        [DataMember(Name = "iss_base_amount")]
        public decimal round_iss_base_amount { get { return Math.Round(iss_base_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        //public decimal pis_base_amount { get; set; }
        [IgnoreDataMember]
        public decimal pis_base_amount { get; set; }
        [DataMember(Name = "pis_base_amount")]
        public decimal round_pis_base_amount { get { return Math.Round(pis_base_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal pis_tax_rate { get; set; }
        [IgnoreDataMember]
        public decimal pis_tax_rate { get; set; }
        [DataMember(Name = "pis_tax_rate")]
        public decimal round_pis_tax_rate { get { return Math.Round(pis_tax_rate, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal pis_amount_recover { get; set; }
        [IgnoreDataMember]
        public decimal pis_amount_recover { get; set; }
        [DataMember(Name = "pis_amount_recover")]
        public decimal round_pis_amount_recover { get { return Math.Round(pis_amount_recover, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        //public decimal pis_amount { get; set; }
        [IgnoreDataMember]
        public decimal pis_amount { get; set; }
        [DataMember(Name = "pis_amount")]
        public decimal round_pis_amount { get { return Math.Round(pis_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        public string pis_tributary_code { get; set; }

        //public decimal cofins_base_amount { get; set; }
        [IgnoreDataMember]
        public decimal cofins_base_amount { get; set; }
        [DataMember(Name = "cofins_base_amount")]
        public decimal round_cofins_base_amount { get { return Math.Round(cofins_base_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        //public decimal cofins_tax_rate { get; set; }
        [IgnoreDataMember]
        public decimal cofins_tax_rate { get; set; }
        [DataMember(Name = "cofins_tax_rate")]
        public decimal round_cofins_tax_rate { get { return Math.Round(cofins_tax_rate, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        //public decimal cofins_amount_recover { get; set; }
        [IgnoreDataMember]
        public decimal cofins_amount_recover { get; set; }
        [DataMember(Name = "cofins_amount_recover")]
        public decimal round_cofins_amount_recover { get { return Math.Round(cofins_amount_recover, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        //public decimal cofins_amount { get; set; }
        [IgnoreDataMember]
        public decimal cofins_amount { get; set; }
        [DataMember(Name = "cofins_amount")]
        public decimal round_cofins_amount { get { return Math.Round(cofins_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        public string cofins_tributary_code { get; set; }
        //public decimal iss_tax_amount { get; set; }
        [IgnoreDataMember]
        public decimal iss_tax_amount { get; set; }
        [DataMember(Name = "iss_tax_amount")]
        public decimal round_iss_tax_amount { get { return Math.Round(iss_tax_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public int iss_tax_rate { get; set; }
        [IgnoreDataMember]
        public decimal iss_tax_rate { get; set; }
        [DataMember(Name = "iss_tax_rate")]
        public decimal round_iss_tax_rate { get { return Math.Round(iss_tax_rate, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        public string receipt_flag { get; set; }

        // IBSUF_TAX: Aliquota do IBS da UF (NUM 3,4)
        [IgnoreDataMember]
        public decimal ibsuf_tax { get; set; }
        [DataMember(Name = "IBSUF_TAX")]
        public decimal round_ibsuf_tax { get { return Math.Round(ibsuf_tax, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        // IBSUF_AMOUNT: Total do imposto IBS para UF (DEC 15,2)
        [IgnoreDataMember]
        public decimal ibsuf_amount { get; set; }
        [DataMember(Name = "IBSUF_AMOUNT")]
        public decimal round_ibsuf_amount { get { return Math.Round(ibsuf_amount, 2) + (decimal)0.0001 - (decimal)0.0001; } }

        // IBSMUN_TAX: Aliquota do IBS da MUN (NUM 3,4)
        [IgnoreDataMember]
        public decimal ibsmun_tax { get; set; }
        [DataMember(Name = "IBSMUN_TAX")]
        public decimal round_ibsmun_tax { get { return Math.Round(ibsmun_tax, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        // IBSMUN_AMOUNT: Total do imposto IBS para MUN (DEC 15,2)
        [IgnoreDataMember]
        public decimal ibsmun_amount { get; set; }
        [DataMember(Name = "IBSMUN_AMOUNT")]
        public decimal round_ibsmun_amount { get { return Math.Round(ibsmun_amount, 2) + (decimal)0.0001 - (decimal)0.0001; } }

        // CLASSTRIB: Código da classificação trib. do IBS e CBS (VARCHAR 10)
        public string classtrib { get; set; }

        // CBS_TAX: Aliquota do CBS (NUM 3,4)
        [IgnoreDataMember]
        public decimal cbs_tax { get; set; }
        [DataMember(Name = "CBS_TAX")]
        public decimal round_cbs_tax { get { return Math.Round(cbs_tax, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        // CBS_AMOUNT: Total do imposto CBS (DEC 15,2)
        [IgnoreDataMember]
        public decimal cbs_amount { get; set; }
        [DataMember(Name = "CBS_AMOUNT")]
        public decimal round_cbs_amount { get { return Math.Round(cbs_amount, 2) + (decimal)0.0001 - (decimal)0.0001; } }

        // IBS_CBS_BASE: Base dos impostos CBS e IBS (DEC 15,2)
        [IgnoreDataMember]
        public decimal ibs_cbs_base { get; set; }
        [DataMember(Name = "IBS_CBS_BASE")]
        public decimal round_ibs_cbs_base { get { return Math.Round(ibs_cbs_base, 2) + (decimal)0.0001 - (decimal)0.0001; } }

        // IBS_CBS_TAX_CODE: CST do IBS/CBS (VARCHAR 10)
        public string ibs_cbs_tax_code { get; set; }

        // COD_LINHA_XML: Valor constante = 1 (VARCHAR 1)
        public string cod_linha_xml { get; set; }

        // CRT: Código do regime tributário (VARCHAR 1)
        public string crt { get; set; }
    }
}
