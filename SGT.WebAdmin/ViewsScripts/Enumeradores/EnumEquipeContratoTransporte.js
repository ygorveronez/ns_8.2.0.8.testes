var EnumEquipeContratoTransporteHelper = function () {
    this.AfricaProcurementLogistics = 0;
    this.ANZProcurementLogistics = 1;
    this.BangladeshProcurementLogistics = 2;
    this.EastAfricaProcurementLogistics = 3;
    this.EkaterraTeaLogistics = 4;
    this.FrenchAfricaProcurementLogistics = 5;
    this.HongKongLogistics = 6;
    this.IndiaProcurementLogistics = 7;
    this.IndonesiaProcurementLogistics = 8;
    this.IsraelProcurementLogistics = 9;
    this.JapanLogistics = 10;
    this.JapanProcurement = 11;
    this.MCLProcurementLogistics = 12;
    this.MySgProcurementLogistics = 13;
    this.NameProcurementLogistics = 14;
    this.NorthAmericaProcurementLogistics = 15;
    this.NorthAsiaFinance = 16;
    this.NorthAsiaIE = 17;
    this.NorthAsiaLegal = 18;
    this.NorthAsiaLogistics = 19;
    this.NorthAsiaProcurement = 20;
    this.NorthAsiaProcurementLogistics = 21;
    this.NorthAsiaSupplyChainVPTeam = 22;
    this.PakistanProcurementLogistics = 23;
    this.PhilippinesProcurementLogistics = 24;
    this.PhilippinesSupplyChainLogistics = 25;
    this.SouthAmericaGovernance = 26;
    this.SouthAmericaLOP = 27;
    this.SouthAmericaSupplyChainProcurement = 28;
    this.SouthernAfricaProcurementLogistics = 29;
    this.SriLankaProcurementLogistics = 30;
    this.TaiwanHongKongFINDirectorTeam = 31;
    this.TaiwanHongKongLegalDirectorTeam = 32;
    this.TaiwanHongKongProcurement = 33;
    this.TaiwanHongKongSCDirectorTeam = 34;
    this.TaiwanLogistics = 35;
    this.ThailandProcurementLogistics = 36;
    this.TurkeyProcurementLogistics = 37;
    this.VietnamProcurementLogistics = 38;
    this.WestAfricaProcurementLogistics = 39;
};

EnumEquipeContratoTransporteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Africa-Procurement Logistics", value: this.AfricaProcurementLogistics },
            { text: "ANZ-Procurement Logistics", value: this.ANZProcurementLogistics },
            { text: "Bangladesh-Procurement Logistics", value: this.BangladeshProcurementLogistics },
            { text: "East Africa-Procurement Logistics", value: this.EastAfricaProcurementLogistics },
            { text: "Ekaterra Tea-Logistics", value: this.EkaterraTeaLogistics },
            { text: "French Africa-Procurement Logistics", value: this.FrenchAfricaProcurementLogistics },
            { text: "Hong Kong-Logistics", value: this.HongKongLogistics },
            { text: "India-Procurement Logistics", value: this.IndiaProcurementLogistics },
            { text: "Indonesia-Procurement Logistics", value: this.IndonesiaProcurementLogistics },
            { text: "Israel-Procurement Logistics", value: this.IsraelProcurementLogistics },
            { text: "Japan-Logistics", value: this.JapanLogistics },
            { text: "Japan-Procurement", value: this.JapanProcurement },
            { text: "MCL-Procurement Logistics", value: this.MCLProcurementLogistics },
            { text: "MySg-Procurement Logistics", value: this.MySgProcurementLogistics },
            { text: "Name-Procurement Logistics", value: this.NameProcurementLogistics },
            { text: "North America-Procurement Logistics", value: this.NorthAmericaProcurementLogistics },
            { text: "North Asia-Finance", value: this.NorthAsiaFinance },
            { text: "North Asia-I/E", value: this.NorthAsiaIE },
            { text: "North Asia-Legal", value: this.NorthAsiaLegal },
            { text: "North Asia-Logistics", value: this.NorthAsiaLogistics },
            { text: "North Asia-Procurement", value: this.NorthAsiaProcurement },
            { text: "North Asia-Procurement Logistics", value: this.NorthAsiaProcurementLogistics },
            { text: "North Asia-Supply Chain VP Team", value: this.NorthAsiaSupplyChainVPTeam },
            { text: "Pakistan-Procurement Logistics", value: this.PakistanProcurementLogistics },
            { text: "Philippines-Procurement Logistics", value: this.PhilippinesProcurementLogistics },
            { text: "Philippines-Supply Chain Logistics", value: this.PhilippinesSupplyChainLogistics },
            { text: "South America-Governance", value: this.SouthAmericaGovernance },
            { text: "South America-LOP", value: this.SouthAmericaLOP },
            { text: "South America-Supply Chain Procurement", value: this.SouthAmericaSupplyChainProcurement },
            { text: "Southern Africa-Procurement Logistics", value: this.SouthernAfricaProcurementLogistics },
            { text: "Sri Lanka-Procurement Logistics", value: this.SriLankaProcurementLogistics },
            { text: "Taiwan & Hong Kong-FIN Director Team", value: this.TaiwanHongKongFINDirectorTeam },
            { text: "Taiwan & Hong Kong-Legal Director Team", value: this.TaiwanHongKongLegalDirectorTeam },
            { text: "Taiwan & Hong Kong-Procurement", value: this.TaiwanHongKongProcurement },
            { text: "Taiwan & Hong Kong-SC Director Team", value: this.TaiwanHongKongSCDirectorTeam },
            { text: "Taiwan-Logistics", value: this.TaiwanLogistics },
            { text: "Thailand-Procurement Logistics", value: this.ThailandProcurementLogistics },
            { text: "Turkey-Procurement Logistics", value: this.TurkeyProcurementLogistics },
            { text: "Vietnam-Procurement Logistics", value: this.VietnamProcurementLogistics },
            { text: "West Africa-Procurement Logistics", value: this.WestAfricaProcurementLogistics },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumEquipeContratoTransporte = Object.freeze(new EnumEquipeContratoTransporteHelper());