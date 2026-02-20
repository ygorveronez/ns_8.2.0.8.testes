using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{

    public class cteDataCteNormalModalInformationMultimodal
    {
        /// <summary>
        /// Multimodal Information(tag multimodal)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModal modal { get; } = enumModal.multimodal;

        /// <summary>
        /// Multimodal Transport Operator Certificate Number(tag COTM)
        /// </summary>
        public string multimodalNumber { get; set; }

        /// <summary>
        /// Negotiable Indicator(tag indNegociavel)
        /// </summary>
        public bool allowsNegotiation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public cteDataCteNormalModalInformationMultimodalInsurance insurance { get; set; }
    }

    public class cteDataCteNormalModalInformationMultimodalInsurance
    {
        /// <summary>
        /// Insurance Company Name(tag xSeg)
        /// </summary>
        public string insurerName { get; set; }

        /// <summary>
        /// Insurance Company CNPJ Number(tag CNPJ)
        /// </summary>
        public string insurerDocument { get; set; }

        /// <summary>
        /// Policy Number(tag nApol)
        /// </summary>
        public string policyNumber { get; set; }

        /// <summary>
        /// Endorsement Number(tag nAver)
        /// </summary>
        public string endorsementNumber { get; set; }
    }
}