namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEventoApp
    {
        SendPosition = 1,
        EventsSubmit = 2,
        DeliveryReceiptCreate = 3,
        ChatSendMessage = 4,
        OccurrenceCreate = 5,
        DriverReceiptCreate = 6,
        DriverFreightContactCreate = 7,
        DriverOccurrenceCreate = 8,
        NotDelivered = 9,
        PartialDelivery = 10
    }

    public static class TipoEventoAppHelper
    {
        public static string ObterDescricao(this TipoEventoApp tipo)
        {
            switch (tipo)
            {
                case TipoEventoApp.SendPosition:
                    return "Tracking Posições";
                case TipoEventoApp.EventsSubmit:
                    return "Envio de Evento";
                case TipoEventoApp.DeliveryReceiptCreate:
                case TipoEventoApp.DriverReceiptCreate:
                    return "Envio de Evidencias";
                case TipoEventoApp.ChatSendMessage:
                    return "Chat";
                case TipoEventoApp.OccurrenceCreate:
                    return "Criação de Ocorrência";
                case TipoEventoApp.DriverFreightContactCreate:
                    return "Criação Interesse de Frete";
                case TipoEventoApp.DriverOccurrenceCreate:
                    return "Criação de Ocorrência V3";
                case TipoEventoApp.NotDelivered:
                    return "Não Entregue";
                case TipoEventoApp.PartialDelivery:
                    return "Entrega Parcial";
                default: return string.Empty;
            }
        }

        public static TipoEventoApp ObterEnumPelaDescricao(string evento)
        {
            switch (evento)
            {
                case "tracking.driver.send-position":
                    return TipoEventoApp.SendPosition;
                case "ecosystem.travel-manager.events.submit":
                    return TipoEventoApp.EventsSubmit;
                case "ecosystem.travel-manager.delivery-receipt.create":
                    return TipoEventoApp.DeliveryReceiptCreate;
                case "ecosystem.travel-manager.driver-receipt.create":
                    return TipoEventoApp.DriverReceiptCreate;
                case "ecosystem.core.chat.send-message":
                    return TipoEventoApp.ChatSendMessage;
                case "ecosystem.core.occurrence.create":
                    return TipoEventoApp.OccurrenceCreate;
                case "ecosystem.freight-manager.driver-freight.contact.create":
                    return TipoEventoApp.DriverFreightContactCreate;
                case "ecosystem.core.driver-occurrence.create":
                    return TipoEventoApp.DriverOccurrenceCreate;
                case "ecosystem.travel-manager.not-delivered.create":
                    return TipoEventoApp.NotDelivered;
                case "ecosystem.travel-manager.partial-delivery.create":
                    return TipoEventoApp.PartialDelivery;
                default: return 0;
            }
        }
    }
}
