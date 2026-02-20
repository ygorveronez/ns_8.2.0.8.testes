
// #region Objetos Globais do Arquivo

var _areaRedex;
var _gridAreasRedex;
var _detalhes
// #endregion Objetos Globais do Arquivo

// #region Classes

var AreaRedex = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.AreaRedex = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.AreasRedex.getRequiredFieldDescription(), val: ko.observable(true), enable: ko.observable(true), required: true, options: ko.observable([]), def: 1, visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ type: types.event, eventClick: confirmarEntregaAreaRedexClick, text: Localization.Resources.Cargas.ControleEntrega.ConfirmacaoEntrega, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadAreasRedex() {
    _areaRedex = new AreaRedex();
    KoBindings(_areaRedex, "knockouSelecionarAreaRedex");
}

function abrirModalAreaRedex(entrega, detalhes) {
    console.log(_entrega.AreasRedex.val());

    _areaRedex.Codigo.val(entrega.Codigo.val());
    _detalhes = detalhes;

    _areaRedex.AreaRedex.options(_entrega.AreasRedex.val());

    Global.abrirModal("divModalSelecionarAreaRedex");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function confirmarEntregaAreaRedexClick() {
    if (ValidarCamposObrigatorios(_areaRedex)) {
        confirmarEntrega(_detalhes, _areaRedex);
    }
}

// #endregion Funções Associadas a Eventos

// #endregion Funções Públicas
