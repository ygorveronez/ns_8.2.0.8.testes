using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteSimplifiedData
    {
        public string externalId { get; set; }

        /// <summary>
        /// Tax Code for Operations and Services(tag CFOP)
        /// </summary>
        public string operationNatureCode { get; set; }

        /// <summary>
        /// Nature of the operation(tag natOp)
        /// </summary>
        public string operationNature { get; set; }

        /// <summary>
        /// Numeric code that composes the access key(tag cCT)
        /// </summary>
        public Int64? numericCode { get; set; }

        /// <summary>
        /// CTe series(tag serie)
        /// </summary>
        public int? serie { get; set; }

        /// <summary>
        /// CTe number(tag nCT)
        /// </summary>
        public int? number { get; set; }

        /// <summary>
        /// CTe issue date and time(tag dhEmi)
        /// </summary>
        public string issueDate { get; set; }

        /// <summary>
        /// <para>DACTE print format(tag tpImp)</para>
        /// <para>portrait</para>
        /// <para>landscape</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumPrintFormat printFormat { get; set; }

        /// <summary>
        /// <para>CTe type(tag tpCTe)</para>
        /// <para>simplified</para>
        /// <para>simplified_substitution</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumCteType cteType { get; set; }

        /// <summary>
        /// <para>CTe issue method(tag tpEmis)</para>
        /// <para>default</para>
        /// <para>production</para>
        /// <para>svc</para>
        /// <para>epec</para>
        /// <para>fsda</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssueType issueType { get; set; }

        /// <summary>
        /// <para>Tag modal</para>
        /// <para>road</para>
        /// <para>air</para>
        /// <para>water</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModal modal { get; set; }

        /// <summary>
        /// <para>Service type(tag tpServ)</para>
        /// <para>normal</para>
        /// <para>subcontracting</para>
        /// <para>transshipment</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumServiceType serviceType { get; set; }

        public string originState { get; set; }

        public string destinationState { get; set; }

        /// <summary>
        /// Indicator if the recipient picks up at the airport, branch, port, or destination station(tag retira)
        /// </summary>
        public bool pickup { get; set; }

        /// <summary>
        /// Pickup details(tag xDetRetira)
        /// </summary>
        public string pickupDetail { get; set; }

        public cteDataAdditionalData additionalData { get; set; }

        public cteDataIssuer issuer { get; set; }

        public cteSimplifiedDataTaker taker { get; set; }

        /// <summary>
        /// Cargo Information of the CTe(tag infCarga)
        /// </summary>
        public cteSimplifiedDataCargoInformation cargoInformation { get; set; }

        public List<cteSimplifiedDataDelivery> delivery { get; set; }

        /// <summary>
        /// <para>modal</para>
        /// <para>Objetos: cteDataCteNormalModalInformationRoad, cteDataCteNormalModalInformationAir, cteDataCteNormalModalInformationWater</para>
        /// </summary>
        public object modalInformation { get; set; }

        /// <summary>
        /// CTe Billing Information(tag cobr)
        /// </summary>
        public cteDataCteNormalBilling billing { get; set; }

        /// <summary>
        /// Substitution CTe Information(tag infCteSub)
        /// </summary>
        public cteDataCteNormalSubstitutionCte substitutionCte { get; set; }

        public cteDataTax tax { get; set; }

        public cteSimplifiedDataTotal total { get; set; }

        /// <summary>
        /// Authorized to download the DF-e XML(tag autXML)
        /// </summary>
        public List<cteDataDownloadAuthorization> downloadAuthorization { get; set; }

        public cteDataTechnicalManager technicalManager { get; set; }
    }
}