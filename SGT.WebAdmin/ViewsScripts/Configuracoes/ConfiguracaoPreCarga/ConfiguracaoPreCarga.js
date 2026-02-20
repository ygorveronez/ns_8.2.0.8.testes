/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _crudConfiguracaoPreCarga;
var _configuracaoPreCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CrudConfiguracaoPreCarga = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarConfiguracaoPreCargaClick, type: types.event, text: "Atualizar" });
}

var ConfiguracaoPreCarga = function () {
    this.DiasParaTransportadorAdicionarFilaCarregamentoVeiculo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: "Dias para transportador adicionar veículo na fila de carregamento:" });
    this.DiasTransicaoAutomaticaFilaCarregamentoVeiculo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: "Dias de transição automatica da fila de carregamento:" });
    this.TempoAguardarConfirmacaoTransportador = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: "Tempo aguardar a confirmação do transportador (minutos):" });
    this.VincularFilaCarregamentoVeiculoAutomaticamente = PropertyEntity({ text: "Vincular com a fila de carregamento automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AceitarVinculoFilaCarregamentoVeiculoAutomaticamente = PropertyEntity({ text: "Aceitar vinculo com a fila de carregamento automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VincularPrePlanoSemValidarModeloVeicularCarga = PropertyEntity({ text: "Vincular pré plano sem validar modelo veicular marga", getType: typesKnockout.bool, val: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadConfiguracaoPreCarga() {
    _configuracaoPreCarga = new ConfiguracaoPreCarga();
    KoBindings(_configuracaoPreCarga, "knockoutConfiguracaoPreCarga");

    HeaderAuditoria("ConfiguracaoPreCarga", _configuracaoPreCarga);

    _crudConfiguracaoPreCarga = new CrudConfiguracaoPreCarga();
    KoBindings(_crudConfiguracaoPreCarga, "knockoutCrudConfiguracaoPreCarga");

    buscarConfiguracaoPreCarga();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function atualizarConfiguracaoPreCargaClick() {
    executarReST("ConfiguracaoPreCarga/Atualizar", RetornarObjetoPesquisa(_configuracaoPreCarga), function (retorno) {
        if (retorno.Success)
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso!");
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function buscarConfiguracaoPreCarga() {
    executarReST("ConfiguracaoPreCarga/Buscar", null, function (retorno) {
        PreencherObjetoKnout(_configuracaoPreCarga, retorno);
    });
}

// #endregion Funções Privadas
