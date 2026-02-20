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
/// <reference path="../../../../../ViewsScripts/Consultas/ServicoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOrdemServico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Equipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoOrdemServicoFrota.js" />

var _relatorioOrdemServicoPorMecanico;
var _gridOrdemServicoPorMecanico;
var _pesquisaOrdemServicoPorMecanico;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;

var PesquisaOrdemServicoPorMecanico = function () {
    this.DataInicial = PropertyEntity({ text: "Data Lançamento Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Lançamento Final: ", getType: typesKnockout.date });
    this.NumeroOS = PropertyEntity({ text: "Número OS:" });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.MarcaVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Marca do Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo do Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.LocalManutencao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Local Manutenção:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable([EnumSituacaoOrdemServicoFrota.Finalizada]), def: [EnumSituacaoOrdemServicoFrota.Finalizada], getType: typesKnockout.selectMultiple, options: EnumSituacaoOrdemServicoFrota.ObterOpcoes(), text: "Situação:", visible: ko.observable(true) });
    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Responsável:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Mecanicos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Mecânico:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridOrdemServicoPorMecanico.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaOrdemServicoPorMecanico.Visible.visibleFade()) {
                _pesquisaOrdemServicoPorMecanico.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaOrdemServicoPorMecanico.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

/****** EVENTOS ******/

function loadRelatorioOrdemServicoPorMecanico() {
    _pesquisaOrdemServicoPorMecanico = new PesquisaOrdemServicoPorMecanico();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridOrdemServicoPorMecanico = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/OrdemServicoPorMecanico/Pesquisa", _pesquisaOrdemServicoPorMecanico, null, null, 10);
    _gridOrdemServicoPorMecanico.SetPermitirEdicaoColunas(true);

    _relatorioOrdemServicoPorMecanico = new RelatorioGlobal("Relatorios/OrdemServicoPorMecanico/BuscarDadosRelatorio", _gridOrdemServicoPorMecanico, function () {
        _relatorioOrdemServicoPorMecanico.loadRelatorio(function () {
            KoBindings(_pesquisaOrdemServicoPorMecanico, "knockoutPesquisaOrdemServicoPorMecanico");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaOrdemServicoPorMecanico");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaOrdemServicoPorMecanico");

            new BuscarVeiculos(_pesquisaOrdemServicoPorMecanico.Veiculo);
            new BuscarClientes(_pesquisaOrdemServicoPorMecanico.LocalManutencao);
            new BuscarModelosVeiculo(_pesquisaOrdemServicoPorMecanico.ModeloVeiculo);
            new BuscarTipoOrdemServico(_pesquisaOrdemServicoPorMecanico.Tipo);
            new BuscarMarcasVeiculo(_pesquisaOrdemServicoPorMecanico.MarcaVeiculo);
            new BuscarServicoVeiculo(_pesquisaOrdemServicoPorMecanico.Servico);
            new BuscarProdutoTMS(_pesquisaOrdemServicoPorMecanico.Produto);
            new BuscarEquipamentos(_pesquisaOrdemServicoPorMecanico.Equipamento);
            new BuscarFuncionario(_pesquisaOrdemServicoPorMecanico.Responsavel);
            new BuscarGruposProdutosTMS(_pesquisaOrdemServicoPorMecanico.GrupoProduto);
            new BuscarCentroResultado(_pesquisaOrdemServicoPorMecanico.CentroResultado);
            new BuscarFuncionario(_pesquisaOrdemServicoPorMecanico.Mecanicos);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaOrdemServicoPorMecanico);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioOrdemServicoPorMecanico.gerarRelatorio("Relatorios/OrdemServicoPorMecanico/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioOrdemServicoPorMecanico.gerarRelatorio("Relatorios/OrdemServicoPorMecanico/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}