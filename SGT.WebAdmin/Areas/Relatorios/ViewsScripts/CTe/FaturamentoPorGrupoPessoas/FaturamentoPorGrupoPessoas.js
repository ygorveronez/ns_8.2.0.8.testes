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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFaturamentoPorGrupoPessoas, _pesquisaFaturamentoPorGrupoPessoas, _CRUDRelatorio, _CRUDFiltrosRelatorio, _relatorioFaturamentoPorGrupoPessoas;

var _propriedadesVeiculo = [
    { value: '', text: 'Todos' },
    { value: 'P', text: 'Próprio' },
    { value: 'T', text: 'Terceiro' },
    { value: 'O', text: 'Outros' }
];

var _opcoesDocumentoFaturavel = [
    { value: true, text: "Sim" },
    { value: false, text: "Não" },
    { value: '', text: "Todos" }
];

var PesquisaFaturamentoPorGrupoPessoas = function () {
    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.PrimeiraDataDoMesAtual()), def: Global.PrimeiraDataDoMesAtual() });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.DataInicialAutorizacao = PropertyEntity({ text: "Data Autorização Inicial: ", getType: typesKnockout.date });
    this.DataFinalAutorizacao = PropertyEntity({ text: "Data Autorização Final: ", getType: typesKnockout.date });
    this.DataInicialAutorizacao.dateRangeLimit = this.DataFinalAutorizacao;
    this.DataFinalAutorizacao.dateRangeInit = this.DataInicialAutorizacao;

    this.PropriedadeVeiculo = PropertyEntity({ val: ko.observable(""), options: _propriedadesVeiculo, def: "", text: "Propriedade do Veículo: " });
    this.GruposPessoas = PropertyEntity({ val: ko.observable(new Array()), getType: typesKnockout.selectMultiple, url: "GrupoPessoas/ObterTodos", params: { Tipo: 0, Ativo: _statusPesquisa.Todos }, def: new Array(), text: "Grupo de Pessoas: ", options: ko.observable(new Array()) });
    
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.GruposPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 0 });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Documento:", idBtnSearch: guid(), issue: 0 });
    this.DocumentoFaturavel = PropertyEntity({ text: "Documentos Faturáveis:", options: _opcoesDocumentoFaturavel, val: ko.observable(true), def: true, issue: 0 });
    this.VinculoCarga = PropertyEntity({ text: "Vínculo à carga:", options: Global.ObterOpcoesPesquisaBooleano("Com carga", "Sem carga"), val: ko.observable(true), def: true, issue: 0 });
    this.TipoProposta = PropertyEntity({ text: "Tipo Proposta:", options: EnumTipoPropostaMultimodal.obterOpcoesPesquisa(), val: ko.observable(EnumTipoPropostaMultimodal.Todos), def: EnumTipoPropostaMultimodal.Todos, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFaturamentoPorGrupoPessoas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioFaturamentoPorGrupoPessoas() {

    _pesquisaFaturamentoPorGrupoPessoas = new PesquisaFaturamentoPorGrupoPessoas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFaturamentoPorGrupoPessoas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FaturamentoPorGrupoPessoas/Pesquisa", _pesquisaFaturamentoPorGrupoPessoas);

    _gridFaturamentoPorGrupoPessoas.SetPermitirEdicaoColunas(true);
    _gridFaturamentoPorGrupoPessoas.SetQuantidadeLinhasPorPagina(10);

    _relatorioFaturamentoPorGrupoPessoas = new RelatorioGlobal("Relatorios/FaturamentoPorGrupoPessoas/BuscarDadosRelatorio", _gridFaturamentoPorGrupoPessoas, function () {
        _relatorioFaturamentoPorGrupoPessoas.loadRelatorio(function () {
            KoBindings(_pesquisaFaturamentoPorGrupoPessoas, "knockoutPesquisaFaturamentoPorGrupoPessoas", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFaturamentoPorGrupoPessoas", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFaturamento", false);

            BuscarGruposPessoas(_pesquisaFaturamentoPorGrupoPessoas.GruposPessoas);
            BuscarModeloDocumentoFiscal(_pesquisaFaturamentoPorGrupoPessoas.ModeloDocumentoFiscal);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFaturamentoPorGrupoPessoas);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFaturamentoPorGrupoPessoas.gerarRelatorio("Relatorios/FaturamentoPorGrupoPessoas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFaturamentoPorGrupoPessoas.gerarRelatorio("Relatorios/FaturamentoPorGrupoPessoas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}