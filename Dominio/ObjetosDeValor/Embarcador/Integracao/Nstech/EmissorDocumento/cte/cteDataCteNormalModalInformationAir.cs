using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{

    public class cteDataCteNormalModalInformationAir
    {
        /// <summary>
        /// Air Transportation Modal Information(tag aereo)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModal modal { get; } = enumModal.air;

        /// <summary>
        /// Draft Number(tag nMinu)
        /// </summary>
        public string draftNumber { get; set; }

        /// <summary>
        /// Operational Number of the Air Waybill(tag nOCA)
        /// </summary>
        public string operationalNumber { get; set; }

        /// <summary>
        /// Expected Delivery Date(tag dPrevAereo)
        /// </summary>
        public string deliveryDate { get; set; }

        /// <summary>
        /// Dimension(tag xDime)
        /// </summary>
        public string dimension { get; set; }

        /// <summary>
        /// Handling Information(tag cInfManu)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public List<enumHandlingInformation> handlingInformation { get; set; }

        /// <summary>
        /// <para>Class(tag CL)</para>
        /// <para>minimum_rate</para>
        /// <para>general_rate</para>
        /// <para>specific_rate</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTariffClass tariffClass { get; set; }

        /// <summary>
        /// Tariff Code(tag cTar)
        /// </summary>
        public string tariffCode { get; set; }

        /// <summary>
        /// Tariff Amount(tag vTar)
        /// </summary>
        public decimal tariffAmount { get; set; }

        /// <summary>
        /// Filled out when transporting products classified by the UN as hazardous(tag peri)
        /// </summary>
        public List<cteDataCteNormalModalInformationAirDangerousProduct> dangerousProduct { get; set; }
    }

    public class cteDataCteNormalModalInformationAirDangerousProduct
    {
        /// <summary>
        /// Tag nONU
        /// </summary>
        public string onuNumber { get; set; }

        /// <summary>
        /// Total Quantity of Packages Containing Hazardous Goods(tag qTotEmb)
        /// </summary>
        public string quantityVolumes { get; set; }

        /// <summary>
        /// Total Quantity of Hazardous Goods(tag qTotProd)
        /// </summary>
        public decimal quantityDangerousProduct { get; set; }

        /// <summary>
        /// <para>Unit of Measure(tag uniAP)</para>
        /// <para>kg</para>
        /// <para>kgg</para>
        /// <para>litros</para>
        /// <para>ti</para>
        /// <para>unidades</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDangerousUnit dangerousUnit { get; set; }
    }
}