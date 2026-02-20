using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataDocInfoUnloadCityInfoInfNfe
    {
        /// <summary>
        /// Tag chNFe
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// Tag SegCodBarra
        /// </summary>
        public string barcode { get; set; }

        /// <summary>
        /// Tag indReentrega
        /// </summary>
        public string redeliveryIndicator { get; set; }

        /// <summary>
        /// Tag infUnidTransp
        /// </summary>
        public List<mdfeDataDocInfoUnloadCityInfoInfCteTransportUnitInfo> transportUnitInfo { get; set; }

        /// <summary>
        /// Tag peri
        /// </summary>
        public mdfeDataDocInfoUnloadCityInfoInfCteHazmat hazmat { get; set; }
    }
}