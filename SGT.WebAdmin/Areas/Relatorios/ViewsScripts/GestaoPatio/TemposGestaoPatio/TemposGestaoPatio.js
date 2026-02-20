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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoFluxoGestaoPatio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoEtapaFluxoGestaoPatio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/RotaFrete.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTemposGestaoPatios, _pesquisaTemposGestaoPatios, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioTemposGestaoPatios;

var _etapasFluxoPatio = [
    { text: "Todas", value: EnumEtapaFluxoGestaoPatio.Todas }
];

var _situacoesFluxo = [
    { text: "Todas", value: "" },
    { text: "Aguardando", value: EnumSituacaoEtapaFluxoGestaoPatio.Aguardando },
    { text: "Aprovado", value: EnumSituacaoEtapaFluxoGestaoPatio.Aprovado },
    { text: "Rejeitado", value: EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado }
];

var PesquisaTemposGestaoPatios = function () {
    var dataAtual = moment().format("DD/MM/YYYY");
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.DataInicioCarregamento = PropertyEntity({ text: "Data Início: ", issue: 2, val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.dateTime });
    this.DataFimCarregamento = PropertyEntity({ text: "Data Fim: ", issue: 2, val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.dateTime });
    this.EtapaFluxoGestaoPatio = PropertyEntity({ val: ko.observable(EnumEtapaFluxoGestaoPatio.Todas), options: ko.observableArray(_etapasFluxoPatio), text: "Etapa: ", def: EnumEtapaFluxoGestaoPatio.Todas });
    this.Transportador = PropertyEntity({ text: "Transportador:", issue: 69, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", issue: 145, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", issue: 143, idBtnSearch: guid(), issue: 143 });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:", idBtnSearch: guid() });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "" });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable("desc"), options: _situacoesFluxo, text: "Situação: ", def: "" });
    this.ListarCargasCanceladas = PropertyEntity({ text: "Exibir Cargas Canceladas", getType: typesKnockout.bool, val: ko.observable(false) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTemposGestaoPatios.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioTemposGestaoPatios() {
    _pesquisaTemposGestaoPatios = new PesquisaTemposGestaoPatios();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTemposGestaoPatios = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TemposGestaoPatio/Pesquisa", _pesquisaTemposGestaoPatios, null);

    _gridTemposGestaoPatios.SetPermitirEdicaoColunas(true);
    _gridTemposGestaoPatios.SetQuantidadeLinhasPorPagina(10);

    _relatorioTemposGestaoPatios = new RelatorioGlobal("Relatorios/TemposGestaoPatio/BuscarDadosRelatorio", _gridTemposGestaoPatios, function () {
        _relatorioTemposGestaoPatios.loadRelatorio(function () {
            KoBindings(_pesquisaTemposGestaoPatios, "knockoutPesquisaTemposGestaoPatios", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTemposGestaoPatios", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTemposGestaoPatio", false);

            RecarregarSituacoes();

            new BuscarFilial(_pesquisaTemposGestaoPatios.Filial);
            new BuscarMotoristas(_pesquisaTemposGestaoPatios.Motorista);
            new BuscarVeiculos(_pesquisaTemposGestaoPatios.Veiculo);
            new BuscarTransportadores(_pesquisaTemposGestaoPatios.Transportador);
            new BuscarTiposdeCarga(_pesquisaTemposGestaoPatios.TipoCarga);
            new BuscarTiposOperacao(_pesquisaTemposGestaoPatios.TipoOperacao);
            new BuscarRotasFrete(_pesquisaTemposGestaoPatios.Rota);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTemposGestaoPatios);
}

function RecarregarSituacoes() {
    executarReST("FluxoPatio/ObterEtapasDisponiveis", { Tipo: EnumTipoFluxoGestaoPatio.Origem }, function (arg) {
        if (arg.Success && arg.Data !== false) {
            var formatacaoOption = arg.Data.map(function (sit) {
                return {
                    text: sit.Descricao,
                    value: sit.Enumerador
                }
            });
            var situacoesFilial = _etapasFluxoPatio.concat(formatacaoOption);

            _pesquisaTemposGestaoPatios.EtapaFluxoGestaoPatio.options(situacoesFilial);
        }
    });
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTemposGestaoPatios.gerarRelatorio("Relatorios/TemposGestaoPatio/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTemposGestaoPatios.gerarRelatorio("Relatorios/TemposGestaoPatio/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
