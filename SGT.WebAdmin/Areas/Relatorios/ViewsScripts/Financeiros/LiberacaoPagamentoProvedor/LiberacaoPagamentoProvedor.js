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

var _gridLiberacaoPagamentoProvedor, _pesquisaLiberacaoPagamentoProvedor, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioLoadLiberacaoPagamentoProvedor;

var PesquisaLiberacaoPagamentoProvedor = function () {
    this.DataCargaInicial = PropertyEntity({ text: "Data carga inicial:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataCargaFinal = PropertyEntity({ text: "Data carga final:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), text: "Número da carga:", visible: ko.observable(true) });
    this.NumeroOS = PropertyEntity({ val: ko.observable(""), text: "Número da OS:", visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Tipo de Operação:", issue: 121, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Provedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Provedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.FilialEmissora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial Emissora:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoLiberacaoPagamentoProvedor = PropertyEntity({ val: ko.observable(EnumSituacaoLiberacaoPagamentoProvedor.Todas), options: EnumSituacaoLiberacaoPagamentoProvedor.obterOpcoes(), def: EnumSituacaoLiberacaoPagamentoProvedor.Todas, text: "Situação Liberação Pagamento Provedor:" });
    this.TipoDocumentoProvedor = PropertyEntity({ val: ko.observable(EnumTipoDocumentoProvedor.Nenhum), options: EnumTipoDocumentoProvedor.obterOpcoesPesquisaRelatorio(), def: EnumTipoDocumentoProvedor.CTe, text: "Tipo de Documento:" });
    this.IndicacaoLiberacaoOK = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Indicação Liberação OK:" });
    this.EtapaLiberacaoPagamentoProvedor = PropertyEntity({ val: ko.observable(EnumEtapaLiberacaoPagamentoProvedor.Todos), options: EnumEtapaLiberacaoPagamentoProvedor.obterOpcoes(), def: EnumEtapaLiberacaoPagamentoProvedor.Todos, text: "Etapa Liberação Pagamento Provedor:" });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridLiberacaoPagamentoProvedor.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadLiberacaoPagamentoProvedor() {
    _pesquisaLiberacaoPagamentoProvedor = new PesquisaLiberacaoPagamentoProvedor();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridLiberacaoPagamentoProvedor = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/LiberacaoPagamentoProvedor/Pesquisa", _pesquisaLiberacaoPagamentoProvedor);

    _gridLiberacaoPagamentoProvedor.SetPermitirEdicaoColunas(true);
    _gridLiberacaoPagamentoProvedor.SetQuantidadeLinhasPorPagina(10);

    _relatorioLoadLiberacaoPagamentoProvedor = new RelatorioGlobal("Relatorios/LiberacaoPagamentoProvedor/BuscarDadosRelatorio", _gridLiberacaoPagamentoProvedor, function () {
        _relatorioLoadLiberacaoPagamentoProvedor.loadRelatorio(function () {
            KoBindings(_pesquisaLiberacaoPagamentoProvedor, "knockoutPesquisaLiberacaoPagamentoProvedor", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaLiberacaoPagamentoProvedor", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaLiberacaoPagamentoProvedor", false);

            BuscarTiposOperacao(_pesquisaLiberacaoPagamentoProvedor.TipoOperacao);
            BuscarClientes(_pesquisaLiberacaoPagamentoProvedor.Provedor);
            BuscarEmpresa(_pesquisaLiberacaoPagamentoProvedor.FilialEmissora);
            BuscarClientes(_pesquisaLiberacaoPagamentoProvedor.Tomador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaLiberacaoPagamentoProvedor);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioLoadLiberacaoPagamentoProvedor.gerarRelatorio("Relatorios/LiberacaoPagamentoProvedor/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioLoadLiberacaoPagamentoProvedor.gerarRelatorio("Relatorios/LiberacaoPagamentoProvedor/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
