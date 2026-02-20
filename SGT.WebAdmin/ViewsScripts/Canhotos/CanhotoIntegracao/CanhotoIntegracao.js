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
/// <reference path="../../Consultas/TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaCanhoto;
var _gridCanhoto;
var _buscandoApenasNaoReconhecidos = true;
var _atualizarAoFechar = false;
var _dataGridCarregada = [];
var _indexCanhotoAberto = -1;
var _modalHistoricoIntegracao;
var _tipoIntegracao;

var _situacaoIntegracaoCanhoto = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Aguardando Retorno" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

var _pesquisaHistoricoIntegracao;

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}


var PesquisaCanhoto = function () {
    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.NumeroDocumento = PropertyEntity({ val: ko.observable(""), text: "Número Documento: " });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCanhoto, text: "Situação: " });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga: ", idBtnSearch: guid(), visible: ko.observable(true) })
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroCTe = PropertyEntity({ type: typesKnockout.int, text: "Número CT-e: ", val: ko.observable("") });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Emitente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoIntegracao = PropertyEntity({ text: "Integradora: ", options: _tipoIntegracao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });    
    this.ReenviarIntegracoes = PropertyEntity({
        eventClick: function (e) {
            ReenviarIntegracoes();
        }, type: types.event, text: "Reenviar Integrações", idGrid: guid(), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            RecarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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
function loadCanhotoIntegracao() {
    ObterTiposIntegracao().then(function () {
        _pesquisaCanhoto = new PesquisaCanhoto();
        KoBindings(_pesquisaCanhoto, "knockoutPesquisaCanhoto", false, _pesquisaCanhoto.Pesquisar.id);

        new BuscarClientes(_pesquisaCanhoto.Emitente);
        BuscarFilial(_pesquisaCanhoto.Filial);
        BuscarTransportadores(_pesquisaCanhoto.Transportador);
        BuscarCargas(_pesquisaCanhoto.Carga);
        BuscarIntegracoesCanhoto();

        _modalHistoricoIntegracao = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracao"), { backdrop: true, keyboard: true });
    });
}


//*******MÉTODOS*******
function BuscarIntegracoesCanhoto() {
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("CanhotoIntegracao"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [auditar] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });

    var configExportacao = {
        url: "CanhotoIntegracao/ExportarPesquisa",
        titulo: "Canhoto Integração"
    };

    _gridCanhoto = new GridViewExportacao(_pesquisaCanhoto.Pesquisar.idGrid, "CanhotoIntegracao/Pesquisa", _pesquisaCanhoto, menuOpcoes, configExportacao, null, 10);
    RecarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("CanhotoIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridCanhoto.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarIntegracoes(data) {
    var dados = new Array();
    dados = _gridCanhoto.GridViewTableData();

    var listaCodigosTotais = new Array();

    for (var i = 0; i < dados.length; i++) {
        listaCodigosTotais[i] = dados[i].Codigo;
    }

    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações com falha?", function () {
        executarReST("CanhotoIntegracao/ReenviarIntegracoes", { Codigos: JSON.stringify(listaCodigosTotais) }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridCanhoto.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    _modalHistoricoIntegracao.show();
}


function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "CanhotoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("CanhotoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function GridCarregada(data) {
    if (_buscandoApenasNaoReconhecidos) {
        _dataGridCarregada = data.data;
    }
}

function RecarregarGrid(cb) {
    _gridCanhoto.CarregarGrid(function (data) {
        GridCarregada(data);

        if (cb != null) cb();
    });
}

function ObterTiposIntegracao() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {}, function (r) {
        if (r.Success) {
            _tipoIntegracao = [{ value: "", text: Localization.Resources.Gerais.Geral.Todos }];

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracao.push({ value: r.Data[i].CodigoTipo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}