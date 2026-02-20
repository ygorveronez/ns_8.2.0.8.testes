using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataTot
    {
        /// <summary>
        /// Tag qCTe
        /// </summary>
        public decimal cteQty { get; set; }

        /// <summary>
        /// Tag qNFe
        /// </summary>
        public decimal nfeQty { get; set; }

        /// <summary>
        /// Tag qMDFe
        /// </summary>
        public decimal mdfeQty { get; set; }

        /// <summary>
        /// Tag vCarga
        /// </summary>
        public decimal loadValue { get; set; }

        /// <summary>
        /// Tag cUnid
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumUnitCode unitCode { get; set; }

        /// <summary>
        /// Tag qCarga
        /// </summary>
        public decimal grossWeight { get; set; }
    }
}