

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTrechoBalsa;
var _trechoBalsa;
var _tempoBalsa;
var _pesquisaTrechoBalsa;
var _gridTrechoBalsa;
var _mapaTrechoBalsa;

/*
 * Declaração das Classes
 */

var CRUDTrechoBalsa = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaTrechoBalsa = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto origem:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto destino:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(_statusPesquisa.Ativo), options: _statusPesquisa, def: _statusPesquisa.Ativo, enable: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTrechoBalsa, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var TrechoBalsa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Distancia = PropertyEntity({ text: "Distancia:", required: true, getType: typesKnockout.decimal, val: ko.observable(0.00), def: 0.00 });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(_status.Ativo), options: _status, def: _status.Ativo, enable: ko.observable(true) });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto origem:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto destino:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.Area = PropertyEntity();

    this.ListaTempoBalsa = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), required: false });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTrechoBalsa() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "TrechoBalsa/ExportarPesquisa", titulo: "Trecho Balsa" };

    _gridTrechoBalsa = new GridViewExportacao(_pesquisaTrechoBalsa.Pesquisar.idGrid, "TrechoBalsa/Pesquisa", _pesquisaTrechoBalsa, menuOpcoes, configuracoesExportacao);
    _gridTrechoBalsa.CarregarGrid();
}

function loadTrechoBalsa() {
    _trechoBalsa = new TrechoBalsa();
    KoBindings(_trechoBalsa, "knockoutTrechoBalsa");

    HeaderAuditoria("TrechoBalsa", _trechoBalsa);

    _CRUDTrechoBalsa = new CRUDTrechoBalsa();
    KoBindings(_CRUDTrechoBalsa, "knockoutCRUDTrechoBalsa");

    _pesquisaTrechoBalsa = new PesquisaTrechoBalsa();
    KoBindings(_pesquisaTrechoBalsa, "knockoutPesquisaTrechoBalsa", false, _pesquisaTrechoBalsa.Pesquisar.id);

    loadGridTrechoBalsa();
    LoadTempoBalsa();
    loadMapa();

    new BuscarClientes(_trechoBalsa.PortoOrigem, desenharMarcadorOrigem);
    new BuscarClientes(_trechoBalsa.PortoDestino, desenharMarcadorDestino);
    new BuscarClientes(_pesquisaTrechoBalsa.PortoOrigem);
    new BuscarClientes(_pesquisaTrechoBalsa.PortoDestino);
}

function loadMapa() {
    _mapaTrechoBalsa = new MapaGoogle("map", true);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    _trechoBalsa.Area.val(_mapaTrechoBalsa.draw.getJson());
    preencherListaTempoBalsaParaBackEnd();
    Salvar(_trechoBalsa, "TrechoBalsa/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridTrechoBalsa();
                limparCamposTrechoBalsa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    _trechoBalsa.Area.val(_mapaTrechoBalsa.draw.getJson());
    preencherListaTempoBalsaParaBackEnd();
    Salvar(_trechoBalsa, "TrechoBalsa/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridTrechoBalsa();
                limparCamposTrechoBalsa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposTrechoBalsa();
}

function editarClick(registroSelecionado) {
    limparCamposTrechoBalsa();

    _trechoBalsa.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_trechoBalsa, "TrechoBalsa/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTrechoBalsa.ExibirFiltros.visibleFade(false);

                _mapaTrechoBalsa.draw.setJson(_trechoBalsa.Area.val());
                var isEdicao = true;
                controlarBotoesHabilitados(isEdicao);
                RecarregarListaTempoBalsa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}


function abaTrechoBalsaGeoLocalizacaoClick() {
    if (_mapaTrechoBalsa) {
        setTimeout(function () {
            _mapaTrechoBalsa.draw.centerShapes();
        }, 300);
    }
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_trechoBalsa, "TrechoBalsa/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridTrechoBalsa();
                    limparCamposTrechoBalsa();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _CRUDTrechoBalsa.Atualizar.visible(isEdicao);
    _CRUDTrechoBalsa.Excluir.visible(isEdicao);
    _CRUDTrechoBalsa.Cancelar.visible(isEdicao);
    _CRUDTrechoBalsa.Adicionar.visible(!isEdicao);
}

function limparCamposTrechoBalsa() {
    var isEdicao = false;
    _mapaTrechoBalsa.clear();
    controlarBotoesHabilitados(isEdicao);
    ClearCamposFaixaTempoBalsa();
    LimparCampos(_trechoBalsa);
    RecarregarListaTempoBalsa()
}

function recarregarGridTrechoBalsa() {
    _gridTrechoBalsa.CarregarGrid();
}

function desenharMarcadorOrigem(data) {
    if (data !== null) {
        _trechoBalsa.PortoOrigem.val(data.Descricao);
        _trechoBalsa.PortoOrigem.codEntity(data.Codigo);

        var markerOrigem = novoMarcador("Porto Origem", data.Latitude, data.Longitude);
        
        verificarMarcador(markerOrigem);
        
        _mapaTrechoBalsa.draw.addShape(markerOrigem);
        _mapaTrechoBalsa.draw.centerShapes();
    }
}

function desenharMarcadorDestino(data) {
    if (data !== null) {
        _trechoBalsa.PortoDestino.val(data.Descricao);
        _trechoBalsa.PortoDestino.codEntity(data.Codigo);

        var markerDestino = novoMarcador("Porto Destino", data.Latitude, data.Longitude);
        
        verificarMarcador(markerDestino);

        _mapaTrechoBalsa.draw.addShape(markerDestino);
        _mapaTrechoBalsa.draw.centerShapes();
    }
}

function verificarMarcador(marker) {
    _marcadores = _mapaTrechoBalsa.draw.getShapes();
    for (i = 0; i < _marcadores.length; i++) {
        if (_marcadores[i].title == marker.title) {
            _marcadores[i].setMap(null);
            _marcadores.splice(i, 1);
        }
    }
}

function novoMarcador(titulo, lat, lng) {
    marker = new ShapeMarker();
    marker.title = titulo;
    marker.setPosition(lat, lng);

    return marker;
}

