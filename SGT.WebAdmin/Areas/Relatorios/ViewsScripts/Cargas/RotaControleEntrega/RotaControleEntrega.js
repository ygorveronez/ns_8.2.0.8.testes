/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/NotaFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pedido.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRotaControleEntrega, _pesquisaRotaControleEntrega, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioRotaControleEntrega;

var PesquisaRotaControleEntrega = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Carga = PropertyEntity({ text: ko.observable("Número Carga:"), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pedidos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Pedidos:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, def: "", val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.NumerosNotaFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Nº Notas Fiscais:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículos:", idBtnSearch: guid(), visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "Placa", propCodigo: "Codigo" } });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Transportadores:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Emitente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataEntregaPedidoInicial = PropertyEntity({ text: "Data Ent. Ped. Inicial: ", getType: typesKnockout.date });
    this.DataEntregaPedidoFinal = PropertyEntity({ text: "Data Ent. Ped. Final: ", getType: typesKnockout.date });
    this.DataPrevisaoEntregaPedidoInicial = PropertyEntity({ text: "Data Prev. Ent. Ped. Inicial: ", getType: typesKnockout.dateTime });
    this.DataPrevisaoEntregaPedidoFinal = PropertyEntity({ text: "Data Prev. Ent. Ped. Final: ", getType: typesKnockout.dateTime });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataEntregaPedidoInicial.dateRangeLimit = this.DataEntregaPedidoFinal;
    this.DataEntregaPedidoFinal.dateRangeInit = this.DataEntregaPedidoInicial;

    this.DataPrevisaoEntregaPedidoInicial.dateRangeLimit = this.DataPrevisaoEntregaPedidoFinal;
    this.DataPrevisaoEntregaPedidoFinal.dateRangeInit = this.DataPrevisaoEntregaPedidoInicial;
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridRotaControleEntrega.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaRotaControleEntrega.Visible.visibleFade()) {
                _pesquisaRotaControleEntrega.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaRotaControleEntrega.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadRotaControleEntrega() {
    _pesquisaRotaControleEntrega = new PesquisaRotaControleEntrega();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridRotaControleEntrega = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/RotaControleEntrega/Pesquisa", _pesquisaRotaControleEntrega);

    _gridRotaControleEntrega.SetPermitirEdicaoColunas(true);
    _gridRotaControleEntrega.SetQuantidadeLinhasPorPagina(10);

    _relatorioRotaControleEntrega = new RelatorioGlobal("Relatorios/RotaControleEntrega/BuscarDadosRelatorio", _gridRotaControleEntrega, function () {
        _relatorioRotaControleEntrega.loadRelatorio(function () {
            KoBindings(_pesquisaRotaControleEntrega, "knockoutPesquisaRotaControleEntrega", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRotaControleEntrega", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRotaControleEntrega", false);

            new BuscarClientes(_pesquisaRotaControleEntrega.Destinatario);
            new BuscarClientes(_pesquisaRotaControleEntrega.Emitente);
            new BuscarTransportadores(_pesquisaRotaControleEntrega.Transportador);
            new BuscarVeiculos(_pesquisaRotaControleEntrega.Veiculos);
            new BuscarFilial(_pesquisaRotaControleEntrega.Filial);
            new BuscarXMLNotaFiscal(_pesquisaRotaControleEntrega.NumerosNotaFiscal);
            new BuscarPedidos(_pesquisaRotaControleEntrega.Pedidos);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRotaControleEntrega);
}

function retornoProprietario(row) {
    _pesquisaRotaControleEntrega.Proprietario.codEntity(row.Codigo);
    _pesquisaRotaControleEntrega.Proprietario.val(row.Nome);
}

function retornoSelecaoTransportador(transportadorSelecionado) {
    _pesquisaRotaControleEntrega.Transportador.codEntity(transportadorSelecionado.Codigo);
    _pesquisaRotaControleEntrega.Transportador.val(transportadorSelecionado.Descricao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioRotaControleEntrega.gerarRelatorio("Relatorios/RotaControleEntrega/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioRotaControleEntrega.gerarRelatorio("Relatorios/RotaControleEntrega/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}