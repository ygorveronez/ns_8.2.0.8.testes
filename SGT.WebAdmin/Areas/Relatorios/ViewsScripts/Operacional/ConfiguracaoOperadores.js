/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoBandaRodagemPneu.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPneuTMS.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />

//********MAPEAMENTO KNOCKOUT********

var _relatorioConfiguracaoOperadores, _gridConfiguracaoOperadores, _pesquisaConfiguracaoOperadores, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaConfiguracaoOperadores = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(true), text: "Usuário: ", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(true), text: "Filiais: ", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(true), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(true), text: "Tipo de Carga: ", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoOperadores.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadConfiguracaoOperadores() {
    _pesquisaConfiguracaoOperadores = new PesquisaConfiguracaoOperadores();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConfiguracaoOperadores = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ConfiguracaoOperadores/Pesquisa", _pesquisaConfiguracaoOperadores, null, null, 10);
    _gridConfiguracaoOperadores.SetPermitirEdicaoColunas(true);

    _relatorioConfiguracaoOperadores = new RelatorioGlobal("Relatorios/ConfiguracaoOperadores/BuscarDadosRelatorio", _gridConfiguracaoOperadores, function () {
        _relatorioConfiguracaoOperadores.loadRelatorio(function () {
            KoBindings(_pesquisaConfiguracaoOperadores, "knockoutPesquisaConfiguracaoOperadores", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConfiguracaoOperadores", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConfiguracaoOperadores", false);

            new BuscarFuncionario(_pesquisaConfiguracaoOperadores.Usuario);
            new BuscarFilial(_pesquisaConfiguracaoOperadores.Filial);
            new BuscarTiposdeCarga(_pesquisaConfiguracaoOperadores.TipoCarga);
            new BuscarTiposOperacao(_pesquisaConfiguracaoOperadores.TipoOperacao);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConfiguracaoOperadores);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioConfiguracaoOperadores.gerarRelatorio("Relatorios/ConfiguracaoOperadores/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioConfiguracaoOperadores.gerarRelatorio("Relatorios/ConfiguracaoOperadores/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}