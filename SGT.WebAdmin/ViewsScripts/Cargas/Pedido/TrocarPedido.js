/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoPedido.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoTrocarPedido;
var _detalhePedidoTrocarPedidoAdicionar;
var _gridDetalhePedidoTrocarPedido;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoTrocarPedido = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pedido:", required: true, visible: ko.observable(true), idBtnSearch: guid(), eventChange: pedidoTrocarPedidoBlur });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.Normal), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal, text: "*Tipo do Carregamento: ", required: false, visible: ko.observable(false) });

    this.AdicionarPedidoTroca = PropertyEntity({ eventClick: adicionarPedidoTrocaPedidoClick, type: types.event, text: "Adicionar" });
    this.Trocar = PropertyEntity({ eventClick: trocarPedidoDetalhePedidoClick, type: types.event, text: "Trocar", enable: ko.observable(true) });
}

var DetalhePedidoTrocarPedidoAdicionar = function () {
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pedido:", required: true, visible: ko.observable(true), idBtnSearch: guid(), eventChange: pedidoTrocarPedidoAdicionarBlur, enable: ko.observable(true) });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.Normal), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal, text: "*Tipo do Carregamento: ", required: false, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPedidoTrocaPedidoAdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPedidoTrocaPedidoAdicionarClick, type: types.event, text: "Trocar", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoTrocarPedido() {
    _detalhePedidoTrocarPedido = new DetalhePedidoTrocarPedido();
    KoBindings(_detalhePedidoTrocarPedido, "divModalDetalhesPedidoTrocarPedido");

    _detalhePedidoTrocarPedidoAdicionar = new DetalhePedidoTrocarPedidoAdicionar();
    KoBindings(_detalhePedidoTrocarPedidoAdicionar, "divModalDetalhesPedidoTrocarPedidoAdicionar");

    new BuscarPedidosParaTroca(_detalhePedidoTrocarPedido.Pedido, retornoConsultaDetalhePedidoTrocarPedido, undefined, undefined, _detalhePedidoContainer.Filial);
    new BuscarPedidosParaTroca(_detalhePedidoTrocarPedidoAdicionar.Pedido, retornoConsultaDetalhePedidoTrocarPedidoAdicionar, undefined, undefined, _detalhePedidoContainer.Filial);

    loadGridDetalhePedidoTrocarPedido();
}

function loadGridDetalhePedidoTrocarPedido() {
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarPedidoTrocaPedidoClick, icone: "", visibilidade: isPermitirEditarDetalhePedidoTrocarPedidoAdicionar };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: excluirPedidoTrocaPedidoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoEditar, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "ExigirDefinicaoTipoCarregamentoPedido", visible: false },
        { data: "TipoCarregamentoPedido", visible: false },
        { data: "Descricao", title: "Descrição", width: "50%" },
        { data: "TipoCarregamentoPedidoDescricao", title: "Tipo do Carregamento", className: "text-align-center", width: "30%", visible: _CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido }
    ];

    _gridDetalhePedidoTrocarPedido = new BasicDataTable("grid-detalhes-pedido-trocar-pedido", header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridDetalhePedidoTrocarPedido.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarPedidoTrocaPedidoClick() {
    _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.visible(false);
    _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.required = false;
    _detalhePedidoTrocarPedidoAdicionar.Pedido.enable(true);

    _detalhePedidoTrocarPedidoAdicionar.Adicionar.visible(true);
    _detalhePedidoTrocarPedidoAdicionar.Atualizar.visible(false);

    exibirModalTrocarPedidoDetalhePedidoAdicionar();
}

function adicionarPedidoTrocaPedidoAdicionarClick() {
    var listaPedidoTroca = obterListaPedidoTrocarPedido();
    var pedidoDuplicado = false;

    for (var i = 0; i < listaPedidoTroca.length; i++) {
        if (listaPedidoTroca[i].Codigo == _detalhePedidoTrocarPedidoAdicionar.Pedido.codEntity()) {
            pedidoDuplicado = true;
            break;
        }
    }

    if (pedidoDuplicado) {
        exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "O pedido informado já está adicionado na lista de troca");
        return;
    }

    listaPedidoTroca.push({
        Codigo: _detalhePedidoTrocarPedidoAdicionar.Pedido.codEntity(),
        ExigirDefinicaoTipoCarregamentoPedido: _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.visible(),
        TipoCarregamentoPedido: _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.val(),
        TipoCarregamentoPedidoDescricao: EnumTipoCarregamentoPedido.obterDescricao(_detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.val()),
        Descricao: _detalhePedidoTrocarPedidoAdicionar.Pedido.val(),
    });

    _gridDetalhePedidoTrocarPedido.CarregarGrid(listaPedidoTroca);
    fecharModalTrocarPedidoDetalhePedidoAdicionar();
}

