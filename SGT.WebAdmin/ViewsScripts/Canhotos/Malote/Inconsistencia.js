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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />

var _inconsistencia;
var _modalInconsistencia;

var Inconsistencia = function () {
    this.Codigo = PropertyEntity({});
    this.Motivo = PropertyEntity({ text: "Motivo:", required: true });

    // CRUD
    this.Salvar = PropertyEntity({ eventClick: SalvarInconsistenciaClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

function loadInconsistencia() {
    _inconsistencia = new Inconsistencia();
    KoBindings(_inconsistencia, "KnoutInconsistencia");
    _modalInconsistencia = new bootstrap.Modal(document.getElementById("ModalDivInconsistencia"), { backdrop: true, keyboard: true });
}

function SalvarInconsistenciaClick(e) {
    exibirConfirmacao("Alterar Malote", "Deseja alterar o Malote como Inconsistente?", function () {
        Salvar(_inconsistencia, "MaloteCanhoto/InconsistenciaMalote", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso");
                    _gridMalote.CarregarGrid();
                    _modalInconsistencia.hide();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function LimparCamposInconsistencia() {
    LimparCampos(_inconsistencia);
}