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
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../js/Global/Charts.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaDirecionamentoOperador, _graficoDirecionamentoOperador;

var PesquisaDirecionamentoOperador = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataInicial.dateRangeLimit = this.DataFinalCarregamento;
    this.DataFinal.dateRangeInit = this.DataInicialCarregamento;

    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:",issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:",issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", issue: 320, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:",issue: 143, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:",issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });

    this.GerarGrafico = PropertyEntity({ eventClick: GerarGraficoClick, type: types.event, text: "Gerar Gráfico", idGrid: guid() });
    this.DownloadGrafico = PropertyEntity({ eventClick: DownloadGraficoClick, type: types.event, text: "Download", idGrid: guid() });
}

//*******EVENTOS*******

function loadDirecionamentoOperador() {
    _pesquisaDirecionamentoOperador = new PesquisaDirecionamentoOperador();
    KoBindings(_pesquisaDirecionamentoOperador, "knockoutPesquisaDirecionamentoOperador", false);

    new BuscarClientes(_pesquisaDirecionamentoOperador.Destinatario);
    new BuscarCentrosCarregamento(_pesquisaDirecionamentoOperador.CentroCarregamento);
    new BuscarTransportadores(_pesquisaDirecionamentoOperador.Transportador);
    new BuscarOperador(_pesquisaDirecionamentoOperador.Operador);
    new BuscarVeiculos(_pesquisaDirecionamentoOperador.Veiculo);
    new BuscarFilial(_pesquisaDirecionamentoOperador.Filial);

    var options = {
        type: ChartType.BarHorizontal,
        idContainer: "divGrafico",
        properties: {
            x: 'Quantidade',
            xType: ChartPropertyType.int,
            y: 'Descricao',
            yType: ChartPropertyType.string,
            order: { prop: 'Quantidade', dir: ChartOrder.Descending }
        },
        margin: {
            top: 70,
            right: 50,
            left: 210,
            bottom: 50
        },
        title: "Gráfico de Direcionamento de Cargas por Operador",
        breadcumbTitle: "Quantidades Gerais",
        xTitle: "Quantidade",
        yTitle: "Operadores",
        fileName: "Gráfico de Direcionamento de Cargas por Operador",
        url: "BusinessIntelligence/DirecionamentoOperador/ObterQuantidadesGerais",
        knockoutParams: _pesquisaDirecionamentoOperador,
        drillDownSettings: {
            type: ChartType.GroupBarHorizontal,
            properties: {
                x: [{ prop: 'QuantidadeDirecionada', color: "#4682b4", text: "Direcionadas" }, { prop: 'QuantidadeRejeitada', color: "#b94747", text: "Rejeitadas" }],
                xType: ChartPropertyType.int,
                y: function (obj) {
                    return "(" + obj.CNPJFormatado + ") " + obj.Nome;
                },
                yType: ChartPropertyType.string,
                order: { prop: 'QuantidadeDirecionada', dir: ChartOrder.Descending }
            },
            margin: {
                top: 70,
                right: 50,
                left: 320,
                bottom: 50
            },
            xTitle: 'Quantidade',
            yTitle: 'Transportadores',
            breadcumbTitle: function (obj) {
                return obj.Descricao;
            },
            drillDownParams: [{ property: 'Codigo', as: 'Operador' }],
            url: "BusinessIntelligence/DirecionamentoOperador/ObterQuantidadesPorTransportador",
        }
    };

    _graficoDirecionamentoOperador = new Chart(options);

    _graficoDirecionamentoOperador.init();
}

function GerarGraficoClick() {
    _graficoDirecionamentoOperador.init();
}

function DownloadGraficoClick() {
    _graficoDirecionamentoOperador.download();
}