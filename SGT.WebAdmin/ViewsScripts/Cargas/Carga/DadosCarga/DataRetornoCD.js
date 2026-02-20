/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="Carga.js" />
/// <reference path="DataCarregamento.js" />

// #region Objetos Globais do Arquivo

var _knoutCargaAlterarDataRetornoCD;
var _knoutInfoRetornoCD;

// #endregion Objetos Globais do Arquivo

// #region Classes

var InfoRetornoCD = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataRetornoCD = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataRetornoCD.getRequiredFieldDescription(), required: true, getType: typesKnockout.dateTime, visible: ko.observable(true) });

    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarAlteracaoRetornoCDClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções Associadas a Eventos

function confirmarAlteracaoRetornoCDClick() {
    if (!ValidarCamposObrigatorios(_knoutInfoRetornoCD))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);

    var dados = {
        Carga: _knoutInfoRetornoCD.Carga.val(),
        DataRetornoCD: _knoutInfoRetornoCD.DataRetornoCD.val()
    };
    executarReST("Carga/AlterarDataRetornoCD", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DataAlteradaComSucesso);
                _knoutCargaAlterarDataRetornoCD.DataRetornoCD.val(_knoutInfoRetornoCD.DataRetornoCD.val());
                LimparCampo(_knoutInfoRetornoCD.DataRetornoCD);
                Global.fecharModal('divModalAlterarRetornoCD');
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function alterarDataRetornoCDClick(e) {
    loadInfoRetornoCD();

    _knoutCargaAlterarDataRetornoCD = e;

    _knoutInfoRetornoCD.Carga.val(e.Codigo.val());
        
    Global.abrirModal('divModalAlterarRetornoCD');
}

// #endregion Funções Públicas

// #region Funções Privadas

function loadInfoRetornoCD() {
    if (_knoutInfoRetornoCD)
        return;

    _knoutInfoRetornoCD = new InfoRetornoCD();
    KoBindings(_knoutInfoRetornoCD, "knoutAlterarRetornoCD");
}

// #endregion Funções Privadas