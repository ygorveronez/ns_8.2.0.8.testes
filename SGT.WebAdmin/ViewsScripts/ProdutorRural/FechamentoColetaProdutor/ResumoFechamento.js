/// <reference path="SolicitacaoFechamento.js" />
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

var _resumoFechamento;

var ResumoFechamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(false) });
    this.DescricaoSituacao = PropertyEntity({ text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: true, text: "*Transportador:" });
    this.DescricaoTipoTomador = PropertyEntity({ text: "Tipo do Tomador: " });
    this.MotivoCancelamento = PropertyEntity({ text: "Motivo do Cancelamento: ", visible: ko.observable(false) });
}


//*******EVENTOS*******

function loadResumoFechamento() {
    _resumoFechamento = new ResumoFechamento();
    KoBindings(_resumoFechamento, "knockoutResumoFechamento");
}


//*******MÉTODOS*******

function PreecherResumoFechamento(arg) {
    _resumoFechamento.Numero.visible(true);
    PreencherObjetoKnout(_resumoFechamento, arg);
    if (_resumoFechamento.MotivoCancelamento.val() != null && _resumoFechamento.MotivoCancelamento.val() != "")
        _resumoFechamento.MotivoCancelamento.visible(true);
    else
        _resumoFechamento.MotivoCancelamento.visible(false);
}

function limparResumo() {
    _resumoFechamento.Numero.visible(false);
    LimparCampos(_resumoFechamento);
}
