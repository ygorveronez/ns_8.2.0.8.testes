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
/// <reference path="CotacaoPedido.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _resumoCotacaoPedido;

var ResumoCotacaoPedido = function () {
    this.NumeroCotacaoPedido = PropertyEntity({ text: "Número Cotação: ", visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ text: "Tipo da Operação: ", visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Etapa = PropertyEntity({ text: "Tipo Cliente: ", visible: ko.observable(false) });
    this.Cliente = PropertyEntity({ text: "Cliente: ", visible: ko.observable(true) });
    this.DataPrevisao = PropertyEntity({ text: "Data Previsão Saída: ", visible: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Status: ", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadResumoCotacaoPedido() {
    _resumoCotacaoPedido = new ResumoCotacaoPedido();
    KoBindings(_resumoCotacaoPedido, "knockoutResumoCotacaoPedido");

}

//*******MÉTODOS*******

function preecherResumoCotacaoPedido(data) {
    _resumoCotacaoPedido.NumeroCotacaoPedido.visible(true);
    PreencherObjetoKnout(_resumoCotacaoPedido, data);
}

function limparResumoCotacaoPedido() {
    _resumoCotacaoPedido.NumeroCotacaoPedido.visible(false);
    LimparCampos(_resumoCotacaoPedido);
}