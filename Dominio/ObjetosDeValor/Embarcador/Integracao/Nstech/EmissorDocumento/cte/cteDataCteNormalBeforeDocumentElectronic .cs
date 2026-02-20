using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataCteNormalBeforeDocumentElectronic
    {
        /// <summary>
        /// Issuer of the previous document(tag emiDocAnt)
        /// </summary>
        public cteDataCteNormalBeforeDocumentIssuer issuer { get; set; }

        /// <summary>
        /// electronic
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumBeforeDocumentType type { get; } = enumBeforeDocumentType.electronic;

        /// <summary>
        /// documents
        /// </summary>
        public List<cteDataCteNormalBeforeDocumentElectronicDocument> documents { get; set; }
    }

    public class cteDataCteNormalBeforeDocumentElectronicDocument
    {
        /// <summary>
        /// CTe Access Key(tag chCTe)
        /// </summary>
        public string key { get; set; }
    }

    public class cteDataCteNormalBeforeDocumentIssuer
    {
        /// <summary>
        /// legal
        /// individual
        /// foreign
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumParticipantType type { get; set; }

        /// <summary>
        /// legal - Tag CNPJ
        /// individual - Tag CPF
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Business name or name(tag xNome)
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Tag fone
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// State Registration(tag IE)
        /// </summary>
        public string stateRegistration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string state { get; set; }
    }
}