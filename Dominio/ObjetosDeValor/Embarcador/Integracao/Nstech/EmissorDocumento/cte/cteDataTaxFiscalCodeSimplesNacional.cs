using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataTaxFiscalCodeSimplesNacional
    {
        /// <summary>
        /// Tax Classification of the Service(tag CST)
        /// <para>simples_nacional</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumFiscalCode fiscalCode { get; } = enumFiscalCode.simples_nacional;
    }
}