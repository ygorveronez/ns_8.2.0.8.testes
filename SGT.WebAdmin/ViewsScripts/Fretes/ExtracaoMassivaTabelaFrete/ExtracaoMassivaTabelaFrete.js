/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../js/app.config.js" />
/// <reference path="../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoTabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridConsultaTabelaFrete;
var _pesquisaConsultaTabelaFrete;
var _CRUDRelatorio;

var _relatorioConsultaTabelaFrete;

var PesquisaConsultaTabelaFrete = function () {
    this.DataInicialAlteracao = PropertyEntity({ text: "*Data Inicial de Alteração: ", getType: typesKnockout.date, required: true });
    this.DataFinalAlteracao = PropertyEntity({ text: "*Data Final de Alteração: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date, required: true });
    this.TabelaFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "*Tabela de Frete:", idBtnSearch: guid(), required: true, issue: 78 });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicialAlteracao.dateRangeLimit = this.DataFinalAlteracao;
    this.DataFinalAlteracao.dateRangeInit = this.DataInicialAlteracao;

    this.Preview = PropertyEntity({
        eventClick: function (e) {
            GerarPreviewClick();   
        }, type: types.event, text: "Consultar", idGrid: guid(), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            LimparFiltros();
        }, type: types.event, text: "Limpar Filtros", idGrid: guid(), visible: ko.observable(true), icon: ko.observable("fa fa-recycle")
    });

    this.ConfiguracaoRelatorio = PropertyEntity({
        eventClick: function (e) {
            var valido = ValidarCamposObrigatorios(_pesquisaConsultaTabelaFrete);

            if (valido) {
                if (e.ConfiguracaoRelatorio.visibleFade()) {
                    e.ConfiguracaoRelatorio.visibleFade(false);
                    e.ConfiguracaoRelatorio.icon("fal fa-plus");
                } else {
                    e.ConfiguracaoRelatorio.visibleFade(true);
                    e.ConfiguracaoRelatorio.icon("fal fa-minus");
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
            }
        }, type: types.event, text: "Configuração da Consulta", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {

    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });    
}

//*******EVENTOS*******
function loadExtracaoMassivaTabelaFrete() {

    _pesquisaConsultaTabelaFrete = new PesquisaConsultaTabelaFrete();
    _CRUDRelatorio = new CRUDRelatorio();

    KoBindings(_pesquisaConsultaTabelaFrete, "knockoutPesquisaConsultaTabelaFrete", false);
    KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConsultaTabelaFrete", false);

    let limiteRegistros = 2000;
    new BuscarTabelasDeFrete(_pesquisaConsultaTabelaFrete.TabelaFrete, null, EnumTipoTabelaFrete.tabelaCliente, null, limiteRegistros);

    _gridConsultaTabelaFrete = new GridView(_pesquisaConsultaTabelaFrete.Preview.idGrid, "Relatorios/ExtracaoMassivaTabelaFrete/Pesquisa", _pesquisaConsultaTabelaFrete);
    _gridConsultaTabelaFrete.SetPermitirEdicaoColunas(true);
    _gridConsultaTabelaFrete.SetQuantidadeLinhasPorPagina(25);

    _relatorioConsultaTabelaFrete = new RelatorioGlobal("Relatorios/ExtracaoMassivaTabelaFrete/BuscarDadosRelatorio", _gridConsultaTabelaFrete, function () {
        _relatorioConsultaTabelaFrete.loadRelatorio(function () {
            $("#divConteudoRelatorio").show();

            _relatorioConsultaTabelaFrete.obterKnoutRelatorio().Report.visibleFade(true);
            _relatorioConsultaTabelaFrete.obterKnoutRelatorio().Report.visible(false);
            _relatorioConsultaTabelaFrete.obterKnoutRelatorio().ExibirSumarios.visible(false);
        });

    }, null, null, _pesquisaConsultaTabelaFrete, false);
}

function GerarPreviewClick() {
    var valido = ValidarCamposObrigatorios(_pesquisaConsultaTabelaFrete);

    if (valido)
        _gridConsultaTabelaFrete.CarregarGrid();
    else
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
}

function GerarRelatorioPDFClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_pesquisaConsultaTabelaFrete);

    if (valido)
        _relatorioConsultaTabelaFrete.gerarRelatorio("Relatorios/ExtracaoMassivaTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
    else
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
}

function GerarRelatorioExcelClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_pesquisaConsultaTabelaFrete);

    if (valido)
        _relatorioConsultaTabelaFrete.gerarRelatorio("Relatorios/ExtracaoMassivaTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
    else
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
}

function LimparFiltros() {
    LimparCampos(_pesquisaConsultaTabelaFrete);
}
