/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/OrdemServico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEntradaSaida.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGuaritaTMS, _pesquisaGuaritaTMS, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioGuaritaTMS;
var _pesquisaTipoEntradaSaida = [
    { text: "Todos", value: EnumEntradaSaida.Todos },
    { text: "Entrada", value: EnumEntradaSaida.Entrada },
    { text: "Saída", value: EnumEntradaSaida.Saida }
];

var PesquisaGuaritaTMS = function () {
    this.Veiculo = PropertyEntity({ text: "Veículo:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Carga = PropertyEntity({ text: "Carga:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.OrdemServico = PropertyEntity({ text: "Ordem de Serviço:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ text: "Motorista:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Operador = PropertyEntity({ text: "Operador:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ text: "Empresa:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.KMInicial = PropertyEntity({ text: "Km Inicial:", getType: typesKnockout.int });
    this.KMFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.date });
    this.TipoEntradaSaida = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: _pesquisaTipoEntradaSaida, def: EnumEntradaSaida.Todos, text: "Entrada/Saída: " });    

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridGuaritaTMS.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaGuaritaTMS.Visible.visibleFade() == true) {
                _pesquisaGuaritaTMS.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaGuaritaTMS.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioGuaritaTMS() {
    _pesquisaGuaritaTMS = new PesquisaGuaritaTMS();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridGuaritaTMS = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/GuaritaTMS/Pesquisa", _pesquisaGuaritaTMS);

    _gridGuaritaTMS.SetPermitirEdicaoColunas(true);
    _gridGuaritaTMS.SetQuantidadeLinhasPorPagina(10);

    _relatorioGuaritaTMS = new RelatorioGlobal("Relatorios/GuaritaTMS/BuscarDadosRelatorio", _gridGuaritaTMS, function () {
        _relatorioGuaritaTMS.loadRelatorio(function () {
            KoBindings(_pesquisaGuaritaTMS, "knockoutPesquisaGuaritaTMS", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaGuaritaTMS", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaGuaritaTMS", false);

            // Buscas
            new BuscarCargas(_pesquisaGuaritaTMS.Carga);
            new BuscarOrdemServico(_pesquisaGuaritaTMS.OrdemServico, RetornoOrdemServico);
            new BuscarMotoristas(_pesquisaGuaritaTMS.Motorista);
            new BuscarFuncionario(_pesquisaGuaritaTMS.Operador);
            new BuscarVeiculos(_pesquisaGuaritaTMS.Veiculo);
            new BuscarEmpresa(_pesquisaGuaritaTMS.Empresa, null, true);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaGuaritaTMS);
}

function RetornoOrdemServico(data) {
    _pesquisaGuaritaTMS.OrdemServico.val(data.Numero);
    _pesquisaGuaritaTMS.OrdemServico.codEntity(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioGuaritaTMS.gerarRelatorio("Relatorios/GuaritaTMS/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioGuaritaTMS.gerarRelatorio("Relatorios/GuaritaTMS/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
