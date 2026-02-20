/// <reference path="../../../../../ViewsScripts/Consultas/ServicoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoServico.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumMotivoServicoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoManutencaoServicoVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioServicoVeiculo, _gridServicoVeiculo, _pesquisaServicoVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaServicoVeiculo = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.TipoManutencao = PropertyEntity({ val: ko.observable(EnumTipoManutencaoServicoVeiculo.Todos), options: EnumTipoManutencaoServicoVeiculo.obterOpcoesPesquisa(), def: EnumTipoManutencaoServicoVeiculo.Todos, text: "Tipo Manutenção: " });
    this.Motivo = PropertyEntity({ val: ko.observable(EnumMotivoServicoVeiculo.Todos), options: EnumMotivoServicoVeiculo.obterOpcoesPesquisa(), def: EnumMotivoServicoVeiculo.Todos, text: "Motivo: " });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Servico = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid() });
    this.GrupoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Serviço:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridServicoVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadServicoVeiculo() {
    _pesquisaServicoVeiculo = new PesquisaServicoVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridServicoVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ServicoVeiculo/Pesquisa", _pesquisaServicoVeiculo, null, null, 10);
    _gridServicoVeiculo.SetPermitirEdicaoColunas(true);

    _relatorioServicoVeiculo = new RelatorioGlobal("Relatorios/ServicoVeiculo/BuscarDadosRelatorio", _gridServicoVeiculo, function () {
        _relatorioServicoVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaServicoVeiculo, "knockoutPesquisaServicoVeiculo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaServicoVeiculo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaServicoVeiculo", false);

            new BuscarServicoVeiculo(_pesquisaServicoVeiculo.Servico);
            new BuscarGrupoServico(_pesquisaServicoVeiculo.GrupoServico);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaServicoVeiculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioServicoVeiculo.gerarRelatorio("Relatorios/ServicoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioServicoVeiculo.gerarRelatorio("Relatorios/ServicoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}