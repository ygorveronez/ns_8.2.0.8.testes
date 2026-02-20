
//#region Variaveis Globais
var _aprovacaoTransportadorTermo;
var _gridAprovacaoTransportadorTermo;
//#endregion


//#region Construtores
function AprovacaoTransportadorTermo() {
    this.Grid = PropertyEntity({ idGrid: guid() });
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
}
//#endregion

//#region principais
function loadEtapaAprovacaoTransportador() {
    _aprovacaoTransportadorTermo = new AprovacaoTransportadorTermo();
    KoBindings(_aprovacaoTransportadorTermo, "knockoutAprovacaoTransportador")
    
}
//#endregion

//#region funções auxiliares
function loadAprovacoesTransportador() {

    _gridAprovacaoTransportadorTermo = new GridView(_aprovacaoTransportadorTermo.Grid.idGrid, "TermoQuitacaoFinanceiro/PesquisaAprovacaoPendenteTransportador", _aprovacaoTransportadorTermo);
    _gridAprovacaoTransportadorTermo.CarregarGrid();
}
//#endregion
