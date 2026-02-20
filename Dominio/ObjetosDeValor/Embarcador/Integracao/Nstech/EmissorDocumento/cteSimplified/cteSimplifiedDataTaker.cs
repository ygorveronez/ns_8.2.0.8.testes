using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteSimplifiedDataTaker
    {
        /// <summary>
        /// <para>sender</para>
        /// <para>shipper</para>
        /// <para>receiver</para>
        /// <para>recipient</para>
        /// <para>others</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTakerType type { get; set; }

        /// <summary>
        /// <para>Indicator of the recipient's role in the service provision(tag indIEToma)</para>
        /// <para>taxpayer</para>
        /// <para>exempt_taxpayer</para>
        /// <para>non_taxpayer</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTakerTaxIdication takerTaxIdication { get; set; }

        public cteDataTakerTaker taker { get; set; }
    }
}