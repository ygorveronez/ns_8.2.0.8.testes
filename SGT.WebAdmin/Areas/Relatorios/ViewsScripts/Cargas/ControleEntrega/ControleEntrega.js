/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
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

//********MAPEAMENTO KNOCKOUT********

var _relatorioControleEntrega, _gridControleEntrega, _pesquisaControleEntrega, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaControleEntrega = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataOcorrenciaInicial = PropertyEntity({ text: "Data da Ocorrência Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataOcorrenciaFinal = PropertyEntity({ text: "Data da Ocorrência Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.TipoOcorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo da Ocorrência:", idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "" });
    this.NumeroCTe = PropertyEntity({ text: "Número do CT-e:", val: ko.observable(""), def: "" });
    this.NumeroNotaFiscal = PropertyEntity({ text: "Número da Nota Fiscal:", val: ko.observable(""), def: "" });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.DataPrevisaoEntregaPedidoInicial = PropertyEntity({ text: "Previsão Entrega Pedido Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataPrevisaoEntregaPedidoFinal = PropertyEntity({ text: "Previsão Entrega Pedido Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.UFOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado de Origem:", idBtnSearch: guid() });
    this.UFDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid() });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridControleEntrega.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    /* BUSCA AVANÇADA */
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaControleEntrega.Visible.visibleFade() === true) {
                _pesquisaControleEntrega.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaControleEntrega.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadControleEntrega() {
    _pesquisaControleEntrega = new PesquisaControleEntrega();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridControleEntrega = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ControleEntrega/Pesquisa", _pesquisaControleEntrega, null, null, 10);
    _gridControleEntrega.SetPermitirEdicaoColunas(true);

    _relatorioControleEntrega = new RelatorioGlobal("Relatorios/ControleEntrega/BuscarDadosRelatorio", _gridControleEntrega, function () {
        _relatorioControleEntrega.loadRelatorio(function () {
            KoBindings(_pesquisaControleEntrega, "knockoutPesquisaControleEntrega", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaControleEntrega", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaControleEntrega", false);

            new BuscarTipoOcorrencia(_pesquisaControleEntrega.TipoOcorrencia, null, null, null, null, null, null, null, true);
            new BuscarGruposPessoas(_pesquisaControleEntrega.GrupoPessoas);
            new BuscarTiposOperacao(_pesquisaControleEntrega.TipoOperacao);
            new BuscarVeiculos(_pesquisaControleEntrega.Veiculo);
            new BuscarMotoristas(_pesquisaControleEntrega.Motorista);
            new BuscarEstados(_pesquisaControleEntrega.UFOrigem);
            new BuscarEstados(_pesquisaControleEntrega.UFDestino);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaControleEntrega);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioControleEntrega.gerarRelatorio("Relatorios/ControleEntrega/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioControleEntrega.gerarRelatorio("Relatorios/ControleEntrega/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}