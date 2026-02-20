/// <reference path="RetiradaProduto.js" />
/// <reference path="..\..\Consultas\Pedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridPedido;
var _gridProduto;
var _pedido;
var _pedidoProduto;
var _pedidos = new Array();
var _produtos = new Array();

/*
 * Declaração das Classes
 */

var ProdutoModal = function () {
    this.GridProduto = PropertyEntity({ type: types.local, idGrid: guid() });
    this.QuantidadeRetiradaCheckBox = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.QuantidadeRetiradaCheckBox, val: ko.observable(false), getType: typesKnockout.bool, def: ko.observable(false), visible: ko.observable(true) });
    this.CodigoProduto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuantidadeRetirada = PropertyEntity({ text: 'Quantidade Retirada', getType: typesKnockout.float, val: ko.observable(""), visible: ko.observable(false) });
    this.Salvar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

var Pedido = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Pedido = PropertyEntity({ type: types.event, text: Localization.Resources.Pedidos.RetiradaProduto.descricaoPedido, idBtnSearch: guid(), visible: ko.observable(true) });

    this.CapacidadeVeiculo = PropertyEntity({ text: 'Capacidade do Veículo', getType: typesKnockout.int, val: ko.observable(""), formatado: ko.observable(0) });
    this.PesoTotal = PropertyEntity({ text: 'Peso Total', getType: typesKnockout.float, val: ko.observable(0), formatado: ko.observable(0) });
    this.EspacoDisponivel = PropertyEntity({ text: 'Disponível', getType: typesKnockout.float, val: ko.observable(0), formatado: ko.observable(0) });
    this.Ocupacao = PropertyEntity({ text: 'Ocupação', getType: typesKnockout.float, val: ko.observable(0), formatado: ko.observable(0), color: ko.observable("#57889c"), width: ko.observable("") });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ val: ko.observable(0) });

    this.Proximo = PropertyEntity({ eventClick: PedidosProximaEtapa, type: types.event, text: Localization.Resources.Gerais.Geral.Proximo, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPedido() {
    var opcaoExcluir = { descricao: Localization.Resources.Pedidos.RetiradaProduto.Excluir, id: guid(), metodo: function (data) { excluirPedidoClick(_pedido.Pedido, data) }, visibilidade: visibilidadeGridProdutos };
    var produtos = { descricao: Localization.Resources.Pedidos.RetiradaProduto.Produtos, id: guid(), evento: "onclick", metodo: function (_gridPedido) { mostraProdutos(_gridPedido, true); }, tamanho: "10", icone: "", visibilidade: visibilidadeGridProdutos };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [produtos, opcaoExcluir], tamanho: 10 };

    var header = [
        { data: "NumeroPedidoEmbarcador", title: Localization.Resources.Pedidos.RetiradaProduto.NPedido, width: "20%" },
        { data: "Codigo", title: Localization.Resources.Pedidos.RetiradaProduto.Codigo, width: "20%" },
        { data: "NomeCliente", title: Localization.Resources.Pedidos.RetiradaProduto.NomeCliente, width: "20%" },
        { data: "CnpjCliente", title: Localization.Resources.Pedidos.RetiradaProduto.CNPJ, width: "20%" },
        { data: "Endereco", title: Localization.Resources.Pedidos.RetiradaProduto.Endereco, width: "20%" },
        { data: "CidadeUf", title: Localization.Resources.Pedidos.RetiradaProduto.CidadeUF, width: "10%" },
    ];
    _gridPedido = new BasicDataTable(_pedido.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function visibilidadeGridProdutos() {
    return _retiradaProduto.Situacao.val() != EnumSituacaoCarregamento.FalhaIntegracao;
}

function loadGridProduto() {
    var opcaoExcluir = { descricao: Localization.Resources.Pedidos.RetiradaProduto.Excluir, id: guid(), metodo: function (data) { excluirProdutoClick(_pedidoProduto.GridProduto, data) } };
    var editar = { descricao: Localization.Resources.Pedidos.RetiradaProduto.Editar, id: guid(), metodo: function (data) { editarProdutoClick(_pedidoProduto.GridProduto, data) } };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, opcaoExcluir], tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Pedidos.RetiradaProduto.Descricao, width: "40%" },
        { data: "Observacao", title: Localization.Resources.Pedidos.RetiradaProduto.Observacao, width: "15%" },
        { data: "CodigoIntegracao", title: "Cód. Produto", width: "20%" },
        { data: "Quantidade", title: Localization.Resources.Pedidos.RetiradaProduto.Quantidade, width: "15%" },
        { data: "QuantidadeRetirada", title: Localization.Resources.Pedidos.RetiradaProduto.QuantidadeRetirada, width: "15%" },
    ];
    _gridProduto = new BasicDataTable(_pedidoProduto.GridProduto.idGrid, header, obterMenuOpcoes(menuOpcoes), { column: 1, dir: orderDir.asc });
}

