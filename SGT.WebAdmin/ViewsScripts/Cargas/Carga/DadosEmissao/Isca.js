/// <reference path="../../../Consultas/Isca.js" />

var _cargaDadosEmissaoIsca;
var _gridCargaDadosEmissaoIsca;
var _iscas = [];

var CargaDadosEmissaoIsca = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Iscas = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    
    this.Isca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Isca.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    
    this.Adicionar = PropertyEntity({ eventClick: adicionarCargaDadosEmissaoIscaClick, type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarIsca, visible: ko.observable(true), enable: ko.observable(true) });
}

function loadIsca() {
    verificarExibicaoAbaIsca();

    _cargaDadosEmissaoIsca = new CargaDadosEmissaoIsca();

    KoBindings(_cargaDadosEmissaoIsca, "tabIscas_" + _cargaAtual.DadosEmissaoFrete.id);
    
    BuscarIscas(_cargaDadosEmissaoIsca.Isca);

    loadGridCargaDadosIsca();
    SetarEnableCamposKnockout(_cargaDadosEmissaoIsca, _cargaAtual.EtapaFreteEmbarcador.enable());
}

function loadGridCargaDadosIsca() {
    var linhasPorPaginas = 2;
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerIscaCargaClick, icone: "", visibilidade: _cargaAtual.EtapaFreteEmbarcador.enable };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoIsca", visible: false },
        { data: "Isca", title: Localization.Resources.Cargas.Carga.Isca, width: "50%", className: "text-align-left" },
        { data: "CodigoIntegracao", title: Localization.Resources.Cargas.Carga.CodigoIntegracao, width: "50%", className: "text-align-left" }
    ];

    _gridCargaDadosEmissaoIsca = new BasicDataTable(_cargaDadosEmissaoIsca.Iscas.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridCargaDadosEmissaoIsca.CarregarGrid([]);
}

function preencherCargaDadosEmissaoIsca(dados) {
    if (_gridCargaDadosEmissaoIsca != null) {
        _iscas = dados.Data.Iscas;
        recarregarGridIscas();
    }
}

//#region Métodos publicos

function verificarExibicaoAbaIsca() {
    if (_cargaAtual.PermiteInformarIsca.val())
        $("#tabIscas_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();
    else
        $("#tabIscas_" + _cargaAtual.DadosEmissaoFrete.id + "_li").hide();
}

//#endregion

//#region Métodos privados

function removerIscaCargaClick(registroSelecionado) {
    executarReST("DadosEmissaoIsca/ExcluirIscaCarga", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoRemovidaComSucesso);

                _iscas = _iscas.filter(function (registro) {
                    return registro.Codigo != registroSelecionado.Codigo
                });

                recarregarGridIscas();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function adicionarCargaDadosEmissaoIscaClick() {
    if (!ValidarCamposObrigatorios(_cargaDadosEmissaoIsca)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var iscaJaAdicionada = _iscas.filter(function (isca) {
        return isca.CodigoIsca == _cargaDadosEmissaoIsca.Isca.codEntity();
    }).length > 0;

    if (iscaJaAdicionada)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.EssaIscaJaFoiAdicionada);
    
    _cargaDadosEmissaoIsca.CodigoCarga.val(_cargaAtual.Codigo.val());
   
    executarReST("DadosEmissaoIsca/AdicionarIsca", RetornarObjetoPesquisa(_cargaDadosEmissaoIsca), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.IscaAdicionada);

                var novoRegistro = {
                    Codigo: retorno.Data.Codigo,
                    CodigoIsca: _cargaDadosEmissaoIsca.Isca.codEntity(),
                    Isca: _cargaDadosEmissaoIsca.Isca.val(),
                    CodigoIntegracao: retorno.Data.CodigoIntegracao
                };

                _iscas.push(novoRegistro);

                recarregarGridIscas();

                limparCamposDadosEmissaoIscas();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function cancelarCargaDadosEmissaoIscaClick() {
    limparCamposDadosEmissaoIscas();
}

function recarregarGridIscas() {
    _gridCargaDadosEmissaoIsca.CarregarGrid(_iscas);
}

function limparCamposDadosEmissaoIscas() {
    LimparCampos(_cargaDadosEmissaoIsca);
}

//#endregion