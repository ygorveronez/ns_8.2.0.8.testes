using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataDocInfoUnloadCityInfoInfCteLoadUnit
    {
        /// <summary>
        /// Tag tpUnidCarga
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumLoadUnitType loadUnitType { get; set; }

        /// <summary>
        /// Tag idUnidCarga
        /// </summary>
        public string loadUnitId { get; set; }

        /// <summary>
        /// Tag lacUnidCarga
        /// </summary>
        public List<string> loadUnitSeal { get; set; }

        /// <summary>
        /// Tag qtdRat
        /// </summary>
        public int? allocatedQty { get; set; }
    }
}