function mostraProdutos(_gridPedido) {
    //_pedidoProduto = new ProdutoModal();
    Global.abrirModal('divModalSelecaoProdutos');

    //KoBindings(_pedidoProduto, "knockoutSelecaoPeriodoCarregamento");
    loadGridProduto();
    $("#" + _pedidoProduto.QuantidadeRetiradaCheckBox.id).click(atualizaQuantidadesRetirada);
    for (var i = 0; i < _pedidos.length; i++) {
        if (_gridPedido.Codigo == _pedidos[i].Codigo) {
            _gridProduto.CarregarGrid(_pedidos[i].Produtos);
            _pedidoProduto.GridProduto.basicTable = _gridProduto;
        }
    }
    verificaQuantidadesRetirada();
}

function LoadPedido() {
    _pedido = new Pedido();
    KoBindings(_pedido, "knockoutPedido");

    _pedidoProduto = new ProdutoModal();
    KoBindings(_pedidoProduto, "knockoutSelecaoPeriodoCarregamento");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        _pedidoProduto.QuantidadeRetiradaCheckBox.visible(false);

    loadGridPedido();
    new BuscarPedidosDisponiveisPortalRetira(_pedido.Pedido, function (retorno) {
        if (retorno != null) {
            var codigos = [];
            for (var i = 0; i < retorno.length; i++) {
                for (var i = 0; i < retorno.length; i++) {
                    codigos.push(retorno[i].Codigo);
                }

                executarReST("RetiradaProduto/BuscarPedidoProdutos", { Filial: _pedido.Filial.val(), ProgramaComSessaoRoteirizador: true, Codigos: JSON.stringify(codigos) }, function (arg) {
                    if (!arg.Success)
                        return exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);

                    if (!arg.Data)
                        return exibirMensagem(tipoMensagem.aviso, "Aviso!", arg.Msg);

                    if (arg.Data.Pedidos) {
                        for (var i = 0; i < arg.Data.Pedidos.length; i++) {
                            _pedidos.push(arg.Data.Pedidos[i]);
                        }
                    }
                    _infoCarregamento.TipoOperacaoPedido.val(_pedidos[0].CodigoTipoOperacao);
                    _gridPedido.CarregarGrid(_pedidos);
                    calcularPesoProdutos();
                });
            }
        }
    }, _gridPedido, _pedido.Filial, true, _pedido.CapacidadeVeiculo, _pedido.PesoTotal, _CONFIGURACAO_TMS.NaoExibirPedidosDoDiaAgendamentoPedidos, _pedido.Transportador);

    _pedido.Pedido.basicTable = _gridPedido;
    recarregarGridPedido();
    loadGridProduto();
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function PedidosProximaEtapa() {
    ExibirProximaEtapa("tabDataHora");
}

