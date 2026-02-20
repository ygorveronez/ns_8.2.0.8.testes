/// <reference path="Anexos.js" />

//#region variaveis globais
var _modalCriacaoTermo;
//#endregion

//#region Construtores
function ModalCriacaoTermo() {
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true), required: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true), required: ko.observable(true) });

    this.Cancelar = PropertyEntity({ text: 'Cancelar', eventClick: cancelarGeracaoManualTermo, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ text: 'Adicionar', eventClick: preechertermoFinanceiro, type: types.event, idGrid: guid(), visible: ko.observable(false) });
}
//#endregion

//#region Funções Principais
function loadCriacaoManualTermoQuitacao() {
    _modalCriacaoTermo = new ModalCriacaoTermo();
    KoBindings(_modalCriacaoTermo, "knockoutNovoTermoQuitacaoFinanceiro");

    new BuscarTransportadores(_modalCriacaoTermo.Transportador,null,null,null,null,null,null,null,null,null,null,null,null,null,true);
}
//#endregion

//#region Funções Auxiliares
function cancelarGeracaoManualTermo() {
    LimparCampos(_modalCriacaoTermo);
    Global.fecharModal("divModalNovoTermo");
}
function preechertermoFinanceiro(e) {

    if (!ValidarCamposObrigatorios(_modalCriacaoTermo))
        return exibirMensagem(tipoMensagem.atencao, "Aviso", "Precisa Informar os campos obrigatorios");
        
    BuscarDadosPrevios(e.Transportador.codEntity(), e.DataInicial.val(), e.DataFinal.val());
}

function BuscarDadosPrevios(codigoTransportador,dataInicial,dataFinal) {
    executarReST("TermoQuitacaoFinanceiro/ObterPreviaTermo", { Transportador: codigoTransportador, DataInicial: dataInicial, DataFinal: dataFinal }, (arg) => {

        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Message);

        PreencherObjetoKnout(_termoQuitacao, { Data: arg.Data });
        recarregarGridFiliais();
        setarEtapasSetarEtapaTermoQuitacao();

        FadeTermoQuitacaoDetalhes(true);
        _termoQuitacao.Avancar.visible(true);
        _termoQuitacao.DataInicial.val(dataInicial);
        _termoQuitacao.DataFinal.val(dataFinal);

        cancelarGeracaoManualTermo();
    });
}
//#endregion
