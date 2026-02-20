using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeData
    {
        public string externalId { get; set; }

        /// <summary>
        /// <para>Tag tpEmit</para>
        /// <para>service_provider</para>
        /// <para>own_cargo</para>
        /// <para>globalized_cte</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssuerTypeMDFe issuerType { get; set; }

        /// <summary>
        /// <para>Tag tpTransp</para>
        /// <para>ETC</para>
        /// <para>TAC</para>
        /// <para>CTC</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTransporterType? transporterType { get; set; }

        /// <summary>
        /// Tag serie
        /// </summary>
        public int? serie { get; set; }

        /// <summary>
        /// Tag nMDF
        /// </summary>
        public int? number { get; set; }

        /// <summary>
        /// Tag cMDF
        /// </summary>
        public Int64? numericCode { get; set; }

        /// <summary>
        /// Tag modal
        /// <para>road</para>
        /// <para>air</para>
        /// <para>water</para>
        /// <para>rail</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModalMDFe modal { get; set; }

        /// <summary>
        /// Tag dhEmi
        /// </summary>
        public string issueDate { get; set; }

        /// <summary>
        /// Tag tpEmis
        /// <para>default</para>
        /// <para>offline_contingency</para>
        /// <para>nff_special_regime</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssueTypeMDFe? issueType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumOfflineEmissionMode? offlineEmissionMode { get; set; }

        /// <summary>
        /// Tag UFIni
        /// </summary>
        public string originState { get; set; }

        /// <summary>
        /// Tag UFFIm
        /// </summary>
        public string destinationState { get; set; }

        /// <summary>
        /// Tag infMunCarrega
        /// </summary>
        public List<mdfeDataLoadingCitiesInfo> loadingCitiesInfo { get; set; }

        /// <summary>
        /// Tag infPercurso
        /// </summary>
        public List<string> routeInfo { get; set; }

        /// <summary>
        /// Tag dhIniViagem
        /// </summary>
        public string startDate { get; set; }

        /// <summary>
        /// indCanalVerde
        /// </summary>
        public string indCanalVerde { get; set; }

        /// <summary>
        /// Tag indCarregaPosterior
        /// </summary>
        public string subsequentLoad { get; set; }

        /// <summary>
        /// Tag emit
        /// </summary>
        public mdfeDataIssuer issuer { get; set; }

        /// <summary>
        /// <para>modal</para>
        /// <para>Objetos: mdfeDataModalInformationRoad, mdfeDataModalInformationAir,</para>
        /// <para>         mdfeDataModalInformationWater, mdfeDataModalInformationRail</para>
        /// </summary>
        public object modalInformation { get; set; }

        /// <summary>
        /// Tag infDoc
        /// </summary>
        public mdfeDataDocInfo docInfo { get; set; }

        /// <summary>
        /// Tag seg
        /// </summary>
        public List<mdfeDataInsurance> insurance { get; set; }

        /// <summary>
        /// Tag prodPred
        /// </summary>
        public mdfeDataPredProduct predProduct { get; set; }

        /// <summary>
        /// Tag tot
        /// </summary>
        public mdfeDataTot tot { get; set; }

        /// <summary>
        /// Tag lacres
        /// </summary>
        public List<string> seal { get; set; }

        /// <summary>
        /// Tag autXML
        /// </summary>
        public List<mdfeDataDownloadAuthorization> downloadAuthorization { get; set; }

        /// <summary>
        /// Tag infAdic
        /// </summary>
        public mdfeDataAdditionalInfo additionalInfo { get; set; }

        /// <summary>
        /// Tag infRespTec
        /// </summary>
        public mdfeDataTechnicalManager technicalManager { get; set; }

        /// <summary>
        /// Tag infSolicNFF
        /// </summary>
        public mdfeDataNffInfo nffInfo { get; set; }

        /// <summary>
        /// Tag infPAA
        /// </summary>
        public mdfeDataPaaInfo paaInfo { get; set; }

    }
}