/// <reference path="../../../Enumeradores/EnumTipoObservacaoCTe.js" />

var _cargaDadosEmissaoObservacaoFiscoContribuinte;
var _gridCargaDadosEmissaoObservacaoFiscoContribuinte;
var _observacoesFiscoContribuinte = [];

var CargaDadosEmissaoObservacaoFiscoContribuinte = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ObservacoesFiscoContribuinte = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Adicionar, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.TipoObservacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoObservacao.getRequiredFieldDescription(), visible: ko.observable(true), enable: ko.observable(true), options: EnumTipoObservacaoCTe.ObterOpcoes(), val: ko.observable(EnumTipoObservacaoCTe.Contribuinte), def: EnumTipoObservacaoCTe.Contribuinte });
    this.Identificador = PropertyEntity({ required: true, text: Localization.Resources.Cargas.Carga.Identificador.getRequiredFieldDescription(), visible: ko.observable(true), enable: ko.observable(true), val: ko.observable(""), maxlength: 20 });
    this.Descricao = PropertyEntity({ required: true, text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), visible: ko.observable(true), enable: ko.observable(true), val: ko.observable(""), maxlength: 160 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCargaDadosEmissaoObservacaoFiscoContribuinteClick, type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarObservacao, visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCargaDadosEmissaoObservacaoFiscoContribuinteClick, type: types.event, text: Localization.Resources.Cargas.Carga.AtualizarObservacao, visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarCargaDadosEmissaoObservacaoFiscoContribuinteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: ko.observable(true) });
}

function loadCargaDadosEmissaoObservacaoFiscoContribuinte() {
    _cargaDadosEmissaoObservacaoFiscoContribuinte = new CargaDadosEmissaoObservacaoFiscoContribuinte();

    KoBindings(_cargaDadosEmissaoObservacaoFiscoContribuinte, "tabObservacoesFiscoContribuinte_" + _cargaAtual.DadosEmissaoFrete.id);

    $("#tabObservacoesFiscoContribuinte_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();

    loadGridCargaDadosEmissaoObservacaoFiscoContribuinte();
    SetarEnableCamposKnockout(_cargaDadosEmissaoObservacaoFiscoContribuinte, _cargaAtual.EtapaFreteEmbarcador.enable());
}

function loadGridCargaDadosEmissaoObservacaoFiscoContribuinte() {
    var linhasPorPaginas = 2;
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarObservacaoFiscoContribuinteClick, icone: "", visibilidade: _cargaAtual.EtapaFreteEmbarcador.enable() };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerObservacaoFiscoContribuinteClick, icone: "", visibilidade: _cargaAtual.EtapaFreteEmbarcador.enable() };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoEditar, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", title: Localization.Resources.Cargas.Carga.Tipo, width: "20%", className: "text-align-left" },
        { data: "TipoCodigo", visible: false },
        { data: "Identificador", title: Localization.Resources.Cargas.Carga.Identificador, width: "20%", className: "text-align-left" },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%", className: "text-align-left" }
    ];

    _gridCargaDadosEmissaoObservacaoFiscoContribuinte = new BasicDataTable(_cargaDadosEmissaoObservacaoFiscoContribuinte.ObservacoesFiscoContribuinte.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridCargaDadosEmissaoObservacaoFiscoContribuinte.CarregarGrid([]);
}

function preencherCargaDadosEmissaoObservacaoFiscoContribuinte(dados) {
    if (_gridCargaDadosEmissaoObservacaoFiscoContribuinte != null) {
        _observacoesFiscoContribuinte = dados.Data.ObservacoesFiscoContribuinte;
        recarregarGridObservacaoFiscoContribuinte();
    }
}

//#region Métodos privados

function editarObservacaoFiscoContribuinteClick(registroSelecionado) {
    var data = {
        Codigo: registroSelecionado.Codigo,
        TipoObservacao: registroSelecionado.TipoCodigo,
        Identificador: registroSelecionado.Identificador,
        Descricao: registroSelecionado.Descricao
    };

    PreencherObjetoKnout(_cargaDadosEmissaoObservacaoFiscoContribuinte, { Data: data });

    _cargaDadosEmissaoObservacaoFiscoContribuinte.Adicionar.visible(false);
    _cargaDadosEmissaoObservacaoFiscoContribuinte.Atualizar.visible(true);
    _cargaDadosEmissaoObservacaoFiscoContribuinte.Cancelar.visible(true);
}

