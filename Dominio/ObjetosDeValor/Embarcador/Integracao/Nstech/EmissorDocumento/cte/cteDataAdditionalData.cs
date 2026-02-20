using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataAdditionalData
    {
        /// <summary>
        /// Additional transportation feature(tag xCaracAd)
        /// </summary>
        public string additionalCharacteristicTransport { get; set; }

        /// <summary>
        /// Additional Service Feature(tag xCaracSer)
        /// </summary>
        public string additionalCharacteristicService { get; set; }

        /// <summary>
        /// CTe Issuing Employee(tag eEmi)
        /// </summary>
        public string issuingEmployee { get; set; }

        /// <summary>
        /// Cargo Flow Forecast(tag fluxo)
        /// </summary>
        public cteDataAdditionalDataFlow flow { get; set; }

        /// <summary>
        /// <para>Delivery without a defined date(tag semData)</para>
        /// <para>Delivery with a defined date(tag comData)</para>
        /// <para>Delivery within the defined period(tag noPeriodo)</para>
        /// </summary>
        public cteDataAdditionalDataDeliveryDate deliveryDate { get; set; }

        /// <summary>
        /// <para>Delivery without defined time(tag semHora)</para>
        /// <para>Delivery with defined time(tag comHora)</para>
        /// <para>Delivery within the defined time range(tag noInter)</para>
        /// </summary>
        public cteDataAdditionalDataDeliveryTime deliveryTime { get; set; }

        /// <summary>
        /// Origin Municipality for Freight Calculation(tag origCalc)
        /// </summary>
        public string commercialOriginCity { get; set; }

        /// <summary>
        /// Destination Municipality for Freight Calculation(tag destCalc)
        /// </summary>
        public string commercialDestinationCity { get; set; }

        /// <summary>
        /// General Remarks(tag xObs)
        /// </summary>
        public string observation { get; set; }

        /// <summary>
        /// Free Use Field for the Taxpayer(tag ObsCont)
        /// </summary>
        public List<cteDataAdditionalDataIssuerObservation> issuerObservation { get; set; }

        /// <summary>
        /// Free Use Field for the Taxpayer(tag ObsFisco)
        /// </summary>
        public List<cteDataAdditionalDataFiscalObservation> fiscalObservation { get; set; }
    }

    public class cteDataAdditionalDataFlow
    {
        /// <summary>
        /// Acronym or Internal Code of the Branch/Port/Station/Airport of Origin(tag xOrig)
        /// </summary>
        public string originAcronym { get; set; }

        /// <summary>
        /// Tag pass
        /// </summary>
        public List<cteDataAdditionalDataFlowTransit> transit { get; set; }

        /// <summary>
        /// Acronym or Internal Code of the Branch/Port/Station/Airport of Destination(tag xDest)
        /// </summary>
        public string destinationAcronym { get; set; }

        /// <summary>
        /// Delivery Route Code(tag xRota)
        /// </summary>
        public string deliveryRoute { get; set; }
    }

    public class cteDataAdditionalDataFlowTransit
    {
        /// <summary>
        /// Acronym or Internal Code of the Branch/Port/Station/Airport of Transit(tag xPass)
        /// </summary>
        public string transitAcronym { get; set; }
    }

    public class cteDataAdditionalDataDeliveryDate
    {
        /// <summary>
        /// <para>Type of date/period scheduled for delivery(tag tpPer)</para>
        /// <para>no_defined_date</para>
        /// <para>on_defined_date</para>
        /// <para>until_defined_date</para>
        /// <para>from_defined_date</para>
        /// <para>defined_period</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDeliveryDateType deliveryDateType { get; set; }

        /// <summary>
        /// on_defined_date, until_defined_date, from_defined_date - Scheduled date(tag dProg)
        /// </summary>
        public string deliveryDate { get; set; }

        /// <summary>
        /// defined_period - Start date(tag dIni)
        /// </summary>
        public string startDeliveryDate { get; set; }

        /// <summary>
        /// defined_period - End date(tag dFim)
        /// </summary>
        public string endDeliveryDate { get; set; }
    }

    public class cteDataAdditionalDataDeliveryTime
    {
        /// <summary>
        /// <para>Type of time(tag tpHor)</para>
        /// <para>no_defined_time</para>
        /// <para>on_defined_time</para>
        /// <para>until_defined_time</para>
        /// <para>from_defined_time</para>
        /// <para>defined_period</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDeliveryTimeType deliveryTimeType { get; set; }

        /// <summary>
        /// on_defined_time, until_defined_time, from_defined_time - Scheduled time(tag hProg)
        /// </summary>
        public string deliveryTime { get; set; }

        /// <summary>
        /// defined_period - Start time(tag hIni)
        /// </summary>
        public string startDeliveryTime { get; set; }

        /// <summary>
        /// defined_period - End time(tag hFim)
        /// </summary>
        public string endDeliveryTime { get; set; }
    }

    public class cteDataAdditionalDataIssuerObservation
    {
        public string field { get; set; }
        public string text { get; set; }
    }

    public class cteDataAdditionalDataFiscalObservation
    {
        public string field { get; set; }
        public string text { get; set; }
    }
}