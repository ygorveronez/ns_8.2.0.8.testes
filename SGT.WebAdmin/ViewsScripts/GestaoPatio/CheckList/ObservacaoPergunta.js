/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="CheckList.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _observacaoPergunta;

/*
 * Declaração das Classes
 */

var ObservacaoPergunta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pergunta = PropertyEntity({ text: "Pergunta: ", val: ko.observable(''), def: '' });
    this.Observacao = PropertyEntity({ text: "Observações para ", val: ko.observable(''), def: '', enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: salvarObservacaoClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadObservacaoPergunta() {
    _observacaoPergunta = new ObservacaoPergunta();
    KoBindings(_observacaoPergunta, "knockoutObservacao");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function salvarObservacaoClick(pergunta) {
    var dados = {
        Codigo: pergunta.Codigo.val(),
        Observacao: pergunta.Observacao.val()
    };

    atualizarObservacao(dados);
    fecharModalObservacaoPergunta();
}

/*
 * Declaração das Funções Públicas
 */

function editarObservacaoPergunta(pergunta) {
    _observacaoPergunta.Codigo.val(pergunta.Codigo);
    _observacaoPergunta.Pergunta.val(pergunta.Descricao.replace(':', ''));
    _observacaoPergunta.Observacao.val(pergunta.Observacao);

    exibirModalObservacaoPergunta();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalObservacaoPergunta() {
    Global.abrirModal('divModalObservacaoPergunta');

    $("#divModalObservacaoPergunta").one('shown.bs.modal', function () {
        $("#" + _observacaoPergunta.Observacao.id).focus();
    });

    $("#divModalObservacaoPergunta").one('hidden.bs.modal', function () {
        LimparCampos(_observacaoPergunta);
    });
}

function fecharModalObservacaoPergunta() {
    Global.fecharModal('divModalObservacaoPergunta');
}
