/// <reference path="../PedidoVenda/PedidoVendaEtapa.js" />

var _ordemServicoPetItens;
var _gridVendaItens;

var ItemMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Produto = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Servico = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoPedidoVenda = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoNCM = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DescricaoItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Quantidade = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorUnitario = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorTotalItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable("") });
};

var OrdemServicoPetItens = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoItem = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumProdutoServico.Produto), options: EnumProdutoServico.obterOpcoes(), def: EnumProdutoServico.Produto, enable: ko.observable(true), eventChange: tipoItemChange });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(false) });
    this.Quantidade = PropertyEntity({ text: "Quantidade:", def: "", val: ko.observable(""), required: ko.observable(true), getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: _casasQuantidadeProdutoNFe, allowZero: false } });
    this.ValorUnitario = PropertyEntity({ text: "Valor Unitário:", def: "", val: ko.observable(""), required: ko.observable(true), getType: typesKnockout.decimal, maxlength: 21, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: _casasValorProdutoNFe, allowZero: false } });
    this.ValorTotalItem = PropertyEntity({ text: "Valor Total:", def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, val: ko.observable("") });

    this.CodigoItem = PropertyEntity({ text: "*Código Item:", getType: typesKnockout.string, maxlength: 100, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DescricaoItem = PropertyEntity({ text: "*Descrição Item:", getType: typesKnockout.string, maxlength: 500, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoNCM = PropertyEntity({ text: "NCM:", getType: typesKnockout.string, maxlength: 500, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(false) });

    this.ValorProdutos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.ValorServicos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.ValorTotal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(false) });

    this.SalvarItem = PropertyEntity({ eventClick: salvarItemClick, type: types.event, text: "Salvar Item", visible: ko.observable(true), enable: ko.observable(true) });
    this.Gravar = PropertyEntity({ eventClick: gravarOrdemServicoPetItensClick, type: types.event, text: "Gravar orçamento", visible: ko.observable(true), enable: ko.observable(true) });

    this.ItensOrdemServico = PropertyEntity({ type: types.local, id: guid() });
};

function loadOrdemServicoPetItens() {
    _ordemServicoPetItens = new OrdemServicoPetItens();
    KoBindings(_ordemServicoPetItens, "knockoutItensPedido");

    new BuscarProdutoTMS(_ordemServicoPetItens.Produto, function (data) {
        _ordemServicoPetItens.Produto.codEntity(data.Codigo);
        _ordemServicoPetItens.Produto.val(data.Descricao);
        if (data.ValorVenda != null && data.ValorVenda != "") {
            var valorUnitario = Globalize.parseFloat(data.ValorVenda.toString());
            if (valorUnitario > 0)
                _ordemServicoPetItens.ValorUnitario.val(Globalize.format(valorUnitario, "n" + _casasValorProdutoNFe));
            else
                _ordemServicoPetItens.ValorUnitario.val("");
        }
        _ordemServicoPetItens.CodigoNCM.val(data.CodigoNCM);
        _ordemServicoPetItens.DescricaoItem.val(data.Descricao);
        if (data.CodigoProduto != "")
            _ordemServicoPetItens.CodigoItem.val(data.CodigoProduto);
        else
            _ordemServicoPetItens.CodigoItem.val(data.Codigo);
        $("#" + _ordemServicoPetItens.Quantidade.id).focus();
    }, null, null, null, null, true);

    new BuscarServicoTMS(_ordemServicoPetItens.Servico, function (data) {
        _ordemServicoPetItens.Servico.codEntity(data.Codigo);
        _ordemServicoPetItens.Servico.val(data.Descricao);
        if (data.ValorVenda != null && data.ValorVenda != "") {
            var valorUnitario = Globalize.parseFloat(data.ValorVenda.toString());
            if (valorUnitario > 0)
                _ordemServicoPetItens.ValorUnitario.val(Globalize.format(valorUnitario, "n" + _casasValorProdutoNFe));
            else
                _ordemServicoPetItens.ValorUnitario.val("");
        }
        _ordemServicoPetItens.CodigoNCM.val("");
        _ordemServicoPetItens.DescricaoItem.val(data.Descricao);
        _ordemServicoPetItens.CodigoItem.val(data.Codigo);
        $("#" + _ordemServicoPetItens.Quantidade.id).focus();
    });

    var editarItem = { descricao: "Editar", id: guid(), metodo: editarVendaItemClick, icone: "" };
    var excluirItem = { descricao: "Excluir", id: guid(), metodo: excluirVendaItemClick, icone: "" };

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

    _gridVendaItens = new BasicDataTable(_ordemServicoPetItens.ItensOrdemServico.id, header, menuOpcoes);

    recarregarGridListaItens();

}

function salvarItemClick() {

    var valido = ValidarCamposObrigatorios(_ordemServicoPetItens);
    if (!valido)
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");

    var codigoItem = _ordemServicoPetItens.Codigo.val()
    if (codigoItem != undefined && codigoItem > "0") {
        for (var i = 0; i < _ordemServicoPet.ListaItens.list.length; i++) {
            if (codigoItem == _ordemServicoPet.ListaItens.list[i].Codigo.val) {
                _ordemServicoPet.ListaItens.list.splice(i, 1);
                break;
            }
        }
    }
    _ordemServicoPetItens.Codigo.val(guid());

    var listaItensGrid = new ItemMap();

    listaItensGrid.Codigo.val = _ordemServicoPetItens.Codigo.val();
    listaItensGrid.Produto.val = _ordemServicoPetItens.Produto.codEntity();
    listaItensGrid.Servico.val = _ordemServicoPetItens.Servico.codEntity();
    listaItensGrid.CodigoPedidoVenda.val = _ordemServicoPet.Codigo.val();
    listaItensGrid.CodigoItem.val = _ordemServicoPetItens.CodigoItem.val();
    listaItensGrid.CodigoNCM.val = _ordemServicoPetItens.CodigoNCM.val();
    listaItensGrid.DescricaoItem.val = _ordemServicoPetItens.DescricaoItem.val();
    listaItensGrid.Quantidade.val = _ordemServicoPetItens.Quantidade.val();
    listaItensGrid.ValorUnitario.val = _ordemServicoPetItens.ValorUnitario.val();
    listaItensGrid.ValorTotalItem.val = _ordemServicoPetItens.ValorTotalItem.val();
    listaItensGrid.Observacao.val = _ordemServicoPetItens.Observacao.val();

    _ordemServicoPet.ListaItens.list.push(listaItensGrid);

    recarregarGridListaItens();
    atualizarTotalizadores();
    limparCampos();
    $("#" + _ordemServicoPetItens.Produto.id).focus();

}

function editarVendaItemClick(data) {
    if (_ordemServicoPet.Status.val() != EnumStatusPedidoVenda.Aberta) {
        exibirMensagem("atencao", "Pedido Concluído", "Só é possível editar itens de Pedido em Aberto!");
        return;
    }
    if (_ordemServicoPetItens.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return;
    }
    if (_ordemServicoPetItens.TipoItem.val() == EnumProdutoServico.Produto && _ordemServicoPetItens.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return;
    }
    if (_ordemServicoPetItens.TipoItem.val() == EnumProdutoServico.Servico && _ordemServicoPetItens.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return;
    }

    limparCampos();
    removerValorItemTotalizador(data);

    if (data.Servico > 0)
        _ordemServicoPetItens.TipoItem.val(EnumProdutoServico.Servico);
    else if (data.Produto > 0)
        _ordemServicoPetItens.TipoItem.val(EnumProdutoServico.Produto);
    tipoItemChange();

    _ordemServicoPetItens.Codigo.val(data.Codigo);
    _ordemServicoPetItens.Produto.codEntity(data.Produto);
    _ordemServicoPetItens.Produto.val(data.DescricaoItem);
    _ordemServicoPetItens.Servico.codEntity(data.Servico);
    _ordemServicoPetItens.Servico.val(data.DescricaoItem);
    _ordemServicoPetItens.CodigoNCM.val(data.CodigoNCM);
    _ordemServicoPetItens.CodigoItem.val(data.CodigoItem);
    _ordemServicoPetItens.DescricaoItem.val(data.DescricaoItem);
    _ordemServicoPetItens.Quantidade.val(data.Quantidade);
    _ordemServicoPetItens.ValorUnitario.val(data.ValorUnitario);
    _ordemServicoPetItens.ValorTotalItem.val(data.ValorTotalItem);
    _ordemServicoPetItens.Observacao.val(data.Observacao);
}

function removerValorItemTotalizador(itemPedido) {
    for (var i = 0; i < _ordemServicoPet.ListaItens.list.length; i++) {
        if (itemPedido.Codigo == _ordemServicoPet.ListaItens.list[i].Codigo.val) {
            var valorTotalItem = Globalize.parseFloat(itemPedido.ValorTotalItem.toString());
            var valorTotal = Globalize.parseFloat(_ordemServicoPet.ValorTotal.val());

            var valorTotalProdutos = 0;
            var valorTotalServicos = 0;
            if (itemPedido.Produto > 0) {
                var valorTotalProdutosTotal = Globalize.parseFloat(_ordemServicoPet.ValorProdutos.val());

                valorTotalProdutos = valorTotalProdutosTotal - valorTotalItem;
                _ordemServicoPet.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
            }
            else {
                var valorTotalServicosTotal = Globalize.parseFloat(_ordemServicoPet.ValorServicos.val());

                valorTotalServicos = valorTotalServicosTotal - valorTotalItem;
                _ordemServicoPet.ValorServicos.val(Globalize.format(valorTotalServicos, "n2"));
            }

            valorTotal = valorTotal - valorTotalItem;
            _ordemServicoPet.ValorTotal.val(Globalize.format(valorTotal, "n2"));

            break;
        }
    }
}

function excluirVendaItemClick(data) {
    if (_ordemServicoPet.Status.val() != EnumStatusPedidoVenda.Aberta) {
        exibirMensagem("atencao", "Pedido Concluído", "Só é possível excluir itens de Pedido em Aberto!");
        return;
    }
    if (_ordemServicoPetItens.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return;
    }
    if (_ordemServicoPetItens.TipoItem.val() == EnumProdutoServico.Produto && _ordemServicoPetItens.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return;
    }
    if (_ordemServicoPetItens.TipoItem.val() == EnumProdutoServico.Servico && _ordemServicoPetItens.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return;
    }

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && _bloquearDataEntregaDiferenteAtual && _ordemServicoPet.Codigo.val() > 0) {
        exibirMensagem(tipoMensagem.aviso, "Permissão de Exclusão", "Seu usuário não possui permissão para excluir itens do pedido.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o item " + data.DescricaoItem + "?", function () {
        $.each(_ordemServicoPet.ListaItens.list, function (i, listaItens) {
            if (data.Codigo == listaItens.Codigo.val) {
                removerValorItemTotalizador(data);
                _ordemServicoPet.ListaItens.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridListaItens();
    });
}

function gravarOrdemServicoPetItensClick(e, sender) {
    if (_ordemServicoPetItens.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return
    }
    if (_ordemServicoPetItens.TipoItem.val() == EnumProdutoServico.Produto && _ordemServicoPetItens.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return
    }
    if (_ordemServicoPetItens.TipoItem.val() == EnumProdutoServico.Servico && _ordemServicoPetItens.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return
    }

    let fotoAux = _ordemServicoPet.FotoPet.val();
    _ordemServicoPet.FotoPet.val('')

    Salvar(_ordemServicoPet, "OrdemServicoPet/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                $("#" + _etapaPedidoVenda.Etapa1.idTab).click();

                editarOrdemServico(arg.Data);
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

    _ordemServicoPet.FotoPet.val(fotoAux);
}

function calcularTotalItemPedidoVendaItens() {
    var quantidade = Globalize.parseFloat(_ordemServicoPetItens.Quantidade.val());
    var valorUnitario = Globalize.parseFloat(_ordemServicoPetItens.ValorUnitario.val());

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _ordemServicoPetItens.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
    } else
        _ordemServicoPetItens.ValorTotalItem.val(Globalize.format(0, "n2"));
}

function atualizarTotalizadores() {
    var valorTotalItem = Globalize.parseFloat(_ordemServicoPetItens.ValorTotalItem.val());
    var valorTotal = Globalize.parseFloat(_ordemServicoPet.ValorTotal.val());

    var valorTotalProdutos = 0;
    var valorTotalServicos = 0;
    if (_ordemServicoPetItens.Produto.codEntity() > 0) {
        var valorTotalProdutosTotal = Globalize.parseFloat(_ordemServicoPet.ValorProdutos.val());

        valorTotalProdutos = valorTotalItem + valorTotalProdutosTotal;
        _ordemServicoPet.ValorProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
    }
    else {
        var valorTotalServicosTotal = Globalize.parseFloat(_ordemServicoPet.ValorServicos.val());

        valorTotalServicos = valorTotalItem + valorTotalServicosTotal;
        _ordemServicoPet.ValorServicos.val(Globalize.format(valorTotalServicos, "n2"));
    }

    valorTotal = valorTotal + valorTotalItem;
    _ordemServicoPet.ValorTotal.val(Globalize.format(valorTotal, "n2"));
}

function tipoItemChange() {

    if (_ordemServicoPetItens.TipoItem.val() == EnumProdutoServico.Produto) {
        _ordemServicoPetItens.Produto.required(true);
        _ordemServicoPetItens.Servico.required(false);
        _ordemServicoPetItens.Produto.visible(true);
        _ordemServicoPetItens.Servico.visible(false);
        LimparCampoEntity(_ordemServicoPetItens.Servico);
    } else if (_ordemServicoPetItens.TipoItem.val() == EnumProdutoServico.Servico) {
        _ordemServicoPetItens.Produto.required(false);
        _ordemServicoPetItens.Servico.required(true);
        _ordemServicoPetItens.Produto.visible(false);
        _ordemServicoPetItens.Servico.visible(true);
        LimparCampoEntity(_ordemServicoPetItens.Produto);
    }
}

function recarregarGridListaItens() {

    var data = new Array();

    $.each(_ordemServicoPet.ListaItens.list, function (i, listaItens) {
        var listaItensGrid = new Object();

        listaItensGrid.Codigo = listaItens.Codigo.val;
        listaItensGrid.Produto = listaItens.Produto.val;
        listaItensGrid.Servico = listaItens.Servico.val;
        listaItensGrid.CodigoPedidoVenda = _ordemServicoPet.Codigo.val();
        listaItensGrid.CodigoItem = listaItens.CodigoItem.val;
        listaItensGrid.CodigoNCM = listaItens.CodigoNCM.val;
        listaItensGrid.DescricaoItem = listaItens.DescricaoItem.val;
        listaItensGrid.Quantidade = listaItens.Quantidade.val;
        listaItensGrid.ValorUnitario = listaItens.ValorUnitario.val;
        listaItensGrid.ValorTotalItem = listaItens.ValorTotalItem.val;
        listaItensGrid.Observacao = listaItens.Observacao.val;

        data.push(listaItensGrid);
    });

    _gridVendaItens.CarregarGrid(data);
}

function limparCampos() {
    LimparCampos(_ordemServicoPetItens);
    tipoItemChange();

    _ordemServicoPetItens.ValorProdutos.val(_ordemServicoPet.ValorProdutos.val());
    _ordemServicoPetItens.ValorServicos.val(_ordemServicoPet.ValorServicos.val());
    _ordemServicoPetItens.ValorTotal.val(_ordemServicoPet.ValorTotal.val());
}