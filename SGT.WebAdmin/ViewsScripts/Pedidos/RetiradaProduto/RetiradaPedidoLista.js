/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaRetiradaPedidoLista;
var _pedidoModal;
var _gridAgendamentos;

var RetiradaPedidoLista = function () {
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557, val: ko.observable(true), options: _statusPesquisa, def: true });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaPedidoLista.NumeroDoPedido.getRequiredFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaPedidoLista.DataInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaPedidoLista.DataFim.getFieldDescription(), getType: typesKnockout.date });
    this.Filial = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaPedidoLista.Unidade.getRequiredFieldDescription(), required: true, def: true, options: ko.observable([]), getType: typesKnockout.string, val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRetiradaProduto.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var PedidoModal = function () {
    this.GridProduto = PropertyEntity({ type: types.local, idGrid: guid() });
    this.Filial = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaPedidoLista.Filial, getType: typesKnockout.string, val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaPedidoLista.TipoOperacao.getFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaPedidoLista.Cliente, getType: typesKnockout.string, val: ko.observable("") });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaPedidoLista.NumeroPedidoEmbarcador, getType: typesKnockout.string, val: ko.observable("") });
    this.ProdutosEmbarcador = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaPedidoLista.Produtos, getType: typesKnockout.string, val: ko.observable("") });
    this.Agendamentos = PropertyEntity({ type: types.local, idGrid: guid() });
    this.CodigoPedido = PropertyEntity({ getType: typesKnockout.int, val: ko.observable("") });


    //status

    this.DataCriacaoPedido = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: Localization.Resources.Pedidos.RetiradaPedidoLista.DataCriacaoDoPedido, visible: true, cssClass: ko.observable("step")});
    this.DataLiberacaoComercial = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: Localization.Resources.Pedidos.RetiradaPedidoLista.DataLiberacaoComercial, visible: true, cssClass: ko.observable("step")});
    this.DataLiberacaoFinanceira = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: Localization.Resources.Pedidos.RetiradaPedidoLista.DataLiberacaoFinanceira, visible: true, cssClass: ko.observable("step")});
    this.DataAgendamento = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: Localization.Resources.Pedidos.RetiradaPedidoLista.DataAgendamento, visible: true, cssClass: ko.observable("step")});
    this.DataRemessaConcluida = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: Localization.Resources.Pedidos.RetiradaPedidoLista.DataRemessa, visible: true, cssClass: ko.observable("step")});
    this.DataCarregamentoConcluido = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: Localization.Resources.Pedidos.RetiradaPedidoLista.DataCarregamento, visible: true, cssClass: ko.observable("step")});
    this.DataFaturamentoConcluido = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: Localization.Resources.Pedidos.RetiradaPedidoLista.DataFaturamento, visible: true, cssClass: ko.observable("step") });

    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("13%") });

}

function buscarRetiradaPedidoLista() {
    //-- Grid
    // Opcoes    
    var editar = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: function (_gridRetiradaProduto) { mostrarDetalhes(_gridRetiradaProduto, true); }, tamanho: "7", icone: "" };
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "RetiradaProduto/ExportarPesquisa",
        titulo: "Retirada Produto"
    };

    // Inicia Grid de busca
    _gridRetiradaProduto = new GridViewExportacao(_pesquisaRetiradaPedidoLista.Pesquisar.idGrid, "Pedido/Pesquisa", _pesquisaRetiradaPedidoLista, menuOpcoes, configExportacao);
    _gridRetiradaProduto.CarregarGrid();
}
function loadGridProdutoPedido() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (data) { excluirProdutoClick(_pedidoProduto.GridProduto, data) } };
    var editar = { descricao: "Editar", id: guid(), metodo: function (data) { editarProdutoClick(_pedidoProduto.GridProduto, data) } };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, opcaoExcluir], tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%" },
    ];
    _gridProdutoPedido = new BasicDataTable(_pedidoModal.GridProduto.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function mostrarDetalhes(_gridPedido) {

    _pedidoModal = new PedidoModal();
    _pedidoModal.CodigoPedido.val(_gridPedido.Codigo);
    KoBindings(_pedidoModal, "divModalSelecaoPedido");
    Global.abrirModal('divModalSelecaoPedido');

    executarReST("RetiradaProduto/BuscarPedidoPorCodigo", { Codigo: _gridPedido.Codigo }, function (retorno) {
        if (retorno.Success) {
            PreencherObjetoKnout(_pedidoModal, retorno);
            loadGridProdutoPedido();

            _gridProdutoPedido.CarregarGrid(_pedidoModal.ProdutosEmbarcador.val());
            _gridProdutoPedido.DesabilitarOpcoes();
            _pedidoModal.GridProduto.basicTable = _gridProdutoPedido;
            PreencherObjetoKnout(_pedidoModal, { Data: retorno.Data.SituacaoCarregamentoPedido });
            setaClasses();
        }
    });

    loadGridAgendamentos(_gridPedido);

    
}
function setaClasses() {
    if (_pedidoModal.DataCriacaoPedido.val())
        _pedidoModal.DataCriacaoPedido.cssClass("step green");
    if (_pedidoModal.DataLiberacaoComercial.val())
        _pedidoModal.DataLiberacaoComercial.cssClass("step green");
    if (_pedidoModal.DataLiberacaoFinanceira.val())
        _pedidoModal.DataLiberacaoFinanceira.cssClass("step green");
    if (_pedidoModal.DataAgendamento.val())
        _pedidoModal.DataAgendamento.cssClass("step green");
    if (_pedidoModal.DataRemessaConcluida.val())
        _pedidoModal.DataRemessaConcluida.cssClass("step green");
    if (_pedidoModal.DataCarregamentoConcluido.val())
        _pedidoModal.DataCarregamentoConcluido.cssClass("step green");
    if (_pedidoModal.DataFaturamentoConcluido.val())
        _pedidoModal.DataFaturamentoConcluido.cssClass("step green");
}
function loadGridAgendamentos(){
    _gridAgendamentos = new GridView(_pedidoModal.Agendamentos.idGrid, "RetiradaProduto/BuscarAgendamentos",_pedidoModal, null, null);
    _gridAgendamentos.CarregarGrid();
}

function loadRetiradaPedidoLista() {
    _pesquisaRetiradaPedidoLista = new RetiradaPedidoLista();
    KoBindings(_pesquisaRetiradaPedidoLista, "knockoutRetiradaPedidoLista", false, _pesquisaRetiradaPedidoLista.Pesquisar.id);

    buscarRetiradaPedidoLista();
    buscaFiliaisPedido();
}


function buscaFiliaisPedido() {
    executarReST("RetiradaProduto/BuscarFiliais", false, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var _filiais = retorno.Data.Filiais.map(function (d) { return { value: d.Codigo, text: d.Descricao } });
                _pesquisaRetiradaPedidoLista.Filial.options(_filiais);              
            }
        }
    });
}