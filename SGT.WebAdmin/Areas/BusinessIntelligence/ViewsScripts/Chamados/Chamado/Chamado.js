/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../js/Global/Charts.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaChamado, _graficoChamado;

var PesquisaChamado = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", val: ko.observable(PrimeiroDiaMes()), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", val: ko.observable(UltimoDiaMes()), getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinalCarregamento;
    this.DataFinal.dateRangeInit = this.DataInicialCarregamento;

    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Responsável:", idBtnSearch: guid() });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", issue: 69, idBtnSearch: guid() });

    this.GerarGrafico = PropertyEntity({ eventClick: GerarGraficoClick, type: types.event, text: "Gerar Gráfico", idGrid: guid() });
    this.DownloadGrafico = PropertyEntity({ eventClick: DownloadGraficoClick, type: types.event, text: "Download", idGrid: guid() });
}

//*******EVENTOS*******

function LoadChamado() {
    _pesquisaChamado = new PesquisaChamado();
    KoBindings(_pesquisaChamado, "knockoutPesquisaValorMedioFrete", false);

    new BuscarFuncionario(_pesquisaChamado.Responsavel);
    new BuscarMotivoChamado(_pesquisaChamado.Motivo);
    new BuscarTransportadores(_pesquisaChamado.Transportador);

    var options = {
        type: ChartType.Bar,
        idContainer: "divGrafico",
        properties: {
            y: 'Quantidade',
            yText: 'Descricao',
            x: 'DescricaoSituacao',
            xType: ChartPropertyType.int,
            //color: 'Cor'
        },
        margin: {
            top: 70,
            right: 50,
            left: 80,
            bottom: 50
        },
        title: "Gráfico de Chamados",
        yTitle: "Quantidade",
        xTitle: "Situação",
        fileName: "Gráfico de Chamados",
        url: "BusinessIntelligence/Chamado/ObterDadosChamado",
        knockoutParams: _pesquisaChamado,
        breadcumbTitle: "Dados Gerais"
    };

    _graficoChamado = new Chart(options);

    _graficoChamado.init();
}

function GerarGraficoClick() {
    _graficoChamado.init();
}

function DownloadGraficoClick() {
    _graficoChamado.download();
}



//*******METODOS*******
function PrimeiroDiaMes() {
    return "01/" + moment(new Date).format("MM/YYYY");
}

function UltimoDiaMes() {
    var date = new Date;
    var ultimoDia = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    return moment(ultimoDia).format("DD/MM/YYYY");
}