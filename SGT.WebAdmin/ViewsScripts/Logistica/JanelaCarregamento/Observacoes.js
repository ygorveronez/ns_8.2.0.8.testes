/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="RetornoMultiplaSelecao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _observacaoFluxoPatio;
var _observacaoTransportador;

/*
 * Declaração das Classes
 */

var ObservacaoFluxoPatio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Observacao.getFieldDescription(), maxlength: 1000 });

    this.Salvar = PropertyEntity({ eventClick: salvarObservacaoFluxoPatioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
};

var ObservacaoTransportador = function () {
    this.Codigos;
    this.Observacao = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.ObservacaesParaOsTransportadores.getRequiredFieldDescription(), maxlength: 500, visible: ko.observable(true) });

    this.Salvar = PropertyEntity({ eventClick: salvarObservacaoTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadObservacoes() {
    _observacaoFluxoPatio = new ObservacaoFluxoPatio();
    KoBindings(_observacaoFluxoPatio, "knockoutObservacaoFluxoPatio");

    _observacaoTransportador = new ObservacaoTransportador();
    KoBindings(_observacaoTransportador, "knockoutObservacaoTransportador");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function salvarObservacaoFluxoPatioClick() {
    executarReST("JanelaCarregamento/AlterarObservacaoFluxoPatio", RetornarObjetoPesquisa(_observacaoFluxoPatio), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoDoFluxoDePatioAtualizadaComSucesso);
                fecharModalObservacaoFluxoPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function salvarObservacaoTransportadorClick() {
    var dados = RetornarObjetoPesquisa(_observacaoTransportador);

    dados.Codigos = JSON.stringify(_observacaoTransportador.Codigos);

    executarReST("JanelaCarregamento/AlterarObservacaoTransportador", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirRetornoMultiplaSelecao(retorno.Data);
                fecharModalObservacaoTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirObservacaoFluxoPatio(cargaJanelaCarregamento) {
    _observacaoFluxoPatio.Codigo.val(cargaJanelaCarregamento.Codigo);
    _observacaoFluxoPatio.Observacao.val(cargaJanelaCarregamento.ObservacaoFluxoPatio);

    exibirModalObservacaoFluxoPatio();
}

function exibirObservacaoTransportador(cargaJanelaCarregamento) {
    _observacaoTransportador.Codigos = [cargaJanelaCarregamento.Codigo];
    _observacaoTransportador.Observacao.val(cargaJanelaCarregamento.ObservacaoTransportador);

    exibirModalObservacaoTransportador();
}

function exibirObservacaoTransportadores(codigos) {
    _observacaoTransportador.Codigos = codigos;

    exibirModalObservacaoTransportador();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalObservacaoFluxoPatio() {
    Global.abrirModal('divModalObservacaoFluxoPatio');
    $("#divModalObservacaoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_observacaoFluxoPatio);
    });
}

function exibirModalObservacaoTransportador() {
    Global.abrirModal('divModalObservacaoTransportador');
    $("#divModalObservacaoTransportador").one('hidden.bs.modal', function () {
        LimparCampos(_observacaoTransportador);
    });
}

function fecharModalObservacaoFluxoPatio() {
    Global.fecharModal('divModalObservacaoFluxoPatio');
}

function fecharModalObservacaoTransportador() {
    Global.fecharModal('divModalObservacaoTransportador');
}
