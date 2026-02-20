/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoQuantidadeCarga.js" />
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
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/RotaFrete.js" />
/// <reference path="../../../../../js/Global/Charts.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaFaturamentoTransportador, _graficoFaturamentoTransportador;

var PesquisaFaturamentoTransportador = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataInicial.dateRangeLimit = this.DataFinalCarregamento;
    this.DataFinal.dateRangeInit = this.DataInicialCarregamento;

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:",issue: 63, idBtnSearch: guid(),  visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", issue: 320, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:",issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", issue: 69, idBtnSearch: guid(),  visible: ko.observable(true) });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:", issue: 830, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Veículo:", issue: 44, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", issue: 53, idBtnSearch: guid(), visible: ko.observable(true) });

    this.GerarGrafico = PropertyEntity({ eventClick: GerarGraficoClick, type: types.event, text: "Gerar Gráfico", idGrid: guid() });
    this.DownloadGrafico = PropertyEntity({ eventClick: DownloadGraficoClick, type: types.event, text: "Download", idGrid: guid() });
}

//*******EVENTOS*******

function LoadFaturamentoTransportador() {
    _pesquisaFaturamentoTransportador = new PesquisaFaturamentoTransportador();
    KoBindings(_pesquisaFaturamentoTransportador, "knockoutPesquisaFaturamentoTransportador", false);

    new BuscarClientes(_pesquisaFaturamentoTransportador.Destinatario);
    new BuscarTransportadores(_pesquisaFaturamentoTransportador.Transportador);
    new BuscarCentrosCarregamento(_pesquisaFaturamentoTransportador.CentroCarregamento);
    new BuscarTiposdeCarga(_pesquisaFaturamentoTransportador.TipoCarga);
    new BuscarModelosVeicularesCarga(_pesquisaFaturamentoTransportador.ModeloVeiculo);
    new BuscarRotasFrete(_pesquisaFaturamentoTransportador.Rota);
    new BuscarFilial(_pesquisaFaturamentoTransportador.Filial);

    var options = {
        type: ChartType.BarHorizontal,
        idContainer: "divGrafico",
        properties: {
            x: 'Valor',
            xType: ChartPropertyType.decimal,
            xText: 'DescricaoValor',
            y: 'Descricao',
            yType: ChartPropertyType.string,
            order: { prop: 'Valor', dir: ChartOrder.Descending }
        },
        margin: {
            top: 70,
            right: 200,
            left: 280,
            bottom: 50
        },
        title: "Gráfico de Faturamento por Transportador",
        xTitle: "Valor Total de Fretes Realizados",
        yTitle: "Transportadores",
        fileName: "Gráfico de Faturamento por Transportador",
        url: "BusinessIntelligence/FaturamentoTransportador/ObterValoresGerais",
        knockoutParams: _pesquisaFaturamentoTransportador
    };

    _graficoFaturamentoTransportador = new Chart(options);

    _graficoFaturamentoTransportador.init();
}

function GerarGraficoClick() {
    _graficoFaturamentoTransportador.init();
}

function DownloadGraficoClick() {
    _graficoFaturamentoTransportador.download();
}