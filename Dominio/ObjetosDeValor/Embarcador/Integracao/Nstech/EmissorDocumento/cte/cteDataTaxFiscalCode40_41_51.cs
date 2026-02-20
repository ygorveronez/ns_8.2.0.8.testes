using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataTaxFiscalCode40_41_51
    {
        /// <summary>
        /// Tax Classification of the Service(tag CST)
        /// <para>40, 41, 51</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumFiscalCode40_41_51 fiscalCode { get; set; }
    }
}