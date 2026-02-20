var EnumSubCategoriaContratoTransporteHelper = function () {
    this.Todos = "";
    this.Selecione = "";
    this.CrossDock = 0;
    this.Freight = 1;
    this.Firework = 2;
    this.Shuttle = 3;
    this.DistributionRoutePlanning = 4;
    this.ThirdParty = 5;
    this.ModernTrade = 6;
    this.GeneralTrade = 7;
    this.PrimarySecondaryFreighICD = 8;
    this.ExportByRoad = 9;
    this.ImportByRoad = 10;
    this.Crossborder = 11;
    this.YardManagement = 12;
    this.Labour = 13;
    this.InternalOperations = 14;
    this.TransportRelatedServices = 15;
    this.Other = 16;
};

EnumSubCategoriaContratoTransporteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Cross Dock", value: this.CrossDock },
            {text: "Freight (Air/ Ocean/ Rail/ Road)", value: this.Freight },
            { text: "Firework", value: this.Firework },
            { text: "Shuttle", value: this.Shuttle },
            { text: "DRP (Distribution Route Planning)", value: this.DistributionRoutePlanning },
            { text: "3rd Party", value: this.ThirdParty },
            { text: "Modern Trade", value: this.ModernTrade },
            { text: "General Trade", value: this.GeneralTrade },
            { text: "Primary & Secondary Freight - ICD", value: this.PrimarySecondaryFreighICD },
            { text: "Export (By Road)", value: this.ExportByRoad },
            { text: "Import (By Road)", value: this.ImportByRoad },
            { text: "Crossborder", value: this.Crossborder },
            { text: "Yard Management", value: this.YardManagement },
            { text: "Labour", value: this.Labour },
            { text: "Internal Operations", value: this.InternalOperations },
            { text: "Transport related services", value: this.TransportRelatedServices },
            { text: "Other", value: this.Other },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    },
    ObterOpcoesPesquisaIntegracao: function () {
        return [{ text: "Selecione uma opção", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumSubCategoriaContratoTransporte = Object.freeze(new EnumSubCategoriaContratoTransporteHelper());