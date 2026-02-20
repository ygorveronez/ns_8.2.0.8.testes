/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoLancamentoNFSManual.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridDocumentoEmissaoNFSManual, _pesquisaDocumentoEmissaoNFSManual, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioDocumentoEmissaoNFSManual;

var _tipoPossuiNFSGerada = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

/*
 * Declaração das Classes
 */

var PesquisaDocumentoEmissaoNFSManual = function () {
    var configuracaoInteiro = { precision: 0, allowZero: false, thousands: '' };
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({
        visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador ? true : false )
        , type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador"), idBtnSearch: guid()
    });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? true : false)) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade da Prestação:", idBtnSearch: guid() });
    this.EstadoPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado da Prestação:", idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoNFSInicial = PropertyEntity({ text: "Data NFS Inicial: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoNFSFinal = PropertyEntity({ text: "Data NFS Final: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, val: ko.observable(""), def: "", configInt: configuracaoInteiro });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, val: ko.observable(""), def: "", configInt: configuracaoInteiro });
    this.NumeroNFSInicial = PropertyEntity({ text: "Número NFS Inicial: ", getType: typesKnockout.int, val: ko.observable(""), def: "", configInt: configuracaoInteiro });
    this.NumeroNFSFinal = PropertyEntity({ text: "Número NFS Final: ", getType: typesKnockout.int, val: ko.observable(""), def: "", configInt: configuracaoInteiro });
    this.PossuiNFSGerada = PropertyEntity({ text: "NFS Gerada:", options: _tipoPossuiNFSGerada, val: ko.observable(""), def: "" });
    this.SituacaoNFS = PropertyEntity({ text: "Situação NFS:", getType: typesKnockout.selectMultiple, options: EnumSituacaoLancamentoNFSManual.obterOpcoesPesquisa(), val: ko.observable(EnumSituacaoLancamentoNFSManual.Todas), def: EnumSituacaoLancamentoNFSManual.Todas });
    this.NumeroPedidoCliente = PropertyEntity({ text: "Nº do Pedido Cliente:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoNFSInicial.dateRangeLimit = this.DataEmissaoNFSFinal;
    this.DataEmissaoNFSFinal.dateRangeInit = this.DataEmissaoNFSInicial;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoEmissaoNFSManual.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio"
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaDocumentoEmissaoNFSManual.Visible.visibleFade() == true) {
                _pesquisaDocumentoEmissaoNFSManual.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            }
            else {
                _pesquisaDocumentoEmissaoNFSManual.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus")
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadDocumentoEmissaoNFSManual() {
    _pesquisaDocumentoEmissaoNFSManual = new PesquisaDocumentoEmissaoNFSManual();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDocumentoEmissaoNFSManual = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DocumentoEmissaoNFSManual/Pesquisa", _pesquisaDocumentoEmissaoNFSManual);

    _gridDocumentoEmissaoNFSManual.SetPermitirEdicaoColunas(true);
    _gridDocumentoEmissaoNFSManual.SetQuantidadeLinhasPorPagina(10);

    _relatorioDocumentoEmissaoNFSManual = new RelatorioGlobal("Relatorios/DocumentoEmissaoNFSManual/BuscarDadosRelatorio", _gridDocumentoEmissaoNFSManual, function () {
        _relatorioDocumentoEmissaoNFSManual.loadRelatorio(function () {
            KoBindings(_pesquisaDocumentoEmissaoNFSManual, "knockoutPesquisaDocumentoEmissaoNFSManual", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDocumentoEmissaoNFSManual", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDocumentoEmissaoNFSManual", false);

            new BuscarCargas(_pesquisaDocumentoEmissaoNFSManual.Carga);
            new BuscarTransportadores(_pesquisaDocumentoEmissaoNFSManual.Empresa, null, null, true);
            new BuscarFilial(_pesquisaDocumentoEmissaoNFSManual.Filial);
            new BuscarGruposPessoas(_pesquisaDocumentoEmissaoNFSManual.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarClientes(_pesquisaDocumentoEmissaoNFSManual.Remetente);
            new BuscarClientes(_pesquisaDocumentoEmissaoNFSManual.Destinatario);
            new BuscarClientes(_pesquisaDocumentoEmissaoNFSManual.Tomador);
            new BuscarLocalidades(_pesquisaDocumentoEmissaoNFSManual.LocalidadePrestacao);
            new BuscarEstados(_pesquisaDocumentoEmissaoNFSManual.EstadoPrestacao);
            new BuscarTiposOperacao(_pesquisaDocumentoEmissaoNFSManual.TipoOperacao);
            new BuscarTransportadores(_pesquisaDocumentoEmissaoNFSManual.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDocumentoEmissaoNFSManual);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDocumentoEmissaoNFSManual.gerarRelatorio("Relatorios/DocumentoEmissaoNFSManual/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDocumentoEmissaoNFSManual.gerarRelatorio("Relatorios/DocumentoEmissaoNFSManual/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
