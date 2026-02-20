using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataCteNormal
    {
        /// <summary>
        /// Cargo Information of the CTe(tag infCarga)
        /// </summary>
        public cteDataCteNormalCargoInformation cargoInformation { get; set; }

        /// <summary>
        /// <para>Information on Documents Transported by the CTe Optional for Intermediate Relay and Service Linked to Multimodal Transport(tag infDoc)</para>
        /// <para>Objetos: cteDataCteNormalDocumentNf, cteDataCteNormalDocumentNfe, cteDataCteNormalDocumentOther</para>
        /// </summary>
        public object document { get; set; }

        /// <summary>
        /// <para>Previous Transport Documents(tag docAnt)</para>
        /// <para>Objetos: cteDataCteNormalBeforeDocumentElectronic, cteDataCteNormalBeforeDocumentPaper</para>
        /// </summary>
        public List<object> beforeDocument {get; set;}

        /// <summary>
        /// Information on Transported Vehicles(tag veicNovos)
        /// </summary>
        public List<cteDataCteNormalNewVehicles> newVehicles { get; set; }

        /// <summary>
        /// CTe Billing Information(tag cobr)
        /// </summary>
        public cteDataCteNormalBilling billing { get; set; }

        /// <summary>
        /// Substitution CTe Information(tag infCteSub)
        /// </summary>
        public cteDataCteNormalSubstitutionCte substitutionCte { get; set; }

        /// <summary>
        /// Globalized CTe Information(tag infGlobalizado)
        /// </summary>
        public string globalizedDescription { get; set; }

        /// <summary>
        /// Linked multimodal CTe information(tag infCTeMultimodal)
        /// </summary>
        public List<cteDataCteNormalMultimodalCTe> multimodalCTe { get; set; }

        /// <summary>
        /// <para>modal</para>
        /// <para>Objetos: cteDataCteNormalModalInformationRoad, cteDataCteNormalModalInformationMultimodal, cteDataCteNormalModalInformationAir,</para>
        /// <para>         cteDataCteNormalModalInformationWater, cteDataCteNormalModalInformationRail, cteDataCteNormalModalInformationPipeline</para>
        /// </summary>
        public object modalInformation { get; set; }
    }

    public class cteDataCteNormalCargoInformation
    {
        /// <summary>
        /// Total Cargo Value(tag vCarga)
        /// </summary>
        public decimal cargoAmount { get; set; }

        /// <summary>
        /// Predominant Product(tag proPred)
        /// </summary>
        public string predominantProduct { get; set; }

        /// <summary>
        /// Other Cargo Characteristics(tag xOutCat)
        /// </summary>
        public string otherCharacteristics { get; set; }

        /// <summary>
        /// Cargo Quantity Information of the CTe(tag infQ)
        /// </summary>
        public List<cteDataCteNormalCargoInformationQuantity> quantity { get; set; }

        /// <summary>
        /// Cargo Value for Insurance Purposes(tag vCargaAverb)
        /// </summary>
        public decimal endorsementAmount { get; set; }
    }

    public class cteDataCteNormalCargoInformationQuantity
    {
        /// <summary>
        /// <para>Unit of Measure Code(tag cUnid)</para>
        /// <para>m3</para>
        /// <para>kg</para>
        /// <para>ton</para>
        /// <para>unidade</para>
        /// <para>litros</para>
        /// <para>mmbtu</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumQuantityUnit unit { get; set; }

        /// <summary>
        /// Type of Measurement(tag tpMed)
        /// </summary>
        public string typeMeasure { get; set; }

        /// <summary>
        /// Quantity(tag qCarga)
        /// </summary>
        public decimal quantity { get; set; }
    }

    public class cteDataCteNormalNewVehicles
    {
        /// <summary>
        /// Vehicle Chassis(tag chassi)
        /// </summary>
        public string chassi { get; set; }

        /// <summary>
        /// Vehicle Color(tag cCor)
        /// </summary>
        public string colorCode { get; set; }

        /// <summary>
        /// Color Description(tag xCor)
        /// </summary>
        public string colorName { get; set; }

        /// <summary>
        /// Brand Model Code(tag cMod)
        /// </summary>
        public string mrandModelCode { get; set; }

        /// <summary>
        /// Unit Value of the Vehicle(tag vUnit)
        /// </summary>
        public decimal unitAmount { get; set; }

        /// <summary>
        /// Unit Freight(tag vFrete)
        /// </summary>
        public decimal freightAmount { get; set; }
    }

    public class cteDataCteNormalBilling
    {
        /// <summary>
        /// Invoice Number(tag nFat)
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Original Invoice Amount(tag vOrig)
        /// </summary>
        public decimal originalAmount { get; set; }

        /// <summary>
        /// Invoice Discount Amount(tag vDesc)
        /// </summary>
        public decimal discountAmount { get; set; }

        /// <summary>
        /// Net Invoice Amount(tag vLiq)
        /// </summary>
        public decimal netAmount { get; set; }

        /// <summary>
        /// Bill of Exchange Information(tag dup)
        /// </summary>
        public List<cteDataCteNormalBillingDuplicates> duplicates { get; set; }
    }

    public class cteDataCteNormalBillingDuplicates
    {
        /// <summary>
        /// Bill of Exchange Number(tag nDup)
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Due Date of the Bill of Exchange(tag dVenc)
        /// </summary>
        public string expirationOn { get; set; }

        /// <summary>
        /// Bill of Exchange Amount(tag vDup)
        /// </summary>
        public decimal amount { get; set; }
    }

    public class cteDataCteNormalSubstitutionCte
    {
        /// <summary>
        /// Tag chCte
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// CTe Recipient Change Indicator(tag indAlteraToma)
        /// </summary>
        public bool takerChange { get; set; }
    }

    public class cteDataCteNormalMultimodalCTe
    {
        /// <summary>
        /// Multimodal CTe access key(tag chCTeMultimodal)
        /// </summary>
        public string key { get; set; }
    }
}