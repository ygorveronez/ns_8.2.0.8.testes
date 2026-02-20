/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoManutencaoServicoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ServicoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Equipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloEquipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaEquipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioManutencaoVeiculo, _gridManutencaoVeiculo, _pesquisaManutencaoVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaManutencaoVeiculo = function () {
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid() });
    this.Servico = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Serviços:", idBtnSearch: guid() });
    this.LocalManutencao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local Manutenção:", idBtnSearch: guid() });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Veículo:", idBtnSearch: guid() });
    this.MarcaVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca do Veículo:", idBtnSearch: guid() });
    this.ModeloEquipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Equipamento:", idBtnSearch: guid() });
    this.MarcaEquipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca do Equipamento:", idBtnSearch: guid() });
    this.SegmentoVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Segmento do Veículo:", idBtnSearch: guid() });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Responsável:", idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });

    this.KMAtual = PropertyEntity({ text: "KM Atual:", getType: typesKnockout.int, maxlength: 100, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "", val: ko.observable("") });
    this.HorimetroAtual = PropertyEntity({ text: "Horímetro Atual:", getType: typesKnockout.int, maxlength: 100, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "", val: ko.observable("") });

    this.VisualizarSomenteVeiculosAtivos = PropertyEntity({ text: "Visualizar somente veículos ativos?", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.VisualizarServicosPendentesManutencao = PropertyEntity({ text: "Visualizar somente serviços pendentes de manutenção?", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.VisualizarSomenteServicosExecutadosAnteriormente = PropertyEntity({ text: "Visualizar somente serviços que já foram executados anteriormente?", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.VisualizarVeiculosEquipamentosAcoplados = PropertyEntity({ text: "Visualizar veículos e equipamentos acoplados?", val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.TipoManutencao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Tipo Manutenção: ", options: EnumTipoManutencaoServicoVeiculo.obterOpcoes(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridManutencaoVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaManutencaoVeiculo.Visible.visibleFade()) {
                _pesquisaManutencaoVeiculo.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaManutencaoVeiculo.Visible.visibleFade(true);
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

function loadRelatorioManutencaoVeiculo() {

    _pesquisaManutencaoVeiculo = new PesquisaManutencaoVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridManutencaoVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ManutencaoVeiculo/Pesquisa", _pesquisaManutencaoVeiculo, null, null, 10);
    _gridManutencaoVeiculo.SetPermitirEdicaoColunas(true);

    _relatorioManutencaoVeiculo = new RelatorioGlobal("Relatorios/ManutencaoVeiculo/BuscarDadosRelatorio", _gridManutencaoVeiculo, function () {
        _relatorioManutencaoVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaManutencaoVeiculo, "knockoutPesquisaManutencaoVeiculo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaManutencaoVeiculo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaManutencaoVeiculo");

            new BuscarVeiculos(_pesquisaManutencaoVeiculo.Veiculo);
            new BuscarServicoVeiculo(_pesquisaManutencaoVeiculo.Servico);
            new BuscarClientes(_pesquisaManutencaoVeiculo.LocalManutencao);
            new BuscarEquipamentos(_pesquisaManutencaoVeiculo.Equipamento);
            new BuscarModelosVeiculo(_pesquisaManutencaoVeiculo.ModeloVeiculo);
            new BuscarMarcasVeiculo(_pesquisaManutencaoVeiculo.MarcaVeiculo);
            new BuscarModeloEquipamentos(_pesquisaManutencaoVeiculo.ModeloEquipamento);
            new BuscarMarcaEquipamentos(_pesquisaManutencaoVeiculo.MarcaEquipamento);
            new BuscarSegmentoVeiculo(_pesquisaManutencaoVeiculo.SegmentoVeiculo);
            new BuscarFuncionario(_pesquisaManutencaoVeiculo.FuncionarioResponsavel);
            new BuscarCentroResultado(_pesquisaManutencaoVeiculo.CentroResultado);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaManutencaoVeiculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioManutencaoVeiculo.gerarRelatorio("Relatorios/ManutencaoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioManutencaoVeiculo.gerarRelatorio("Relatorios/ManutencaoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}