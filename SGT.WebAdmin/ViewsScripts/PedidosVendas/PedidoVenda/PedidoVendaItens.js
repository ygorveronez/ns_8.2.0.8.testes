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
/// <reference path="PedidoVendaEtapa.js" />
/// <reference path="PedidoVenda.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Servico.js" />
/// <reference path="../../Enumeradores/EnumProdutoServico.js" />
/// <reference path="../../Enumeradores/EnumStatusPedidoVenda.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pedidoVendaItens;
var _gridVendaItens;

var ItemMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Produto = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Servico = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoPedidoVenda = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoNCM = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.NumeroOrdemCompra = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.NumeroItemOrdemCompra = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DescricaoItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Quantidade = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorUnitario = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorTotalItem = PropertyEntity({ type: types.map, val: ko.observable("") });
};

var PedidoVendaItens = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoItem = PropertyEntity({ val: ko.observable(EnumProdutoServico.Produto), options: EnumProdutoServico.obterOpcoes(), def: EnumProdutoServico.Produto, text: "*Tipo: ", enable: ko.observable(true), eventChange: TipoItemChange });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    this.CodigoItem = PropertyEntity({ text: "*Código Item:", getType: typesKnockout.string, maxlength: 100, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DescricaoItem = PropertyEntity({ text: "*Descrição Item:", getType: typesKnockout.string, maxlength: 500, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoNCM = PropertyEntity({ text: "NCM:", getType: typesKnockout.string, maxlength: 500, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(false) });    

    this.NumeroOrdemCompra = PropertyEntity({ text: "Número Pedido:", getType: typesKnockout.string, maxlength: 100, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroItemOrdemCompra = PropertyEntity({ text: "N. Item Pedido:", getType: typesKnockout.string, maxlength: 100, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.Quantidade = PropertyEntity({ def: "", val: ko.observable(""), text: "*Quantidade:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: _casasQuantidadeProdutoNFe, allowZero: false } });
    this.ValorUnitario = PropertyEntity({ def: "", val: ko.observable(""), text: "*Valor Unitário:", getType: typesKnockout.decimal, maxlength: 21, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: _casasValorProdutoNFe, allowZero: false } });
    this.ValorTotalItem = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.ValorProdutos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Produtos:", maxlength: 15, enable: ko.observable(false) });
    this.ValorServicos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Serviços:", maxlength: 15, enable: ko.observable(false) });
    this.ValorTotal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Total:", maxlength: 15, enable: ko.observable(false) });

    this.SalvarItem = PropertyEntity({ eventClick: SalvarItemClick, type: types.event, text: "Salvar Item", visible: ko.observable(true), enable: ko.observable(true) });
    this.Gravar = PropertyEntity({ eventClick: GravarPedidoVendaItensClick, type: types.event, text: "Gravar Pedido", visible: ko.observable(true), enable: ko.observable(true) });

    this.ItensPedidoVenda = PropertyEntity({ type: types.local, id: guid() });
};

//*******EVENTOS*******

function loadPedidoVendaItens() {
    _pedidoVendaItens = new PedidoVendaItens();
    KoBindings(_pedidoVendaItens, "knockoutItensPedido");

    new BuscarProdutoTMS(_pedidoVendaItens.Produto, function (data) {
        _pedidoVendaItens.Produto.codEntity(data.Codigo);
        _pedidoVendaItens.Produto.val(data.Descricao);
        if (data.ValorVenda != null && data.ValorVenda != "") {
            var valorUnitario = Globalize.parseFloat(data.ValorVenda.toString());
            if (valorUnitario > 0)
                _pedidoVendaItens.ValorUnitario.val(Globalize.format(valorUnitario, "n" + _casasValorProdutoNFe));
            else
                _pedidoVendaItens.ValorUnitario.val("");
        }
        _pedidoVendaItens.CodigoNCM.val(data.CodigoNCM);
        _pedidoVendaItens.DescricaoItem.val(data.Descricao);
        if (data.CodigoProduto != "")
            _pedidoVendaItens.CodigoItem.val(data.CodigoProduto);
        else
            _pedidoVendaItens.CodigoItem.val(data.Codigo);
        $("#" + _pedidoVendaItens.Quantidade.id).focus();
    }, null, null, null, null, true);

    new BuscarServicoTMS(_pedidoVendaItens.Servico, function (data) {
        _pedidoVendaItens.Servico.codEntity(data.Codigo);
        _pedidoVendaItens.Servico.val(data.Descricao);
        if (data.ValorVenda != null && data.ValorVenda != "") {
            var valorUnitario = Globalize.parseFloat(data.ValorVenda.toString());
            if (valorUnitario > 0)
                _pedidoVendaItens.ValorUnitario.val(Globalize.format(valorUnitario, "n" + _casasValorProdutoNFe));
            else
                _pedidoVendaItens.ValorUnitario.val("");
        }
        _pedidoVendaItens.CodigoNCM.val("");
        _pedidoVendaItens.DescricaoItem.val(data.Descricao);
        _pedidoVendaItens.CodigoItem.val(data.Codigo);
        $("#" + _pedidoVendaItens.Quantidade.id).focus();
    });

    var editarItem = { descricao: "Editar", id: guid(), metodo: EditarVendaItemClick, icone: "" };
    var excluirItem = { descricao: "Excluir", id: guid(), metodo: ExcluirVendaItemClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [editarItem, excluirItem] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Produto", visible: false },
        { data: "Servico", visible: false },
        { data: "CodigoPedidoVenda", visible: false },
        { data: "CodigoItem", title: "Código", width: "10%" },
        { data: "DescricaoItem", title: "Descrição", width: "50%" },
        { data: "CodigoNCM", title: "NCM", width: "7%" },
        { data: "Quantidade", title: "Quantidade", width: "10%" },
        { data: "ValorUnitario", title: "Valor Unit.", width: "10%" },
        { data: "ValorTotalItem", title: "Total", width: "10%" }
    ];

    _gridVendaItens = new BasicDataTable(_pedidoVendaItens.ItensPedidoVenda.id, header, menuOpcoes);

    recarregarGridListaItens();
}

function TipoItemChange() {
    if (_pedidoVendaItens.TipoItem.val() == EnumProdutoServico.Produto) {
        _pedidoVendaItens.Produto.required(true);
        _pedidoVendaItens.Produto.visible(true);
        _pedidoVendaItens.Servico.required(false);
        _pedidoVendaItens.Servico.visible(false);
        LimparCampoEntity(_pedidoVendaItens.Servico);
    } else if (_pedidoVendaItens.TipoItem.val() == EnumProdutoServico.Servico) {
        _pedidoVendaItens.Produto.required(false);
        _pedidoVendaItens.Produto.visible(false);
        _pedidoVendaItens.Servico.required(true);
        _pedidoVendaItens.Servico.visible(true);
        LimparCampoEntity(_pedidoVendaItens.Produto);
    }
}

function EditarVendaItemClick(data) {
    if (_pedidoVenda.Status.val() != EnumStatusPedidoVenda.Aberta) {
        exibirMensagem("atencao", "Pedido Concluído", "Só é possível editar itens de Pedido em Aberto!");
        return
    }
    if (_pedidoVendaItens.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return
    }
    if (_pedidoVendaItens.TipoItem.val() == EnumProdutoServico.Produto && _pedidoVendaItens.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return
    }
    if (_pedidoVendaItens.TipoItem.val() == EnumProdutoServico.Servico && _pedidoVendaItens.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return
    }

    limparCamposPedidoVendaItens();
    RemoverValorItemTotalizador(data);

    if (data.Servico > 0)
        _pedidoVendaItens.TipoItem.val(EnumProdutoServico.Servico);
    else if (data.Produto > 0)
        _pedidoVendaItens.TipoItem.val(EnumProdutoServico.Produto);
    TipoItemChange();

    _pedidoVendaItens.Codigo.val(data.Codigo);
    _pedidoVendaItens.Produto.codEntity(data.Produto);
    _pedidoVendaItens.Produto.val(data.DescricaoItem);
    _pedidoVendaItens.Servico.codEntity(data.Servico);
    _pedidoVendaItens.Servico.val(data.DescricaoItem);
    _pedidoVendaItens.CodigoNCM.val(data.CodigoNCM);
    _pedidoVendaItens.CodigoItem.val(data.CodigoItem);
    _pedidoVendaItens.NumeroOrdemCompra.val(data.NumeroOrdemCompra);
    _pedidoVendaItens.NumeroItemOrdemCompra.val(data.NumeroItemOrdemCompra);
    _pedidoVendaItens.DescricaoItem.val(data.DescricaoItem);
    _pedidoVendaItens.Quantidade.val(data.Quantidade);
    _pedidoVendaItens.ValorUnitario.val(data.ValorUnitario);
    _pedidoVendaItens.ValorTotalItem.val(data.ValorTotalItem);
}

function ExcluirVendaItemClick(data) {
    if (_pedidoVenda.Status.val() != EnumStatusPedidoVenda.Aberta) {
        exibirMensagem("atencao", "Pedido Concluído", "Só é possível excluir itens de Pedido em Aberto!");
        return;
    }
    if (_pedidoVendaItens.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return;
    }
    if (_pedidoVendaItens.TipoItem.val() == EnumProdutoServico.Produto && _pedidoVendaItens.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return;
    }
    if (_pedidoVendaItens.TipoItem.val() == EnumProdutoServico.Servico && _pedidoVendaItens.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return;
    }

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && _bloquearDataEntregaDiferenteAtual && _pedidoVenda.Codigo.val() > 0) {
        exibirMensagem(tipoMensagem.aviso, "Permissão de Exclusão", "Seu usuário não possui permissão para excluir itens do pedido.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o item " + data.DescricaoItem + "?", function () {
        $.each(_pedidoVenda.ListaItens.list, function (i, listaItens) {
            if (data.Codigo == listaItens.Codigo.val) {
                RemoverValorItemTotalizador(data);
                _pedidoVenda.ListaItens.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridListaItens();
    });
}

function SalvarItemClick() {
    var valido = ValidarCamposObrigatorios(_pedidoVendaItens);
    if (valido) {
        if (_pedidoVendaItens.Codigo.val() > "0" && _pedidoVendaItens.Codigo.val() != undefined) {
            for (var i = 0; i < _pedidoVenda.ListaItens.list.length; i++) {
                if (_pedidoVendaItens.Codigo.val() == _pedidoVenda.ListaItens.list[i].Codigo.val) {
                    _pedidoVenda.ListaItens.list.splice(i, 1);
                    break;
                }
            }
        }
        _pedidoVendaItens.Codigo.val(guid());

        var listaItensGrid = new ItemMap();

        listaItensGrid.Codigo.val = _pedidoVendaItens.Codigo.val();
        listaItensGrid.Produto.val = _pedidoVendaItens.Produto.codEntity();
        listaItensGrid.Servico.val = _pedidoVendaItens.Servico.codEntity();
        listaItensGrid.CodigoPedidoVenda.val = _pedidoVenda.Codigo.val();
        listaItensGrid.CodigoItem.val = _pedidoVendaItens.CodigoItem.val();
        listaItensGrid.CodigoNCM.val = _pedidoVendaItens.CodigoNCM.val();        
        listaItensGrid.NumeroOrdemCompra.val = _pedidoVendaItens.NumeroOrdemCompra.val();
        listaItensGrid.NumeroItemOrdemCompra.val = _pedidoVendaItens.NumeroItemOrdemCompra.val();
        listaItensGrid.DescricaoItem.val = _pedidoVendaItens.DescricaoItem.val();
        listaItensGrid.Quantidade.val = _pedidoVendaItens.Quantidade.val();
        listaItensGrid.ValorUnitario.val = _pedidoVendaItens.ValorUnitario.val();
        listaItensGrid.ValorTotalItem.val = _pedidoVendaItens.ValorTotalItem.val();

        _pedidoVenda.ListaItens.list.push(listaItensGrid);

        //_pedidoVenda.ListaItens.list.push(SalvarListEntity(_pedidoVendaItens));
        recarregarGridListaItens();
        AtualizarTotalizadores();
        limparCamposPedidoVendaItens();
        $("#" + _pedidoVendaItens.Produto.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function GravarPedidoVendaItensClick(e, sender) {
    if (_pedidoVendaItens.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return
    }
    if (_pedidoVendaItens.TipoItem.val() == EnumProdutoServico.Produto && _pedidoVendaItens.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return
    }
    if (_pedidoVendaItens.TipoItem.val() == EnumProdutoServico.Servico && _pedidoVendaItens.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return
    }

    _pedidoVenda.FormaPagamento.val(_pedidoVenda.ParcelamentoPedido.FormaPagamento.val());
    _pedidoVenda.CondicaoPagamentoPadrao.codEntity(_pedidoVenda.ParcelamentoPedido.CondicaoPagamentoPadrao.codEntity());
    _pedidoVenda.CondicaoPagamentoPadrao.val(_pedidoVenda.ParcelamentoPedido.CondicaoPagamentoPadrao.val());

    Salvar(_pedidoVenda, "PedidoVenda/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                $("#" + _etapaPedidoVenda.Etapa1.idTab).click();

                editarPedidoVenda(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, function () {
        $("#" + _etapaPedidoVenda.Etapa1.idTab).click();
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    });
}

//*******MÉTODOS*******

function recarregarGridListaItens() {

    var data = new Array();

    $.each(_pedidoVenda.ListaItens.list, function (i, listaItens) {
        var listaItensGrid = new Object();

        listaItensGrid.Codigo = listaItens.Codigo.val;
        listaItensGrid.Produto = listaItens.Produto.val;
        listaItensGrid.Servico = listaItens.Servico.val;
        listaItensGrid.CodigoPedidoVenda = _pedidoVenda.Codigo.val();
        listaItensGrid.CodigoItem = listaItens.CodigoItem.val;
        listaItensGrid.CodigoNCM = listaItens.CodigoNCM.val;
        listaItensGrid.NumeroOrdemCompra = listaItens.NumeroOrdemCompra.val;
        listaItensGrid.NumeroItemOrdemCompra = listaItens.NumeroItemOrdemCompra.val;
        listaItensGrid.DescricaoItem = listaItens.DescricaoItem.val;
        listaItensGrid.Quantidade = listaItens.Quantidade.val;
        listaItensGrid.ValorUnitario = listaItens.ValorUnitario.val;
        listaItensGrid.ValorTotalItem = listaItens.ValorTotalItem.val;

        data.push(listaItensGrid);
    });

    _gridVendaItens.CarregarGrid(data);
}

function CalcularTotalItemPedidoVendaItens() {
    var quantidade = Globalize.parseFloat(_pedidoVendaItens.Quantidade.val());
    var valorUnitario = Globalize.parseFloat(_pedidoVendaItens.ValorUnitario.val());

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _pedidoVendaItens.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
    } else
        _pedidoVendaItens.ValorTotalItem.val(Globalize.format(0, "n2"));
}

function AtualizarTotalizadores() {
    var valorTotalItem = Globalize.parseFloat(_pedidoVendaItens.ValorTotalItem.val());
    var valorTotal = Globalize.parseFloat(_pedidoVendaItens.ValorTotal.val());

    var valorTotalProdutos = 0;
    var valorTotalServicos = 0;
    if (_pedidoVendaItens.Produto.codEntity() > 0) {
        var valorTotalProdutosTotal = Globalize.parseFloat(_pedidoVendaItens.ValorProdutos.val());

        valorTotalProdutos = valorTotalItem + valorTotalProdutosTotal;
        _pedidoVenda.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
    }
    else {
        var valorTotalServicosTotal = Globalize.parseFloat(_pedidoVendaItens.ValorServicos.val());

        valorTotalServicos = valorTotalItem + valorTotalServicosTotal;
        _pedidoVenda.ValorServicos.val(Globalize.format(valorTotalServicos, "n2"));
    }

    valorTotal = valorTotal + valorTotalItem;
    _pedidoVenda.ValorTotal.val(Globalize.format(valorTotal, "n2"));
}

function RemoverValorItemTotalizador(itemPedido) {
    for (var i = 0; i < _pedidoVenda.ListaItens.list.length; i++) {
        if (itemPedido.Codigo == _pedidoVenda.ListaItens.list[i].Codigo.val) {
            var valorTotalItem = Globalize.parseFloat(itemPedido.ValorTotalItem.toString());
            var valorTotal = Globalize.parseFloat(_pedidoVendaItens.ValorTotal.val());

            var valorTotalProdutos = 0;
            var valorTotalServicos = 0;
            if (itemPedido.Produto > 0) {
                var valorTotalProdutosTotal = Globalize.parseFloat(_pedidoVendaItens.ValorProdutos.val());

                valorTotalProdutos = valorTotalProdutosTotal - valorTotalItem;
                _pedidoVendaItens.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
                _pedidoVenda.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
            }
            else {
                var valorTotalServicosTotal = Globalize.parseFloat(_pedidoVendaItens.ValorServicos.val());

                valorTotalServicos = valorTotalServicosTotal - valorTotalItem;
                _pedidoVendaItens.ValorServicos.val(Globalize.format(valorTotalServicos, "n2"));
                _pedidoVenda.ValorServicos.val(Globalize.format(valorTotalServicos, "n2"));
            }

            valorTotal = valorTotal - valorTotalItem;
            _pedidoVendaItens.ValorTotal.val(Globalize.format(valorTotal, "n2"));
            _pedidoVenda.ValorTotal.val(Globalize.format(valorTotal, "n2"));

            break;
        }
    }
}

function limparCamposPedidoVendaItens() {
    LimparCampos(_pedidoVendaItens);
    TipoItemChange();

    _pedidoVendaItens.ValorProdutos.val(_pedidoVenda.ValorProdutos.val());
    _pedidoVendaItens.ValorServicos.val(_pedidoVenda.ValorServicos.val());
    _pedidoVendaItens.ValorTotal.val(_pedidoVenda.ValorTotal.val());
}