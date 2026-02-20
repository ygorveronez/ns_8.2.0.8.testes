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
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoRecebimentoMercadoria.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConferenciaVolume, _pesquisaConferenciaVolume, _CRUDRelatorio, _relatorioConferenciaVolume, _CRUDFiltrosRelatorio;

var _situacaoRecebimento = [{ text: "Todos", value: EnumSituacaoRecebimentoMercadoria.Todos },
{ text: "Finalizado", value: EnumSituacaoRecebimentoMercadoria.Finalizado },
{ text: "Iniciado", value: EnumSituacaoRecebimentoMercadoria.Iniciado },
{ text: "Cancelado", value: EnumSituacaoRecebimentoMercadoria.Cancelado }];


var PesquisaConferenciaVolume = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MDFe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "MDF-e:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoRecebimentoMercadoria.Todos), options: _situacaoRecebimento, def: EnumSituacaoRecebimentoMercadoria.Todos, text: "Situação: " });

    this.DataConferenciaInicial = PropertyEntity({ text: "Data Conferência de: ", getType: typesKnockout.date });
    this.DataConferenciaFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.DataEmbarqueInicial = PropertyEntity({ text: "Data Embarque de: ", getType: typesKnockout.date });
    this.DataEmbarqueFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.Conferente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Conferente:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: " });
    this.NumeroNota = PropertyEntity({ text: "Número Nota: " });
    this.CodigoBarras = PropertyEntity({ text: "Cód. Barras: " });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridConferenciaVolume.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioConferenciaVolume() {

    _pesquisaConferenciaVolume = new PesquisaConferenciaVolume();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConferenciaVolume = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/ConferenciaVolume/Pesquisa", _pesquisaConferenciaVolume, null, null, 10);
    _gridConferenciaVolume.SetPermitirEdicaoColunas(true);

    _relatorioConferenciaVolume = new RelatorioGlobal("Relatorios/ConferenciaVolume/BuscarDadosRelatorio", _gridConferenciaVolume, function () {
        _relatorioConferenciaVolume.loadRelatorio(function () {
            KoBindings(_pesquisaConferenciaVolume, "knockoutPesquisaConferenciaVolume");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConferenciaVolume");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConferenciaVolume");

            new BuscarCargas(_pesquisaConferenciaVolume.Carga);
            new BuscarVeiculos(_pesquisaConferenciaVolume.Veiculo);
            new BuscarFuncionario(_pesquisaConferenciaVolume.Conferente);
            new BuscarMDFes(_pesquisaConferenciaVolume.MDFe, RetornoMDFe);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConferenciaVolume);
}

function RetornoMDFe(dataRetorno) {
    _pesquisaConferenciaVolume.MDFe.codEntity(dataRetorno.Codigo);
    _pesquisaConferenciaVolume.MDFe.val(dataRetorno.Numero);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioConferenciaVolume.gerarRelatorio("Relatorios/ConferenciaVolume/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioConferenciaVolume.gerarRelatorio("Relatorios/ConferenciaVolume/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
