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
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoOrdemServicoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioDespesaOrdemServicoProduto, _gridDespesaOrdemServicoProduto, _pesquisaDespesaOrdemServicoProduto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaDespesaOrdemServicoProduto = function () {
    this.DataInicial = PropertyEntity({ text: "Data Lançamento Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Lançamento Final: ", getType: typesKnockout.date });
    this.NumeroOS = PropertyEntity({ text: "Número OS:" });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.MarcaVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca do Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo do Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.LocalManutencao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local Manutenção:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.GrupoProduto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable([EnumSituacaoOrdemServicoFrota.Finalizada]), def: [EnumSituacaoOrdemServicoFrota.Finalizada], getType: typesKnockout.selectMultiple, options: EnumSituacaoOrdemServicoFrota.ObterOpcoes(), text: "Situação:", visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDespesaOrdemServicoProduto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioDespesaOrdemServicoProduto() {

    _pesquisaDespesaOrdemServicoProduto = new PesquisaDespesaOrdemServicoProduto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDespesaOrdemServicoProduto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DespesaOrdemServicoProduto/Pesquisa", _pesquisaDespesaOrdemServicoProduto, null, null, 10);
    _gridDespesaOrdemServicoProduto.SetPermitirEdicaoColunas(true);

    _relatorioDespesaOrdemServicoProduto = new RelatorioGlobal("Relatorios/DespesaOrdemServicoProduto/BuscarDadosRelatorio", _gridDespesaOrdemServicoProduto, function () {
        _relatorioDespesaOrdemServicoProduto.loadRelatorio(function () {
            KoBindings(_pesquisaDespesaOrdemServicoProduto, "knockoutPesquisaDespesaOrdemServicoProduto");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDespesaOrdemServicoProduto");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDespesaOrdemServicoProduto");

            new BuscarVeiculos(_pesquisaDespesaOrdemServicoProduto.Veiculo);
            new BuscarClientes(_pesquisaDespesaOrdemServicoProduto.LocalManutencao);
            new BuscarModelosVeiculo(_pesquisaDespesaOrdemServicoProduto.ModeloVeiculo);
            new BuscarTipoOrdemServico(_pesquisaDespesaOrdemServicoProduto.Tipo);
            new BuscarMarcasVeiculo(_pesquisaDespesaOrdemServicoProduto.MarcaVeiculo);
            new BuscarServicoVeiculo(_pesquisaDespesaOrdemServicoProduto.Servico);
            new BuscarProdutoTMS(_pesquisaDespesaOrdemServicoProduto.Produto);
            new BuscarGruposProdutosTMS(_pesquisaDespesaOrdemServicoProduto.GrupoProduto);
            new BuscarEquipamentos(_pesquisaDespesaOrdemServicoProduto.Equipamento);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDespesaOrdemServicoProduto);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDespesaOrdemServicoProduto.gerarRelatorio("Relatorios/DespesaOrdemServicoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDespesaOrdemServicoProduto.gerarRelatorio("Relatorios/DespesaOrdemServicoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}