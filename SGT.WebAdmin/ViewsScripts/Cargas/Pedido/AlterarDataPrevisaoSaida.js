/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAlterarPrevisaoSaida;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAlterarPrevisaoSaida = function () {
    this.PedidoAlterar = undefined;
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataPrevisaoSaida = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NovaPrevisaoDeSaida.getFieldDescription(), getType: typesKnockout.dateTime, required: true });
    this.Responsavel = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Responsavel.getFieldDescription(), required: ko.observable(false), visible: ko.observable(false), val: ko.observable(EnumResponsavelAlteracaoDataPedido.Embarcador), options: EnumResponsavelAlteracaoDataPedido.obterOpcoes(), def: EnumResponsavelAlteracaoDataPedido.Embarcador });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Observacao.getFieldDescription(), getType: typesKnockout.string, required: ko.observable(false), visible: ko.observable(false), val: ko.observable("") });
    this.Alterar = PropertyEntity({ type: types.event, eventClick: alterarDataPrevisaoSaidaDetalhePedidoClick, text: Localization.Resources.Gerais.Geral.Alterar, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAlterarPrevisaoSaida() {

    LoadLocalizationResources("Cargas.Carga").then(function () {

        _detalhePedidoAlterarPrevisaoSaida = new DetalhePedidoAlterarPrevisaoSaida();
        var habilitado = _CONFIGURACAO_TMS.NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga;
        _detalhePedidoAlterarPrevisaoSaida.Responsavel.visible(habilitado);
        _detalhePedidoAlterarPrevisaoSaida.Responsavel.required(habilitado);
        _detalhePedidoAlterarPrevisaoSaida.Observacao.visible(habilitado);
        _detalhePedidoAlterarPrevisaoSaida.Observacao.required(habilitado);
        KoBindings(_detalhePedidoAlterarPrevisaoSaida, "knoutDetalhesPedidoAlterarPrevisaoSaida");

        _detalhePedidoAlterarPrevisaoSaida.DataPrevisaoSaida.minDate(Global.DataHoraAtual());

        LocalizeCurrentPage();
    })
};

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function alterarDataPrevisaoSaidaDetalhePedidoClick() {
    Salvar(_detalhePedidoAlterarPrevisaoSaida, "Carga/AlterarDataPrevisaoSaidaPedido", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DataAlteradaComSucesso);
                _detalhePedidoAlterarPrevisaoSaida.PedidoAlterar.DataPrevisaoSaida.val(_detalhePedidoAlterarPrevisaoSaida.DataPrevisaoSaida.val());
                fecharModalAlterarDataPrevisaoSaidaDetalhePedido();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function alterarDataPrevisaoSaidaDetalhePedido(e) {
    _detalhePedidoAlterarPrevisaoSaida.Pedido.val(e.Codigo.val());
    _detalhePedidoAlterarPrevisaoSaida.DataPrevisaoSaida.val(e.DataPrevisaoSaida.val());
    _detalhePedidoAlterarPrevisaoSaida.PedidoAlterar = e;
    exibirModalAlterarDataPrevisaoSaidaDetalhePedido();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalAlterarDataPrevisaoSaidaDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoAlterarPrevisaoSaida');
    $("#divModalDetalhesPedidoAlterarPrevisaoSaida").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAlterarPrevisaoSaida);
        _detalhePedidoAlterarPrevisaoSaida.PedidoAlterar = undefined;
    });
}

function fecharModalAlterarDataPrevisaoSaidaDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoAlterarPrevisaoSaida');
}

// #endregion Funções Públicas
