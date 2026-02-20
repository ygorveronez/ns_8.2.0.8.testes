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

var _externalID;

var ExternalID = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroID = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ExternalID = PropertyEntity({ required: true, text: "*ExternalID:", val: ko.observable(""), def: "", maxlength: 150 });

    this.Confirmar = PropertyEntity({ type: types.event, eventClick: confirmarAlterarExternalId, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: cancelarAlterarExternalId, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

function alterarExternalId(e, numeroID) {
    _externalID = new ExternalID();
    KoBindings(_externalID, "knockoutAlterarExternalID");

    _externalID.Carga.val(e.Codigo.val());
    _externalID.NumeroID.val(numeroID);

    Global.abrirModal("divModalAlterarExternalID");
}


function confirmarAlterarExternalId(e) {
    if (!ValidarCamposObrigatorios(_externalID)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var dados = {
        Carga: _externalID.Carga.val(),
        ExternalID: _externalID.ExternalID.val(),
        NumeroID: _externalID.NumeroID.val()
    };

    executarReST("Carga/AlterarExternalID", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _listaKnoutsCarga.forEach(carga => {
                    if (carga.Codigo.val() == _externalID.Carga.val()) {
                        if (retorno.Data.NumeroExternalID == 1) 
                            carga.ExternalDT1.val(retorno.Data.ExternalID);
                        else 
                            carga.ExternalDT2.val(retorno.Data.ExternalID);
                    }
                });

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ExternalIDAlteradoSucesso);
                limparCamposExternalID();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function cancelarAlterarExternalId() {
    limparCamposExternalID();
}

function limparCamposExternalID() {
    LimparCampos(_externalID);
    Global.fecharModal("divModalAlterarExternalID");
}