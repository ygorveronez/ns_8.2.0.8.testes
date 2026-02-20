/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
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

var _gridTaxasDescarga, _pesquisaTaxasDescarga, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioTaxasDescarga;

var PesquisaTaxasDescarga = function () {

    var dataAtual = moment().format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Veículo:", issue: 44, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", issue: 53, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(0), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: 0, text: "Status: " });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAjusteConfiguracaoDescargaCliente.Todos), options: EnumSituacaoAjusteConfiguracaoDescargaCliente.obterOpcoesPesquisa(), def: EnumSituacaoAjusteConfiguracaoDescargaCliente.Todos , text: "Situacão: " });
    this.DataInicioVigencia = PropertyEntity({ text: "Data início vigência:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFimVigencia = PropertyEntity({ text: "Data fim vigência:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Cliente:", issue: 52, idBtnSearch: guid(), enable: ko.observable(true) });
    this.GrupoCliente = PropertyEntity({ text: "Grupo de Clientes:", issue: 58, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Tipo de Operação:", issue: 121, idBtnSearch: guid(), enable: ko.observable(true) });
    this.SomenteVigentes = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Somente Vigentes", def: false, visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTaxasDescarga.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTaxasDescarga.Visible.visibleFade() == true) {
                _pesquisaTaxasDescarga.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaTaxasDescarga.Visible.visibleFade(true);
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

function LoadTaxasDescarga() {
    _pesquisaTaxasDescarga = new PesquisaTaxasDescarga();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTaxasDescarga = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TaxasDescarga/Pesquisa", _pesquisaTaxasDescarga);

    _gridTaxasDescarga.SetPermitirEdicaoColunas(true);
    _gridTaxasDescarga.SetQuantidadeLinhasPorPagina(20);

    _relatorioTaxasDescarga = new RelatorioGlobal("Relatorios/TaxasDescarga/BuscarDadosRelatorio", _gridTaxasDescarga, function () {
        _relatorioTaxasDescarga.loadRelatorio(function () {
            KoBindings(_pesquisaTaxasDescarga, "knockoutPesquisaTaxasDescarga", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTaxasDescarga", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTaxasDescarga", false);

            new BuscarTiposdeCarga(_pesquisaTaxasDescarga.TipoCarga);
            new BuscarModelosVeicularesCarga(_pesquisaTaxasDescarga.ModeloVeiculo);
            new BuscarFilial(_pesquisaTaxasDescarga.Filial);
            new BuscarClientes(_pesquisaTaxasDescarga.Cliente);
            new BuscarGruposPessoas(_pesquisaTaxasDescarga.GrupoCliente);
            new BuscarTiposOperacao(_pesquisaTaxasDescarga.TipoOperacao);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTaxasDescarga);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTaxasDescarga.gerarRelatorio("Relatorios/TaxasDescarga/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTaxasDescarga.gerarRelatorio("Relatorios/TaxasDescarga/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
