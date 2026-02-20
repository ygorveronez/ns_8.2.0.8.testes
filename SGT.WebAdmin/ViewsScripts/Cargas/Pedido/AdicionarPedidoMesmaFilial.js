/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../Enumeradores/EnumNumeroReboque.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoPedido.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAdicionarPedidoMesmaFilial;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAdicionarPedidoMesmaFilial = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroReboque = PropertyEntity({ val: ko.observable(EnumNumeroReboque.SemReboque), options: EnumNumeroReboque.obterOpcoes(), def: EnumNumeroReboque.SemReboque, text: "*Número do Reboque: ", required: false, visible: ko.observable(false) });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.Normal), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal, text: "*Tipo do Carregamento: ", required: false, visible: ko.observable(false) });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pedido:", idBtnSearch: guid(), enable: ko.observable(true), eventChange: pedidoAdicionarPedidoMesmaFilialBlur });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPedidoMesmaFilialDetalhePedidoClick, type: types.event, text: "Adicionar", enable: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAdicionarPedidoMesmaFilial() {
    _detalhePedidoAdicionarPedidoMesmaFilial = new DetalhePedidoAdicionarPedidoMesmaFilial();
    KoBindings(_detalhePedidoAdicionarPedidoMesmaFilial, "divModalDetalhesPedidoAdicionarPedidoMesmaFilial");

    new BuscarPedidosDisponiveis(_detalhePedidoAdicionarPedidoMesmaFilial.Pedido, retornoConsultaDetalhePedidoAdicionarPedidoMesmaFilial, null, _detalhePedidoContainer.Filial);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarPedidoMesmaFilialDetalhePedidoClick() {
    if (!ValidarCamposObrigatorios(_detalhePedidoAdicionarPedidoMesmaFilial)) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    var data = {
        CodigosPedidos: JSON.stringify([_detalhePedidoAdicionarPedidoMesmaFilial.Pedido.codEntity()]),
        CodigoCarga: _detalhePedidoAdicionarPedidoMesmaFilial.CodigoCarga.val(),
        NumeroReboque: _detalhePedidoAdicionarPedidoMesmaFilial.NumeroReboque.val(),
        TipoCarregamentoPedido: _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.val(),
        PermitirSomentePedidoMesmaFilial: true
    };

    adicionarPedidoMesmaFilialCargaDetalhePedido(data);
}

function pedidoAdicionarPedidoMesmaFilialBlur() {
    if (_detalhePedidoAdicionarPedidoMesmaFilial.Pedido.val() == "") {
        _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
        _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.visible(false);
        _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.required = false;
    }
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function adicionarPedidoMesmaFilialDetalhePedido(codigoCarga, exigirDefinicaoReboquePedido) {
    _detalhePedidoAdicionarPedidoMesmaFilial.CodigoCarga.val(codigoCarga);
    _detalhePedidoAdicionarPedidoMesmaFilial.NumeroReboque.visible(exigirDefinicaoReboquePedido);
    _detalhePedidoAdicionarPedidoMesmaFilial.NumeroReboque.required = exigirDefinicaoReboquePedido;
    _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.visible(false);
    _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.required = false;

    exibirModalAdicionarPedidoMesmaFilialDetalhePedido();
}

// #endregion Funções Públicas

// #region Funções Privadas

function adicionarPedidoMesmaFilialCargaDetalhePedido(data) {
    executarReST("Carga/AdicionarPedidoCarga", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.ConfirmarSeparacaoMercadoriaInformada) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.MensagemErro + " " + Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
                        adicionarPedidoMesmaFilialCargaDetalhePedido({
                            CodigosPedidos: data.CodigosPedidos,
                            CodigoCarga: data.CodigoCarga,
                            NumeroReboque: data.NumeroReboque,
                            TipoCarregamentoPedido: data.TipoCarregamentoPedido,
                            PermitirSomentePedidoMesmaFilial: data.PermitirSomentePedidoMesmaFilial,
                            PermitirSeparacaoMercadoriaInformada: true,
                            PermitirAlteracoesPedidos: data.PermitirAlteracoesPedidos
                        });
                    })
                }
                else if (retorno.Data.ConfirmarAlteracoesPedidos) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.MensagemErro + " " + Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
                        adicionarPedidoMesmaFilialCargaDetalhePedido({
                            CodigosPedidos: data.CodigosPedidos,
                            CodigoCarga: data.CodigoCarga,
                            NumeroReboque: data.NumeroReboque,
                            TipoCarregamentoPedido: data.TipoCarregamentoPedido,
                            PermitirSomentePedidoMesmaFilial: data.PermitirSomentePedidoMesmaFilial,
                            PermitirSeparacaoMercadoriaInformada: data.PermitirSeparacaoMercadoriaInformada,
                            PermitirAlteracoesPedidos: true
                        });
                    })
                }
                else {
                    fecharModalAdicionarPedidoMesmaFilialDetalhePedido();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pedido adicionado com sucesso");
                    exibirDetalhesPedidos(data.CodigoCarga);
                    IniciarBindKnoutCarga(_cargaAtual, retorno.Data);
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirModalAdicionarPedidoMesmaFilialDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoAdicionarPedidoMesmaFilial');
    $("#divModalDetalhesPedidoAdicionarPedidoMesmaFilial").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAdicionarPedidoMesmaFilial);
    });
}

function fecharModalAdicionarPedidoMesmaFilialDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoAdicionarPedidoMesmaFilial');
}

function retornoConsultaDetalhePedidoAdicionarPedidoMesmaFilial(pedidoSelecionado) {
    _detalhePedidoAdicionarPedidoMesmaFilial.Pedido.codEntity(pedidoSelecionado.Codigo);
    _detalhePedidoAdicionarPedidoMesmaFilial.Pedido.val(pedidoSelecionado.Descricao);
    _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.visible(_CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedidoSelecionado.PedidoDestinadoAFilial);
    _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.required = _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.visible();

    if (!_detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.visible())
        _detalhePedidoAdicionarPedidoMesmaFilial.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
}

// #endregion Funções Privadas
