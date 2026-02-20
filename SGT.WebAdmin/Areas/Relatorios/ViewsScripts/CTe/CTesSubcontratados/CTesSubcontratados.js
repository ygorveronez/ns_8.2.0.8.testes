/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCTesSubcontratados, _gridCTesSubcontratados, _pesquisaCTesSubcontratados, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaCTesSubcontratados = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    var dataAtual = Global.DataAtual();
    this.DataInicialEmissao = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual, text: "Data Inicial Emissão:", required: ko.observable(true) });
    this.DataFinalEmissao = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual, text: "Data Final Emissão:", required: ko.observable(true) });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.Carga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Número Carga:" });
    this.EmpresaCTeOriginal = PropertyEntity({ idBtnSearch: guid(), type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "Transportadora CTe Original:" });
    this.EmpresaCTeSubcontratado = PropertyEntity({ idBtnSearch: guid(), type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "Transportadora CTe Subcontratado:" });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarFiltrosObrigatorios())
                _gridCTesSubcontratados.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCTesSubcontratados.Visible.visibleFade()) {
                _pesquisaCTesSubcontratados.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCTesSubcontratados.Visible.visibleFade(true);
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

function loadRelatorioCTesSubcontratados() {
    _pesquisaCTesSubcontratados = new PesquisaCTesSubcontratados();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCTesSubcontratados = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CTesSubcontratados/Pesquisa", _pesquisaCTesSubcontratados, null, null, 10);
    _gridCTesSubcontratados.SetPermitirEdicaoColunas(true);

    _relatorioCTesSubcontratados = new RelatorioGlobal("Relatorios/CTesSubcontratados/BuscarDadosRelatorio", _gridCTesSubcontratados, function () {
        _relatorioCTesSubcontratados.loadRelatorio(function () {
            KoBindings(_pesquisaCTesSubcontratados, "knockoutPesquisaCTesSubcontratados", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCTesSubcontratados", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCTesSubcontratados", false);

            new BuscarEmpresa(_pesquisaCTesSubcontratados.EmpresaCTeOriginal);
            new BuscarEmpresa(_pesquisaCTesSubcontratados.EmpresaCTeSubcontratado);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCTesSubcontratados);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioCTesSubcontratados.gerarRelatorio("Relatorios/CTesSubcontratados/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioCTesSubcontratados.gerarRelatorio("Relatorios/CTesSubcontratados/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ValidarFiltrosObrigatorios() {
    var tudoCerto = true;
    var valido = ValidarCamposObrigatorios(_pesquisaCTesSubcontratados);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Filtros Obrigatórios", "Informe os filtros obrigatórios!");
        tudoCerto = false;
    }

    var totalDias = Global.ObterDiasEntreDatas(_pesquisaCTesSubcontratados.DataInicialEmissao.val(), _pesquisaCTesSubcontratados.DataFinalEmissao.val());
    if (_CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios > 0 && totalDias > _CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios) {
        exibirMensagem(tipoMensagem.atencao, "Datas Inválidas", "A diferença das datas não pode ser maior que " + _CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios + " dias.");
        tudoCerto = false;
    }

    return tudoCerto;
}