function atualizaQuantidadesRetirada() {
    var produtos = _pedidoProduto.GridProduto.basicTable.BuscarRegistros();
    if (_pedidoProduto.QuantidadeRetiradaCheckBox.val()) {
        for (var i = 0; i < produtos.length; i++) {
            produtos[i].QuantidadeRetirada = produtos[i].Quantidade;
        }
    } else {
        for (var i = 0; i < produtos.length; i++) {
            produtos[i].QuantidadeRetirada = 0;
        }
    }
    _gridProduto.CarregarGrid(produtos);
}
function verificaQuantidadesRetirada() {
    var produtos = _pedidoProduto.GridProduto.basicTable.BuscarRegistros();
    var validador = true;
    for (var i = 0; i < produtos.length; i++) {
        if (produtos[i].QuantidadeRetirada != produtos[i].Quantidade)
            validador = false;
    }
    if (validador)
        $("#" + _pedidoProduto.QuantidadeRetiradaCheckBox.id).prop("checked", true);
    _pedidoProduto.QuantidadeRetiradaCheckBox.val(validador);
}
function excluirPedidoClick(knoutPedido, data) {
    var pedidos = knoutPedido.basicTable.BuscarRegistros();

    for (var i = 0; i < pedidos.length; i++) {
        if (data.Codigo == pedidos[i].Codigo) {
            pedidos.splice(i, 1);
            break;
        }
    }
    knoutPedido.basicTable.CarregarGrid(pedidos);
    calcularPesoProdutos();
}

function excluirProdutoClick(knoutProduto, data) {
    var produtos = knoutProduto.basicTable.BuscarRegistros();

    for (var i = 0; i < produtos.length; i++) {
        if (data.Codigo == produtos[i].Codigo) {
            produtos.splice(i, 1);
            break;
        }
    }
    knoutProduto.basicTable.CarregarGrid(produtos);
    calcularPesoProdutos();
}

function editarProdutoClick(knoutProduto, data) {
    _pedidoProduto.QuantidadeRetirada.visible(true);
    _pedidoProduto.CodigoProduto.val(data.Codigo);
    _pedidoProduto.QuantidadeRetirada.val(data.QuantidadeRetirada);
}

function salvarClick() {
    _pedidoProduto.QuantidadeRetirada.visible(false);
    var produtos = _pedidoProduto.GridProduto.basicTable.BuscarRegistros();
    if (_pedidoProduto.QuantidadeRetirada.val() == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível selecionar a quantidade 0");
        return;
    }

    for (var i = 0; i < produtos.length; i++) {
        if (_pedidoProduto.CodigoProduto.val() == produtos[i].Codigo) {
            if (produtos[i].Quantidade >= _pedidoProduto.QuantidadeRetirada.val()) {
                produtos[i].QuantidadeRetirada = _pedidoProduto.QuantidadeRetirada.val();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", "Quantidade de Retirada não pode ser maior que a quantidade do pedido");
                return;
            }
        }
    }
    _gridProduto.CarregarGrid(produtos);
    calcularPesoProdutos();
}


/*
 * Declaração das Funções
 */
function getVeiculo() {
    for (var i = 0; i < _veiculos.length; i++) {
        if (_retiradaProduto.ModeloVeiculo.val() == _veiculos[i].value) {
            return _veiculos[i];
        }
    }
}

function LimparCamposPedido() {
    //LimparCampos(_pedido);
    _gridPedido.CarregarGrid(new Array());
    _pedidos = new Array();
    calcularPesoProdutos();
}

function obterPedidos() {
    return JSON.stringify(_pedido.Pedido.basicTable.BuscarRegistros());
}

function recarregarGridPedido() {
    var pedidos = _retiradaProduto.Pedidos.val().slice();
    _gridPedido.CarregarGrid(pedidos);
    _pedidos = new Array();
    for (var i = 0; i < pedidos.length; i++) {
        _pedidos.push(pedidos[i]);
    }
    calcularPesoProdutos();
}

function recarregarGridProduto() {
    var produtos = _retiradaProduto.Produtos.val();
    _gridProduto.CarregarGrid(produtos);
    produtos = new Array();
    for (var i = 0; i < produtos.length; i++) {
        _produtos.push({ Codigo: produtos[i].Codigo, NumeroPedidoEmbarcador: produtos[i].NumeroPedidoEmbarcador });
    }
}

function obterTransportador() {
    if (_retiradaProduto.Transportador.codEntity() > 0)
        _pedido.Transportador.val(_retiradaProduto.Transportador.codEntity());
    else
        _pedido.Transportador.val('');
}

var obterMenuOpcoes = (objOpcoes) => _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe ? null : objOpcoes;