function removerObservacaoFiscoContribuinteClick(registroSelecionado) {
    executarReST("ObservacaoContribuinte/ExcluirPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoRemovidaComSucesso);

                _observacoesFiscoContribuinte = _observacoesFiscoContribuinte.filter(function (registro) {
                    return registro.Codigo != registroSelecionado.Codigo
                });

                recarregarGridObservacaoFiscoContribuinte();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function adicionarCargaDadosEmissaoObservacaoFiscoContribuinteClick() {
    if (!ValidarCamposObrigatorios(_cargaDadosEmissaoObservacaoFiscoContribuinte)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    _cargaDadosEmissaoObservacaoFiscoContribuinte.CodigoCarga.val(_cargaAtual.Codigo.val());
    
    executarReST("DadosEmissaoObservacaoFiscoContribuinte/AdicionarObservacao", RetornarObjetoPesquisa(_cargaDadosEmissaoObservacaoFiscoContribuinte), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoAdicionada);

                var novoRegistro = {
                    Codigo: retorno.Data.Codigo,
                    TipoCodigo: _cargaDadosEmissaoObservacaoFiscoContribuinte.TipoObservacao.val(),
                    Tipo: EnumTipoObservacaoCTe.ObterDescricao(_cargaDadosEmissaoObservacaoFiscoContribuinte.TipoObservacao.val()),
                    Identificador: _cargaDadosEmissaoObservacaoFiscoContribuinte.Identificador.val(),
                    Descricao: _cargaDadosEmissaoObservacaoFiscoContribuinte.Descricao.val()
                };

                _observacoesFiscoContribuinte.push(novoRegistro);

                recarregarGridObservacaoFiscoContribuinte();

                limparCamposObservacaoFiscoContribuinte();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function cancelarCargaDadosEmissaoObservacaoFiscoContribuinteClick() {
    limparCamposObservacaoFiscoContribuinte();
}

function atualizarCargaDadosEmissaoObservacaoFiscoContribuinteClick() {
    if (!ValidarCamposObrigatorios(_cargaDadosEmissaoObservacaoFiscoContribuinte)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    executarReST("DadosEmissaoObservacaoFiscoContribuinte/AtualizarObservacao", RetornarObjetoPesquisa(_cargaDadosEmissaoObservacaoFiscoContribuinte), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoAtualizada);
                
                var registroAtualizado = {
                    TipoCodigo: _cargaDadosEmissaoObservacaoFiscoContribuinte.TipoObservacao.val(),
                    Tipo: EnumTipoObservacaoCTe.ObterDescricao(_cargaDadosEmissaoObservacaoFiscoContribuinte.TipoObservacao.val()),
                    Identificador: _cargaDadosEmissaoObservacaoFiscoContribuinte.Identificador.val(),
                    Descricao: _cargaDadosEmissaoObservacaoFiscoContribuinte.Descricao.val()
                };

                for (var i = 0; i < _observacoesFiscoContribuinte.length; i++) {
                    var registroAtual = _observacoesFiscoContribuinte[i];
                    if (registroAtual.Codigo == _cargaDadosEmissaoObservacaoFiscoContribuinte.Codigo.val()) {
                        registroAtual.Tipo = registroAtualizado.Tipo;
                        registroAtual.Identificador = registroAtualizado.Identificador;
                        registroAtual.Descricao = registroAtualizado.Descricao;
                        registroAtual.TipoCodigo = registroAtualizado.TipoCodigo;
                        break;
                    }
                }

                recarregarGridObservacaoFiscoContribuinte();

                limparCamposObservacaoFiscoContribuinte();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function recarregarGridObservacaoFiscoContribuinte() {
    _gridCargaDadosEmissaoObservacaoFiscoContribuinte.CarregarGrid(_observacoesFiscoContribuinte);
}

function limparCamposObservacaoFiscoContribuinte() {
    _cargaDadosEmissaoObservacaoFiscoContribuinte.Adicionar.visible(true);
    _cargaDadosEmissaoObservacaoFiscoContribuinte.Atualizar.visible(false);
    _cargaDadosEmissaoObservacaoFiscoContribuinte.Cancelar.visible(false);
    
    LimparCampos(_cargaDadosEmissaoObservacaoFiscoContribuinte);
}

//#endregion