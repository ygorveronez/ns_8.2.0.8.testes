var EnumFormaRateioSVMHelper = function () {
    this.Nenhum = 0;
    this.UmCTeMultimodalParaUmCTeAquaviario = 1;
    this.AgruparPorTerminalOrigemDestino = 2;
    this.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedor = 3;
    this.AgruparPorRemetenteDestinatario = 4;
    this.AgruparPorSacado = 5;
    this.AgruparPorTerminalOrigemDestinoSacado = 6;
    this.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedorSacado = 7;
    this.AgruparPorRemetenteDestinatarioSacado = 8;
}

EnumFormaRateioSVMHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nenhum: return "Nenhum";
            case this.UmCTeMultimodalParaUmCTeAquaviario: return "1 CT-e Multimodal para 1 CT-e Aquaviário";
            case this.AgruparPorTerminalOrigemDestino: return "Agrupar por Terminal de Origem/Terminal de Destino";
            case this.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedor: return "Agrupar por Booking e Remetente/Destinatario e Expedidor/Recebedor";
            case this.AgruparPorRemetenteDestinatario: return "Agrupar por Remetente/Destinatario";
            case this.AgruparPorSacado: return "Agrupar por Sacado do CT-e Multimodal";
            case this.AgruparPorTerminalOrigemDestinoSacado: return "Agrupar por Terminal de Origem/Terminal de Destino e Sacado do CTe";
            case this.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedorSacado: return "Agrupar por Booking e Remetente/Destinatario e Expedidor/Recebedor e Sacado do CTe";
            case this.AgruparPorRemetenteDestinatarioSacado: return "Agrupar por Remetente/Destinatario e Sacado do CTe";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "1 CT-e Multimodal para 1 CT-e Aquaviário", value: this.UmCTeMultimodalParaUmCTeAquaviario },
            { text: "Agrupar por Terminal de Origem/Terminal de Destino", value: this.AgruparPorTerminalOrigemDestino },
            { text: "Agrupar por Booking e Remetente/Destinatario e Expedidor/Recebedor", value: this.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedor },
            { text: "Agrupar por Remetente/Destinatario", value: this.AgruparPorRemetenteDestinatario },
            { text: "Agrupar por Sacado do CT-e Multimodal", value: this.AgruparPorSacado },
            { text: "Agrupar por Terminal de Origem/Terminal de Destino e Sacado do CTe", value: this.AgruparPorTerminalOrigemDestinoSacado },
            { text: "Agrupar por Booking e Remetente/Destinatario e Expedidor/Recebedor e Sacado do CTe", value: this.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedorSacado },
            { text: "Agrupar por Remetente/Destinatario e Sacado do CTe", value: this.AgruparPorRemetenteDestinatarioSacado }
        ];
    }
}

var EnumFormaRateioSVM = Object.freeze(new EnumFormaRateioSVMHelper());