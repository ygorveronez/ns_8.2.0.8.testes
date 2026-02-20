/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCustoRentabilidadeCteCrt, _gridCustoRentabilidadeCteCrt, _pesquisaCustoRentabilidadeCteCrt, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _serieConfig = {
    precision: 0,
    allowZero: false,
    allowNegative: false,
    thousands: ""
};

var PesquisaCustoRentabilidadeCteCrt = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    var dataAtual = Global.DataAtual();

    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(false) });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(false) });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.NumeroInicial = PropertyEntity({ text: "Núm. Inicial: ", getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroFinal = PropertyEntity({ text: "Núm. Final: ", getType: typesKnockout.int, visible: ko.observable(true) });

    this.Pedido = PropertyEntity({ type: types.map, text: "Pedido:", visible: ko.observable(true) });
    this.NFe = PropertyEntity({ text: "Núm. NF-e: ", getType: typesKnockout.string, visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.Serie = PropertyEntity({ text: "Série:", getType: typesKnockout.int, val: ko.observable(""), visible: ko.observable(true), configInt: _serieConfig, maxlength: 4, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.TipoServico = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoServicoCTe.obterOpcoes(), text: "Tipo de Serviço:", visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });

    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoCTe.obterOpcoes(), text: "Situação do CT-e:", issue: 120, visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.CTeVinculadoACarga = PropertyEntity({ val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("CT-e com Carga", "CT-e sem Carga"), def: true, text: "Vínculo à Carga:", visible: ko.observable(true) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });

    this.Veiculo = PropertyEntity({ text: "Veículo:", issue: 143, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" } });
    this.Filial = PropertyEntity({ text: "Filial:", issue: 70, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Transportador = PropertyEntity({ text: "Transportador:", issue: 69, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CTe = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "CTe:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.ModeloDocumento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Mod. Documento:", idBtnSearch: guid(), visible: ko.observable(true) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarFiltrosObrigatorios())
                _gridCustoRentabilidadeCteCrt.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCustoRentabilidadeCteCrt.Visible.visibleFade()) {
                _pesquisaCustoRentabilidadeCteCrt.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCustoRentabilidadeCteCrt.Visible.visibleFade(true);
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

function loadRelatorioCustoRentabilidadeCteCrt() {
    _pesquisaCustoRentabilidadeCteCrt = new PesquisaCustoRentabilidadeCteCrt();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCustoRentabilidadeCteCrt = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CustoRentabilidadeCteCrt/Pesquisa", _pesquisaCustoRentabilidadeCteCrt, null, null, 10);
    _gridCustoRentabilidadeCteCrt.SetPermitirEdicaoColunas(true);

    _relatorioCustoRentabilidadeCteCrt = new RelatorioGlobal("Relatorios/CustoRentabilidadeCteCrt/BuscarDadosRelatorio", _gridCustoRentabilidadeCteCrt, function () {
        _relatorioCustoRentabilidadeCteCrt.loadRelatorio(function () {
            KoBindings(_pesquisaCustoRentabilidadeCteCrt, "knockoutPesquisaCustoRentabilidadeCteCrt", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCustoRentabilidadeCteCrt", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCustoRentabilidadeCteCrt", false);

            BuscarClientes(_pesquisaCustoRentabilidadeCteCrt.Remetente);
            BuscarClientes(_pesquisaCustoRentabilidadeCteCrt.Destinatario);
            BuscarClientes(_pesquisaCustoRentabilidadeCteCrt.Tomador);
            BuscarVeiculos(_pesquisaCustoRentabilidadeCteCrt.Veiculo);
            BuscarCargas(_pesquisaCustoRentabilidadeCteCrt.Carga);
            BuscarFilial(_pesquisaCustoRentabilidadeCteCrt.Filial);
            BuscarTransportadores(_pesquisaCustoRentabilidadeCteCrt.Transportador, null, null, null, null, null, null, true);
            BuscarTiposOperacao(_pesquisaCustoRentabilidadeCteCrt.TipoOperacao);
            BuscarConhecimentoNotaReferencia(_pesquisaCustoRentabilidadeCteCrt.CTe);
            BuscarModeloDocumentoFiscal(_pesquisaCustoRentabilidadeCteCrt.ModeloDocumento, null, null, null, null, true);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCustoRentabilidadeCteCrt);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioCustoRentabilidadeCteCrt.gerarRelatorio("Relatorios/CustoRentabilidadeCteCrt/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioCustoRentabilidadeCteCrt.gerarRelatorio("Relatorios/CustoRentabilidadeCteCrt/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ValidarFiltrosObrigatorios() {
    var tudoCerto = true;
    var valido = ValidarCamposObrigatorios(_pesquisaCustoRentabilidadeCteCrt);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Filtros Obrigatórios", "Informe os filtros obrigatórios!");
        tudoCerto = false;
    }

    var totalDias = Global.ObterDiasEntreDatas(_pesquisaCustoRentabilidadeCteCrt.DataInicialEmissao.val(), _pesquisaCustoRentabilidadeCteCrt.DataFinalEmissao.val());
    if (_CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios > 0 && totalDias > _CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios) {
        exibirMensagem(tipoMensagem.atencao, "Datas Inválidas", "A diferença das datas não pode ser maior que " + _CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios + " dias.");
        tudoCerto = false;
    }

    return tudoCerto;
}