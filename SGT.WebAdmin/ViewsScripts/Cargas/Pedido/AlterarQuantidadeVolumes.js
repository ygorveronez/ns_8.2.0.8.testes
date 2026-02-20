/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAlterarQuantidadeVolumes;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAlterarQuantidadeVolumes = function () {
    this.PedidoAlterar = undefined;
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.QuantidadeVolumes = PropertyEntity({ text: "Nova quantidade de volumes: ", required: true, getType: typesKnockout.int });

    this.Alterar = PropertyEntity({ type: types.event, eventClick: alterarQuantidadeVolumesDetalhePedidoClick, text: "Alterar", visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAlterarQuantidadeVolumes() {
    _detalhePedidoAlterarQuantidadeVolumes = new DetalhePedidoAlterarQuantidadeVolumes();

    KoBindings(_detalhePedidoAlterarQuantidadeVolumes, "knoutDetalhesPedidoAlterarQuantidadeVolumes");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function alterarQuantidadeVolumesDetalhePedidoClick() {
    Salvar(_detalhePedidoAlterarQuantidadeVolumes, "Carga/AlterarQuantidadeVolumesPedido", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Volumes alterados com sucesso");
                _detalhePedidoAlterarQuantidadeVolumes.PedidoAlterar.QuantidadeVolumes.val(_detalhePedidoAlterarQuantidadeVolumes.QuantidadeVolumes.val());
                fecharModalAlterarQuantidadeVolumesDetalhePedido();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function alterarQuantidadeVolumesDetalhePedido(e) {
    _detalhePedidoAlterarQuantidadeVolumes.Pedido.val(e.Codigo.val());
    _detalhePedidoAlterarQuantidadeVolumes.QuantidadeVolumes.val(e.QuantidadeVolumes.val());
    _detalhePedidoAlterarQuantidadeVolumes.PedidoAlterar = e;

    exibirModalAlterarQuantidadeVolumesDetalhePedido();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalAlterarQuantidadeVolumesDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoAlterarQuantidadeVolumes');
    $("#divModalDetalhesPedidoAlterarQuantidadeVolumes").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAlterarQuantidadeVolumes);
        _detalhePedidoAlterarQuantidadeVolumes.PedidoAlterar = undefined;
    });
}

function fecharModalAlterarQuantidadeVolumesDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoAlterarQuantidadeVolumes');
}

// #endregion Funções Públicas
