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
/// <reference path="../../Consultas/GrupoImposto.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridAlteracaoProduto;
var _alteracaoProduto;

var ProdutoMap = function () {
    this.CodigoProduto = PropertyEntity({ val: 0, def: 0 });
    this.CodigoGrupoImposto = PropertyEntity({ val: 0, def: 0 });
    this.IdProduto = PropertyEntity({ val: "", def: "" });
    this.Codigo = PropertyEntity({ val: "", def: "" });
    this.Descricao = PropertyEntity({ val: "", def: "" });
    this.NCM = PropertyEntity({ val: "", def: "" });
    this.CEST = PropertyEntity({ val: "", def: "" });
    this.CodigoBarrasEAN = PropertyEntity({ val: "", def: "" });
    this.Valor = PropertyEntity({ val: 0, def: 0 });
}

var PesquisaAlteracaoProduto = function () {

    this.GrupoImpostoAlterar = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Selecione um grupo de imposto para alterar aos itens selecionados: ", idBtnSearch: guid(), visible: true, required: false });
    this.GrupoImposto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Imposto: ", idBtnSearch: guid(), visible: true, required: false });
    this.CodigoNCM = PropertyEntity({ text: "Cód. NCM: " });
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoBarrasEAN = PropertyEntity({ text: "Cód. Barras EAN: " });

    this.SalvarProdutos2 = PropertyEntity({ eventClick: SalvarProdutoClick, type: types.event, text: "Salvar Produtos", icon: ko.observable("fa fa-save"), idGrid: guid(), visible: ko.observable(true) });
    this.BuscarProdutos = PropertyEntity({ eventClick: BuscarProdutosClick, type: types.event, text: "Buscar Produtos", icon: ko.observable("fa fa-search"), idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar / Limpar", icon: ko.observable("fa fa-recycle"), idGrid: guid(), visible: ko.observable(true) });
    this.SalvarProdutos = PropertyEntity({ eventClick: SalvarProdutoClick, type: types.event, text: "Salvar Produtos", icon: ko.observable("fa fa-save"), idGrid: guid(), visible: ko.observable(false) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ListaProdutos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Produtos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(false) });
    this.AtualizarGrupoImposto = PropertyEntity({ eventClick: AtualizarGrupoImpostoClick, type: types.event, text: "Atualizar Grupo Imposto", idGrid: guid(), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadAlteracaoProduto() {
    _alteracaoProduto = new PesquisaAlteracaoProduto();
    KoBindings(_alteracaoProduto, "knockoutPesquisaProduto", false);

    new BuscarGrupoImposto(_alteracaoProduto.GrupoImposto);
    new BuscarGrupoImposto(_alteracaoProduto.GrupoImpostoAlterar);
}

function buscarProdutos() {
    _alteracaoProduto.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _alteracaoProduto.SelecionarTodos,
        somenteLeitura: false
    }
    var editarColuna = { permite: true, callback: null, atualizarRow: true };

    _gridAlteracaoProduto = new GridView(_alteracaoProduto.Produtos.idGrid, "AlteracaoProduto/PesquisarProduto", _alteracaoProduto, null, null, 20, null, null, null, multiplaescolha, null, editarColuna);
    _gridAlteracaoProduto.CarregarGrid();
}

//*******MÉTODOS*******

function BuscarProdutosClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        buscarProdutos();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Selecione os campos obrigatórios antes de buscar os produtos.");
    }
}

function CancelarClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar / limpar todos os dados?", function () {
        LimparCamposAlterecaoProduto();
    });
}

function SalvarProdutoClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja salvar as alterações feitas?", function () {
        CarregarListaProduto();
        var data = { ListaProdutos: _alteracaoProduto.ListaProdutos.val(), GrupoImpostoAlterar: _alteracaoProduto.GrupoImpostoAlterar.codEntity() };
        executarReST("AlteracaoProduto/SalvarProdutos", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Produtos alterados com sucesso.");
                LimparCamposAlterecaoProduto();
                //buscarProdutos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    });
}

function CarregarListaProduto() {
    _alteracaoProduto.ListaProdutos.val("");
    var produtosSelecionados = _gridAlteracaoProduto.ObterMultiplosSelecionados();

    if (produtosSelecionados.length > 0) {
        var dataGrid = new Array();

        $.each(produtosSelecionados, function (i, produto) {

            var obj = new Object();
            obj.CodigoProduto = produto.CodigoProduto;
            obj.CodigoGrupoImposto = produto.CodigoGrupoImposto;
            obj.IdProduto = produto.IdProduto;
            obj.Codigo = produto.Codigo;
            obj.Descricao = produto.Descricao;
            obj.NCM = produto.NCM;
            obj.CEST = produto.CEST;
            obj.CodigoBarrasEAN = produto.CodigoBarrasEAN;
            obj.Valor = produto.Valor;

            dataGrid.push(obj);
        });

        _alteracaoProduto.ListaProdutos.val(JSON.stringify(dataGrid));
    }
}

function LimparCamposAlterecaoProduto() {
    LimparCampos(_alteracaoProduto);
    _alteracaoProduto.ListaProdutos.val("");

    _gridAlteracaoProduto = null;
    buscarProdutos();
}

function AtualizarGrupoImpostoClick(e) {
    if (_alteracaoProduto.GrupoImpostoAlterar.codEntity() > 0) {
        exibirConfirmacao("Confirmação", "Realmente deseja alterar o Grupo de Imposto para todos os Produtos do filtro realizado?", function () {
            Salvar(_alteracaoProduto, "AlteracaoProduto/AtualizarGrupoImposto", function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Produtos alterados com sucesso.");
                    LimparCamposAlterecaoProduto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Selecione um Grupo de Imposto para alterar em todos os Produtos filtrados.");
}