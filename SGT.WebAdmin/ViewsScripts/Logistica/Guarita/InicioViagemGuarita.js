/*InicioViagem.js*/
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _inicioViagem;
var _mapaInicioViagem = null;
var _modalInicioViagem;

var InicioViagem = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.DataInicioViagem = PropertyEntity({ text: "Data Início da Viagem:", val: ko.observable(""), def: "" });
    this.InformarInicioViagem = PropertyEntity({ eventClick: informarInicioViagemClick, type: types.event, text: "Iniciar Viagem", idGrid: guid(), visible: ko.observable(false) });
    this.DataInicioViagemInformada = PropertyEntity({ text: "Data/hora de início da viagem: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(false) });
    this.LatitudeInicioViagem = PropertyEntity({ text: "LatitudeInicioViagem", val: ko.observable(""), def: "" });
    this.LongitudeInicioViagem = PropertyEntity({ text: "LatitudeInicioViagem", val: ko.observable(""), def: "" });

    this.InformarInicioViagemAlterar = PropertyEntity({ eventClick: informarInicioViagemAlterarClick, type: types.event, text: "Alterar data início", idGrid: guid(), visible: ko.observable(false) });
}

function loadInicioViagem() {
    _inicioViagem = new InicioViagem();
    KoBindings(_inicioViagem, "knockoutInicioViagemGuarita");

    _modalInicioViagem = new bootstrap.Modal(document.getElementById("divModalInicioViagem"), { backdrop: 'static', keyboard: true });
}

function exibirDetalhesInicioViagem(codigoCarga) {
    LimparCampos(_inicioViagem);
    _inicioViagem.Carga.val(codigoCarga);
    executarReST("ControleEntregaInicioViagem/BuscarPorCarga", { Carga: codigoCarga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _inicioViagem.InformarInicioViagem.visible(false);
                _inicioViagem.InformarInicioViagemAlterar.visible(false);

                PreencherObjetoKnout(_inicioViagem, arg);

                if (!arg.Data.ViagemAberta && !arg.Data.ViagemFinalizada && arg.Data.PermiteAlterarDataInicioCarregamento) {
                    _inicioViagem.InformarInicioViagemAlterar.visible(true);
                    _inicioViagem.DataInicioViagemInformada.val(arg.Data.DataInicioViagem);

                } else {
                    if (arg.Data.ViagemAberta)
                        _inicioViagem.InformarInicioViagem.visible(true);
                    if (arg.Data.DataInicioViagemSugerida)
                        _inicioViagem.DataInicioViagemInformada.val(arg.Data.DataInicioViagemSugerida);
                }

                _modalInicioViagem.show();
                $("#divModalInicioViagem").one('hidden.bs.modal', function () { LimparCampos(_inicioViagem); });

            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}

//********MÉTODOS*******
function informarInicioViagemClick() {
    if (!ValidarCamposObrigatorios(_inicioViagem)) {
        exibirMensagemCamposObrigatorio();
        return;
    }
    exibirConfirmacao("Confirmação", "Você tem certeza que deseja iniciar a viagem?", function () {
        executarReST("ControleEntregaInicioViagem/InformarInicioViagem", {
            Carga: _inicioViagem.Carga.val(),
            DataInicioViagemInformada: _inicioViagem.DataInicioViagemInformada.val()
        }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Viagem iniciada com sucesso!");

                    _modalInicioViagem.hide();

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function informarInicioViagemAlterarClick() {
    if (!ValidarCamposObrigatorios(_inicioViagem)) {
        exibirMensagemCamposObrigatorio();
        return;
    }
    exibirConfirmacao("Confirmação", "Você tem certeza que deseja alterar a data de início do carregamento?", function () {
        executarReST("ControleEntregaInicioViagem/InformarInicioViagemAlterar", {
            Carga: _inicioViagem.Carga.val(),
            DataInicioViagemInformadaAlterar: _inicioViagem.DataInicioViagemInformada.val()
        }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Data de início alterada com sucesso!");

                    _modalInicioViagem.hide();

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

