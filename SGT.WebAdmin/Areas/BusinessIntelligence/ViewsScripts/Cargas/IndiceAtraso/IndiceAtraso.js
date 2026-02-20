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

var _pesquisaIndiceAtraso, _graficoIndiceAtraso;

var PesquisaIndiceAtraso = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataInicial.dateRangeLimit = this.DataFinalCarregamento;
    this.DataFinal.dateRangeInit = this.DataInicialCarregamento;

    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:",issue: 320, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:",issue: 70,  idBtnSearch: guid(), visible: ko.observable(true) });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:",issue: 830, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo de Veículo:", issue: 44, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:",issue: 53,  idBtnSearch: guid(), visible: ko.observable(true) });

    this.GerarGrafico = PropertyEntity({ eventClick: GerarGraficoClick, type: types.event, text: "Gerar Gráfico", idGrid: guid() });
    this.DownloadGrafico = PropertyEntity({ eventClick: DownloadGraficoClick, type: types.event, text: "Download", idGrid: guid() });
}

//*******EVENTOS*******

function LoadIndiceAtraso() {
    _pesquisaIndiceAtraso = new PesquisaIndiceAtraso();
    KoBindings(_pesquisaIndiceAtraso, "knockoutPesquisaValorMedioFrete", false);

    new BuscarClientes(_pesquisaIndiceAtraso.Destinatario);
    new BuscarCentrosCarregamento(_pesquisaIndiceAtraso.CentroCarregamento);
    new BuscarTransportadores(_pesquisaIndiceAtraso.Transportador);
    new BuscarOperador(_pesquisaIndiceAtraso.Operador);
    new BuscarFilial(_pesquisaIndiceAtraso.Filial);
    new BuscarRotasFrete(_pesquisaIndiceAtraso.Rota);
    new BuscarTiposdeCarga(_pesquisaIndiceAtraso.TipoCarga);
    new BuscarModelosVeicularesCarga(_pesquisaIndiceAtraso.ModeloVeiculo);

    var options = {
        type: ChartType.Bar,
        idContainer: "divGrafico",
        properties: {
            y: 'Valor',
            yText: 'DescricaoValor',
            x: 'Descricao',
            xType: ChartPropertyType.string,
            color: 'Cor'
        },
        margin: {
            top: 70,
            right: 50,
            left: 80,
            bottom: 50
        },
        title: "Gráfico de Índice de Atraso",
        yTitle: "Total de Cargas",
        xTitle: "Situação",
        fileName: "Gráfico de Índice de Atraso",
        url: "BusinessIntelligence/IndiceAtraso/ObterValoresGerais",
        knockoutParams: _pesquisaIndiceAtraso,
        breadcumbTitle: "Dados Gerais",
        drillDownSettings: function (obj) {
            if (obj.ForaPrazo) {
                return {
                    breadcumbTitle: function (obj) {
                        return "Fora do Prazo - " + obj.DescricaoValor;
                    },
                    properties: {
                        y: 'Valor',
                        yText: 'DescricaoValor',
                        x: 'Descricao',
                        xType: ChartPropertyType.string,
                        color: 'Cor'
                    },
                    url: "BusinessIntelligence/IndiceAtraso/ObterValoresForaPrazo"
                };
            } else {
                return null;
            }
        }
    };

    _graficoIndiceAtraso = new Chart(options);

    _graficoIndiceAtraso.init();
}

function GerarGraficoClick() {
    _graficoIndiceAtraso.init();
}

function DownloadGraficoClick() {
    _graficoIndiceAtraso.download();
}