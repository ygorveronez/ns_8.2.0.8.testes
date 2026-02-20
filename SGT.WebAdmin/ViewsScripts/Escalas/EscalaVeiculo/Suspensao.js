/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/MotivoRemocaoVeiculoEscala.js" />
/// <reference path="EscalaVeiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _escalaVeiculoSuspensao;
var _modalEscalaVeiculoSuspensao;

/*
 * Declaração das Classes
 */

var EscalaVeiculoSuspensao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataPrevisaoRetorno = PropertyEntity({ text: "Data Prevista de Retorno: ", getType: typesKnockout.date });
    this.MotivoRemocao = PropertyEntity({ text: "*Motivo da Suspensão:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });

    this.Cancelar = PropertyEntity({ eventClick: cancelarSuspensaoEscalaVeiculoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Suspender = PropertyEntity({ eventClick: suspenderEscalaVeiculoClick, type: types.event, text: "Suspender", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEscalaVeiculoSuspensao() {
    _escalaVeiculoSuspensao = new EscalaVeiculoSuspensao();
    KoBindings(_escalaVeiculoSuspensao, "knockoutEscalaVeiculoSuspensao");

    new BuscarMotivoRemocaoVeiculoEscala(_escalaVeiculoSuspensao.MotivoRemocao);
    _modalEscalaVeiculoSuspensao = new bootstrap.Modal(document.getElementById("divModalEscalaVeiculoSuspensao"), { backdrop: true, keyboard: true });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function cancelarSuspensaoEscalaVeiculoClick() {
    fecharModalEscalaVeiculoSuspensao();
}

function suspenderEscalaVeiculoClick() {
    if (!ValidarCamposObrigatorios(_escalaVeiculoSuspensao)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    executarReST("EscalaVeiculo/Suspender", RetornarObjetoPesquisa(_escalaVeiculoSuspensao), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Veículo suspenso da escala com sucesso");
                fecharModalEscalaVeiculoSuspensao();
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

function suspenderEscalaVeiculo(registroSelecionado) {
    _escalaVeiculoSuspensao.Codigo.val(registroSelecionado.Codigo);
    _escalaVeiculoSuspensao.DataPrevisaoRetorno.minDate(Global.DataAtual());

    exibirModalEscalaVeiculoSuspensao();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalEscalaVeiculoSuspensao() {
    _modalEscalaVeiculoSuspensao.show();
    $("#divModalEscalaVeiculoSuspensao").one('hidden.bs.modal', function () {
        LimparCampos(_escalaVeiculoSuspensao);
    });
}

function fecharModalEscalaVeiculoSuspensao() {
    _modalEscalaVeiculoSuspensao.hide();
}
