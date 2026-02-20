/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />

var _fimPreTrip;

var FimPreTrip = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.DataFimPreTrip = PropertyEntity({ getType: typesKnockout.dateTime, text: "Data fim pré-trip", val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: informarFimPreTripClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, idGrid: guid(), visible: ko.observable(true) });
}

function loadInformarFimPreTrip() {
    _fimPreTrip = new FimPreTrip();
    KoBindings(_fimPreTrip, "knockoutFimPreTrip");
}

function informarFimPreTrip(fluxoEntrega, etapa) {

    _etapaAtualFluxo = fluxoEntrega;
    executarReST("ControleEntregaInicioViagem/BuscarPorCarga", { Carga: etapa.Carga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _fimPreTrip.Carga.val(etapa.Carga);

                if (_CONFIGURACAO_TMS.NaoPermitirInformarInicioEFimPreTrip) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Você não possui permissão para realizar essa ação.");
                    return;
                }

                Global.abrirModal('divModalInformarFimPreTrip');

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function informarFimPreTripClick(e) {
    executarReST("ControleEntregaFimViagem/InformarFimPreTrip", { CodigoCarga: _fimPreTrip.Carga.val(), DataFimPreTrip: _fimPreTrip.DataFimPreTrip.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Fim da pré trip informado com sucesso.");

                Global.fecharModal('divModalInformarFimPreTrip');
                atualizarControleEntrega();

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}