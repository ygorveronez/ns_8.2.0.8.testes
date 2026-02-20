using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteSimplifiedDataDelivery
    {
        /// <summary>
        /// itemNumber
        /// </summary>
        public int itemNumber { get; set; }

        public cteSimplifiedDataDeliveryCity originCity { get; set; }

        public cteSimplifiedDataDeliveryCity destinationCity { get; set; }

        /// <summary>
        /// Total Service Provision Value(tag vTPrest)
        /// </summary>
        public decimal serviceAmount { get; set; }

        /// <summary>
        /// Amount to Receive(tag vRec)
        /// </summary>
        public decimal receivableServiceAmount { get; set; }

        /// <summary>
        /// Components of the Service Value(tag Comp)
        /// </summary>
        public List<cteDataServiceAmountServiceCompoment> serviceCompoment { get; set; }

        /// <summary>
        /// <para>Objetos: cteSimplifiedDataDeliveryDocumentsNFe, cteSimplifiedDataDeliveryDocumentsBefore</para>
        /// </summary>
        public object documents { get; set; }
    }
}