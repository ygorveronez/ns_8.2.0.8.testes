using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataTaxFiscalCode00
    {
        /// <summary>
        /// Tax Classification of the Service(tag CST)
        /// <para>00</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumFiscalCode fiscalCode { get; } = enumFiscalCode.cst00;

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
    }
}