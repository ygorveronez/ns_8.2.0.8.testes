/// <reference path="Pedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridOcorrenciasPedido;
var _ocorrenciasPedido;

/*
 * Declaração das Classes
 */

var OcorrenciaPedido = function () {
    this.Ocorrencia = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.OcorrenciasPedido, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.LinkRastreio = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.LinkRastreio.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadOcorrenciaPedido() {
    _ocorrenciasPedido = new OcorrenciaPedido();
    KoBindings(_ocorrenciasPedido, "knockoutOcorrenciasPedido");

    var header = [
        { data: "Codigo", visible: false },
        { data: "DataOcorrencia", title: Localization.Resources.Pedidos.Pedido.DataOcorrencia, width: "10%", className: "text-align-center" },
        { data: "Descricao", title: Localization.Resources.Pedidos.Pedido.Ocorrencia, width: "20%", className: "text-align-left" },
        { data: "Cliente", title: Localization.Resources.Pedidos.Pedido.LocalOcorrencia, width: "20%", className: "text-align-left" },
        { data: "Pacote", title: Localization.Resources.Pedidos.Pedido.Pacote, width: "8%", className: "text-align-left", visible: VisibilidadePacotesOcorrencia() },
        { data: "Volumes", title: Localization.Resources.Pedidos.Pedido.Volumes, width: "7%", className: "text-align-right", visible: VisibilidadePacotesOcorrencia() },
        { data: "Observacao", title: Localization.Resources.Gerais.Geral.Observacao, width: "25%", className: "text-align-left" },
    ];
    _gridOcorrenciasPedido = new BasicDataTable(_ocorrenciasPedido.Ocorrencia.idGrid, header, null);
    carregarGridOcorrenciaPedido(new Array());
}

function carregarGridOcorrenciaPedido(dataGrid) {

    _gridOcorrenciasPedido.CarregarGrid(dataGrid);
}

function VisibilidadePacotesOcorrencia() {
    if (_CONFIGURACAO_TMS.ExibirPacotesOcorrencia)
        return true;

    return false;
}
