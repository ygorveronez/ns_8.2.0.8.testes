/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Deposito.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/LinhaSeparacao.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/pedido.js" />
/// <reference path="../../Consultas/Carga.js" />


var _pesquisaCancelamentoSaldoReserva;
var _gridPedidosCancelamentoSaldoReserva;

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaCancelamentoSaldoReserva = function () {

    var dataInicial = moment().add(-1, 'days').format("DD/MM/YYYY");
    var dataFinal = moment().add(6, 'days').format("DD/MM/YYYY");

    this.DataInicio = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataPedido.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(dataInicial), required: true });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataPedidoAte.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date, val: ko.observable(dataFinal), required: true });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroPedidoEmbarcado.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Filial.getFieldDescription(), issue: 70, idBtnSearch: guid(), required: true });
    this.CodigosPedidos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.DescricaoPedido.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Deposito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Deposito.getFieldDescription(), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.CodigosCanalEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.CanalEntrega.getFieldDescription(), idBtnSearch: guid() });
    this.CodigosLinhaSeparacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.LinhaSeparacao.getFieldDescription(), idBtnSearch: guid() });
    this.CodigosTipoDeCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.TipoCarga.getFieldDescription(), idBtnSearch: guid() });
    this.CodigosProdutos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Produtos.getFieldDescription(), idBtnSearch: guid() });
    this.CodigosGrupoProdutos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.GrupoProdutos.getFieldDescription(), idBtnSearch: guid() });
    this.EstadosOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.UFOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.EstadosDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.UFDestino, idBtnSearch: guid() });
    this.GrupoPessoaRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.GrupoPessoasRemetente.getFieldDescription(), idBtnSearch: guid() });
    this.GrupoPessoaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.GrupoPessoasDestinatario.getFieldDescription(), idBtnSearch: guid() });
    this.CodigosCategoriaClientes = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.CategoriaClientes.getFieldDescription(), idBtnSearch: guid() });
    this.IdDemanda = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.IDDemanda.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CodigosCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Carga.getFieldDescription(), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarCancelamentoSaldoReserva();
        }, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var PedidosCancelamentoSaldoReserva = function () {

    this.CancelarSaldoReserva = PropertyEntity({
        eventClick: function (e) {
            cancelarSaldoReservaProdutosSelecionados();
        }, type: types.event, text: Localization.Resources.Pedidos.Pedido.CancelarReserva, idGrid: guid(), visible: ko.observable(true)
    });

    this.Grid = PropertyEntity({ type: types.local });
}

function loadCancelamentoSaldoReserva() {

    _pesquisaCancelamentoSaldoReserva = new PesquisaCancelamentoSaldoReserva();
    KoBindings(_pesquisaCancelamentoSaldoReserva, "knockoutPesquisaCancelamentoSaldoReserva");

    new BuscarFilial(_pesquisaCancelamentoSaldoReserva.Filial);
    new BuscarPedidosDisponiveis(_pesquisaCancelamentoSaldoReserva.CodigosPedidos, null, null, _pesquisaCancelamentoSaldoReserva.Filial, true);
    new BuscarDeposito(_pesquisaCancelamentoSaldoReserva.Deposito);
    new BuscarClientes(_pesquisaCancelamentoSaldoReserva.Destinatario);
    new BuscarCanaisEntrega(_pesquisaCancelamentoSaldoReserva.CodigosCanalEntrega);
    new BuscarLinhasSeparacao(_pesquisaCancelamentoSaldoReserva.CodigosLinhaSeparacao);
    new BuscarTiposdeCarga(_pesquisaCancelamentoSaldoReserva.CodigosTipoDeCarga);
    new BuscarProdutos(_pesquisaCancelamentoSaldoReserva.CodigosProdutos);
    new BuscarGruposProdutos(_pesquisaCancelamentoSaldoReserva.CodigosGrupoProdutos);
    new BuscarEstados(_pesquisaCancelamentoSaldoReserva.EstadosDestino);
    new BuscarEstados(_pesquisaCancelamentoSaldoReserva.EstadosOrigem);
    new BuscarGruposPessoas(_pesquisaCancelamentoSaldoReserva.GrupoPessoaRemetente);
    new BuscarGruposPessoas(_pesquisaCancelamentoSaldoReserva.GrupoPessoaDestinatario);
    new BuscarCategoriaPessoa(_pesquisaCancelamentoSaldoReserva.CodigosCategoriaClientes);
    new BuscarCargas(_pesquisaCancelamentoSaldoReserva.CodigosCarga);

    // Inicializando o Grid de pedidos cancelamento saldo reserva...
    loadGridPedidosCancelamentoSaldoReserva();

}

