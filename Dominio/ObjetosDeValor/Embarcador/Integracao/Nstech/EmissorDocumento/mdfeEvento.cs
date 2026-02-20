using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeEvento
    {
        public mdfeEventoData data { get; set; }

        public mdfeOptions options { get; set; }
    }

    public class mdfeEventoData
    {
        /// <summary>
        /// 
        /// </summary>
        public string externalId { get; set; }

        /// <summary>
        /// Tag dhEvento
        /// </summary>
        public string eventDate { get; set; }

        /// <summary>
        /// Tag chMDFe
        /// </summary>
        public string mdfeKey { get; set; }

        /// <summary>
        /// Tag nSeqEvento
        /// </summary>
        public int eventSequence { get; set; }

        public mdfeEventoIssuer issuer { get; set; }

        /// <summary>
        /// <para>modal</para>
        /// <para>Objetos: mdfeEventoIssuerCancel, mdfeEventoIssuerClose, mdfeEventoIssuerIncludeDriver</para>
        /// </summary>
        [JsonProperty("event")]
        public object evento { get; set; }

        /// <summary>
        /// Tag infSolicNFF
        /// </summary>
        public mdfeDataNffInfo nffInfo { get; set; }

        /// <summary>
        /// Tag infPAA
        /// </summary>
        public mdfeDataPaaInfo paaInfo { get; set; }
    }

    public class mdfeEventoIssuer
    {
        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssuerType type { get; set; }

        /// <summary>
        /// <para>legal - Tag CNPJ</para>
        /// <para>individual - Tag CPF</para>
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag UF
        /// </summary>
        public string state { get; set; }
    }

    public class mdfeEventoIssuerCancel
    {
        /// <summary>
        /// Tag descEvento
        /// </summary>
        public string eventName { get; } = "cancel";

        /// <summary>
        /// Tag nProt
        /// </summary>
        public string protocolNumber { get; set; }

        /// <summary>
        /// Tag nProt
        /// </summary>
        public string reason { get; set; }
    }

    public class mdfeEventoIssuerClose
    {
        /// <summary>
        /// Tag descEvento
        /// </summary>
        public string eventName { get; } = "close";

        /// <summary>
        /// Tag nProt
        /// </summary>
        public string protocolNumber { get; set; }

        /// <summary>
        /// Tag dtEnc
        /// </summary>
        public string closeDate { get; set; }

        /// <summary>
        /// Tag cUF
        /// </summary>
        public string closeState { get; set; }

        /// <summary>
        /// Tag cMun
        /// </summary>
        public string closeCity { get; set; }

        /// <summary>
        /// Tag indEncPorTerceiro
        /// </summary>
        public string closeByThird { get; set; }
    }

    public class mdfeEventoIssuerIncludeDriver
    {
        /// <summary>
        /// Tag descEvento
        /// </summary>
        public string eventName { get; } = "include_driver";

        /// <summary>
        /// Tag nProt
        /// </summary>
        public string protocolNumber { get; set; }

        /// <summary>
        /// Tag condutor
        /// </summary>
        public mdfeEventoIssuerIncludeDriverDriver driver { get; set; }
    }

    public class mdfeEventoIssuerIncludeDriverDriver
    {
        /// <summary>
        /// Tag CPF
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag xNome
        /// </summary>
        public string name { get; set; }
    }
}