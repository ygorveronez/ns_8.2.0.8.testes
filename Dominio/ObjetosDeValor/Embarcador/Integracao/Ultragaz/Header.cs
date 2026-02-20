using System;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public class Header
    {
        public string source { get; set; }
        public int process_flag { get; set; }
        public string gl_date { get; set; }
        public string freight_flag { get; set; }
        public string document_type { get; set; }
        public string cnpj_issuer { get; set; }
        public int invoice_num { get; set; }
        public string series { get; set; }
        public string fiscal_document_model { get; set; }
        public string cnpj_recipient { get; set; }
        //public decimal invoice_amount { get; set; }
       
        [IgnoreDataMember]
        public decimal invoice_amount { get; set; }

        [DataMember(Name = "invoice_amount")]
        public decimal round_invoice_amount { get { return Math.Round(invoice_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        //public decimal gross_total_amount { get; set; }

        [IgnoreDataMember]
        public decimal gross_total_amount { get; set; }

        [DataMember(Name = "gross_total_amount")]
        public decimal round_gross_total_amount { get { return Math.Round(gross_total_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        public string invoice_date { get; set; }
        public string freight_spec { get; set; }
        public int icms_tax { get; set; }
        public string icms_type { get; set; }

        //public decimal icms_base { get; set; }

        [IgnoreDataMember]
        public decimal icms_base { get; set; }

        [DataMember(Name = "icms_base")]
        public decimal round_icms_base { get { return Math.Round(icms_base, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal icms_amount { get; set; }

        [IgnoreDataMember]
        public decimal icms_amount { get; set; }
        [DataMember(Name = "icms_amount")]
        public decimal round_icms_amount { get { return Math.Round(icms_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }


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

        // public decimal icms_st_amount_recover { get; set; }

        [IgnoreDataMember]
        public decimal icms_st_amount_recover { get; set; }
        [DataMember(Name = "icms_st_amount_recover")]
        public decimal round_icms_st_amount_recover { get { return Math.Round(icms_st_amount_recover, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal ipi_amount { get; set; }

        [IgnoreDataMember]
        public decimal ipi_amount { get; set; }
        [DataMember(Name = "ipi_amount")]
        public decimal round_ipi_amount { get { return Math.Round(ipi_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        public string origin_state { get; set; }
        public string destination_state { get; set; }
        public string ir_vendor { get; set; }
        public string ir_categ { get; set; }
        public string source_items { get; set; }
        public string terms_date { get; set; }
        public string eletronic_invoice_key { get; set; }
        public int inss_base { get; set; }
        public int inss_tax { get; set; }
        //public decimal inss_amount { get; set; }

        [IgnoreDataMember]
        public decimal inss_amount { get; set; }
        [DataMember(Name = "inss_amount")]
        public decimal round_inss_amount { get { return Math.Round(inss_amount, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        public int invoice_weight { get; set; }
        //public int iss_tax { get; set; }


        [IgnoreDataMember]
        public decimal iss_tax { get; set; }
        [DataMember(Name = "iss_tax")]
        public decimal round_iss_tax { get { return Math.Round(iss_tax, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        //public decimal ISS_AMOUNT { get; set; }

        [IgnoreDataMember]
        public decimal ISS_AMOUNT { get; set; }
        [DataMember(Name = "ISS_AMOUNT")]
        public decimal round_ISS_AMOUNT { get { return Math.Round(ISS_AMOUNT, 4) + (decimal)0.0001 - (decimal)0.0001; } }

        //public decimal iss_base { get; set; }
        [IgnoreDataMember]
        public decimal iss_base { get; set; }
        [DataMember(Name = "iss_base")]
        public decimal round_iss_base { get { return Math.Round(iss_base, 4) + (decimal)0.0001 - (decimal)0.0001; } }


        //public int terms_id { get; set; }
        public string reo_attribute2 { get; set; }
        public string reo_attribute3 { get; set; }
        public string reo_attribute4 { get; set; }
        public string reo_attribute5 { get; set; }
        public string reo_attribute12 { get; set; }
        public string reo_attribute13 { get; set; }
        public string reo_attribute14 { get; set; }

        //public string reo_attribute6 { get; set; }
        public string ship_via_lookup_code { get; set; }
        public string iss_city_code { get; set; }
        public string receive_date { get; set; }
        public string reo_attribute10 { get; set; }
        public string source_ibge_code { get; set; }
        public string destination_ibge_code { get; set; }
        public string model { get; set; }
        public string protocol { get; set; }
    }
}
