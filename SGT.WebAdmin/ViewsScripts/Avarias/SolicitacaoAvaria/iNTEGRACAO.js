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
/// <reference path="../../Enumeradores/EnumSituacaoAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracao;

var Integracao = function () {
    // Mensagem
    this.Mensagem = PropertyEntity({ text: ko.observable(""), cssClass: ko.observable(""), visible: ko.observable(false) });
}

//*******EVENTOS*******
function loadIntegracao() {
    _integracao = new Integracao();
    KoBindings(_integracao, "knockoutIntegracao");
}



//*******MÉTODOS*******
function MensagemIntegracao() {
    var situacao = _solicitacaoAvaria.Situacao.val();

    if (situacao == EnumSituacaoAvaria.RejeitadaAutorizacao) {
        _integracao.Mensagem.visible(true);
        _integracao.Mensagem.text("Autorização foi rejeitada");
        _integracao.Mensagem.cssClass("danger");
    }
    else {
        _integracao.Mensagem.visible(false);
    }
}