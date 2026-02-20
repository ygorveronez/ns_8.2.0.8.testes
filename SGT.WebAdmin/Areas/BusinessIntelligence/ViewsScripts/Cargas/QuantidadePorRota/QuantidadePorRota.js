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
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/RotaFrete.js" />
/// <reference path="../../../../../js/Global/Charts.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaQuantidadesPorRota, _graficoQuantidadesPorRota;

var PesquisaQuantidadesPorRota = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataInicialCarregamento = PropertyEntity({ text: "*Data Inicial de Carregamento:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataFinalCarregamento = PropertyEntity({ text: "*Data Final de Carregamento:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataInicialCarregamento.dateRangeLimit = this.DataFinalCarregamento;
    this.DataFinalCarregamento.dateRangeInit = this.DataInicialCarregamento;
    
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", issue: 320, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:",issue: 830,  idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:",issue: 70,  idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:",issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:",issue: 143, idBtnSearch: guid(), visible: ko.observable(true) });

    this.GerarGrafico = PropertyEntity({ eventClick: GerarGraficoClick, type: types.event, text: "Gerar Gráfico", idGrid: guid() });
    this.DownloadGrafico = PropertyEntity({ eventClick: DownloadGraficoClick, type: types.event, text: "Download", idGrid: guid() });
    this.ExportarGraficoExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Exportar Gráfico", idGrid: guid() });
}

//*******EVENTOS*******

function loadQuantidadesPorRota() {
    _pesquisaQuantidadesPorRota = new PesquisaQuantidadesPorRota();
    KoBindings(_pesquisaQuantidadesPorRota, "knockoutPesquisaQuantidadeRota", false);
    
    new BuscarCentrosCarregamento(_pesquisaQuantidadesPorRota.CentroCarregamento);
    new BuscarRotasFrete(_pesquisaQuantidadesPorRota.Rota);
    new BuscarFilial(_pesquisaQuantidadesPorRota.Filial);
    new BuscarVeiculos(_pesquisaQuantidadesPorRota.Veiculo);
    new BuscarTransportadores(_pesquisaQuantidadesPorRota.Transportador);

    var options = {
        type: ChartType.BarHorizontal,
        idContainer: "divGrafico",
        properties: {
            x: 'Quantidade',
            xType: ChartPropertyType.int,
            y: 'Rota',
            yType: ChartPropertyType.string,
            color: 'Cor',
            order: 'Ordem'
        },
        margin: {
            top: 70,
            right: 50,
            left: 210,
            bottom: 50
        },
        title: "Gráfico de Quantidade de Cargas Por Rota",
        breadcumbTitle: "QuantidadesPorRota Gerais",
        xTitle: "Quantidade",
        yTitle: "Rota",
        fileName: "Gráfico de QuantidadesPorRota de Cargas por Rota",
        url: "BusinessIntelligence/QuantidadePorRota/ObterQuantidadesGeraisPorRota",
        knockoutParams: _pesquisaQuantidadesPorRota
        //drillDownSettings: function (obj) {

        //    var defaultDownSettings = {
        //        properties: {
        //            x: 'DiasAtraso',
        //            xType: ChartPropertyType.int,
        //            y: 'NumeroCarga',
        //            yType: ChartPropertyType.string,
        //            color: 'Cor',
        //            order: { prop: 'DiasAtraso', dir: ChartOrder.Descending }
        //        },
        //        margin: {
        //            top: 70,
        //            right: 50,
        //            left: 100,
        //            bottom: 50
        //        },
        //        xTitle: 'Dias em Atraso',
        //        yTitle: 'Cargas',
        //        breadcumbTitle: function (obj) {
        //            return obj.DescricaoTipo;
        //        },
        //        drillDownParams: [{ property: 'Tipo', as: 'Tipo' }],
        //        url: "BusinessIntelligence/Quantidade/ObterCargasPorTipo",
        //        drillDownSettings: {
        //            properties: {
        //                x: 'ValorFrete',
        //                xType: ChartPropertyType.decimal,
        //                y: function (obj) {
        //                    return "(" + obj.CPFCNPJFormatado + ") " + obj.Nome;
        //                },
        //                yType: ChartPropertyType.string,
        //                color: 'Cor'
        //            },
        //            margin: {
        //                top: 70,
        //                right: 50,
        //                left: 320,
        //                bottom: 50
        //            },
        //            xTitle: 'Valor do Frete',
        //            yTitle: 'Destinatários',
        //            breadcumbTitle: function (obj) {
        //                return "Carga " + obj.NumeroCarga;
        //            },
        //            drillDownParams: [{ property: 'Tipo', as: 'Tipo' },
        //            { property: 'Codigo', as: 'Carga' }],
        //            url: "BusinessIntelligence/Quantidade/ObterDestinatariosPorCarga"
        //        }
        //    };

        //    if (obj.Tipo == EnumTipoQuantidadeCarga.EmAtraso) {
        //        return {
        //            properties: {
        //                x: 'Quantidade',
        //                xType: ChartPropertyType.int,
        //                y: 'DescricaoTipo',
        //                yType: ChartPropertyType.string,
        //                color: 'Cor',
        //                order: 'Ordem'
        //            },
        //            margin: {
        //                top: 70,
        //                right: 50,
        //                left: 210,
        //                bottom: 50
        //            },
        //            xTitle: "Quantidade",
        //            yTitle: "Situação",
        //            breadcumbTitle: "Em Atraso",
        //            drillDownParams: [{ property: 'Tipo', as: 'TipoPai' }],
        //            url: "BusinessIntelligence/Quantidade/ObterQuantidadesPorRotaGerais",
        //            drillDownSettings: defaultDownSettings
        //        };
        //    } else {
        //        return defaultDownSettings;
        //    }
        //}
    };

    _graficoQuantidadesPorRota = new Chart(options);

    _graficoQuantidadesPorRota.init();
}

function GerarGraficoClick() {
    _graficoQuantidadesPorRota.init();
}

function DownloadGraficoClick() {
    _graficoQuantidadesPorRota.download();
}

function GerarRelatorioExcelClick(e) {
    executarDownload("BusinessIntelligence/QuantidadePorRota/ExportarRelatorioExcel", RetornarObjetoPesquisa(e));

}