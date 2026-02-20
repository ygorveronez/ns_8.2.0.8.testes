using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{

    public class cteDataCteNormalModalInformationRail
    {
        /// <summary>
        /// Rail Transportation Modal Information(tag ferrov)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModal modal { get; } = enumModal.rail;

        /// <summary>
        /// <para>Traffic Type(tag tpTraf)</para>
        /// <para>proprio</para>
        /// <para>mutuo</para>
        /// <para>rodoferroviario</para>
        /// <para>rodoviario</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTrafficType trafficType { get; set; }

        /// <summary>
        /// Details of Information for Mutual Traffic(tag trafMut)
        /// </summary>
        public cteDataCteNormalModalInformationRailMutualTraffic mutualTraffic { get; set; }

        /// <summary>
        /// Railway Flow(tag fluxo)
        /// </summary>
        public string railwayFlow { get; set; }
    }

    public class cteDataCteNormalModalInformationRailMutualTraffic
    {
        /// <summary>
        /// <para>Billing Responsible(tag respFat)</para>
        /// <para>ferrovia_origem</para>
        /// <para>ferrovia_destino</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumBillingResponsible billingResponsible { get; set; }

        /// <summary>
        /// <para>Railway Issuer of the CTe(tag ferrEmi)</para>
        /// <para>ferrovia_origem</para>
        /// <para>ferrovia_destino</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssueResponsible issueResponsible { get; set; }

        /// <summary>
        /// Freight Value of Mutual Traffic(tag vFrete)
        /// </summary>
        public decimal freightAmount { get; set; }

        /// <summary>
        /// Access Key of the CTe Issued by the Originating Railway(tag chCTeFerroOrigem)
        /// </summary>
        public string cteKeyOriginrailway { get; set; }

        /// <summary>
        /// Information on the Involved Railways(tag ferroEnv)
        /// </summary>
        public List<cteDataCteNormalModalInformationRailMutualTrafficRailways> railways { get; set; }
    }

    public class cteDataCteNormalModalInformationRailMutualTrafficRailways
    {
        /// <summary>
        /// <para>legal</para>
        /// <para>foreign</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumRailwaysType type { get; set; }

        /// <summary>
        /// legal - Tag CNPJ
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Business name or name(tag xNome)
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// State Registration(tag IE)
        /// </summary>
        public string stateRegistration { get; set; }

        /// <summary>
        /// Internal Code of the Involved Railway(tag cInt)
        /// </summary>
        public string internalCode { get; set; }

        /// <summary>
        /// address
        /// </summary>
        public cteDataCteNormalModalInformationRailMutualTrafficRailwaysAddress address { get; set; }
    }

    public class cteDataCteNormalModalInformationRailMutualTrafficRailwaysAddress
    {
        /// <summary>
        /// Street address(tag xLgr)
        /// </summary>
        public string street { get; set; }

        /// <summary>
        /// Address number(tag nro)
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Address complement(tag xCpl)
        /// </summary>
        public string complement { get; set; }

        /// <summary>
        /// Neighborhood of the address(tag xBairro)
        /// </summary>
        public string neighborhood { get; set; }

        /// <summary>
        /// ZIP Code(tag CEP)
        /// </summary>
        public string zipCode { get; set; }

        /// <summary>
        /// city
        /// </summary>
        public cteDataCteNormalModalInformationRailMutualTrafficRailwaysAddressCity city { get; set; }
    }

    public class cteDataCteNormalModalInformationRailMutualTrafficRailwaysAddressCity
    {
        /// <summary>
        /// Municipality name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Municipality code
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// State (UF) of the municipality
        /// </summary>
        public string state { get; set; }
    }
}