//*******MAPEAMENTO KNOUCKOUT*******

var _valoresPosicaoContasAReceber;

var ValoresPosicaoContasAReceber = function () {
    var dataAtual = Global.DataAtual();

    this.DataPosicao = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual, text: "Data da Posição:" });
    this.UtilizarDataBaseLiquidacaoTitulos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Utilizar a data base de liquidação para validação dos títulos financeiros" });

    this.DownloadRelatorioAnalitico = PropertyEntity({ eventClick: ImprimirRelatorioAnaliticoPosicaoContasAReceberClick, type: types.event, text: "Gerar Relatório Analítico", visible: ko.observable(true), icon: "fal fa-print" });
}

//*******EVENTOS*******

function LoadValoresPosicaoContasAReceber() {
    _valoresPosicaoContasAReceber = new ValoresPosicaoContasAReceber();
    KoBindings(_valoresPosicaoContasAReceber, "knockoutPosicaoContasAReceber");
}

function ImprimirRelatorioAnaliticoPosicaoContasAReceberClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente gerar o relatório de posição de contas a receber para o dia " + _valoresPosicaoContasAReceber.DataPosicao.val() + "?", function () {
        executarDownload("ConsultaValores/DownloadRelatorioPosicaoContasAReceberAnalitico", { DataPosicao: _valoresPosicaoContasAReceber.DataPosicao.val(), UtilizarDataBaseLiquidacaoTitulos: _valoresPosicaoContasAReceber.UtilizarDataBaseLiquidacaoTitulos.val() });
    });
}