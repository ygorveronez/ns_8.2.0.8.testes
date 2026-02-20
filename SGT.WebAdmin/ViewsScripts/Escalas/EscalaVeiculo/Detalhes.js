/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _escalaVeiculoDetalhe;
var _gridEscalaVeiculoDetalhes;
var _modalEscalaVeiculoDetalhe;
/*
 * Declaração das Classes
 */

var EscalaVeiculoDetalhe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoAtual = PropertyEntity({ text: "Situação Atual: ", type: types.local });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", type: types.local });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEscalaVeiculoDetalhe() {
    _escalaVeiculoDetalhe = new EscalaVeiculoDetalhe();
    KoBindings(_escalaVeiculoDetalhe, "knockoutEscalaVeiculoDetalhe");

    loadGridEscalaVeiculoDetalhe();
    _modalEscalaVeiculoDetalhe = new bootstrap.Modal(document.getElementById("divModalEscalaVeiculoDetalhe"), { backdrop: true, keyboard: true });
}

function loadGridEscalaVeiculoDetalhe() {
    _gridEscalaVeiculoDetalhes = new GridView("grid-detalhes-escala-veiculo", "EscalaVeiculo/PesquisaDetalhes", _escalaVeiculoDetalhe, null, { column: 1, dir: orderDir.desc });
}

/*
 * Declaração das Funções Públicas
 */

function exibirEscalaVeiculoDetalhe(registroSelecionado) {
    _escalaVeiculoDetalhe.Codigo.val(registroSelecionado.Codigo);
    _escalaVeiculoDetalhe.SituacaoAtual.val(registroSelecionado.SituacaoDescricao);
    _escalaVeiculoDetalhe.Veiculo.val(registroSelecionado.Placa);

    _gridEscalaVeiculoDetalhes.CarregarGrid();

    exibirModalEscalaVeiculoDetalhe();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalEscalaVeiculoDetalhe() {
    _modalEscalaVeiculoDetalhe.show();
    $("#divModalEscalaVeiculoDetalhe").one('hidden.bs.modal', function () {
        LimparCampos(_escalaVeiculoDetalhe);
    });
}
