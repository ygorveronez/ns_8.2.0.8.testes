/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="EscalaVeiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _escalaVeiculoAdicionar;
var _modalEscalaVeiculoAdicionar;

/*
 * Declaração das Classes
 */

var EscalaVeiculoAdicionar = function () {
    this.Veiculo = PropertyEntity({ text: "*Veiculo", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });

    this.Cancelar = PropertyEntity({ eventClick: cancelarAdicaoEscalaVeiculoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarEscalaVeiculoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEscalaVeiculoAdicionar() {
    _escalaVeiculoAdicionar = new EscalaVeiculoAdicionar();
    KoBindings(_escalaVeiculoAdicionar, "knockoutEscalaVeiculoAdicionar");

    new BuscarVeiculos(_escalaVeiculoAdicionar.Veiculo);
    _modalEscalaVeiculoAdicionar = new bootstrap.Modal(document.getElementById("divModalEscalaVeiculoAdicionar"), { backdrop: true, keyboard: true });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function cancelarAdicaoEscalaVeiculoClick() {
    fecharModalEscalaVeiculoAdicionar();
}

function adicionarEscalaVeiculoClick() {
    if (!ValidarCamposObrigatorios(_escalaVeiculoAdicionar)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    executarReST("EscalaVeiculo/Adicionar", RetornarObjetoPesquisa(_escalaVeiculoAdicionar), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Veículo adicionado na escala com sucesso");
                fecharModalEscalaVeiculoAdicionar();
                recarregarGridEscalaVeiculo();
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

function adicionarEscalaVeiculo() {
    exibirModalEscalaVeiculoAdicionar();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalEscalaVeiculoAdicionar() {
    _modalEscalaVeiculoAdicionar.show();
    $("#divModalEscalaVeiculoAdicionar").one('hidden.bs.modal', function () {
        LimparCampos(_escalaVeiculoAdicionar);
    });
}

function fecharModalEscalaVeiculoAdicionar() {
    _modalEscalaVeiculoAdicionar.hide();
}
