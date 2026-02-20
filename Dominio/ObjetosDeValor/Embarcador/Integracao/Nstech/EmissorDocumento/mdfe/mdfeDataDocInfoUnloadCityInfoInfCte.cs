using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataDocInfoUnloadCityInfoInfCte
    {
        /// <summary>
        /// Tag chCTe
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
        public List<mdfeDataDocInfoUnloadCityInfoInfCteHazmat> hazmat { get; set; }

        /// <summary>
        /// Tag infEntregaParcial
        /// </summary>
        public mdfeDataDocInfoUnloadCityInfoInfCtePartialDeliveryInfo partialDeliveryInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public mdfeDataDocInfoUnloadCityInfoInfCtePartialPerformanceIndicator partialPerformanceIndicator { get; set; }
    }

    public class mdfeDataDocInfoUnloadCityInfoInfCteTransportUnitInfo
    {
        /// <summary>
        /// Tag tpUnidTransp
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTransportUnitTypeMDFe transportUnitType { get; set; }

        /// <summary>
        /// Tag idUnidTransp
        /// </summary>
        public string transportUnitId { get; set; }

        /// <summary>
        /// Tag lacUnidTransp
        /// </summary>
        public List<string> transportUnitSeal { get; set; }

        /// <summary>
        /// Tag infUnidCarga
        /// </summary>
        public List<mdfeDataDocInfoUnloadCityInfoInfCteLoadUnit> loadUnit { get; set; }
    }

    public class mdfeDataDocInfoUnloadCityInfoInfCteHazmat
    {
        /// <summary>
        /// Tag nONU
        /// </summary>
        public string onuNumber { get; set; }

        /// <summary>
        /// Tag xNomeAE
        /// </summary>
        public string productShippingName { get; set; }

        /// <summary>
        /// Tag xClaRisco
        /// </summary>
        public string hazardClass { get; set; }

        /// <summary>
        /// Tag grEmb
        /// </summary>
        public string packagingGroup { get; set; }

        /// <summary>
        /// Tag qTotProd
        /// </summary>
        public decimal totalQtyProduct { get; set; }

        /// <summary>
        /// Tag qVolTipo
        /// </summary>
        public string qtyVolumeType { get; set; }
    }

    public class mdfeDataDocInfoUnloadCityInfoInfCtePartialDeliveryInfo
    {
        /// <summary>
        /// Tag qtdTotal
        /// </summary>
        public decimal total { get; set; }

        /// <summary>
        /// Tag qtdParcial
        /// </summary>
        public decimal partial { get; set; }
    }

    public class mdfeDataDocInfoUnloadCityInfoInfCtePartialPerformanceIndicator
    {
        /// <summary>
        /// Tag indPrestacaoParcial
        /// </summary>
        public string partialInd { get; set; }

        /// <summary>
        /// Tag infNFePrestParcial
        /// </summary>
        public List<string> partialIndNFe { get; set; }
    }
}