using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataTaxFiscalCode90
    {
        /// <summary>
        /// Tax Classification of the Service(tag CST)
        /// <para>90</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumFiscalCode fiscalCode { get; } = enumFiscalCode.cst90;

        /// <summary>
        /// <para>Taxable Base Reduction Percentage(tag pRedBC)</para>
        /// </summary>
        public decimal taxBaseAmountReduction { get; set; }

        /// <summary>
        /// <para>ICMS Taxable Base Value(tag vBC)</para>
        /// </summary>
        public decimal taxBaseAmount { get; set; }

        /// <summary>
        /// <para>ICMS Rate(tag pICMS)</para>
        /// </summary>
        public decimal taxRate { get; set; }

        /// <summary>
        /// <para>ICMS Amount(tag vICMS)</para>
        /// </summary>
        public decimal taxAmount { get; set; }

        /// <summary>
        /// <para>Granted/Presumed Credit Amount(tag vCred)</para>
        /// </summary>
        public decimal creditAmount { get; set; }
    }
}