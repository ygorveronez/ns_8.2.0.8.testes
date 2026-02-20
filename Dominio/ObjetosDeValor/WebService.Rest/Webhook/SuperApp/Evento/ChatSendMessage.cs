using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento
{
    public class DataChatSendMessage : SuperAppData
    {
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public Chat Chat { get; set; }
    }
}
