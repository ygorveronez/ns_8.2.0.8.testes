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
/// <reference path="../../../js/libs/jquery.signalR-2.2.0.js" />
/// <reference path="../../Enumeradores/EnumSituacaoNotificacao.js" />
/// <reference path="../SignalR/SignalR.js" />
/// <reference path="../../Enumeradores/EnumTipoNotificacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _centralProcessamentoGlobal;


var CentralProcessamentoGlobal = function () {
    this.NadaParaProcessar = PropertyEntity({ val: ko.observable(true), def: true, type: types.local, cssClass: ko.observable("btn-group dropup"), eventClick : mostrarOcultarClick });
    this.Processamentos = ko.observableArray();
}

var DetalhesProcessar = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity();
    this.Titulo = PropertyEntity();
}



//*******EVENTOS*******

function loadCentralProcessamentoGlobal(callback) {

    _centralProcessamentoGlobal = new CentralProcessamentoGlobal();
    KoBindings(_centralProcessamentoGlobal, "knoutCentralProcessamento");
    BuscarProcessamentosPendentes(callback);
}

function mostrarOcultarClick(e) {
    //if (_centralProcessamentoGlobal.NadaParaProcessar.cssClass() == "btn-group dropup") {
    //    _centralProcessamentoGlobal.NadaParaProcessar.cssClass("btn-group dropup open");
    //} else {
    //    _centralProcessamentoGlobal.NadaParaProcessar.cssClass("btn-group dropup");
    //}
}

function BuscarProcessamentosPendentes(callback) {
    _RequisicaoIniciada = true;
    executarReST("CentralProcessamento/BuscarProcessamentoPendentes", null, function (arg) {
        if (arg.Success) {
            _centralProcessamentoGlobal.Processamentos.removeAll();
            var retorno = arg.Data;
            _centralProcessamentoGlobal.NadaParaProcessar.val(retorno.NadaParaProcessar)
            if (retorno.Processamentos.length > 0) {
                $.each(retorno.Processamentos, function (i, download) {
                    preecherProcessarGlobal(download);
                });
                _centralProcessamentoGlobal.NadaParaProcessar.cssClass("btn-group dropup open");
            } else {
                _centralProcessamentoGlobal.NadaParaProcessar.cssClass("btn-group dropup");
            }
            if (callback != null) {
                callback();
            }
        }
        _RequisicaoIniciada = false;
    });
}



function preecherProcessarGlobal(download) {
    var knouProcessar = new DetalhesProcessar();
    var data = { Data: download }
    PreencherObjetoKnout(knouProcessar, data);
    _centralProcessamentoGlobal.Processamentos.push(knouProcessar);

    return knouProcessar;
}
