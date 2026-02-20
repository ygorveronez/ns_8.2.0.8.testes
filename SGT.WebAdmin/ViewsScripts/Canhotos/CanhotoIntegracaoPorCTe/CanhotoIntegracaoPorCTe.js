/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaCanhoto;
var _gridCanhotoPorCTe;
var _buscandoApenasNaoReconhecidos = true;
var _atualizarAoFechar = false;
var _dataGridCarregada = [];
var _indexCanhotoAberto = -1;
var _modalHistoricoIntegracao;

var _situacaoIntegracaoCanhoto = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Aguardando Retorno" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

var _tipo = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.Imagem, text: "Imagem" },
{ value: EnumSituacaoIntegracaoCarga.Confirmacao, text: "Confirmação" }];

var _pesquisaHistoricoIntegracaoPorCTe;

var PesquisaHistoricoIntegracaoPorCTe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var PesquisaCanhoto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.NumeroDocumento = PropertyEntity({ val: ko.observable(""), text: "Número Documento: " });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCanhoto, text: "Situação: " });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoCargaEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoRegistro = PropertyEntity({ val: ko.observable(EnumImagemConfirmacao.Todos), options: EnumImagemConfirmacao.obterOpcoesPesquisa(), def: EnumImagemConfirmacao.Todos, text: "Tipo: ", visible: ko.observable(true) });
    this.DataEmissaoNFeInicial = PropertyEntity({ text: "Data Emissão NF-e Inicial: ", val: ko.observable(true), getType: typesKnockout.date });
    this.DataEmissaoNFeFinal = PropertyEntity({ text: "Data Emissão NF-e Final: ", val: ko.observable(true), getType: typesKnockout.date });
    this.DataEntregaInicial = PropertyEntity({ text: "Data Entrega Inicial: ", val: ko.observable(true), getType: typesKnockout.date });
    this.DataEntregaFinal = PropertyEntity({ text: "Data Entrega Final: ", val: ko.observable(true), getType: typesKnockout.date });
    this.DataDigitalizacaoInicial = PropertyEntity({ text: "Data Digitalização Inicial: ", val: ko.observable(true), getType: typesKnockout.date });
    this.DataDigitalizacaoFinal = PropertyEntity({ text: "Data Digitalização Final: ", val: ko.observable(true), getType: typesKnockout.date });
    this.DataAprovacaoInicial = PropertyEntity({ text: "Data Aprovação Inicial: ", val: ko.observable(true), getType: typesKnockout.date });
    this.DataAprovacaoFinal = PropertyEntity({ text: "Data Aprovação Final: ", val: ko.observable(true), getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            RecarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodos();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

//*******EVENTOS*******
function loadCanhotoIntegracaoPorCTe() {
    _pesquisaCanhoto = new PesquisaCanhoto();
    KoBindings(_pesquisaCanhoto, "knockoutPesquisaCanhotoPorCTe", false, _pesquisaCanhoto.Pesquisar.id);

    _pesquisaHistoricoIntegracaoPorCTe = new PesquisaHistoricoIntegracaoPorCTe();

    new BuscarTransportadores(_pesquisaCanhoto.Emitente);
    new BuscarCargas(_pesquisaCanhoto.CodigoCargaEmbarcador);

    BuscarIntegracoesCanhoto();

    _modalHistoricoIntegracao = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracaoPorCTe"), { backdrop: true, keyboard: true });
}


//*******MÉTODOS*******
function BuscarIntegracoesCanhoto() {
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("CanhotoIntegracao"), icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [auditar] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });

    var configExportacao = {
        url: "CanhotoIntegracaoPorCTe/ExportarIntegracao",
        btnText: "Exportar excel",
        titulo: "Canhoto Integração por CT-e"
    }

    _gridCanhotoPorCTe = new GridView(_pesquisaCanhoto.Pesquisar.idGrid, "CanhotoIntegracaoPorCTe/Pesquisa", _pesquisaCanhoto, menuOpcoes, null, 10, null, null, null, null, null, null, configExportacao);
    RecarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("CanhotoIntegracaoPorCTe/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridCanhotoPorCTe.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodos(data) {
    var dados = new Array();
    dados = _gridCanhotoPorCTe.GridViewTableData();

    var listaCodigosTotais = new Array();

    for (var i = 0; i < dados.length; i++) {
        listaCodigosTotais[i] = dados[i].Codigo;
    }

    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("CanhotoIntegracaoPorCTe/ReenviarTodos", { Codigos: JSON.stringify(listaCodigosTotais) }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridCanhotoPorCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
            }
        });
    });
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    _modalHistoricoIntegracao.show();
}

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracaoPorCTe.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoPorCTe = new GridView("tabHistoricoIntegracaoPorCte", "CanhotoIntegracaoPorCTe/ConsultarHistoricoIntegracaoPorCTe", _pesquisaHistoricoIntegracaoPorCTe, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoPorCTe.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("CanhotoIntegracaoPorCTe/DownloadArquivosHistoricoIntegracaoPorCTe", { Codigo: historicoConsulta.Codigo });
}

function GridCarregada(data) {
    if (_buscandoApenasNaoReconhecidos) {
        _dataGridCarregada = data.data;
    }
}

function RecarregarGrid(cb) {
    _gridCanhotoPorCTe.CarregarGrid(function (data) {
        GridCarregada(data);

        if (cb != null) cb();
    });
}