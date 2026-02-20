/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../Enumeradores/EnumNumeroReboque.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoPedido.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAdicionarPedidoTroca;
var _gridDetalhePedidoAdicionarPedidoTroca;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAdicionarPedidoTroca = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pedido:", idBtnSearch: guid(), eventChange: pedidoAdicionarPedidoTrocaBlur });
    this.NumeroReboque = PropertyEntity({ val: ko.observable(EnumNumeroReboque.SemReboque), options: EnumNumeroReboque.obterOpcoes(), def: EnumNumeroReboque.SemReboque, text: "*Número do Reboque: ", required: false, visible: ko.observable(false) });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.Normal), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal, text: "*Tipo do Carregamento: ", required: false, visible: ko.observable(false) });
    this.ListaPedidoTroca = PropertyEntity({ type: types.local, text: "Adicionar", getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid(), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPedidoTrocaDetalhePedidoClick, type: types.event, text: "Adicionar", enable: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAdicionarPedidoTroca() {
    _detalhePedidoAdicionarPedidoTroca = new DetalhePedidoAdicionarPedidoTroca();
    KoBindings(_detalhePedidoAdicionarPedidoTroca, "divModalDetalhesPedidoAdicionarPedidoTroca");

    new BuscarPedidosParaTroca(_detalhePedidoAdicionarPedidoTroca.Pedido, retornoConsultaDetalhePedidoAdicionarPedidoTroca, undefined, undefined, _detalhePedidoContainer.Filial);

    loadGridDetalhePedidoAdicionarPedidoTroca();
}

function loadGridDetalhePedidoAdicionarPedidoTroca() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirPedidoAdicionarPedidoTrocaClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridDetalhePedidoAdicionarPedidoTroca = new BasicDataTable(_detalhePedidoAdicionarPedidoTroca.ListaPedidoTroca.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarPedidosParaTroca(_detalhePedidoAdicionarPedidoTroca.ListaPedidoTroca , undefined, _gridDetalhePedidoAdicionarPedidoTroca, _detalhePedidoAdicionarPedidoTroca.CodigoCarga);
    _detalhePedidoAdicionarPedidoTroca.ListaPedidoTroca.basicTable = _gridDetalhePedidoAdicionarPedidoTroca;

    _gridDetalhePedidoAdicionarPedidoTroca.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarPedidoTrocaDetalhePedidoClick() {
    if (!ValidarCamposObrigatorios(_detalhePedidoAdicionarPedidoTroca)) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    var listaPedidoTroca = obterListaPedidoAdicionarPedidoTrocaSalvar();

    if (listaPedidoTroca.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, selecione um ou mais pedidos para a troca");
        return;
    }

    exibirConfirmacao("Confirmação", "Deseja realmente realizar a troca " + (listaPedidoTroca.length == 1 ? " do pedido" : " dos pedidos") + "?", function () {
        var data = {
            CodigoPedido: _detalhePedidoAdicionarPedidoTroca.Pedido.codEntity(),
            CodigoCarga: _detalhePedidoAdicionarPedidoTroca.CodigoCarga.val(),
            NumeroReboque: _detalhePedidoAdicionarPedidoTroca.NumeroReboque.val(),
            TipoCarregamentoPedido: _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.val(),
            PedidosTroca: JSON.stringify(listaPedidoTroca)
        };

        executarReST("Carga/AdicionarPedidoTrocaCarga", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalAdicionarPedidoTrocaDetalhePedido();
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

function excluirPedidoAdicionarPedidoTrocaClick(registroSelecionado) {
    var listaPedidoTroca = obterListaPedidoAdicionarPedidoTroca();

    for (var i = 0; i < listaPedidoTroca.length; i++) {
        if (registroSelecionado.Codigo == listaPedidoTroca [i].Codigo) {
            listaPedidoTroca.splice(i, 1);
            break;
        }
    }

    _gridDetalhePedidoAdicionarPedidoTroca.CarregarGrid(listaPedidoTroca );
}

function pedidoAdicionarPedidoTrocaBlur() {
    if (_detalhePedidoAdicionarPedidoTroca.Pedido.val() == "") {
        _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
        _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.visible(false);
        _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.required = false;
    }
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function adicionarPedidoTrocaDetalhePedido(codigoCarga, exigirDefinicaoReboquePedido) {
    _detalhePedidoAdicionarPedidoTroca.CodigoCarga.val(codigoCarga);
    _detalhePedidoAdicionarPedidoTroca.NumeroReboque.visible(exigirDefinicaoReboquePedido);
    _detalhePedidoAdicionarPedidoTroca.NumeroReboque.required = exigirDefinicaoReboquePedido;
    _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.visible(false);
    _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.required = false;

    exibirModalAdicionarPedidoTrocaDetalhePedido();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalAdicionarPedidoTrocaDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoAdicionarPedidoTroca');
    $("#divModalDetalhesPedidoAdicionarPedidoTroca").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAdicionarPedidoTroca);
        _gridDetalhePedidoAdicionarPedidoTroca.CarregarGrid(new Array());
    });
}

function fecharModalAdicionarPedidoTrocaDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoAdicionarPedidoTroca');
}

function obterListaPedidoAdicionarPedidoTroca() {
    return _gridDetalhePedidoAdicionarPedidoTroca.BuscarRegistros();
}

function obterListaPedidoAdicionarPedidoTrocaSalvar() {
    var listaPedidoTroca = obterListaPedidoAdicionarPedidoTroca();
    var listaPedidoTrocaSalvar = new Array();

    for (var i = 0; i < listaPedidoTroca.length; i++)
        listaPedidoTrocaSalvar.push(listaPedidoTroca[i].Codigo);

    return listaPedidoTrocaSalvar;
}

function retornoConsultaDetalhePedidoAdicionarPedidoTroca(pedidoSelecionado) {
    _detalhePedidoAdicionarPedidoTroca.Pedido.codEntity(pedidoSelecionado.Codigo);
    _detalhePedidoAdicionarPedidoTroca.Pedido.val(pedidoSelecionado.Descricao);
    _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.visible(_CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedidoSelecionado.PedidoDestinadoAFilial);
    _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.required = _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.visible();

    if (!_detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.visible())
        _detalhePedidoAdicionarPedidoTroca.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
}

// #endregion Funções Privadas
