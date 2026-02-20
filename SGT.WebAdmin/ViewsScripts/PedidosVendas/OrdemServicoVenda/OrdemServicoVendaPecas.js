/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumStatusPedidoVenda.js" />
/// <reference path="OrdemServicoVendaEtapa.js" />
/// <reference path="OrdemServicoVenda.js" />
/// <reference path="OrdemServicoVendaMaoObra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ordemServicoVendaPecas;
var _gridVendaPecas;

var ItemMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Produto = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoOrdemServicoVenda = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DescricaoItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Quantidade = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorUnitario = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorTotalItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorDesconto = PropertyEntity({ type: types.map, val: ko.observable("") });
};

var OrdemServicoVendaPecas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorDesconto = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.CodigoItem = PropertyEntity({ text: "*Código Item:", getType: typesKnockout.string, maxlength: 100, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DescricaoItem = PropertyEntity({ text: "*Descrição Item:", getType: typesKnockout.string, maxlength: 500, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Quantidade = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Quantidade:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorUnitario = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Unitário:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTotalItem = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.ValorProdutos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Produtos:", maxlength: 15, enable: ko.observable(false) });
    this.ValorTotal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Total:", maxlength: 15, enable: ko.observable(false) });

    //CRUD
    this.SalvarItem = PropertyEntity({ eventClick: SalvarPecaClick, type: types.event, text: "Salvar Item", visible: ko.observable(true), enable: ko.observable(true) });
    this.Gravar = PropertyEntity({ eventClick: GravarOrdemServicoVendaClick, type: types.event, text: "Gravar OS", visible: ko.observable(true), enable: ko.observable(true) });

    this.PecasOrdemServicoVenda = PropertyEntity({ type: types.local, id: guid() });
};

//*******EVENTOS*******

function loadOrdemServicoVendaPecas() {
    _ordemServicoVendaPecas = new OrdemServicoVendaPecas();
    KoBindings(_ordemServicoVendaPecas, "knockoutPecasOrdemServico");

    new BuscarProdutoTMSComEstoque(_ordemServicoVendaPecas.Produto, function (data) {
        _ordemServicoVendaPecas.Produto.codEntity(data.Codigo);
        _ordemServicoVendaPecas.Produto.val(data.Descricao);
        if (data.ValorVenda != null && data.ValorVenda != "") {
            var valorUnitario = Globalize.parseFloat(data.ValorVenda.toString());
            _ordemServicoVendaPecas.ValorUnitario.val(Globalize.format(valorUnitario, "n2"));
        }
        _ordemServicoVendaPecas.DescricaoItem.val(data.Descricao);
        if (data.CodigoProduto != "")
            _ordemServicoVendaPecas.CodigoItem.val(data.CodigoProduto);
        else
            _ordemServicoVendaPecas.CodigoItem.val(data.Codigo);
        $("#" + _ordemServicoVendaPecas.Quantidade.id).focus();
    }, null, true);

    var editarItem = { descricao: "Editar", id: guid(), metodo: EditarPecaClick, icone: "" };
    var excluirItem = { descricao: "Excluir", id: guid(), metodo: ExcluirPecaClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [editarItem, excluirItem] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Produto", visible: false },
        { data: "CodigoOrdemServicoVenda", visible: false },
        { data: "CodigoItem", title: "Código", width: "10%" },
        { data: "DescricaoItem", title: "Descrição", width: "50%" },
        { data: "Quantidade", title: "Quantidade", width: "10%" },
        { data: "ValorUnitario", title: "Valor Unit.", width: "10%" },
        { data: "ValorTotalItem", title: "Total", width: "10%" },
        { data: "ValorDesconto", visible: false }
    ];

    _gridVendaPecas = new BasicDataTable(_ordemServicoVendaPecas.PecasOrdemServicoVenda.id, header, menuOpcoes);

    recarregarGridListaItens();
}

function EditarPecaClick(data) {
    if (_ordemServicoVenda.Status.val() !== EnumStatusPedidoVenda.Aberta) {
        exibirMensagem("atencao", "Pedido Concluído", "Só é possível editar itens de Pedido em Aberto!");
        return;
    }
    if (_ordemServicoVendaPecas.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return;
    }
    if (_ordemServicoVendaPecas.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return;
    }

    limparCamposOrdemServicoVendaPecas();
    RemoverValorItemTotalizador(data);
    _ordemServicoVenda.ValorDesconto.enable(false);
    _ordemServicoVenda.PercentualDesconto.enable(false);

    _ordemServicoVendaPecas.Codigo.val(data.Codigo);
    _ordemServicoVendaPecas.Produto.codEntity(data.Produto);
    _ordemServicoVendaPecas.Produto.val(data.DescricaoItem);
    _ordemServicoVendaPecas.CodigoItem.val(data.CodigoItem);
    _ordemServicoVendaPecas.DescricaoItem.val(data.DescricaoItem);
    _ordemServicoVendaPecas.Quantidade.val(data.Quantidade);
    _ordemServicoVendaPecas.ValorUnitario.val(data.ValorUnitario);
    _ordemServicoVendaPecas.ValorTotalItem.val(data.ValorTotalItem);
    _ordemServicoVendaPecas.ValorDesconto.val(data.ValorDesconto);
}

function ExcluirPecaClick(data) {
    if (_ordemServicoVenda.Status.val() !== EnumStatusPedidoVenda.Aberta) {
        exibirMensagem("atencao", "Pedido Concluído", "Só é possível excluir itens de Pedido em Aberto!");
        return;
    }
    if (_ordemServicoVendaPecas.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return;
    }
    if (_ordemServicoVendaPecas.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o item " + data.DescricaoItem + "?", function () {
        $.each(_ordemServicoVenda.ListaItens.list, function (i, listaItens) {
            if (data.Codigo == listaItens.Codigo.val) {
                RemoverValorItemTotalizador(data);
                _ordemServicoVenda.ListaItens.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridListaItens();
    });
}

function SalvarPecaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_ordemServicoVendaPecas);
    _ordemServicoVendaPecas.Quantidade.requiredClass("form-control");
    _ordemServicoVendaPecas.ValorUnitario.requiredClass("form-control");
    _ordemServicoVendaPecas.ValorTotalItem.requiredClass("form-control");

    if (Globalize.parseFloat(_ordemServicoVendaPecas.Quantidade.val()) <= 0) {
        valido = false;
        _ordemServicoVendaPecas.Quantidade.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_ordemServicoVendaPecas.ValorUnitario.val()) <= 0) {
        valido = false;
        _ordemServicoVendaPecas.ValorUnitario.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_ordemServicoVendaPecas.ValorTotalItem.val()) <= 0) {
        valido = false;
        _ordemServicoVendaPecas.ValorTotalItem.requiredClass("form-control is-invalid");
    }

    if (valido) {
        if (_ordemServicoVendaPecas.Codigo.val() > "0" && _ordemServicoVendaPecas.Codigo.val() != undefined) {
            for (var i = 0; i < _ordemServicoVenda.ListaItens.list.length; i++) {
                if (_ordemServicoVendaPecas.Codigo.val() == _ordemServicoVenda.ListaItens.list[i].Codigo.val) {
                    _ordemServicoVenda.ListaItens.list.splice(i, 1);
                    break;
                }
            }
        }
        _ordemServicoVendaPecas.Codigo.val(guid());

        var listaPecasGrid = new ItemMap();

        listaPecasGrid.Codigo.val = _ordemServicoVendaPecas.Codigo.val();
        listaPecasGrid.Produto.val = _ordemServicoVendaPecas.Produto.codEntity();
        listaPecasGrid.CodigoOrdemServicoVenda.val = _ordemServicoVenda.Codigo.val();
        listaPecasGrid.CodigoItem.val = _ordemServicoVendaPecas.CodigoItem.val();
        listaPecasGrid.DescricaoItem.val = _ordemServicoVendaPecas.DescricaoItem.val();
        listaPecasGrid.Quantidade.val = _ordemServicoVendaPecas.Quantidade.val();
        listaPecasGrid.ValorUnitario.val = _ordemServicoVendaPecas.ValorUnitario.val();
        listaPecasGrid.ValorTotalItem.val = _ordemServicoVendaPecas.ValorTotalItem.val();
        listaPecasGrid.ValorDesconto.val = _ordemServicoVendaPecas.ValorDesconto.val();

        _ordemServicoVenda.ListaItens.list.push(listaPecasGrid);

        recarregarGridListaItens();
        AtualizarTotalizadores();
        limparCamposOrdemServicoVendaPecas();
        $("#" + _ordemServicoVendaPecas.Produto.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

//*******MÉTODOS*******

function recarregarGridListaItens() {

    var data = new Array();

    $.each(_ordemServicoVenda.ListaItens.list, function (i, listaItens) {
        var listaPecasGrid = new Object();

        listaPecasGrid.Codigo = listaItens.Codigo.val;
        listaPecasGrid.Produto = listaItens.Produto.val;
        listaPecasGrid.CodigoOrdemServicoVenda = _ordemServicoVenda.Codigo.val();
        listaPecasGrid.CodigoItem = listaItens.CodigoItem.val;
        listaPecasGrid.DescricaoItem = listaItens.DescricaoItem.val;
        listaPecasGrid.Quantidade = listaItens.Quantidade.val;
        listaPecasGrid.ValorUnitario = listaItens.ValorUnitario.val;
        listaPecasGrid.ValorTotalItem = listaItens.ValorTotalItem.val;
        listaPecasGrid.ValorDesconto = listaItens.ValorDesconto.val;

        data.push(listaPecasGrid);
    });

    _gridVendaPecas.CarregarGrid(data);
}

function CalcularTotalItem() {
    var quantidade = Globalize.parseFloat(_ordemServicoVendaPecas.Quantidade.val());
    var valorUnitario = Globalize.parseFloat(_ordemServicoVendaPecas.ValorUnitario.val());

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _ordemServicoVendaPecas.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
    }
}

function AtualizarTotalizadores() {
    var valorTotalItem = Globalize.parseFloat(_ordemServicoVendaPecas.ValorTotalItem.val());
    var valorTotal = Globalize.parseFloat(_ordemServicoVendaPecas.ValorTotal.val());

    var valorTotalProdutos = 0;
    var valorTotalProdutosTotal = Globalize.parseFloat(_ordemServicoVendaPecas.ValorProdutos.val());
    valorTotalProdutos = valorTotalItem + valorTotalProdutosTotal;
    _ordemServicoVendaPecas.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
    _ordemServicoVenda.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));

    valorTotal = valorTotal + valorTotalItem;
    _ordemServicoVendaPecas.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    _ordemServicoVendaMaoObras.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    _ordemServicoVenda.ValorTotal.val(Globalize.format(valorTotal, "n2"));
}

function RemoverValorItemTotalizador(itemPedido) {
    for (var i = 0; i < _ordemServicoVenda.ListaItens.list.length; i++) {
        if (itemPedido.Codigo == _ordemServicoVenda.ListaItens.list[i].Codigo.val) {
            var valorTotalItem = Globalize.parseFloat(itemPedido.ValorTotalItem.toString());
            var valorTotal = Globalize.parseFloat(_ordemServicoVendaPecas.ValorTotal.val());

            var valorTotalProdutos = 0;
            var valorTotalProdutosTotal = Globalize.parseFloat(_ordemServicoVendaPecas.ValorProdutos.val());
            valorTotalProdutos = valorTotalProdutosTotal - valorTotalItem;
            _ordemServicoVendaPecas.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
            _ordemServicoVenda.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));

            valorTotal = valorTotal - valorTotalItem;
            _ordemServicoVendaPecas.ValorTotal.val(Globalize.format(valorTotal, "n2"));
            _ordemServicoVendaMaoObras.ValorTotal.val(Globalize.format(valorTotal, "n2"));
            _ordemServicoVenda.ValorTotal.val(Globalize.format(valorTotal, "n2"));

            break;
        }
    }
}

function limparCamposOrdemServicoVendaPecas() {
    _ordemServicoVendaPecas.Codigo.val(0);
    _ordemServicoVendaPecas.ValorDesconto.val("0,00");

    LimparCampoEntity(_ordemServicoVendaPecas.Produto);
    _ordemServicoVendaPecas.CodigoItem.val("");
    _ordemServicoVendaPecas.DescricaoItem.val("");
    _ordemServicoVendaPecas.Quantidade.val("0,00");
    _ordemServicoVendaPecas.ValorUnitario.val("0,00");
    _ordemServicoVendaPecas.ValorTotalItem.val("0,00");

    _ordemServicoVenda.ValorDesconto.enable(true);
    _ordemServicoVenda.PercentualDesconto.enable(true);
}