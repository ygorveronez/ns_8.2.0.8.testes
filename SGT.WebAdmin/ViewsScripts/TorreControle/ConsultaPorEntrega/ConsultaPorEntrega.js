/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumConsultaPorEntregaStatus.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConsultaPorEntrega, _pesquisaConsultaPorEntrega, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioConsultaPorEntrega;

var PesquisaConsultaPorEntrega = function () {
    this.Operacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operação:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.NumeroCarga = PropertyEntity({ type: types.string, val: ko.observable(""), text: "Numero da Carga:" });
    this.NumeroNota = PropertyEntity({ type: types.string, val: ko.observable(""), text: "Numero da Nota:" });
    this.Placa = PropertyEntity({ type: types.string, val: ko.observable(""), text: "Placa:" });
    this.DataPrevisaoEntregaInicial = PropertyEntity({ text: "Data Previsão Entrega: ", getType: typesKnockout.date });
    this.DataPrevisaoEntregaFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.DataCriacaoCargaInicial = PropertyEntity({ text: "Data Criação carga: ", getType: typesKnockout.date });
    this.DataCriacaoCargaFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });

    this.Status = PropertyEntity({ val: ko.observable(EnumConsultaPorEntregaStatus.Todos), options: EnumConsultaPorEntregaStatus.obterOpcoesPesquisa(), text: "Status: ", def: EnumConsultaPorEntregaStatus.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaPorEntrega.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: "gridConsultaPorEntrega", visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function LoadConsultaPorEntrega() {
    _pesquisaConsultaPorEntrega = new PesquisaConsultaPorEntrega();
    KoBindings(_pesquisaConsultaPorEntrega, "knockoutPesquisaConsultaPorEntrega", false, _pesquisaConsultaPorEntrega.Pesquisar.idGrid);

    loadGridConsultaPorEntrega();

    new BuscarTransportadores(_pesquisaConsultaPorEntrega.Transportador);
    new BuscarTiposOperacao(_pesquisaConsultaPorEntrega.Operacao);
}

function loadGridConsultaPorEntrega() {
    var draggableRows = false;
    var draggableRows = false;
    var limiteRegistros = 10;
    var totalRegistrosPorPagina = 10;

    _gridConsultaPorEntrega = new GridView("gridConsultaPorEntrega", "ConsultaPorEntrega/Pesquisa", _pesquisaConsultaPorEntrega, null, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, undefined, null, undefined, undefined);
    _gridConsultaPorEntrega.SetPermitirEdicaoColunas(true);
    _gridConsultaPorEntrega.SetSalvarPreferenciasGrid(true);
    _gridConsultaPorEntrega.SetHabilitarScrollHorizontal(true, 100);
    _gridConsultaPorEntrega.CarregarGrid();
}