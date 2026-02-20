/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pedido.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPacotes, _pesquisaPacotes, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioPacotes;

var PesquisaPacotes = function () {

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.LogKey = PropertyEntity({ val: ko.observable(""), text: "Log Key:", visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pedido:", idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Tipo de Operação:", issue: 121, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", issue: 16, idBtnSearch: guid() });
    this.Contratante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Contratante:", idBtnSearch: guid() });
    this.DataRecebimentoInicial = PropertyEntity({ text: "Data recebimento inicial:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataRecebimentoFinal = PropertyEntity({ text: "Data recebimento final:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Cubagem = PropertyEntity({ text: "Cubagem:", visible: ko.observable(true), getType: typesKnockout.decimal });
    this.Peso = PropertyEntity({ maxlength: 12, getType: typesKnockout.decimal, text: "Peso:", required: false, visible: ko.observable(true) });
    this.NumeroCTe = PropertyEntity({ text: "Número CTe:", visible: ko.observable(true), getType: typesKnockout.int });
    this.ChaveCTe = PropertyEntity({ text: "Chave CTe:", visible: ko.observable(true), getType: typesKnockout.string });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPacotes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPacotes.Visible.visibleFade() == true) {
                _pesquisaPacotes.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPacotes.Visible.visibleFade(true);
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

function LoadPacotes() {
    _pesquisaPacotes = new PesquisaPacotes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPacotes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Pacotes/Pesquisa", _pesquisaPacotes);

    _gridPacotes.SetPermitirEdicaoColunas(true);
    _gridPacotes.SetQuantidadeLinhasPorPagina(20);

    _relatorioPacotes = new RelatorioGlobal("Relatorios/Pacotes/BuscarDadosRelatorio", _gridPacotes, function () {
        _relatorioPacotes.loadRelatorio(function () {
            KoBindings(_pesquisaPacotes, "knockoutPesquisaPacotes", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPacotes", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPacotes", false);

            new BuscarClientes(_pesquisaPacotes.Contratante)
            new BuscarPedidos(_pesquisaPacotes.Pedido)
            new BuscarClientes(_pesquisaPacotes.Destino)
            new BuscarClientes(_pesquisaPacotes.Origem)
            new BuscarTiposOperacao(_pesquisaPacotes.TipoOperacao)
            new BuscarCargas(_pesquisaPacotes.Carga);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPacotes);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPacotes.gerarRelatorio("Relatorios/Pacotes/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPacotes.gerarRelatorio("Relatorios/Pacotes/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
