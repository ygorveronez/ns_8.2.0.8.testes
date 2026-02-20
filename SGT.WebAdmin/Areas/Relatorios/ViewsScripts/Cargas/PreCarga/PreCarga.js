/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumFiltroPreCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPreCarga, _pesquisaPreCarga, _CRUDRelatorio, _relatorioPreCarga, _CRUDFiltrosRelatorio;

var PesquisaPreCarga = function () {
    var dataPrevisaoInicioViagem = _CONFIGURACAO_TMS.UtilizarProgramacaoCarga ? "" : Global.DataAtual();

    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(dataPrevisaoInicioViagem), def: dataPrevisaoInicioViagem, visible: !_CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, val: ko.observable(dataPrevisaoInicioViagem), def: dataPrevisaoInicioViagem, visible: !_CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.DataCriacaoPreCargaInicial = PropertyEntity({ text: "Data Criação Inicial: ", getType: typesKnockout.date });
    this.DataCriacaoPreCargaFinal = PropertyEntity({ text: "Data Criação Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumFiltroPreCarga.Todos), def: EnumFiltroPreCarga.Todos, options: EnumFiltroPreCarga.obterOpcoesPesquisa() });
    this.PreCarga = PropertyEntity({ text: "Pré Planejamento (Rota): " });
    this.Carga = PropertyEntity({ text: "Carga (Romaneio): " });
    this.Pedido = PropertyEntity({ text: "Pedido (Missão): " });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente: ", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário: ", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ConfiguracaoProgramacaoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Configuração de Pré Planejamento: ", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador: ", idBtnSearch: guid(), visible: !_CONFIGURACAO_TMS.UtilizarProgramacaoCarga  });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataCriacaoPreCargaInicial.dateRangeLimit = this.DataCriacaoPreCargaFinal;
    this.DataCriacaoPreCargaFinal.dateRangeInit = this.DataCriacaoPreCargaInicial;
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPreCarga.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPreCarga.Visible.visibleFade()) {
                _pesquisaPreCarga.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPreCarga.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(false)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadRelatorioPreCarga() {
    _pesquisaPreCarga = new PesquisaPreCarga();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPreCarga = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/PreCarga/Pesquisa", _pesquisaPreCarga, null, null, 10, null, null, null, null, 20);
    _gridPreCarga.SetPermitirEdicaoColunas(true);

    _relatorioPreCarga = new RelatorioGlobal("Relatorios/PreCarga/BuscarDadosRelatorio", _gridPreCarga, function () {
        _relatorioPreCarga.loadRelatorio(function () {
            KoBindings(_pesquisaPreCarga, "knockoutPesquisaPreCarga");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPreCarga");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPreCarga");

            new BuscarFilial(_pesquisaPreCarga.Filial);
            new BuscarClientes(_pesquisaPreCarga.Remetente);
            new BuscarClientes(_pesquisaPreCarga.Destinatario);
            new BuscarFuncionario(_pesquisaPreCarga.Operador);
            new BuscarConfiguracaoProgramacaoCarga(_pesquisaPreCarga.ConfiguracaoProgramacaoCarga);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _pesquisaPreCarga.Filial.visible(false);
            }
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPreCarga);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPreCarga.gerarRelatorio("Relatorios/PreCarga/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPreCarga.gerarRelatorio("Relatorios/PreCarga/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}