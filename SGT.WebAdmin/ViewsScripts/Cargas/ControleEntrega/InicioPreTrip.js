/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

var _inicioPreTrip;

var InicioPreTrip = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.DataInicioPreTrip = PropertyEntity({ getType: typesKnockout.dateTime, text: "Data Início pré-trip", val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: informarInicioPreTripClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, idGrid: guid(), visible: ko.observable(true) });
}

function loadInformarInicioPreTrip() {
    _inicioPreTrip = new InicioPreTrip();
    KoBindings(_inicioPreTrip, "knockoutInicioPreTrip");
}

function informarInicioPreTrip(fluxoEntrega, etapa) {

    _etapaAtualFluxo = fluxoEntrega;
    LimparCampos(_inicioPreTrip);
    executarReST("ControleEntregaInicioViagem/BuscarPorCarga", { Carga: etapa.Carga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                _inicioPreTrip.Carga.val(etapa.Carga);

                if (_CONFIGURACAO_TMS.NaoPermitirInformarInicioEFimPreTrip) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Você não possui permissão para realizar essa ação.");
                    return;
                }

                Global.abrirModal('divModalInformarInicioPreTrip');

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function informarInicioPreTripClick() {
    executarReST("ControleEntregaInicioViagem/InformarInicioPreTrip", { CodigoCarga: _inicioPreTrip.Carga.val(), DataInicioPreTrip: _inicioPreTrip.DataInicioPreTrip.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Início da pré trip informado com sucesso");

                Global.fecharModal('divModalInformarInicioPreTrip');
                atualizarControleEntrega();

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}