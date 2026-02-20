const EnumTipoEventoAppHelper = function () {
    this.Todas = "";
    this.SendPosition = 1;
    this.EventsSubmit = 2;
    this.DeliveryReceiptCreate = 3;
    this.ChatSendMessage = 4;
    this.OccurrenceCreate = 5;
    this.DriverReceiptCreate = 6;
    this.DriverFreightContactCreate = 7;
    this.DriverOccurrenceCreate = 8;
    this.NotDelivered = 9;
    this.PartialDelivery = 10;
};

EnumTipoEventoAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.Todas), value: this.Todas });
        opcoes.push({ text: this.obterDescricao(this.SendPosition), value: this.SendPosition });
        opcoes.push({ text: this.obterDescricao(this.EventsSubmit), value: this.EventsSubmit });
        opcoes.push({ text: this.obterDescricao(this.DeliveryReceiptCreate), value: this.DeliveryReceiptCreate });
        opcoes.push({ text: this.obterDescricao(this.ChatSendMessage), value: this.ChatSendMessage });
        opcoes.push({ text: this.obterDescricao(this.OccurrenceCreate), value: this.OccurrenceCreate });
        opcoes.push({ text: this.obterDescricao(this.DriverFreightContactCreate), value: this.DriverFreightContactCreate });

        opcoes.push({ text: this.obterDescricao(this.DriverOccurrenceCreate), value: this.DriverOccurrenceCreate });
        opcoes.push({ text: this.obterDescricao(this.NotDelivered), value: this.NotDelivered });
        opcoes.push({ text: this.obterDescricao(this.PartialDelivery), value: this.PartialDelivery });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Todas: return "Todos"
            case this.SendPosition: return "Tracking Posições";
            case this.EventsSubmit: return "Envio de Evento";
            case this.DeliveryReceiptCreate:
            case this.DriverReceiptCreate: return "Envio de Evidencias";
            case this.ChatSendMessage: return "Envio de Mensagem";
            case this.OccurrenceCreate: return "Criação de Ocorrência";
            case this.DriverFreightContactCreate: return "Criação Interesse de Frete";
            case this.DriverOccurrenceCreate: return "Criação de Ocorrência V3";
            case this.NotDelivered: return "Não Entregue";
            case this.PartialDelivery: return "Entrega Parcial";

            default: return "";
        }
    }
};

const EnumTipoEventoApp = Object.freeze(new EnumTipoEventoAppHelper());
