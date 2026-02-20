//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaQuantidadePallets, _graficoQuantidadePallets;

var PesquisaQuantidades = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataInicialCarregamento = PropertyEntity({ text: "*Data Inicial de Carregamento:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataFinalCarregamento = PropertyEntity({ text: "*Data Final de Carregamento:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataInicialCarregamento.dateRangeLimit = this.DataFinalCarregamento;
    this.DataFinalCarregamento.dateRangeInit = this.DataInicialCarregamento;

    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:",issue: 320, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:", issue: 830, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:",issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:",issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:",issue: 143, idBtnSearch: guid(), visible: ko.observable(true) });

    this.GerarGrafico = PropertyEntity({ eventClick: GerarGraficoClick, type: types.event, text: "Gerar Gráfico", idGrid: guid() });
    this.DownloadGrafico = PropertyEntity({ eventClick: DownloadGraficoClick, type: types.event, text: "Download", idGrid: guid() });
}

//*******EVENTOS*******

function LoadQuantidadePallets() {
    _pesquisaQuantidadePallets = new PesquisaQuantidades();
    KoBindings(_pesquisaQuantidadePallets, "knockoutPesquisaQuantidade", false);

    new BuscarClientes(_pesquisaQuantidadePallets.Destinatario);
    new BuscarCentrosCarregamento(_pesquisaQuantidadePallets.CentroCarregamento);
    new BuscarRotasFrete(_pesquisaQuantidadePallets.Rota);
    new BuscarFilial(_pesquisaQuantidadePallets.Filial);
    new BuscarVeiculos(_pesquisaQuantidadePallets.Veiculo);
    new BuscarTransportadores(_pesquisaQuantidadePallets.Transportador);

    var options = {
        type: ChartType.BarHorizontal,
        idContainer: "divGrafico",
        properties: {
            x: 'Quantidade',
            xType: ChartPropertyType.int,
            y: 'DescricaoTipo',
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
        title: "Gráfico de Quantidade de Pallets",
        breadcumbTitle: "Quantidades Gerais",
        xTitle: "Quantidade",
        yTitle: "Situação",
        fileName: "Gráfico de Quantidade de Pallets",
        url: "BusinessIntelligence/QuantidadePallets/ObterQuantidadesGerais",
        knockoutParams: _pesquisaQuantidadePallets,
        drillDownSettings: function (obj) {
            var defaultDownSettings = {
                properties: {
                    x: 'Quantidade',
                    xType: ChartPropertyType.int,
                    y: 'NumeroCarga',
                    yType: ChartPropertyType.string,
                    color: 'Cor',
                    order: { prop: 'Quantidade', dir: ChartOrder.Descending }
                },
                margin: {
                    top: 70,
                    right: 50,
                    left: 100,
                    bottom: 50
                },
                xTitle: 'Quantidade de Pallets',
                yTitle: 'Cargas',
                breadcumbTitle: function (obj) {
                    return obj.DescricaoTipo;
                },
                drillDownParams: [{ property: 'Tipo', as: 'Tipo' }],
                url: "BusinessIntelligence/QuantidadePallets/ObterCargasPorTipo",
                drillDownSettings: {
                    properties: {
                        x: 'Quantidade',
                        xType: ChartPropertyType.int,
                        y: function (obj) {
                            return "(" + obj.CPFCNPJFormatado + ") " + obj.Nome;
                        },
                        yType: ChartPropertyType.string,
                        color: 'Cor'
                    },
                    margin: {
                        top: 70,
                        right: 50,
                        left: 320,
                        bottom: 50
                    },
                    xTitle: 'Quantidade de Pallets',
                    yTitle: 'Destinatários',
                    breadcumbTitle: function (obj) {
                        return "Carga " + obj.NumeroCarga;
                    },
                    drillDownParams: [{ property: 'Tipo', as: 'Tipo' },
                    { property: 'Codigo', as: 'Carga' }],
                    url: "BusinessIntelligence/QuantidadePallets/ObterDestinatariosPorCarga"
                }
            };

            if (obj.Tipo == EnumTipoQuantidadeCarga.EmAtraso) {
                return {
                    properties: {
                        x: 'Quantidade',
                        xType: ChartPropertyType.int,
                        y: 'DescricaoTipo',
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
                    xTitle: "Quantidade",
                    yTitle: "Situação",
                    breadcumbTitle: "Em Atraso",
                    drillDownParams: [{ property: 'Tipo', as: 'TipoPai' }],
                    url: "BusinessIntelligence/QuantidadePallets/ObterQuantidadesGerais",
                    drillDownSettings: defaultDownSettings
                };
            } else {
                return defaultDownSettings;
            }
        }
    };

    _graficoQuantidadePallets = new Chart(options);

    _graficoQuantidadePallets.init();
}

function GerarGraficoClick() {
    _graficoQuantidadePallets.init();
}

function DownloadGraficoClick() {
    _graficoQuantidadePallets.download();
}