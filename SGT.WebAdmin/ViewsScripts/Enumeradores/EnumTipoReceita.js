var EnumTipoReceitaHelper = function () {
    this.NaoInformada = 0;
    this.Feeder = 1;
    this.NoShow = 2;
    this.Cabotage = 3;
    this.CaptainPeter = 4;
    this.ExtraCost = 5;
    this.Detention = 6;
    this.ContainerDevolution = 7;
    this.TaxChargeCTeSubstitution = 8;
    this.OwnFleet = 9;
    this.Infraction = 10;
    this.TakeOrPay = 11;
    this.ContainerCleaning = 12;
    this.FeederDebitNote = 13;
    this.OperationalAndOthers = 14;
    this.ContainerRevenue = 15;
    this.ContainerRevenueRepair = 16;
    this.Reimbursement = 17;
    this.VAS = 18;
    this.Demurrage = 19;
    this.ResidualValue = 20;
};

EnumTipoReceitaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Informada", value: this.NaoInformada },
            { text: "Feeder", value: this.Feeder },
            { text: "No show", value: this.NoShow },
            { text: "Cabotage", value: this.Cabotage },
            { text: "Captain Peter", value: this.CaptainPeter },
            { text: "Extra Cost", value: this.ExtraCost },
            { text: "Detention", value: this.Detention },
            { text: "Container Devolution", value: this.ContainerDevolution },
            { text: "Tax Charge for CT-e Substitution", value: this.TaxChargeCTeSubstitution },
            { text: "Own Fleet", value: this.OwnFleet },
            { text: "Infraction", value: this.Infraction },
            { text: "Take or Pay", value: this.TakeOrPay },
            { text: "Container Cleaning", value: this.ContainerCleaning },
            { text: "Feeder Debit Note", value: this.FeederDebitNote },
            { text: "Operational and Others", value: this.OperationalAndOthers },
            { text: "Container Revenue", value: this.ContainerRevenue },
            { text: "Container Revenue Repair", value: this.ContainerRevenueRepair },
            { text: "Reimbursement", value: this.Reimbursement },
            { text: "VAS", value: this.VAS },
            { text: "Demurrage", value: this.Demurrage },
            { text: "Residual Value", value: this.ResidualValue }
        ];
    },
    obterOpcoesOcorrencia: function () {
        return [
            { text: "Não Informada", value: this.NaoInformada },
            { text: "Extra Cost", value: this.ExtraCost },
            { text: "Captain Peter", value: this.CaptainPeter },
            { text: "Cabotage", value: this.Cabotage },
            { text: "Feeder", value: this.Feeder },
            { text: "VAS", value: this.VAS }
        ];
    }
};

var EnumTipoReceita = Object.freeze(new EnumTipoReceitaHelper());