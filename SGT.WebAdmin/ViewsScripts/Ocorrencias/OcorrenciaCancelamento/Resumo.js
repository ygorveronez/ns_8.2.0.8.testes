/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _resumo;

var Resumo = function () {
    this.Resumo = PropertyEntity({ visible: ko.observable(false) });

    this.Ocorrencia = PropertyEntity({ type: types.map, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Ocorrencia, visible: ko.observable(true) });
    this.DataCancelamento = PropertyEntity({ type: types.map, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DataCancelamento.getFieldDescription(), visible: ko.observable(true) });
    this.Operador = PropertyEntity({ type: types.map, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Operador.getFieldDescription(), visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ type: types.map, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Tipo.getFieldDescription(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ type: types.map, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Situacao.getFieldDescription(), visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadResumo() {
    _resumo = new Resumo();
    KoBindings(_resumo, "knockoutResumoCancelamento");
}


//*******MÉTODOS*******
function PreencherResumo(data) {
    if (data.Resumo != null) {
        PreencherObjetoKnout(_resumo, { Data: data.Resumo });
        _resumo.Resumo.visible(true);
    } else {
        _resumo.Resumo.visible(false);
    }
}

function LimparCamposResumo() {
    LimparCampos(_resumo);
    _resumo.Resumo.visible(false);
}