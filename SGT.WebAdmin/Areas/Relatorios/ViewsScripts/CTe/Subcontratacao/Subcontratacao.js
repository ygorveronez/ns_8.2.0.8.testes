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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoServicoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaMercante.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioSubcontratacao;
var _pesquisaSubcontratacao;
var _gridSubcontratacao;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;

var PesquisaSubcontratacao = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.DataInicialEmissaoCarga = PropertyEntity({ text: "Data Emissão Inicial Carga: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalEmissaoCarga = PropertyEntity({ text: "Data Emissão Final Carga: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialEmissaoCarga.dateRangeLimit = this.DataFinalEmissaoCarga;
    this.DataFinalEmissaoCarga.dateRangeInit = this.DataInicialEmissaoCarga;

    this.DataInicialFinalizacaoEmissao = PropertyEntity({ text: "Data Finalização Carga Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalFinalizacaoEmissao = PropertyEntity({ text: "Data Finalização Carga Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialFinalizacaoEmissao.dateRangeLimit = this.DataFinalFinalizacaoEmissao;
    this.DataFinalFinalizacaoEmissao.dateRangeInit = this.DataInicialFinalizacaoEmissao;

    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, visible: ko.observable(true) });

    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial:", issue: 70, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: ko.observable("Transportador:"), issue: 69, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), val: ko.observable(""), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" } });
    this.Origem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado de Origem:", idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });
    this.EstadoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 58, visible: ko.observable(true) });

    this.TipoCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), text: "Tipo do CT-e:" });
    this.TipoServico = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoServicoCTe.obterOpcoes(), text: "Tipo de Serviço:" });
    this.SituacaoCarga = PropertyEntity({ val: ko.observable([]), getType: typesKnockout.selectMultiple, def: [], text: "Situação da Carga:", options: EnumSituacoesCarga.obterOpcoesTMS(), visible: ko.observable(true) });
    this.SituacaoSEFAZ = PropertyEntity({ val: ko.observable([]), getType: typesKnockout.selectMultiple, def: [], text: "Situação da SEFAZ:", options: EnumSituacaoCTeSEFAZ.obterOpcoes() });
    this.SituacaoCargaMercante = PropertyEntity({ text: "Situação da Carga: ", getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });

    this.NumeroCargaEmbarcador = PropertyEntity({ text: "Nº Carga Embarcador: ", visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridSubcontratacao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaSubcontratacao.Visible.visibleFade()) {
                _pesquisaSubcontratacao.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaSubcontratacao.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function LoadSubcontratacao() {
    _pesquisaSubcontratacao = new PesquisaSubcontratacao();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridSubcontratacao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Subcontratacao/Pesquisa", _pesquisaSubcontratacao, null, null, 10);
    _gridSubcontratacao.SetPermitirEdicaoColunas(true);

    ConfigurarCamposPorTipoSistema();

    _relatorioSubcontratacao = new RelatorioGlobal("Relatorios/Subcontratacao/BuscarDadosRelatorio", _gridSubcontratacao, function () {
        _relatorioSubcontratacao.loadRelatorio(function () {
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaSubcontratacao");
            KoBindings(_pesquisaSubcontratacao, "knockoutPesquisaSubcontratacao", false, _CRUDFiltrosRelatorio.Preview.id);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaSubcontratacao");

            new BuscarCargas(_pesquisaSubcontratacao.Carga);
            new BuscarTransportadores(_pesquisaSubcontratacao.Transportador);
            new BuscarClientes(_pesquisaSubcontratacao.Remetente);
            new BuscarClientes(_pesquisaSubcontratacao.Destinatario);
            new BuscarLocalidades(_pesquisaSubcontratacao.Origem);
            new BuscarLocalidades(_pesquisaSubcontratacao.Destino);
            new BuscarEstados(_pesquisaSubcontratacao.EstadoOrigem);
            new BuscarEstados(_pesquisaSubcontratacao.EstadoDestino);
            new BuscarFilial(_pesquisaSubcontratacao.Filial);
            new BuscarGruposPessoas(_pesquisaSubcontratacao.GrupoPessoas);

            if (_CONFIGURACAO_TMS.AtivarNovosFiltrosConsultaCarga) {
                _pesquisaSubcontratacao.SituacaoCarga.visible(false);
                _pesquisaSubcontratacao.SituacaoCargaMercante.visible(true);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaSubcontratacao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioSubcontratacao.gerarRelatorio("Relatorios/Subcontratacao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioSubcontratacao.gerarRelatorio("Relatorios/Subcontratacao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ConfigurarCamposPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaSubcontratacao.Filial.visible(false);
        _pesquisaSubcontratacao.Transportador.text("Empresa/Filial:");
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaSubcontratacao.Transportador.visible(false);
    }
}