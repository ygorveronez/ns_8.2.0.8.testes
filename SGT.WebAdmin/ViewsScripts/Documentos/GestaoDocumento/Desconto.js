/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _descontoGestaoDocumento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DescontoGestaoDocumento = function () {
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int });
    this.ValorDesconto = PropertyEntity({ text: "Valor de desconto: ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, maxlength: 18, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Confirmar = PropertyEntity({ text: "Confirmar", eventClick: confirmarDescontoGestaoDocumentoClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", eventClick: fecharModalDescontoGestaoDocumento, type: types.event, idGrid: guid(), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDescontoGestaoDocumento() {
    _descontoGestaoDocumento = new DescontoGestaoDocumento();
    KoBindings(_descontoGestaoDocumento, "divModalDescontoGestaoDocumento");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function confirmarDescontoGestaoDocumentoClick() {
    if (ValidarCamposObrigatorios(_descontoGestaoDocumento)) {
        exibirConfirmacao("Confirmação", "Realmente deseja registrar o desconto?", function () {
            executarReST("GestaoDocumento/AplicarDesconto", RetornarObjetoPesquisa(_descontoGestaoDocumento), function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Desconto aplicado com sucesso");
                    fecharModalDescontoGestaoDocumento()
                    _gridGestaoDocumento.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    }
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDescontoGestaoDocumento(registroSelecionado) {
    _descontoGestaoDocumento.Codigo.val(registroSelecionado.Codigo);
    _descontoGestaoDocumento.ValorDesconto.val(registroSelecionado.ValorDesconto);
        
    Global.abrirModal('divModalDescontoGestaoDocumento');
}

// #endregion Funções Públicas

// #region Funções Privadas

function fecharModalDescontoGestaoDocumento() {
    Global.fecharModal('divModalDescontoGestaoDocumento');
}

function mostrarOpcaoDescontoGestaoDocumentoGrid() {
    return _CONFIGURACAO_TMS.HabilitarDescontoGestaoDocumento && permitirEditarInformacoesGestaoDocumentos();
}

// #endregion Funções Privadas
