/// <reference path="../../../../../ViewsScripts/Consultas/Container.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Porto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOcorrencia.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumFatura.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoDocumentoCreditoDebito.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumICMSCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoServicoMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCarroceria.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoProprietarioVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaMercante.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPreCTes, _pesquisaPreCTes, _CRUDRelatorio, _CRUDFiltrosRelatorio;
var _relatorioPreCTes;

var _tipoTomador = [
    { value: EnumTipoTomador.Destinatario, text: "Destinatário" },
    { value: EnumTipoTomador.Expedidor, text: "Expedidor" },
    { value: EnumTipoTomador.Recebedor, text: "Recebedor" },
    { value: EnumTipoTomador.Remetente, text: "Remetente" },
    { value: EnumTipoTomador.Outros, text: "Outros" }
];

var _possuiFRS = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false }
];



var PesquisaPreCTes = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(false) });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(false) });

    this.DataEmissaoFinal.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.NumeroNFe = PropertyEntity({ text: "Número NF-e: ", getType: typesKnockout.int, visible: ko.observable(true) });

    this.TipoTomador = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _tipoTomador, text: "Tipo do Tomador:", visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });

    this.PossuiFRS = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.select, options: _possuiFRS, text: "Possui FRS:", visible: ko.observable(true) });

    this.SituacaoPreCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.select, options: EnumSituacaoPreCTe.obterOpcoesPesquisa(),  text: "Situação:", visible: ko.observable(true) });

    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });

    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo Veículo:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Ocorrência:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Origem:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.EstadoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    

};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPreCTes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPreCTes.Visible.visibleFade()) {
                _pesquisaPreCTes.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPreCTes.Visible.visibleFade(true);
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

function loadRelatorioPreCTes() {
    _pesquisaPreCTes = new PesquisaPreCTes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPreCTes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PreCTe/Pesquisa", _pesquisaPreCTes);

    _gridPreCTes.SetPermitirEdicaoColunas(true);
    _gridPreCTes.SetPermitirRedimencionarColunas(true);
    _gridPreCTes.SetQuantidadeLinhasPorPagina(10);

    _relatorioPreCTes = new RelatorioGlobal("Relatorios/PreCTe/BuscarDadosRelatorio", _gridPreCTes, function () {
        _relatorioPreCTes.loadRelatorio(function () {
            KoBindings(_pesquisaPreCTes, "knockoutPesquisaPreCTes", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPreCTes", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPreCTes", false);

            BuscarClientes(_pesquisaPreCTes.Remetente);
            BuscarClientes(_pesquisaPreCTes.Destinatario);
            BuscarClientes(_pesquisaPreCTes.Tomador);
            BuscarClientes(_pesquisaPreCTes.Recebedor);
            BuscarClientes(_pesquisaPreCTes.Expedidor);
            BuscarTiposOperacao(_pesquisaPreCTes.TipoOperacao);
            BuscarEstados(_pesquisaPreCTes.EstadoOrigem);
            BuscarEstados(_pesquisaPreCTes.EstadoDestino);
            BuscarLocalidades(_pesquisaPreCTes.Origem);
            BuscarLocalidades(_pesquisaPreCTes.Destino);
            BuscarTiposdeCarga(_pesquisaPreCTes.TipoCarga);
            BuscarModelosVeiculo(_pesquisaPreCTes.ModeloVeiculo);
            BuscarTransportadores(_pesquisaPreCTes.Transportador, null, null, null, null, null, null, true);
            BuscarCargas(_pesquisaPreCTes.Carga);
            BuscarFilial(_pesquisaPreCTes.Filial);
            BuscarTipoOcorrencia(_pesquisaPreCTes.TipoOcorrencia);

        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPreCTes);
}

function GerarRelatorioPDFClick(e, sender) {
        _relatorioPreCTes.gerarRelatorio("Relatorios/PreCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
        _relatorioPreCTes.gerarRelatorio("Relatorios/PreCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
