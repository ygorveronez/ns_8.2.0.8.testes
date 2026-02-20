/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEncaixe, _pesquisaEncaixe, _CRUDRelatorio, _relatorioEncaixe, _CRUDFiltrosRelatorio;

var _situacaoCargaEmbarcador = [
    { text: "Com a Logistica", value: EnumSituacoesCarga.NaLogistica },
    { text: "Dados da Carga", value: EnumSituacoesCarga.Nova },
    { text: "NF-e", value: EnumSituacoesCarga.AgNFe },
    { text: "Cálculo de Frete", value: EnumSituacoesCarga.CalculoFrete },
    { text: "Transportador", value: EnumSituacoesCarga.AgTransportador },
    { text: "Emissão dos Documentos", value: EnumSituacoesCarga.PendeciaDocumentos },
    { text: "Integração", value: EnumSituacoesCarga.AgIntegracao },
    { text: "Impressão", value: EnumSituacoesCarga.AgImpressaoDocumentos },
    { text: "Em Transporte", value: EnumSituacoesCarga.EmTransporte },
    { text: "Encerrada", value: EnumSituacoesCarga.Encerrada },
    { text: "Pagamento Liberado", value: EnumSituacoesCarga.LiberadoPagamento },
    { text: "Canceladas", value: EnumSituacoesCarga.Cancelada },
    { text: "Todas", value: EnumSituacoesCarga.Todas }
];


var _situacaoCargaTMS = [
    //{ text: "Todas", value: "" },
    { text: "Em Andamento", value: EnumSituacoesCarga.NaLogistica },
    { text: "Etapa 1 (Carga)", value: EnumSituacoesCarga.Nova },
    { text: "Etapa 2 (NF-e)", value: EnumSituacoesCarga.AgNFe },
    { text: "Etapa 3 (Frete)", value: EnumSituacoesCarga.CalculoFrete },
    { text: "Etapa 4 e 5 (Documentos)", value: EnumSituacoesCarga.PendeciaDocumentos },
    { text: "Etapa 6 (Integração)", value: EnumSituacoesCarga.AgIntegracao },
    { text: "Em Transporte", value: EnumSituacoesCarga.EmTransporte },
    { text: "Finalizada", value: EnumSituacoesCarga.Encerrada },
    { text: "Cancelada", value: EnumSituacoesCarga.Cancelada },
    { text: "Anulada", value: EnumSituacoesCarga.Anulada }
];

var _tipoLocalPrestacao = [
    { text: "Todas", value: EnumTipoLocalPrestacao.todos },
    { text: "Municipal", value: EnumTipoLocalPrestacao.intraMunicipal },
    { text: "Intermunicipal", value: EnumTipoLocalPrestacao.interMunicipal }
]

var PesquisaEncaixes = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Carga Inicial: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Carga Final: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo de Veículo:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.TipoLocalPrestacao = PropertyEntity({ val: ko.observable(EnumTipoLocalPrestacao.todos), options: _tipoLocalPrestacao, def: EnumTipoLocalPrestacao.todos, text: "Tipo da Prestação: " });
    this.CargaEncaixada = PropertyEntity({ type: types.map, text: "Carga Encaixada:" });
    this.PedidoEncaixado = PropertyEntity({ type: types.map, text: "Pedido Encaixado:" });
    this.NotaEncaixada = PropertyEntity({ type: types.map, text: "Nota Encaixada:" });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    var situacaoCargaPesquisa = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? _situacaoCargaEmbarcador : _situacaoCargaTMS;
    var situacoesPreSelecionadas = [EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.EmTransporte];

    this.Situacoes = PropertyEntity({ val: ko.observable(situacoesPreSelecionadas), def: new Array(), getType: typesKnockout.selectMultiple, params: { Tipo: "", Ativo: situacaoCargaPesquisa.Todos, OpcaoSemGrupo: false }, text: "Situações Carga: ", options: ko.observable(situacaoCargaPesquisa), visible: ko.observable(true) });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        this.Filial.visible(false);
        this.GrupoPessoas.visible(true);
        this.Transportador.text("Empresa/Filial:");
    }
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridEncaixe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaEncaixe.Visible.visibleFade() == true) {
                _pesquisaEncaixe.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaEncaixe.Visible.visibleFade(true);
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

function LoadRelatorioEncaixes() {
    _pesquisaEncaixe = new PesquisaEncaixes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridEncaixe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Encaixe/Pesquisa", _pesquisaEncaixe, null, null, 10, null, null, null, null, 20);
    _gridEncaixe.SetPermitirEdicaoColunas(true);

    _relatorioEncaixe = new RelatorioGlobal("Relatorios/Encaixe/BuscarDadosRelatorio", _gridEncaixe, function () {
        _relatorioEncaixe.loadRelatorio(function () {
            KoBindings(_pesquisaEncaixe, "knockoutPesquisaEncaixe");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaEncaixe");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaEncaixes");

            new BuscarTransportadores(_pesquisaEncaixe.Transportador, null, null, true);
            new BuscarVeiculos(_pesquisaEncaixe.Veiculo);
            new BuscarMotoristas(_pesquisaEncaixe.Motorista);
            new BuscarTiposdeCarga(_pesquisaEncaixe.TipoCarga);
            new BuscarModelosVeicularesCarga(_pesquisaEncaixe.ModeloVeiculo);
            new BuscarFilial(_pesquisaEncaixe.Filial);
            new BuscarGruposPessoas(_pesquisaEncaixe.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarClientes(_pesquisaEncaixe.Remetente);
            new BuscarClientes(_pesquisaEncaixe.Destinatario);
            new BuscarLocalidades(_pesquisaEncaixe.Origem);
            new BuscarLocalidades(_pesquisaEncaixe.Destino);
            new BuscarTiposOperacao(_pesquisaEncaixe.TipoOperacao);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaEncaixe);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioEncaixe.gerarRelatorio("Relatorios/Encaixe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioEncaixe.gerarRelatorio("Relatorios/Encaixe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
