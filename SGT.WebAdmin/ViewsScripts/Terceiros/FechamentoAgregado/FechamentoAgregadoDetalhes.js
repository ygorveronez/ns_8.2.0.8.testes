/// <reference path="FechamentoAgregado.js" />
/// <reference path="EtapaFechamentoAgregado.js" />
/// <reference path="Etapa1SelecaoCIOT.js" />
/// <reference path="Etapa2Consolidacao.js" />
/// <reference path="Etapa3Integracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _fechamentoAgregadoDetalhes;
var _gridDetalhes;

var FechamentoAgregadoDetalhes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false, enable: ko.observable(true) });

    this.PesquisarDetalhes = PropertyEntity({
        eventClick: function (e) {
            GridDetalhes();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function LoadFechamentoAgregadoDetalhes() {
    _fechamentoAgregadoDetalhes = new FechamentoAgregadoDetalhes();
    KoBindings(_fechamentoAgregadoDetalhes, "knoutFechamentoAgregadoDetalhes");

    // Inicia grid de dados
}

//*******MÉTODOS*******

function GridDetalhes() {
    let linhasPorPaginas = 5;

    //-- Cabecalho

    let configExportacao = {
        url: "FechamentoAgregado/ExportarPesquisaDetalhes",
        titulo: "Detalhes",
        id: "btnExportarDetalhes"
    };

    _fechamentoAgregadoDetalhes.Codigo.val(_fechamentoAgregado.Codigo.val());
    _gridDetalhes = new GridView(_fechamentoAgregadoDetalhes.PesquisarDetalhes.idGrid, "FechamentoAgregado/PesquisaDetalhes", _fechamentoAgregadoDetalhes, null, null, linhasPorPaginas, null, null, null, null, null, null, configExportacao);
    _gridDetalhes.SetPermitirRedimencionarColunas(true);
    _gridDetalhes.CarregarGrid(function () {
        setTimeout(function () {
            if (_fechamentoAgregado.Codigo.val() > 0)
                $("#btnExportarDetalhes").show();
            else
                $("#btnExportarDetalhes").hide();
        }, 200);
    });
}

function AbrirModalDetalhes(itemGrid) {
    _fechamentoAgregadoDetalhes.CIOT.codEntity(itemGrid.CodigoCIOT);
    _fechamentoAgregadoDetalhes.CIOT.val(itemGrid.NumeroCIOT);

    GridDetalhes();

    Global.abrirModal('divFechamentoAgregadoDetalhes');
}