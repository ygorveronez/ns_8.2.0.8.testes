namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaRateioSVM
    {
        Nenhum = 0,
        UmCTeMultimodalParaUmCTeAquaviario = 1,
        AgruparPorTerminalOrigemDestino = 2,
        AgruparPorBookingRemetenteDestinatarioExpedidorRecebedor = 3,
        AgruparPorRemetenteDestinatario = 4,
        AgruparPorSacado = 5,
        AgruparPorTerminalOrigemDestinoSacado = 6,
        AgruparPorBookingRemetenteDestinatarioExpedidorRecebedorSacado = 7,
        AgruparPorRemetenteDestinatarioSacado = 8
    }

    public static class FormaRateioSVMHelper
    {
        public static string ObterDescricao(this FormaRateioSVM forma)
        {
            switch (forma)
            {
                case FormaRateioSVM.Nenhum: return "Nenhum";
                case FormaRateioSVM.UmCTeMultimodalParaUmCTeAquaviario: return "1 CT-e Multimodal para 1 CT-e Aquaviário";
                case FormaRateioSVM.AgruparPorTerminalOrigemDestino: return "Agrupar por Terminal de Origem/Terminal de Destino";
                case FormaRateioSVM.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedor: return "Agrupar por Booking e Remetente/Destinatario e Expedidor/Recebedor";
                case FormaRateioSVM.AgruparPorRemetenteDestinatario: return "Agrupar por Remetente/Destinatario";
                case FormaRateioSVM.AgruparPorSacado: return "Agrupar por Sacado do CT-e Multimodal";
                case FormaRateioSVM.AgruparPorTerminalOrigemDestinoSacado: return "Agrupar por Terminal de Origem/Terminal de Destino e Sacado do CTe";
                case FormaRateioSVM.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedorSacado: return "Agrupar por Booking e Remetente/Destinatario e Expedidor/Recebedor e Sacado do CTe";
                case FormaRateioSVM.AgruparPorRemetenteDestinatarioSacado: return "Agrupar por Remetente/Destinatario e Sacado do CTe";
                default: return string.Empty;
            }
        }
    }
}
