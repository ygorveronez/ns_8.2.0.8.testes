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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="PosicaoAtual.js" />
/// <reference path="PosicaoTransferir.js" />

var _ControlarManualmenteProgresseTransferencia;

function loadTransferenciaMercadoria() {
    loadAreaPosicaoAtual(function () {
        loadAreaPosicaoTransferir(function () {
        });
    });
}

var z = 5000;
function activateItemWMS(event, ui) {
    $("#divPosicaoAtual").css('z-index', 2);
    $("#divPosicaoTransferir").css('z-index', 1);
    //$("#divPosicaoAtual").css('overflow', 'hidden');

    var tamanho = $("#" + ui.draggable[0].id).width();
    $(ui.helper[0]).width(tamanho);

    if (ui.draggable[0].id.split("_")[0] == "PosicaoAtual") {
        $("#divPosicaoAtual").css('z-index', 4999);
        $("#divPosicaoTransferir").css('z-index', 3999);
    }

    $.blockUI({ message: "" });
}

function deactivateItemWMS(event, ui) {
    $.unblockUI();
    if (_RequisicaoIniciada)
        iniciarRequisicao();

    $("#" + _areaPosicaoAtual.PosicaoAtual.id).css('z-index', 1);
    $("#" + _areaPosicaoTransferir.PosicaoTransferir.id).css('z-index', 2);

    $("#divPosicaoAtual").css('z-index', 1);
    $("#divPosicaoTransferir").css('z-index', 2);
}

function droppableItemRetirar(event, ui) {
    var id = ui.draggable[0].id.split("_")[2];
    var operacao = ui.draggable[0].id.split("_")[0];
    var idDestino = event.target.id.split("_")[2];

    if (operacao == "PosicaoAtual")
        transferirLote(id, idDestino);
}

function transferirLote(idLote, idDestino) {
    var data = {
        CodigoLote: idLote,
        CodigoDestino: idDestino
    }
    executarReST("TransferenciaMercadoria/TransferirLote", data, function (e) {
        if (e.Success) {
            $("#" + _areaPosicaoTransferir.PosicaoTransferir.id).html("");
            _areaPosicaoTransferir.Inicio.val(0);
            _knoutsPosicaoTransferir = new Array();
            buscarPosicaoTransferir();

            $("#" + _areaPosicaoAtual.PosicaoAtual.id).html("");
            _areaPosicaoAtual.Inicio.val(0);
            _knoutsPosicaoAtual = new Array();
            buscarPosicaoAtual();
        }
        else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
        }
    });
}