function atualizarPedidoTrocaPedidoAdicionarClick() {
    var listaPedidoTroca = obterListaPedidoTrocarPedido();

    for (var i = 0; i < listaPedidoTroca.length; i++) {
        var pedidoTroca = listaPedidoTroca[i];

        if (pedidoTroca.Codigo == _detalhePedidoTrocarPedidoAdicionar.Pedido.codEntity()) {
            pedidoTroca.ExigirDefinicaoTipoCarregamentoPedido = _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.visible();
            pedidoTroca.TipoCarregamentoPedido = _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.val();
            pedidoTroca.TipoCarregamentoPedidoDescricao = EnumTipoCarregamentoPedido.obterDescricao(_detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.val());
            break;
        }
    }

    _gridDetalhePedidoTrocarPedido.CarregarGrid(listaPedidoTroca);
    fecharModalTrocarPedidoDetalhePedidoAdicionar();
}

function editarPedidoTrocaPedidoClick(registroSelecionado) {
    var pedidoTroca = obterPedidoTrocaPedidoPorCodigo(registroSelecionado.Codigo);

    if (!pedidoTroca)
        return;

    _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.val(pedidoTroca.TipoCarregamentoPedido);
    _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.visible(pedidoTroca.ExigirDefinicaoTipoCarregamentoPedido);
    _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.required = pedidoTroca.ExigirDefinicaoTipoCarregamentoPedido;
    _detalhePedidoTrocarPedidoAdicionar.Pedido.codEntity(pedidoTroca.Codigo);
    _detalhePedidoTrocarPedidoAdicionar.Pedido.val(pedidoTroca.Descricao);
    _detalhePedidoTrocarPedidoAdicionar.Pedido.enable(false);

    _detalhePedidoTrocarPedidoAdicionar.Adicionar.visible(false);
    _detalhePedidoTrocarPedidoAdicionar.Atualizar.visible(true);

    exibirModalTrocarPedidoDetalhePedidoAdicionar();
}

function excluirPedidoTrocaPedidoClick(registroSelecionado) {
    var listaPedidoTroca = obterListaPedidoTrocarPedido();

    for (var i = 0; i < listaPedidoTroca.length; i++) {
        if (registroSelecionado.Codigo == listaPedidoTroca[i].Codigo) {
            listaPedidoTroca.splice(i, 1);
            break;
        }
    }

    _gridDetalhePedidoTrocarPedido.CarregarGrid(listaPedidoTroca);
}

function pedidoTrocarPedidoBlur() {
    if (_detalhePedidoTrocarPedido.Pedido.val() == "") {
        _detalhePedidoTrocarPedido.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
        _detalhePedidoTrocarPedido.TipoCarregamentoPedido.visible(false);
        _detalhePedidoTrocarPedido.TipoCarregamentoPedido.required = false;
    }
}

function pedidoTrocarPedidoAdicionarBlur() {
    if (_detalhePedidoTrocarPedidoAdicionar.Pedido.val() == "") {
        _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
        _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.visible(false);
        _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.required = false;
    }
}

