/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Enumeradores/EnumPeriodicidade.js" />

var _automacaoNFSManual;

var _periodicidade = [
    { text: "Não Automatizar", value: EnumPeriodicidade.Nenhuma },
    { text: "Diariamente", value: EnumPeriodicidade.Diario },
    { text: "Semanalmente", value: EnumPeriodicidade.Semanal },
    { text: "Mensalmente", value: EnumPeriodicidade.Mensal }
];

var _diaMes = [
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 },
    { text: "8", value: 8 },
    { text: "9", value: 9 },
    { text: "10", value: 10 },
    { text: "11", value: 11 },
    { text: "12", value: 12 },
    { text: "13", value: 13 },
    { text: "14", value: 14 },
    { text: "15", value: 15 },
    { text: "16", value: 16 },
    { text: "17", value: 17 },
    { text: "18", value: 18 },
    { text: "19", value: 19 },
    { text: "20", value: 20 },
    { text: "21", value: 21 },
    { text: "22", value: 22 },
    { text: "23", value: 23 },
    { text: "24", value: 24 },
    { text: "25", value: 25 },
    { text: "26", value: 26 },
    { text: "27", value: 27 },
    { text: "28", value: 28 },
    { text: "29", value: 29 },
    { text: "30", value: 30 },
    { text: "31", value: 31 },
];

var AutomacaoNFSManual = function () {
    this.Periodicidade = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Periodicidade.getFieldDescription(), options: _periodicidade, val: ko.observable(EnumPeriodicidade.Nenhuma), def: EnumPeriodicidade.Nenhuma, eventChange: configurarPeriodicidade });
    this.DiaSemana = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DiaSemana.getFieldDescription(), options: EnumDiaSemana.obterOpcoes(), val: ko.observable(EnumDiaSemana.Segunda), def: EnumDiaSemana.Segunda, visible: ko.observable(false) });
    this.DiaMes = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DiaMes.getFieldDescription(), options: _diaMes, val: ko.observable(1), def: 1, visible: ko.observable(false), maxlength: 2, eventChange: mensagemDiaMes });
}

function loadAutomacaoNFSManual() {
    _automacaoNFSManual = new AutomacaoNFSManual();
    KoBindings(_automacaoNFSManual, "knoutAutomacaoNFSManual");

    $("#liTabAutomacaoEmissaoNFSManual").show();

    configurarPeriodicidade();
}



function limparAutomacaoNFSManual() {
    LimparCampos(_automacaoNFSManual);
}

function configurarPeriodicidade() {
    if (_automacaoNFSManual.Periodicidade.val() == EnumPeriodicidade.Semanal) {
        _automacaoNFSManual.DiaMes.visible(false);
        _automacaoNFSManual.DiaSemana.visible(true);
    }
    else if (_automacaoNFSManual.Periodicidade.val() == EnumPeriodicidade.Mensal) {
        _automacaoNFSManual.DiaSemana.visible(false);
        _automacaoNFSManual.DiaMes.visible(true);
    }
    else {
        _automacaoNFSManual.DiaSemana.visible(false);
        _automacaoNFSManual.DiaMes.visible(false);
    }
}

function mensagemDiaMes() {
    console.log(_automacaoNFSManual.DiaMes.val());
    if (_automacaoNFSManual.DiaMes.val() > 28) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Transportadores.Transportador.ParaMesesQueNaoTemDiaNoCalendarioConsiderarUltimoDiaMes, 10000);
    }

    validarAutomacaoNFSManual();
}
