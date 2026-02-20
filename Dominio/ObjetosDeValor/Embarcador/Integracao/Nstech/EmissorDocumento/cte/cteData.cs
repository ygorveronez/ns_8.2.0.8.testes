using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteData
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
        /// <para>normal</para>
        /// <para>complement</para>
        /// <para>substitution</para>
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
        /// Globalized CTe indicator(tag indGlobalizado)
        /// </summary>
        public bool globalized { get; set; }

        /// <summary>
        /// <para>Tag modal</para>
        /// <para>road</para>
        /// <para>air</para>
        /// <para>water</para>
        /// <para>rail</para>
        /// <para>pipeline</para>
        /// <para>multimodal</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModal modal { get; set; }

        /// <summary>
        /// <para>Service type(tag tpServ)</para>
        /// <para>normal</para>
        /// <para>subcontracting</para>
        /// <para>transshipment</para>
        /// <para>intermediate_transshipment</para>
        /// <para>multimodal_associated</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumServiceType serviceType { get; set; }

        public cteDataOriginCity originCity { get; set; }

        public cteDataDestinationCity destinationCity { get; set; }

        /// <summary>
        /// Indicator if the recipient picks up at the airport, branch, port, or destination station(tag retira)
        /// </summary>
        public bool pickup { get; set; }

        /// <summary>
        /// Pickup details(tag xDetRetira)
        /// </summary>
        public string pickupDetail { get; set; }

        /// <summary>
        /// <para>Indicator of the recipient's role in the service provision(tag indIEToma)</para>
        /// <para>taxpayer</para>
        /// <para>exempt_taxpayer</para>
        /// <para>non_taxpayer</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTakerTaxIdication takerTaxIdication { get; set; }

        public cteDataIssuer issuer { get; set; }

        public cteDataTaker taker { get; set; }

        public cteDataSender sender { get; set; }

        public cteDataShipper shipper { get; set; }

        public cteDataReceiver receiver { get; set; }

        public cteDataRecipient recipient { get; set; }

        public cteDataAdditionalData additionalData { get; set; }

        /// <summary>
        /// Service Provision Values(tag vPrest)
        /// </summary>
        public cteDataServiceAmount serviceAmount { get; set; }

        public cteDataTax tax { get; set; }

        /// <summary>
        /// Group of information for regular and substitute CTe(tag infCTeNorm)
        /// </summary>
        public cteDataCteNormal cteNormal { get; set; }

        /// <summary>
        /// Details of the complemented CTe(tag infCteComp)
        /// </summary>
        public List<cteDataCteComplement> cteComplement { get; set; }

        /// <summary>
        /// Authorized to download the DF-e XML(tag autXML)
        /// </summary>
        public List<cteDataDownloadAuthorization> downloadAuthorization { get; set; }

        public cteDataTechnicalManager technicalManager { get; set; }
    }
}