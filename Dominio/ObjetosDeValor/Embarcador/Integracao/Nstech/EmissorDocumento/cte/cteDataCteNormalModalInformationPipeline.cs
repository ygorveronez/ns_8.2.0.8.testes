using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{

    public class cteDataCteNormalModalInformationPipeline
    {
        /// <summary>
        /// Information on the Pipeline Transport Modal(tag duto)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModal modal { get; } = enumModal.pipeline;

        /// <summary>
        /// Tariff Amount(tag vTar)
        /// </summary>
        public decimal tariffAmount { get; set; }

        /// <summary>
        /// Service Start Date(tag dIni)
        /// </summary>
        public string startDate { get; set; }

        /// <summary>
        /// Service End Date(tag dFim)
        /// </summary>
        public string endDate { get; set; }
    }
}