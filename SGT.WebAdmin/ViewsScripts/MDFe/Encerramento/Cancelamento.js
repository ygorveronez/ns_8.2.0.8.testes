/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cancelamento;

var Cancelamento = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, def: 0 });
    this.Numero = PropertyEntity({ getType: typesKnockout.int, def: 0 });
    this.DataHora = PropertyEntity({ text: "Data Hora:", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
    this.Justificativa = PropertyEntity({ text: "Justificativa:", getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.Cancelar = PropertyEntity({ eventClick: cancelarMDFE, type: types.event, text: "Cancelar MDF-e", idGrid: guid(), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadCancelamento() {
    _cancelamento = new Cancelamento();
    KoBindings(_cancelamento, "knockoutCancelamento");
}

//*******MÉTODOS*******

function cancelarMDFE() {
    exibirConfirmacao("Atenção!", "Deseja realmente cancelar o MDF-e " + _cancelamento.Numero.val() + "?", function () {
        var dados = {
            MDFe: _cancelamento.Codigo.val(),
            Justificativa: _cancelamento.Justificativa.val(),
            DataHora: _cancelamento.DataHora.val(),
        };
        executarReST("Encerramento/Cancelar", dados, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "MDF-e enviado para cancelamento com sucesso!");
                    Global.fecharModal('divModalCancelamento');
                    LimparCampos(_cancelamento);
                    _gridMDFe.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function abrirCancelamento(dataGrid) {
    _cancelamento.Codigo.val(dataGrid.Codigo);
    _cancelamento.Numero.val(dataGrid.Numero);
    SetaDataHoraAtual();
    Global.abrirModal("divModalCancelamento");
}

function SetaDataHoraAtual() {
    var data = moment(new Date).format("DD/MM/YYYY HH:mm");
    _cancelamento.DataHora.val(data);
}

function CancelarMDFVisibilidade(dataRow) {
    return dataRow.Situacao == "Autorizado";
}