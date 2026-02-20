using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteSimplifiedDataDeliveryDocumentsBefore
    {
        /// <summary>
        /// nfe
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDocumentTypeSimplified type { get; } = enumDocumentTypeSimplified.before;

        /// <summary>
        /// documents
        /// </summary>
        public List<cteSimplifiedDataDeliveryDocumentsBeforeDocuments> documents { get; set; }
    }

    public class cteSimplifiedDataDeliveryDocumentsBeforeDocuments
    {
        /// <summary>
        /// CTe Access Key(tag cteKey)
        /// </summary>
        public string cteKey { get; set; }

        /// <summary>
        /// <para>Objetos: cteSimplifiedDataDeliveryDocumentsBeforeDocumentsDeliveryPartial, cteSimplifiedDataDeliveryDocumentsBeforeDocumentsDeliveryTotal</para>
        /// </summary>
        public object delivery { get; set; }
    }

    public class cteSimplifiedDataDeliveryDocumentsBeforeDocumentsDeliveryTotal
    {
        /// <summary>
        /// total
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDeliveryDocumentTypeSimplified type { get; } = enumDeliveryDocumentTypeSimplified.total;
    }

    public class cteSimplifiedDataDeliveryDocumentsBeforeDocumentsDeliveryPartial
    {
        /// <summary>
        /// partial
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDeliveryDocumentTypeSimplified type { get; } = enumDeliveryDocumentTypeSimplified.partial;

        public List<cteSimplifiedDataDeliveryDocumentsBeforeDocumentsDeliveryPartialNFe> nfe { get; set; }
    }

    public class cteSimplifiedDataDeliveryDocumentsBeforeDocumentsDeliveryPartialNFe
    {
        public string nfeKey { get; set; }
    }
}