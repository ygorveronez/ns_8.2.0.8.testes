/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../Enumeradores/EnumNumeroReboque.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoPedido.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAdicionarPedido;
var _adicionarPorNumeroCarregamentoPedido;
var _gridAdicionarPorNumeroCarregamentoPedido;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAdicionarPedido = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroReboque = PropertyEntity({ val: ko.observable(EnumNumeroReboque.SemReboque), options: EnumNumeroReboque.obterOpcoes(), def: EnumNumeroReboque.SemReboque, text: "*Número do Reboque: ", required: false, visible: ko.observable(false) });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.Normal), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal, text: "*Tipo do Carregamento: ", required: false, visible: ko.observable(false) });
    this.Pedido = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: true, text: "*Pedido:", idBtnSearch: guid(), enable: ko.observable(true), eventChange: pedidoAdicionarPedidoBlur });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPedidoDetalhePedidoClick, type: types.event, text: "Adicionar", enable: ko.observable(true) });
    this.BotaoAdicionarPorNumeroCarregamentoPedido = PropertyEntity({ eventClick: AbrirModalAdicionarPorNumeroCarregamentoPedido, type: types.event, text: "Adicionar Por Nº do Carregamento do Pedido", visible: ko.observable(false) });
}

var AdicionarPorNumeroCarregamentoPedido = function () {
    this.CodigoPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AdicionarMultiplosPedidos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.NumeroCarregamentoPedido = PropertyEntity({ text: "Nº Carregamento do Pedido: ", visible: ko.observable(true), val: ko.observable(""), def: "", required: ko.observable(true) });
    this.GridListaPedidos = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({ eventClick: PesquisarPedidoPorNumeroCarregamentoPedido, type: types.event, text: "Pesquisar", enable: ko.observable(true) });

    this.Cancelar = PropertyEntity({ eventClick: CancelarAdicionarPorNumeroCarregamentoPedido, type: types.event, text: "Cancelar", enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarPorNumeroCarregamento, type: types.event, text: "Adicionar 50 Primeiros", enable: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAdicionarPedido() {
    _detalhePedidoAdicionarPedido = new DetalhePedidoAdicionarPedido();
    KoBindings(_detalhePedidoAdicionarPedido, "divModalDetalhesPedidoAdicionarPedido");

    _adicionarPorNumeroCarregamentoPedido = new AdicionarPorNumeroCarregamentoPedido();
    KoBindings(_adicionarPorNumeroCarregamentoPedido, "knockoutAdicionarPorNumeroCarregamentoPedido");

    new BuscarPedidosDisponiveis(_detalhePedidoAdicionarPedido.Pedido, retornoConsultaDetalhePedidoAdicionarPedido, null, null, null, _detalhePedidoContainer.PermitirAdicionarPedidosNaEtapaUm);
}

function CarregarGridAdicionarPorNumeroCarregamento() {
    _gridAdicionarPorNumeroCarregamentoPedido = new GridView(_adicionarPorNumeroCarregamentoPedido.GridListaPedidos.idGrid, "Pedido/PesquisaDisponiveis", _adicionarPorNumeroCarregamentoPedido, null, null, 100, null, null, null, null, 100);
    _gridAdicionarPorNumeroCarregamentoPedido.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarPedidoDetalhePedidoClick() {
    if (!ValidarCamposObrigatorios(_detalhePedidoAdicionarPedido)) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    var data = {
        CodigosPedidos: JSON.stringify(_detalhePedidoAdicionarPedido.Pedido.multiplesEntities().map((x) => x.Codigo)),
        CodigoCarga: _detalhePedidoAdicionarPedido.CodigoCarga.val(),
        NumeroReboque: _detalhePedidoAdicionarPedido.NumeroReboque.val(),
        TipoCarregamentoPedido: _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.val()
    };

    adicionarPedidoCargaDetalhePedido(data);
}

function pedidoAdicionarPedidoBlur() {
    if (_detalhePedidoAdicionarPedido.Pedido.val() == "") {
        _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
        _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.visible(false);
        _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.required = false;
    }
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function adicionarPedidoDetalhePedido(codigoCarga, exigirDefinicaoReboquePedido) {
    _detalhePedidoAdicionarPedido.CodigoCarga.val(codigoCarga);
    _detalhePedidoAdicionarPedido.NumeroReboque.visible(exigirDefinicaoReboquePedido);
    _detalhePedidoAdicionarPedido.NumeroReboque.required = exigirDefinicaoReboquePedido;
    _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.visible(false);
    _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.required = false;

    exibirModalAdicionarPedidoDetalhePedido();
}

// #endregion Funções Públicas

// #region Funções Privadas

function adicionarPedidoCargaDetalhePedido(data) {
    if (_adicionarPorNumeroCarregamentoPedido != null && _adicionarPorNumeroCarregamentoPedido.CodigoCarga.val() == 0)
        _adicionarPorNumeroCarregamentoPedido.CodigoCarga.val(data.CodigoCarga);

    if (data.CodigoCarga == 0 && _adicionarPorNumeroCarregamentoPedido != null && _adicionarPorNumeroCarregamentoPedido.CodigoCarga.val() > 0)
        data.CodigoCarga = _adicionarPorNumeroCarregamentoPedido.CodigoCarga.val();

    executarReST("Carga/AdicionarPedidoCarga", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.ConfirmarSeparacaoMercadoriaInformada) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.MensagemErro + " " + Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
                        adicionarPedidoCargaDetalhePedido({
                            CodigosPedidos: data.CodigosPedidos,
                            CodigoCarga: data.CodigoCarga,
                            NumeroReboque: data.NumeroReboque,
                            TipoCarregamentoPedido: data.TipoCarregamentoPedido,
                            PermitirSeparacaoMercadoriaInformada: true,
                            PermitirAlteracoesPedidos: data.PermitirAlteracoesPedidos
                        });
                    })
                }
                else if (retorno.Data.ConfirmarAlteracoesPedidos) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.MensagemErro + " " + Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
                        adicionarPedidoCargaDetalhePedido({
                            CodigosPedidos: data.CodigosPedidos,
                            CodigoCarga: data.CodigoCarga,
                            NumeroReboque: data.NumeroReboque,
                            TipoCarregamentoPedido: data.TipoCarregamentoPedido,
                            PermitirSeparacaoMercadoriaInformada: data.PermitirSeparacaoMercadoriaInformada,
                            PermitirAlteracoesPedidos: true
                        });
                    })
                }
                else {
                    fecharModalAdicionarPedidoDetalhePedido();
                    _adicionarPorNumeroCarregamentoPedido.NumeroCarregamentoPedido.val("");
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pedido adicionado com sucesso");
                    exibirDetalhesPedidos(data.CodigoCarga);
                    IniciarBindKnoutCarga(_cargaAtual, retorno.Data);
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            

            if (_gridAdicionarPorNumeroCarregamentoPedido != null)
                _gridAdicionarPorNumeroCarregamentoPedido.CarregarGrid();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirModalAdicionarPedidoDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoAdicionarPedido');
    $("#divModalDetalhesPedidoAdicionarPedido").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAdicionarPedido);
    });
}

function fecharModalAdicionarPedidoDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoAdicionarPedido');
}

function retornoConsultaDetalhePedidoAdicionarPedido(pedidoSelecionado) {
    _detalhePedidoAdicionarPedido.Pedido.codEntity(pedidoSelecionado.Codigo);
    _detalhePedidoAdicionarPedido.Pedido.val(pedidoSelecionado.Descricao);
    _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.visible(_CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedidoSelecionado.PedidoDestinadoAFilial);
    _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.required = _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.visible();

    if (!_detalhePedidoAdicionarPedido.TipoCarregamentoPedido.visible())
        _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
}

function AbrirModalAdicionarPorNumeroCarregamentoPedido() {
    Global.abrirModal('divModalAdicionarPorNumeroCarregamentoPedido');
    CarregarGridAdicionarPorNumeroCarregamento();
}

function CancelarAdicionarPorNumeroCarregamentoPedido() {
    LimparCampos(_adicionarPorNumeroCarregamentoPedido);
    Global.fecharModal('divModalAdicionarPorNumeroCarregamentoPedido');
}

function PesquisarPedidoPorNumeroCarregamentoPedido() {
    CarregarGridAdicionarPorNumeroCarregamento();
}

function AdicionarPorNumeroCarregamento() {
    if (!ValidarCamposObrigatorios(_adicionarPorNumeroCarregamentoPedido)) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    var data = {
        NumeroCarregamentoPedido: _adicionarPorNumeroCarregamentoPedido.NumeroCarregamentoPedido.val(),
        CodigosPedidos: JSON.stringify(_detalhePedidoAdicionarPedido.Pedido.multiplesEntities().map((x) => x.Codigo)),
        CodigoCarga: _detalhePedidoAdicionarPedido.CodigoCarga.val(),
        NumeroReboque: _detalhePedidoAdicionarPedido.NumeroReboque.val(),
        TipoCarregamentoPedido: _detalhePedidoAdicionarPedido.TipoCarregamentoPedido.val()
    };

    adicionarPedidoCargaDetalhePedido(data);
}

// #endregion Funções Privadas
