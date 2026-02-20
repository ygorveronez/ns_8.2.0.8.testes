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
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PlanoConta.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioExtratoMotorista, _gridExtratoMotorista, _pesquisaExtratoMotorista, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaExtratoMotorista = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, def: ko.observable("") });
    this.Plano = PropertyEntity({ text: "Conta Gerencial Analítica:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ text: "Período inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoMovimento = PropertyEntity({ text: "Tipo Movimento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GerarRelatorioAgrupado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Visualizar os lançamentos agrupados?", visible: ko.observable(false) })

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridExtratoMotorista.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioMovimento = PropertyEntity({ eventClick: GerarRelatorioMovimentoClick, type: types.event, text: "Gerar Rel. Movimentos ao Motorista" });
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioExtratoMotorista() {

    _pesquisaExtratoMotorista = new PesquisaExtratoMotorista();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridExtratoMotorista = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ExtratoMotorista/Pesquisa", _pesquisaExtratoMotorista, null, null, 1000, null, null, null, null, 1000);
    _gridExtratoMotorista.SetPermitirEdicaoColunas(true);

    _relatorioExtratoMotorista = new RelatorioGlobal("Relatorios/ExtratoMotorista/BuscarDadosRelatorio", _gridExtratoMotorista, function () {
        _relatorioExtratoMotorista.loadRelatorio(function () {
            KoBindings(_pesquisaExtratoMotorista, "knockoutPesquisaExtratoMotorista");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaExtratoMotorista");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaExtratoMotorista");

            new BuscarPlanoConta(_pesquisaExtratoMotorista.Plano, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
            new BuscarMotoristas(_pesquisaExtratoMotorista.Motorista, retornoMotorista);
            new BuscarTipoMovimento(_pesquisaExtratoMotorista.TipoMovimento);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaExtratoMotorista);
}

function retornoMotorista(data) {
    _pesquisaExtratoMotorista.Motorista.codEntity(data.Codigo);
    _pesquisaExtratoMotorista.Motorista.val(data.Nome);
}

function GerarRelatorioMovimentoClick(e, sender) {
    var data = {
        Codigo: _pesquisaExtratoMotorista.Codigo.val(),
        Plano: _pesquisaExtratoMotorista.Plano.codEntity(),
        DataInicial: _pesquisaExtratoMotorista.DataInicial.val(),
        DataFinal: _pesquisaExtratoMotorista.DataFinal.val(),
        Motorista: _pesquisaExtratoMotorista.Motorista.codEntity()
    };
    executarReST("Relatorios/ExtratoMotorista/GerarRelatorioMovimento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    })
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioExtratoMotorista.gerarRelatorio("Relatorios/ExtratoMotorista/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioExtratoMotorista.gerarRelatorio("Relatorios/ExtratoMotorista/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}