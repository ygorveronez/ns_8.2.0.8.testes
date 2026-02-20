using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataModalInformationWater
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModalMDFe modal { get; } = enumModalMDFe.water;

        /// <summary>
        /// Tag irin
        /// </summary>
        public string irin { get; set; }

        /// <summary>
        /// Tag tpEmb
        /// </summary>
        public string vesselTypeCode { get; set; }

        /// <summary>
        /// Tag cEmbar
        /// </summary>
        public string vesselCode { get; set; }

        /// <summary>
        /// Tag xEmbar
        /// </summary>
        public string vesselName { get; set; }

        /// <summary>
        /// Tag nViag
        /// </summary>
        public string journeyNumber { get; set; }

        /// <summary>
        /// Tag cPrtEmb
        /// </summary>
        public string embarkationPortCode { get; set; }

        /// <summary>
        /// Tag cPrtDest
        /// </summary>
        public string destinationPortCode { get; set; }

        /// <summary>
        /// Tag prTrans
        /// </summary>
        public string transshipmentPort { get; set; }

        /// <summary>
        /// Tag tpNav
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumNavigationType navigationType { get; set; }

        /// <summary>
        /// Tag infTermCarreg
        /// </summary>
        public List<mdfeDataModalInformationWaterLoadingTerminalsInfo> loadingTerminalsInfo { get; set; }

        /// <summary>
        /// Tag infTermDescarreg
        /// </summary>
        public List<mdfeDataModalInformationWaterUnloadingTerminalsInfo> unloadingTerminalsInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<mdfeDataModalInformationWaterConvoyVesselInfo> convoyVesselInfo { get; set; }

        /// <summary>
        /// Tag infEmbComb
        /// </summary>
        public List<mdfeDataModalInformationWaterEmptyCargoUnitInfo> emptyCargoUnitInfo { get; set; }

        /// <summary>
        /// Tag infUnidTranspVazia
        /// </summary>
        public List<mdfeDataModalInformationWaterEmptyTransportUnitInfo> emptyTransportUnitInfo { get; set; }
    }

    public class mdfeDataModalInformationWaterLoadingTerminalsInfo
    {
        /// <summary>
        /// Tag cTermCarreg
        /// </summary>
        public string loadingTerminalCode { get; set; }

        /// <summary>
        /// Tag xTermCarreg
        /// </summary>
        public string loadingTerminalName { get; set; }
    }

    public class mdfeDataModalInformationWaterUnloadingTerminalsInfo
    {
        /// <summary>
        /// Tag cTermDescarreg
        /// </summary>
        public string unloadingTerminalCode { get; set; }

        /// <summary>
        /// Tag xTermDescarreg
        /// </summary>
        public string unloadingTerminalName { get; set; }
    }

    public class mdfeDataModalInformationWaterConvoyVesselInfo
    {
        /// <summary>
        /// Tag cEmbComb
        /// </summary>
        public string convoyVesselCode { get; set; }

        /// <summary>
        /// Tag xBalsa
        /// </summary>
        public string ferryIdentifier { get; set; }
    }

    public class mdfeDataModalInformationWaterEmptyCargoUnitInfo
    {
        /// <summary>
        /// Tag idUnidCargaVazia
        /// </summary>
        public string emptyCargoUnitId { get; set; }

        /// <summary>
        /// Tag tpUnidCargaVazia
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumEmptyCargoUnitType emptyCargoUnitType { get; set; }
    }

    public class mdfeDataModalInformationWaterEmptyTransportUnitInfo
    {
        /// <summary>
        /// Tag idUnidTranspVazia
        /// </summary>
        public string emptyTransportUnitId { get; set; }

        /// <summary>
        /// Tag tpUnidTranspVazia
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumEmptyTransportUnitType emptyTransportUnitType { get; set; }
    }
}