/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoModeloVeicular.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MotivoRetiradaFilaCarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoFilaCarregamentoVeiculoHistorico.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridFilaCarregamentoHistorico;
var _pesquisaFilaCarregamentoHistorico;
var _relatorioFilaCarregamentoHistorico;

/*
 * Declaração das Classes
 */

var PesquisaFilaCarregamentoHistorico = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Centro de Carregamento:"), idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.GrupoModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Grupo de Modelo Veicular:"), idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Modelo Veicular:"), idBtnSearch: guid() });
    this.MotivoRetiradaFilaCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Motivo da Retirada:"), idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Motorista:"), idBtnSearch: guid() });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoFilaCarregamentoVeiculoHistorico.Todos), options: EnumTipoFilaCarregamentoVeiculoHistorico.obterOpcoesPesquisa(), def: EnumTipoFilaCarregamentoVeiculoHistorico.Todos, text: "Tipo: " });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo:"), idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFilaCarregamentoHistorico.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadFilaCarregamentoHistorico() {
    _pesquisaFilaCarregamentoHistorico = new PesquisaFilaCarregamentoHistorico();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridFilaCarregamentoHistorico = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FilaCarregamentoHistorico/Pesquisa", _pesquisaFilaCarregamentoHistorico);

    _gridFilaCarregamentoHistorico.SetPermitirEdicaoColunas(true);
    _gridFilaCarregamentoHistorico.SetQuantidadeLinhasPorPagina(20);

    _relatorioFilaCarregamentoHistorico = new RelatorioGlobal("Relatorios/FilaCarregamentoHistorico/BuscarDadosRelatorio", _gridFilaCarregamentoHistorico, function () {
        _relatorioFilaCarregamentoHistorico.loadRelatorio(function () {
            KoBindings(_pesquisaFilaCarregamentoHistorico, "knockoutPesquisaFilaCarregamentoHistorico", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFilaCarregamentoHistorico", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFilaCarregamentoHistorico", false);

            new BuscarCentrosCarregamento(_pesquisaFilaCarregamentoHistorico.CentroCarregamento);
            new BuscarGrupoModeloVeicular(_pesquisaFilaCarregamentoHistorico.GrupoModeloVeicularCarga);
            new BuscarModelosVeicularesCarga(_pesquisaFilaCarregamentoHistorico.ModeloVeicularCarga);
            new BuscarMotivoRetiradaFilaCarregamento(_pesquisaFilaCarregamentoHistorico.MotivoRetiradaFilaCarregamento, null, true);
            new BuscarMotoristas(_pesquisaFilaCarregamentoHistorico.Motorista);
            new BuscarVeiculos(_pesquisaFilaCarregamentoHistorico.Veiculo);

            var knoutRelatorio = _relatorioFilaCarregamentoHistorico.obterKnoutRelatorio();

            knoutRelatorio.AgruparRelatorio.visible(false);
            knoutRelatorio.PropriedadeAgrupa.visible(false);
            knoutRelatorio.AgruparRelatorio.val(true);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFilaCarregamentoHistorico);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioFilaCarregamentoHistorico.gerarRelatorio("Relatorios/FilaCarregamentoHistorico/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioFilaCarregamentoHistorico.gerarRelatorio("Relatorios/FilaCarregamentoHistorico/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}