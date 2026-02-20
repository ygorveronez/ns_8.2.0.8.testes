/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _observacaoFluxoPatio;

/*
 * Declaração das Classes
 */

var ObservacaoFluxoPatio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 1000 });

    this.Salvar = PropertyEntity({ eventClick: salvarObservacaoFluxoPatioClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadObservacoes() {
    _observacaoFluxoPatio = new ObservacaoFluxoPatio();
    KoBindings(_observacaoFluxoPatio, "knockoutObservacaoFluxoPatio");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function salvarObservacaoFluxoPatioClick() {
    executarReST("JanelaDescarga/AlterarObservacaoFluxoPatio", RetornarObjetoPesquisa(_observacaoFluxoPatio), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Observação do fluxo de pátio atualizada com sucesso!");
                fecharModalObservacaoFluxoPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirObservacaoFluxoPatio(cargaJanelaDescarregamento) {
    _observacaoFluxoPatio.Codigo.val(cargaJanelaDescarregamento.Codigo);
    _observacaoFluxoPatio.Observacao.val(cargaJanelaDescarregamento.ObservacaoFluxoPatio);

    exibirModalObservacaoFluxoPatio();
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

function fecharModalObservacaoFluxoPatio() {
    Global.fecharModal('divModalObservacaoFluxoPatio');
}