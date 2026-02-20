using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataTaxFiscalCode60
    {
        /// <summary>
        /// Tax Classification of the Service(tag CST)
        /// <para>60</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumFiscalCode fiscalCode { get; } = enumFiscalCode.cst60;

        /// <summary>
        /// Value of the Withheld ICMS ST Taxable Base(tag vBCSTRet)
        /// </summary>
        public decimal withheldTaxBase { get; set; }

        /// <summary>
        /// ICMS Rate(tag pICMSSTRet)
        /// </summary>
        public decimal withheldTaxRate { get; set; }

        /// <summary>
        /// Withheld ICMS ST Amount(tag vICMSSTRet)
        /// </summary>
        public decimal withheldTaxAmount { get; set; }

        /// <summary>
        /// <para>Granted/Presumed Credit Amount(tag vCred)</para>
        /// </summary>
        public decimal creditAmount { get; set; }
    }
}