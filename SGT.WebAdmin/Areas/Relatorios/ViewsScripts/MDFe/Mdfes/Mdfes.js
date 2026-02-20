/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SerieEmpresa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoMDFe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoSerie.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridMdfe;
var _pesquisaMdfe;
var _relatorioMdfe;

var _tipoMDFeVinculadoCarga = [
    { text: "Todos", value: "" },
    { text: "MDF-e com Carga", value: true },
    { text: "MDF-e sem Carga", value: false }
];

/*
 * Declaração das Classes
 */

var PesquisaMdfe = function () {
    this.CpfMotorista = PropertyEntity({ text: "CPF Motorista:", maxlength: 15 });
    this.DataAutorizacaoInicial = PropertyEntity({ text: "Data Autorização Início: ", getType: typesKnockout.date });
    this.DataAutorizacaoLimite = PropertyEntity({ text: "Data Autorização Limite: ", dateRangeInit: this.DataAutorizacaoInicial, getType: typesKnockout.date });
    this.DataCancelamentoInicial = PropertyEntity({ text: "Data Cancelamento Início: ", getType: typesKnockout.date });
    this.DataCancelamentoLimite = PropertyEntity({ text: "Data Cancelamento Limite: ", dateRangeInit: this.DataCancelamentoInicial, getType: typesKnockout.date });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Início: ", getType: typesKnockout.date });
    this.DataEmissaoLimite = PropertyEntity({ text: "Data Emissão Limite: ", dateRangeInit: this.DataEmissaoInicial, getType: typesKnockout.date });
    this.DataEncerramentoInicial = PropertyEntity({ text: "Data Encerramento Início: ", getType: typesKnockout.date });
    this.DataEncerramentoLimite = PropertyEntity({ text: "Data Encerramento Limite: ", dateRangeInit: this.DataEncerramentoInicial, getType: typesKnockout.date });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador"), idBtnSearch: guid() });
    this.EstadoCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado Carregamento:", idBtnSearch: guid() });
    this.EstadoDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado Descarregamento:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.MunicipioDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Município Descarregamento:", idBtnSearch: guid() });

    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroLimite = PropertyEntity({ text: "Número Limite:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.PlacaVeiculo = PropertyEntity({ text: "Placa Veículo:", maxlength: 10 });
    this.Serie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Série:", idBtnSearch: guid() });
    this.StatusMdfe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoMDFe.obterOpcoesPesquisa(), text: "Situação:" });
    this.NumeroCTe = PropertyEntity({ text: "Número CT-e:", getType: typesKnockout.int });
    this.NumeroCarga = PropertyEntity({ text: "Número Carga:", maxlength: 50 });
    this.ExibirCTes = PropertyEntity({ text: "Exibir CT-es dos MDF-es?", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.MDFeVinculadoACarga = PropertyEntity({ val: ko.observable(""), options: _tipoMDFeVinculadoCarga, def: "", text: "Vínculo à Carga:" });

    this.DataAutorizacaoInicial.dateRangeLimit = this.DataAutorizacaoLimite;
    this.DataAutorizacaoLimite.dateRangeInit = this.DataAutorizacaoInicial;
    this.DataCancelamentoInicial.dateRangeLimit = this.DataCancelamentoLimite;
    this.DataCancelamentoLimite.dateRangeInit = this.DataCancelamentoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoLimite;
    this.DataEmissaoLimite.dateRangeInit = this.DataEmissaoInicial;
    this.DataEncerramentoInicial.dateRangeLimit = this.DataEncerramentoLimite;
    this.DataEncerramentoLimite.dateRangeInit = this.DataEncerramentoInicial;

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMdfe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaMdfe.Visible.visibleFade()) {
                _pesquisaMdfe.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaMdfe.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadMdfe() {
    _pesquisaMdfe = new PesquisaMdfe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridMdfe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Mdfes/Pesquisa", _pesquisaMdfe);

    _gridMdfe.SetPermitirEdicaoColunas(true);
    _gridMdfe.SetQuantidadeLinhasPorPagina(20);

    _relatorioMdfe = new RelatorioGlobal("Relatorios/Mdfes/BuscarDadosRelatorio", _gridMdfe, function () {
        _relatorioMdfe.loadRelatorio(function () {
            KoBindings(_pesquisaMdfe, "knockoutPesquisaMdfe", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMdfe", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMdfe", false);

            new BuscarEmpresa(_pesquisaMdfe.Empresa);
            new BuscarEstados(_pesquisaMdfe.EstadoCarregamento);
            new BuscarEstados(_pesquisaMdfe.EstadoDescarregamento);
            new BuscarSerieEmpresa(_pesquisaMdfe.Serie, null, null, null, null, EnumTipoSerie.MDFe);
            new BuscarTiposOperacao(_pesquisaMdfe.TipoOperacao);
            new BuscarLocalidades(_pesquisaMdfe.MunicipioDescarregamento);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMdfe);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioMdfe.gerarRelatorio("Relatorios/Mdfes/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioMdfe.gerarRelatorio("Relatorios/Mdfes/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}