function trocarPedidoDetalhePedidoClick() {
    var listaPedidoTroca = obterListaPedidoTrocarPedidoSalvar();

    if (!listaPedidoTroca)
        return;

    exibirConfirmacao("Confirmação", "Deseja realmente realizar a troca do pedido?", function () {
        var data = {
            CodigoPedido: _detalhePedidoTrocarPedido.CodigoPedido.val(),
            CodigoCarga: _detalhePedidoTrocarPedido.CodigoCarga.val(),
            PedidosTroca: JSON.stringify(listaPedidoTroca)
        };

        executarReST("Carga/TrocarPedidoCarga", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalTrocarPedidoDetalhePedido();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pedido trocado com sucesso");
                    exibirDetalhesPedidos(data.CodigoCarga);
                    IniciarBindKnoutCarga(_cargaAtual, retorno.Data);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function desfazerTrocaPedidoDetalhePedido(codigoCarga, codigoPedido) {
    exibirConfirmacao("Confirmação", "Deseja realmente desfazer a troca do pedido?", function () {
        var data = {
            CodigoPedido: codigoPedido,
            CodigoCarga: codigoCarga
        };

        executarReST("Carga/DesfazerTrocaPedidoCarga", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Troca de pedido revertida com sucesso");
                    exibirDetalhesPedidos(data.CodigoCarga);
                    IniciarBindKnoutCarga(_cargaAtual, retorno.Data);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function trocarPedidoDetalhePedido(codigoCarga, codigoPedido, permitirTrocarMultiplosPedidos) {
    _detalhePedidoTrocarPedido.CodigoCarga.val(codigoCarga);
    _detalhePedidoTrocarPedido.CodigoPedido.val(codigoPedido);
    _detalhePedidoTrocarPedido.Pedido.visible(!permitirTrocarMultiplosPedidos);
    _detalhePedidoTrocarPedido.TipoCarregamentoPedido.visible(false);
    _detalhePedidoTrocarPedido.TipoCarregamentoPedido.required = false;

    if (permitirTrocarMultiplosPedidos)
        $("#divModalDetalhesPedidoTrocarPedido .modal-dialog").removeClass("modal-sm");
    else
        $("#divModalDetalhesPedidoTrocarPedido .modal-dialog").addClass("modal-sm");

    exibirModalTrocarPedidoDetalhePedido();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalTrocarPedidoDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoTrocarPedido');
    $("#divModalDetalhesPedidoTrocarPedido").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoTrocarPedido);
        _gridDetalhePedidoTrocarPedido.CarregarGrid(new Array());
    });
}

function exibirModalTrocarPedidoDetalhePedidoAdicionar() {
    Global.abrirModal('divModalDetalhesPedidoTrocarPedidoAdicionar');
    $("#divModalDetalhesPedidoTrocarPedidoAdicionar").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoTrocarPedidoAdicionar);
    });
}

function fecharModalTrocarPedidoDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoTrocarPedido');
}

function fecharModalTrocarPedidoDetalhePedidoAdicionar() {
    Global.fecharModal('divModalDetalhesPedidoTrocarPedidoAdicionar');
}

function isPermitirEditarDetalhePedidoTrocarPedidoAdicionar(registroSelecionado) {
    return registroSelecionado.ExigirDefinicaoTipoCarregamentoPedido;
}

function obterListaPedidoTrocarPedido() {
    return _gridDetalhePedidoTrocarPedido.BuscarRegistros().slice();
}

function obterListaPedidoTrocarPedidoSalvar() {
    if (_detalhePedidoTrocarPedido.Pedido.visible()) {
        var campoPedidoNaoInformado = !ValidarCampoObrigatorioEntity(_detalhePedidoTrocarPedido.Pedido);
        var campoTipoCarregamentoPedidoNaoInformado = !ValidarCampoObrigatorioMap(_detalhePedidoTrocarPedido.TipoCarregamentoPedido);

        if (campoPedidoNaoInformado || campoTipoCarregamentoPedidoNaoInformado) {
            exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, selecione um pedido para a troca");
            return undefined;
        }

        return [
            {
                Pedido: _detalhePedidoTrocarPedido.Pedido.codEntity(),
                TipoCarregamentoPedido: _detalhePedidoTrocarPedido.TipoCarregamentoPedido.val()
            }
        ];
    }

    var listaPedidoTroca = obterListaPedidoTrocarPedido();
    var listaPedidoTrocaSalvar = new Array();

    for (var i = 0; i < listaPedidoTroca.length; i++)
        listaPedidoTrocaSalvar.push({
            Pedido: listaPedidoTroca[i].Codigo,
            TipoCarregamentoPedido: listaPedidoTroca[i].TipoCarregamentoPedido
        });

    if (listaPedidoTroca.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, selecione um ou mais pedidos para a troca");
        return undefined;
    }

    return listaPedidoTrocaSalvar;
}

function obterPedidoTrocaPedidoPorCodigo(codigoPedido) {
    var listaPedidoTroca = obterListaPedidoTrocarPedido();

    for (var i = 0; i < listaPedidoTroca.length; i++) {
        if (codigoPedido == listaPedidoTroca[i].Codigo)
            return listaPedidoTroca[i];
    }

    return undefined;
}

function retornoConsultaDetalhePedidoTrocarPedido(pedidoSelecionado) {
    _detalhePedidoTrocarPedido.Pedido.codEntity(pedidoSelecionado.Codigo);
    _detalhePedidoTrocarPedido.Pedido.val(pedidoSelecionado.Descricao);
    _detalhePedidoTrocarPedido.TipoCarregamentoPedido.visible(_CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedidoSelecionado.PedidoDestinadoAFilial);
    _detalhePedidoTrocarPedido.TipoCarregamentoPedido.required = _detalhePedidoTrocarPedido.TipoCarregamentoPedido.visible();

    if (!_detalhePedidoTrocarPedido.TipoCarregamentoPedido.visible())
        _detalhePedidoTrocarPedido.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
}

function retornoConsultaDetalhePedidoTrocarPedidoAdicionar(pedidoSelecionado) {
    _detalhePedidoTrocarPedidoAdicionar.Pedido.codEntity(pedidoSelecionado.Codigo);
    _detalhePedidoTrocarPedidoAdicionar.Pedido.val(pedidoSelecionado.Descricao);
    _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.visible(_CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedidoSelecionado.PedidoDestinadoAFilial);
    _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.required = _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.visible();

    if (!_detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.visible())
        _detalhePedidoTrocarPedidoAdicionar.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
}

// #endregion Funções Privadas
