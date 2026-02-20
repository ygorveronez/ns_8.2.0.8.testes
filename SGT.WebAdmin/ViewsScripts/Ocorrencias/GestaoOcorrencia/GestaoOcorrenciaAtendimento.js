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

var _gestaoOcorrenciaAtendimento;

var GestaoOcorrenciaAtendimento = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Numero = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, text: "Número da Ocorrência: " });
    this.SituacaoDescricao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Situação: " });
    this.Empresa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Empresa: " });
    this.Carga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Carga: " });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Observação: " });
    this.NotasFiscais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Notas Fiscais: " });
    
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
}

function loadGestaoOcorrenciaAtendimento() {
    _gestaoOcorrenciaAtendimento = new GestaoOcorrenciaAtendimento();
    KoBindings(_gestaoOcorrenciaAtendimento, "knockoutAtendimento");
}

function setarVisibilidadeFaixaTabAtendimento() {
    var idFaixa = "faixa-tab-atendimento";
    var idDiv = "informacoes-tab-atendimento";

    if (_gestaoOcorrenciaAtendimento.Codigo.val() > 0) {
        $("#" + idDiv).show();
        $("#" + idFaixa).hide();
    }
    else {
        $("#" + idFaixa).show();
        $("#" + idDiv).hide();
    }
}