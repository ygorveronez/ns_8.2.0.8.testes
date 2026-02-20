using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento
{
    public class SalvarDevolucao : SuperAppData
    {
        public string _id { get; set; }
        public string Source { get; set; }
        public Category Reason { get; set; }
        public StoppingPointDocument StoppingPointDocument { get; set; }
        public List<StoppingPointDocumentItem> StoppingPointDocumentItems { get; set; }
        public Response? Response { get; set; }
    }

}
