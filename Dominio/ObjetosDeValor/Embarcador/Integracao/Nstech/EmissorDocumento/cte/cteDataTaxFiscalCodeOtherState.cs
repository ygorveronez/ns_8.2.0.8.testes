using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataTaxFiscalCodeOtherState
    {
        /// <summary>
        /// Tax Classification of the Service(tag CST)
        /// <para>other_state</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumFiscalCode fiscalCode { get; } = enumFiscalCode.other_state;

        /// <summary>
        /// <para>Taxable Base Reduction Percentage(tag pRedBCOutraUF)</para>
        /// </summary>
        public decimal taxBaseAmountReduction { get; set; }

        /// <summary>
        /// <para>ICMS Taxable Base Value(tag vBCOutraUF)</para>
        /// </summary>
        public decimal taxBaseAmount { get; set; }

        /// <summary>
        /// <para>ICMS Rate(tag pICMSOutraUF)</para>
        /// </summary>
        public decimal taxRate { get; set; }

        /// <summary>
        /// <para>ICMS Amount(tag vICMSOutraUF)</para>
        /// </summary>
        public decimal taxAmount { get; set; }
    }
}