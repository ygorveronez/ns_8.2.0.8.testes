//#region Referências

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
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPagamentoMotorista.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEtapaPagamentoMotorista.js" />

//#endregion

var _pesquisaPagamentoMotoristaTMS;
var _relatorioPagamentoMotoristaTMS;
var _gridPagamentoMotoristaTMS;
var _CRUDFiltrosRelatorio;
var _CRUDRelatorio;

var _etapaPagamentoMotorista = [
    { text: "Todos", value: EnumEtapaPagamentoMotorista.Todos },
    { text: "Iniciada", value: EnumEtapaPagamentoMotorista.Iniciada },
    { text: "Ag. Autorizacao", value: EnumEtapaPagamentoMotorista.AgAutorizacao },
    { text: "Integração", value: EnumEtapaPagamentoMotorista.Integracao }
];

var PesquisaPagamentoMotoristaTMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroPagamento = PropertyEntity({ text: "Número do Pagamento:", val: ko.observable(""), def: "" });
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "" });
    this.NumeroDocumento = PropertyEntity({ text: "Número do Documento:", val: ko.observable(""), def: "" });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.PagamentosSemAcertoViagem = PropertyEntity({ text: "Visualizar pagamentos que não estão em acerto de viagem ", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataEfetivacaoInicial = PropertyEntity({ text: "Data Efetivação Inicial:", getType: typesKnockout.date });
    this.DataEfetivacaoFinal = PropertyEntity({ text: "Data Efetivação Final:", getType: typesKnockout.date });
    this.DataEfetivacaoInicial.dateRangeLimit = this.DataEfetivacaoFinal;
    this.DataEfetivacaoFinal.dateRangeInit = this.DataEfetivacaoInicial;

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPagamentoMotorista.Todas), options: EnumSituacaoPagamentoMotorista.obterOpcoesPesquisa(), def: EnumSituacaoPagamentoMotorista.Todas, text: "Situação: " });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaPagamentoMotorista.Todos), options: _etapaPagamentoMotorista, def: EnumEtapaPagamentoMotorista.Todos, text: "Etapa: " });

    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.TipoPagamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Pagamento:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Favorecido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Favorecido:", idBtnSearch: guid() });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPagamentoMotoristaTMS.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPagamentoMotoristaTMS.Visible.visibleFade()) {
                _pesquisaPagamentoMotoristaTMS.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPagamentoMotoristaTMS.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

function LoadPagamentoMotoristaTMS() {
    _pesquisaPagamentoMotoristaTMS = new PesquisaPagamentoMotoristaTMS();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPagamentoMotoristaTMS = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PagamentoMotoristaTMS/Pesquisa", _pesquisaPagamentoMotoristaTMS, null, null, 10);
    _gridPagamentoMotoristaTMS.SetPermitirEdicaoColunas(true);

    _relatorioPagamentoMotoristaTMS = new RelatorioGlobal("Relatorios/PagamentoMotoristaTMS/BuscarDadosRelatorio", _gridPagamentoMotoristaTMS, function () {
        _relatorioPagamentoMotoristaTMS.loadRelatorio(function () {
            KoBindings(_pesquisaPagamentoMotoristaTMS, "knockoutPesquisaPagamentoMotorista");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPagamentoMotoristaTMS");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMotoristaTMS");

            new BuscarFuncionario(_pesquisaPagamentoMotoristaTMS.Operador);
            new BuscarMotorista(_pesquisaPagamentoMotoristaTMS.Motorista);
            new BuscarPagamentoMotoristaTipo(_pesquisaPagamentoMotoristaTMS.TipoPagamento);
            new BuscarClientes(_pesquisaPagamentoMotoristaTMS.Favorecido);
            new BuscarClientes(_pesquisaPagamentoMotoristaTMS.Tomador);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPagamentoMotoristaTMS);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPagamentoMotoristaTMS.gerarRelatorio("Relatorios/PagamentoMotoristaTMS/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPagamentoMotoristaTMS.gerarRelatorio("Relatorios/PagamentoMotoristaTMS/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
