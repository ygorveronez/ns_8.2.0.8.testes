using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento
{
    public class DataDeliveryReceiptCreate : SuperAppData
    {
        public StoppingPointDocument StoppingPointDocument { get; set; }
        public bool Checked { get; set; }
        public List<Evidence> Evidences { get; set; }
        public Location Location { get; set; }
    }

    public class StoppingPointDocument
    {
        public string _id { get; set; }
        public string ExternalId { get; set; }
    }
}
