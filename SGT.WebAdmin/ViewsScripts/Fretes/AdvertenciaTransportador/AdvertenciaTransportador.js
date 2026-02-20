/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/MotivoAdvertenciaTransportador.js" />
/// <reference path="../../Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _advertenciaTransportador;
var _gridAdvertenciaTransportador;
var _pesquisaAdvertenciaTransportador;

/*
 * Declaração das Classes
 */

var AdvertenciaTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });

    this.Motivo = PropertyEntity({ text: "*Motivo da Advertência:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Transportador = PropertyEntity({ text: "*Transportador:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Data = PropertyEntity({ text: "Data:", visible: ko.observable(false), getType: typesKnockout.dateTime, val: ko.observable("") })

    this.Cancelar = PropertyEntity({ eventClick: cancelarAdicaoAdvertenciaTransportadorClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarAdvertenciaTransportadorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarAdvertenciaTransportadorClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
}

var PesquisaAdvertenciaTransportador = function () {
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Motivo = PropertyEntity({ text: "Motivo da Advertência", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: adicionarAdvertenciaTransportadorModalClick, text: "Adicionar" });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function () {
            _pesquisaAdvertenciaTransportador.ExibirFiltros.visibleFade(false);
            recarregarGridAdvertenciaTransportador();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAdvertenciaTransportador() {
    _advertenciaTransportador = new AdvertenciaTransportador();
    KoBindings(_advertenciaTransportador, "knockoutAdvertenciaTransportador");

    _pesquisaAdvertenciaTransportador = new PesquisaAdvertenciaTransportador();
    KoBindings(_pesquisaAdvertenciaTransportador, "knockoutPesquisaAdvertenciaTransportador");

    new BuscarMotivoAdvertenciaTransportador(_advertenciaTransportador.Motivo);
    new BuscarTransportadores(_advertenciaTransportador.Transportador);

    new BuscarMotivoAdvertenciaTransportador(_pesquisaAdvertenciaTransportador.Motivo);
    new BuscarTransportadores(_pesquisaAdvertenciaTransportador.Transportador);

    loadGridAdvertenciaTransportador();
}

function loadGridAdvertenciaTransportador() {
    var opcaoExcluir = { descricao: "Remover", id: guid(), evento: "onclick", metodo: excluirAdvertenciaTransportadorClick, tamanho: "10", icone: "", visibilidade: obterVisibilidadeExcluirOuAlterar };
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: abrirModalEditarAdvertenciaTransportadorClick, tamanho: "10", icone: "", visibilidade: obterVisibilidadeExcluirOuAlterar };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoEditar, opcaoExcluir] };

    var configuracaoExportacao = { url: "AdvertenciaTransportador/ExportarPesquisa", titulo: "Advertências de Transportadores" };

    _gridAdvertenciaTransportador = new GridView(_pesquisaAdvertenciaTransportador.Pesquisar.idGrid, "AdvertenciaTransportador/Pesquisa", _pesquisaAdvertenciaTransportador, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    _gridAdvertenciaTransportador.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAdvertenciaTransportadorClick() {
    if (!ValidarCamposObrigatorios(_advertenciaTransportador)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    executarReST("AdvertenciaTransportador/Adicionar", RetornarObjetoPesquisa(_advertenciaTransportador), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Advertência adicionada com sucesso");
                fecharModalAdvertenciaTransportadorAdicionar();
                recarregarGridAdvertenciaTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function adicionarAdvertenciaTransportadorModalClick() {
    _advertenciaTransportador.Transportador.codEntity(_pesquisaAdvertenciaTransportador.Transportador.codEntity());
    _advertenciaTransportador.Transportador.entityDescription(_pesquisaAdvertenciaTransportador.Transportador.entityDescription());
    _advertenciaTransportador.Transportador.val(_pesquisaAdvertenciaTransportador.Transportador.val());

    exibirModalAdvertenciaTransportadorAdicionar();
}

function cancelarAdicaoAdvertenciaTransportadorClick() {
    fecharModalAdvertenciaTransportadorAdicionar();
}

function excluirAdvertenciaTransportadorClick(registroSelecionado) {
    executarReST("AdvertenciaTransportador/ExcluirPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro excluído com sucesso.");
                _gridAdvertenciaTransportador.CarregarGrid();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function atualizarAdvertenciaTransportadorClick() {
    if (!ValidarCamposObrigatorios(_advertenciaTransportador)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    executarReST("AdvertenciaTransportador/Atualizar", RetornarObjetoPesquisa(_advertenciaTransportador), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Advertência atualizada com sucesso");
                fecharModalAdvertenciaTransportadorAdicionar();
                recarregarGridAdvertenciaTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function abrirModalEditarAdvertenciaTransportadorClick(registroSelecionado) {
    _advertenciaTransportador.Transportador.enable(false);
    _advertenciaTransportador.Adicionar.visible(false);
    _advertenciaTransportador.Atualizar.visible(true);
    _advertenciaTransportador.Data.visible(true);

    _advertenciaTransportador.Codigo.val(registroSelecionado.Codigo);
    _advertenciaTransportador.Data.val(registroSelecionado.Data);
    _advertenciaTransportador.Observacao.val(registroSelecionado.Observacao);
    _advertenciaTransportador.Transportador.val(registroSelecionado.Transportador);
    _advertenciaTransportador.Transportador.codEntity(registroSelecionado.CodigoTransportador);
    _advertenciaTransportador.Motivo.val(registroSelecionado.Motivo);
    _advertenciaTransportador.Motivo.codEntity(registroSelecionado.CodigoMotivo);

    exibirModalAdvertenciaTransportadorAdicionar();
}

/*
 * Declaração das Funções
 */

function exibirModalAdvertenciaTransportadorAdicionar() {
    Global.abrirModal('divModalAdvertenciaTransportadorAdicionar');
    $("#divModalAdvertenciaTransportadorAdicionar").one('hidden.bs.modal', function () {
        LimparCampos(_advertenciaTransportador);
        _advertenciaTransportador.Transportador.enable(true);
        _advertenciaTransportador.Adicionar.visible(true);
        _advertenciaTransportador.Atualizar.visible(false);
        _advertenciaTransportador.Data.visible(false);
    });
}

function fecharModalAdvertenciaTransportadorAdicionar() {
    Global.fecharModal('divModalAdvertenciaTransportadorAdicionar');
}

function recarregarGridAdvertenciaTransportador() {
    _gridAdvertenciaTransportador.CarregarGrid();
}

function obterVisibilidadeExcluirOuAlterar(registroSelecionado) {
    return registroSelecionado.PermiteAlterarOuExcluirAdvertencia;
}