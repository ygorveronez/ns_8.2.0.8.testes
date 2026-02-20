using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteEvento
    {
        public cteEventoData data { get; set; }

        public cteOptions options { get; set; }
    }

    public class cteEventoData
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
        /// Tag chCTe
        /// </summary>
        public string cteKey { get; set; }

        /// <summary>
        /// Tag nSeqEvento
        /// </summary>
        public int eventSequence { get; set; }

        /// <summary>
        /// <para>CTe event issue method</para>
        /// <para>default</para>
        /// <para>production</para>
        /// <para>svc</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssueType issueType { get; set; }

        public cteEventoIssuer issuer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("event")]
        public object evento { get; set; }
    }

    public class cteEventoIssuer
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

    public class cteEventoIssuerCorrectionLetter
    {
        /// <summary>
        /// Tag descEvento
        /// </summary>
        public string eventName { get; } = "correction_letter";

        /// <summary>
        /// Tag infCorrecao
        /// </summary>
        public List<cteEventoIssuerCorrectionLetterFields> fields { get; set; }
    }

    public class cteEventoIssuerCancel
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

    public class cteEventoIssuerCorrectionLetterFields
    {
        /// <summary>
        /// Tag grupoAlterado
        /// </summary>
        public string group { get; set; }

        /// <summary>
        /// Tag campoAlterado
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// Tag valorAlterado
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? itemIndex { get; set; }
    }
}