function loadGridPedidosCancelamentoSaldoReserva() {

    var opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesPedidoCarregamentoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoDetalhes] };

    var configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true };

    var header = [
        { data: "CodigoPedido", visible: false },
        { data: "Codigo", visible: false }, // CodigoPedidoProduto
        { data: "Emitente", title: Localization.Resources.Pedidos.Pedido.Filial, className: "text-align-center", width: "10%", widthDefault: "10%", visible: true },
        { data: "NumeroPedidoEmbarcador", title: Localization.Resources.Pedidos.Pedido.NPedido, className: "text-align-center", width: "10%", widthDefault: "10%", visible: true },
        { data: "Destinatario", title: Localization.Resources.Gerais.Geral.Destinatario, width: "10%", widthDefault: "10%", visible: true }, // , callbackToolTip: retornoCallbackRecebedorToolTip
        { data: "CanalEntrega", title: Localization.Resources.Pedidos.Pedido.CanalEntrega, width: "10%", widthDefault: "10%", visible: true },
        { data: "LinhaSeparacao", title: Localization.Resources.Pedidos.Pedido.LinhaSeparacao, width: "15%", widthDefault: "15%", visible: true },
        { data: "GrupoProduto", title: Localization.Resources.Pedidos.Pedido.GrupoProduto, width: "15%", widthDefault: "15%", visible: true },
        { data: "TipoDeCarga", title: Localization.Resources.Pedidos.Pedido.TipoCarga, width: "10%", widthDefault: "10%", visible: true },
        { data: "Produto", title: Localization.Resources.Pedidos.Pedido.Produto, width: "15%", widthDefault: "15%", visible: true },
        { data: "Situacao", title: Localization.Resources.Gerais.Geral.Situacao, width: "5%", widthDefault: "5%", visible: true },
        { data: "Qtde", title: Localization.Resources.Pedidos.Pedido.Qtde, width: "5%", widthDefault: "15%", visible: true },
        { data: "QtdeCarregado", title: Localization.Resources.Pedidos.Pedido.QCarregado, width: "5%", widthDefault: "15%", visible: true },
        { data: "SaldoQtde", title: Localization.Resources.Pedidos.Pedido.Saldo, width: "5%", widthDefault: "15%", visible: true },
        { data: "IdDemanda", title: Localization.Resources.Pedidos.Pedido.IDDemanda, width: "5%", widthDefault: "15%", visible: true },
        { data: "PalletFechado", title: Localization.Resources.Pedidos.Pedido.PlFechado, width: "5%", widthDefault: "15%", visible: true },
        { data: "CodigosCarga", title: Localization.Resources.Pedidos.Pedido.Carga, width: "5%", widthDefault: "15%", visible: true },
    ]
        ;

    var pedidosCancelamentoSaldoReserva = new PedidosCancelamentoSaldoReserva();
    KoBindings(pedidosCancelamentoSaldoReserva, "knockoutPedidosCancelamentoSaldoReserva");

    _gridPedidosCancelamentoSaldoReserva = new BasicDataTable(pedidosCancelamentoSaldoReserva.Grid.id, header, null /* menuOpcoes */, { column: 1, dir: "asc" }, configRowsSelect, 10, null, null, null, null, null, null, null, null, null, false, null, null, "Pedidos/CancelamentoSaldoReserva", "grid-cancelamento-saldo-reserva");

    _gridPedidosCancelamentoSaldoReserva.SetPermitirEdicaoColunas(true);
    _gridPedidosCancelamentoSaldoReserva.SetSalvarPreferenciasGrid(true);
    _gridPedidosCancelamentoSaldoReserva.SetPermitirRedimencionarColunas(true);

    _gridPedidosCancelamentoSaldoReserva.CarregarGrid([]);
}

function pesquisarCancelamentoSaldoReserva() {

    if (ValidarCamposObrigatorios(_pesquisaCancelamentoSaldoReserva)) {

        var data = RetornarObjetoPesquisa(_pesquisaCancelamentoSaldoReserva);

        executarReST("CancelamentoSaldoReserva/ObterPedidos", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    _pesquisaCancelamentoSaldoReserva.ExibirFiltros.visibleFade(false);
                    _gridPedidosCancelamentoSaldoReserva.CarregarGrid(arg.Data);
                    if (arg.Msg != undefined && arg.Msg != null && arg.Msg != "") {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Pedidos.Pedido.InformeCampoObrigatorio);
    }
}

function detalhesPedidoCarregamentoClick() {
    // Esse procedimento vamos mostrar informações do saldo do produto.. pedido.. carga...

}

function cancelarSaldoReservaProdutosSelecionados() {
    var selecionados = _gridPedidosCancelamentoSaldoReserva.ListaSelecionados();
    if (selecionados.length == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pedidos.Pedido.NenhumProdutoParaCancelamentoSaldoReserva);
        return;
    }

    var dados = selecionados.map(function (pedidoProduto) {
        return {
            CodigoPedido: pedidoProduto.CodigoPedido,
            CodigoPedidoProduto: pedidoProduto.Codigo
        };
    });

    var data = { Saldos: JSON.stringify(dados) };

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.TemCertezaQueDesejaCancelarSaldoReservaProdutosSelecionados, function () {
        executarReST("CancelamentoSaldoReserva/CancelarReserva", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                    pesquisarCancelamentoSaldoReserva();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}