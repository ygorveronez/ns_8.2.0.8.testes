using Dominio.ObjetosDeValor.Embarcador.Atendimento;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{

    public class cteDataCteNormalModalInformationRoad
    {
        /// <summary>
        /// Road Transportation Modal Information(tag rodo)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModal modal { get; } = enumModal.road;

        /// <summary>
        /// Tag RNTRC
        /// </summary>
        public string rntrc { get; set; }

        /// <summary>
        /// Associated Collection Orders(tag occ)
        /// </summary>
        public List<cteDataCteNormalModalInformationRoadCollectionOrders> collectionOrders { get; set; }
    }

    public class cteDataCteNormalModalInformationRoadCollectionOrders
    {
        /// <summary>
        /// OCC Series(tag serie)
        /// </summary>
        public string serie { get; set; }

        /// <summary>
        /// OCC Number(tag nOcc)
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Collection Order Issue Date(tag dEmi)
        /// </summary>
        public string issueDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public cteDataCteNormalModalInformationRoadCollectionOrdersIssuer issuer {get; set;}
    }

    public class cteDataCteNormalModalInformationRoadCollectionOrdersIssuer
    {
        /// <summary>
        /// Tag CNPJ
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag fone
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// State Registration(tag IE)
        /// </summary>
        public string stateRegistration { get; set; }

        /// <summary>
        /// State Abbreviation(tag UF)
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// Internal Code for Carrier Use(tag cInt)
        /// </summary>
        public string internalCode { get; set; }
    }
}