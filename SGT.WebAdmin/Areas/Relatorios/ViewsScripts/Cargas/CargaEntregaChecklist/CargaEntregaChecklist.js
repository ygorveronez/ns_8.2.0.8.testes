/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Globais.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CheckListTipo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

// #region Objetos Globais do Arquivo

var _relatorioCargaEntregaChecklist
var _gridCargaEntregaChecklist;
var _pesquisaCargaEntregaChecklist;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _parametrosBuscarDadosRelatorio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaCheckList = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", getType: typesKnockout.string });
    this.CheckListTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Check List:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataCarregamentoInicial = PropertyEntity({ text: "Data Carregamento Inicial:", getType: typesKnockout.dateTime });
    this.DataCarregamentoFinal = PropertyEntity({ text: "Data Carregamento Final:", getType: typesKnockout.dateTime });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.RemetentePecuaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataCargaInicial = PropertyEntity({ text: "Data Carga Inicial:", getType: typesKnockout.dateTime });
    this.DataCargaFinal = PropertyEntity({ text: "Data Carga Final:", getType: typesKnockout.dateTime });

    this.DataCarregamentoInicial.dateRangeLimit = this.DataCarregamentoFinal;
    this.DataCarregamentoFinal.dateRangeInit = this.DataCarregamentoInicial;
    this.DataCargaInicial.dateRangeLimit = this.DataCargaFinal;
    this.DataCargaFinal.dateRangeInit = this.DataCargaInicial;

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            recarregarGridCargaEntregaChecklist();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaEntregaChecklist.Visible.visibleFade()) {
                _pesquisaCargaEntregaChecklist.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaEntregaChecklist.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadRelatorioCargaEntregaChecklist() {
    _pesquisaCargaEntregaChecklist = new PesquisaCheckList();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _parametrosBuscarDadosRelatorio = obterParametrosBuscarDadosRelatorio();

    _gridCargaEntregaChecklist = new GridView("gridPreviewRelatorio", "Relatorios/CargaEntregaChecklist/Pesquisa", _pesquisaCargaEntregaChecklist, null, null, 10, null, null, null, null, null, null, null);
    _gridCargaEntregaChecklist.SetPermitirEdicaoColunas(true);
    _gridCargaEntregaChecklist.SetQuantidadeLinhasPorPagina(10);

    _relatorioCargaEntregaChecklist = new RelatorioGlobal("Relatorios/CargaEntregaChecklist/BuscarDadosRelatorio", _gridCargaEntregaChecklist, function () {
        _relatorioCargaEntregaChecklist.loadRelatorio(function () {
            KoBindings(_pesquisaCargaEntregaChecklist, "knockoutPesquisaCargaEntregaChecklist");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaEntregaChecklist");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaEntregaChecklist");

            BuscarCheckListTipo(_pesquisaCargaEntregaChecklist.CheckListTipo);
            BuscarTiposOperacao(_pesquisaCargaEntregaChecklist.TipoOperacao);
            BuscarFilial(_pesquisaCargaEntregaChecklist.Filial);
            BuscarTransportadores(_pesquisaCargaEntregaChecklist.Transportador);
            BuscarMotoristas(_pesquisaCargaEntregaChecklist.Motorista);
            BuscarClientes(_pesquisaCargaEntregaChecklist.RemetentePecuaria);
        });
    }, _parametrosBuscarDadosRelatorio, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaEntregaChecklist);
}

// #endregion Funções de Inicialização

// #region Funções associadas a Eventos

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaEntregaChecklist.gerarRelatorio("Relatorios/CargaEntregaChecklist/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

// #endregion Funções associadas a Eventos

// #region Funções Privadas

function obterParametrosBuscarDadosRelatorio() {
    return { CheckListTipo: _pesquisaCargaEntregaChecklist.CheckListTipo.codEntity() };
}

function recarregarGridCargaEntregaChecklist() {
    let parametrosBuscarDadosRelatorio = obterParametrosBuscarDadosRelatorio();
    let parametrosBuscarDadosRelatorioAlterados = false;

    for (let parametro in parametrosBuscarDadosRelatorio) {
        if (parametrosBuscarDadosRelatorio[parametro] != _parametrosBuscarDadosRelatorio[parametro]) {
            parametrosBuscarDadosRelatorioAlterados = true;
            break;
        }
    }

    if (parametrosBuscarDadosRelatorioAlterados) {
        _parametrosBuscarDadosRelatorio = parametrosBuscarDadosRelatorio;

        _relatorioCargaEntregaChecklist.atualizarDadosRelatorio(_parametrosBuscarDadosRelatorio, function () {
            _gridCargaEntregaChecklist.CarregarGrid();
        });
    }
    else
        _gridCargaEntregaChecklist.CarregarGrid();
}

// #endregion Funções Privadas
