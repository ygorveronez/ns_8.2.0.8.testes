//*******MAPEAMENTO KNOUCKOUT*******

var _valoresPosicaoContasAPagar;

var ValoresPosicaoContasAPagar = function () {
    var dataAtual = Global.DataAtual();

    this.DataPosicao = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual, text: "Data da Posição:" });

    this.DownloadRelatorioAnalitico = PropertyEntity({ eventClick: ImprimirRelatorioAnaliticoPosicaoContasAPagarClick, type: types.event, text: "Gerar Relatório Analítico", visible: ko.observable(true), icon: "fa fa-print" });
}

//*******EVENTOS*******

function loadConsultaValoresPagar() {
    _valoresPosicaoContasAPagar = new ValoresPosicaoContasAPagar();
    KoBindings(_valoresPosicaoContasAPagar, "knockoutContasAPagar");
}

function ImprimirRelatorioAnaliticoPosicaoContasAPagarClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente gerar o relatório de posição de contas a pagar para o dia " + _valoresPosicaoContasAPagar.DataPosicao.val() + "?", function () {
        executarDownload("ConsultaValoresPagar/DownloadRelatorioPosicaoContasAPagarAnalitico", { DataPosicao: _valoresPosicaoContasAPagar.DataPosicao.val() });
    });
}