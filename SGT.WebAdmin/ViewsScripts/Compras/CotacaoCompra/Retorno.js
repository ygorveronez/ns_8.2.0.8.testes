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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/CondicaoPagamento.js" />
/// <reference path="Mercadoria.js" />

var _retornoCotacaoCompra;
var _retornoProdutoFornecedor;
var _gridRetorno;
var _gridRetornoProdutoFornecedor;
var _modalRetornoMercadoriaCotacaoCompra;

var RetornoProdutoFornecedorMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.string });
    this.CodigoCotacao = PropertyEntity({ type: types.map, val: "" });
    this.CodigoFornecedor = PropertyEntity({ type: types.map, val: "" });
    this.CodigoCondicaoPagamento = PropertyEntity({ type: types.map, val: "" });
    this.CodigoProduto = PropertyEntity({ type: types.map, val: "" });
    this.CodigoMercadoria = PropertyEntity({ type: types.map, val: "" });

    this.QuantidadeOriginal = PropertyEntity({ type: types.map, val: "" });
    this.ValorUnitarioOriginal = PropertyEntity({ type: types.map, val: "" });
    this.ValorTotalItemOriginal = PropertyEntity({ type: types.map, val: "" });

    this.Fornecedor = PropertyEntity({ type: types.map, val: "" });
    this.CondicaoPagamento = PropertyEntity({ type: types.map, val: "" });
    this.Observacao = PropertyEntity({ type: types.map, val: "" });
    this.Produto = PropertyEntity({ type: types.map, val: "" });
    this.Marca = PropertyEntity({ type: types.map, val: "" });

    this.QuantidadeRetorno = PropertyEntity({ type: types.map, val: "" });
    this.ValorUnitarioRetorno = PropertyEntity({ type: types.map, val: "" });
    this.ValorTotalRetorno = PropertyEntity({ type: types.map, val: "" });
    this.GerarOrdemCompra = PropertyEntity({ type: types.map, val: "" });
    this.BollGerarOrdemCompra = PropertyEntity({ type: types.map, val: "" });
};

var CodigoFornecedorMap = function () {
    this.CodigoFornecedor = PropertyEntity({ type: types.map, val: "" });
};

var RetornoCotacaoCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ItensCotacao = PropertyEntity({ type: types.local });
};

var RetornoProdutoFornecedor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCotacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoProduto = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.CodigoMercadoria = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.CodigoFornecedor = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.ListaFornecedoresRetorno = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Produto = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.QuantidadeOriginal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ValorUnitarioOriginal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ValorTotalItemOriginal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Fornecedor:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.CondicaoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Condição Pagamento:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 2000, enable: ko.observable(true), required: false });
    this.Marca = PropertyEntity({ text: "Marca:", maxlength: 500, enable: ko.observable(true), required: false });

    this.Quantidade = PropertyEntity({ getType: typesKnockout.decimal, required: ko.observable(true), text: ko.observable("*Quantidade:"), maxlength: 10, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ getType: typesKnockout.decimal, required: ko.observable(true), text: ko.observable("*Val. Unit.:"), maxlength: 18, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: false, allowNegative: false } });
    this.ValorTotalItem = PropertyEntity({ getType: typesKnockout.decimal, required: ko.observable(false), text: ko.observable("*Total:"), maxlength: 10, visible: ko.observable(true), enable: false });
    this.GerarOrdemCompra = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar O.C. para este retorno?", enable: ko.observable(true) });

    this.SalvarItem = PropertyEntity({ type: types.event, eventClick: SalvarItemRetornoClick, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Fechar = PropertyEntity({ type: types.event, eventClick: FecharItemRetornoClick, text: "Fechar", visible: ko.observable(true) });

    this.RetornoMercadoria = PropertyEntity({ type: types.local });
};

//*******EVENTOS*******

function loadRetornoCotacaoCompra() {
    _retornoCotacaoCompra = new RetornoCotacaoCompra();
    KoBindings(_retornoCotacaoCompra, "knockoutCotacao_Retorno");

    $.get("Content/Static/Compras/CotacaoModais.html?dyn=" + guid(), function (htmlCotacaoModais) {
        $("#CotacaoModais").html(htmlCotacaoModais);

        _retornoProdutoFornecedor = new RetornoProdutoFornecedor();
        KoBindings(_retornoProdutoFornecedor, "knoutRetornoMercadoria");

        new BuscarClientes(_retornoProdutoFornecedor.Fornecedor, null, null, null, null, null, null, null, null, null, null, _retornoProdutoFornecedor.ListaFornecedoresRetorno);
        new BuscarCondicaoPagamento(_retornoProdutoFornecedor.CondicaoPagamento);

        CarregarRetornosCotacao();
        CarregarRetornoProdutoFornecedor();

        _modalRetornoMercadoriaCotacaoCompra = new bootstrap.Modal(document.getElementById("divRetornoMercadoria"), { backdrop: 'static', keyboard: true });
    });
}

function PreencherListaFornecedoresRetorno() {
    _retornoProdutoFornecedor.ListaFornecedoresRetorno.list = new Array();

    $.each(_cotacaoCompra.ListaFornecedor.list, function (i, fornecedor) {
        var codigoFornecedor = new CodigoFornecedorMap();
        codigoFornecedor.CodigoFornecedor.val = fornecedor.CodigoFornecedor.val;

        _retornoProdutoFornecedor.ListaFornecedoresRetorno.list.push(codigoFornecedor);
    });
}

function CarregarRetornosCotacao() {
    var selecionar = { descricao: "Selecionar", id: guid(), evento: "onclick", metodo: SelecionarRetornoCotacaoClick, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [selecionar], descricao: "Opções", tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCotacao", visible: false },
        { data: "CodigoProduto", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%" },
        { data: "Quantidade", title: "Qtd.", width: "10%" },
        { data: "ValorUnitario", title: "Val. Unit.", width: "10%" },
        { data: "ValorTotal", title: "Val. Total", width: "10%" }
    ];

    _gridRetorno = new BasicDataTable(_retornoCotacaoCompra.ItensCotacao.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });
    RegarregarGridRetornos();
}

function CarregarRetornoProdutoFornecedor() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarRetornoProdutoFornecedorClick, tamanho: "10", icone: "" };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: ExcluirRetornoProdutoFornecedorClick, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, excluir], descricao: "Opções", tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCotacao", visible: false },
        { data: "CodigoMercadoria", visible: false },
        { data: "CodigoProduto", visible: false },
        { data: "CodigoCondicaoPagamento", visible: false },
        { data: "CodigoFornecedor", visible: false },
        { data: "Observacao", visible: false },
        { data: "CondicaoPagamento", visible: false },
        { data: "BollGerarOrdemCompra", visible: false },
        { data: "Fornecedor", title: "Fornecedor", width: "20%" },
        { data: "Produto", title: "Produto", width: "20%" },
        { data: "Marca", title: "Marca", width: "10%" },
        { data: "QuantidadeOriginal", title: "Qtd.", width: "8%" },
        { data: "QuantidadeRetorno", title: "Qtd. Ret.", width: "8%" },
        { data: "ValorUnitarioOriginal", title: "Val. Unit.", width: "8%" },
        { data: "ValorUnitarioRetorno", title: "Val. Unit. Ret.", width: "8%" },
        { data: "ValorTotalItemOriginal", title: "Val. Total", width: "10%" },
        { data: "ValorTotalRetorno", title: "Val. Total Ret", width: "10%" },
        { data: "GerarOrdemCompra", title: "Gera O.C.?", width: "8%" }
    ];

    _gridRetornoProdutoFornecedor = new BasicDataTable(_retornoProdutoFornecedor.RetornoMercadoria.id, header, menuOpcoes, { column: 4, dir: orderDir.asc });
    RegarregarGridRetornoProdutoFornecedor();
}

function RegarregarGridRetornos() {
    var data = new Array();

    $.each(_cotacaoCompra.ListaMercadoria.list, function (i, listaMercadoria) {
        var listaMercadoriaGrid = new Object();

        listaMercadoriaGrid.Codigo = listaMercadoria.Codigo.val;
        listaMercadoriaGrid.CodigoCotacao = listaMercadoria.CodigoCotacao.val;
        listaMercadoriaGrid.CodigoProduto = listaMercadoria.CodigoProduto.val;
        listaMercadoriaGrid.Descricao = listaMercadoria.Descricao.val;
        listaMercadoriaGrid.Quantidade = listaMercadoria.Quantidade.val;
        listaMercadoriaGrid.ValorUnitario = listaMercadoria.ValorUnitario.val;
        listaMercadoriaGrid.ValorTotal = listaMercadoria.ValorTotal.val;

        data.push(listaMercadoriaGrid);
    });

    _gridRetorno.CarregarGrid(data);
}

function RegarregarGridRetornoProdutoFornecedor() {
    var data = new Array();

    $.each(_cotacaoCompra.ListaRetornoProdutoFornecedor.list, function (i, listaMercadoria) {
        if (listaMercadoria.CodigoProduto.val == _retornoProdutoFornecedor.CodigoProduto.val()) {
            var listaMercadoriaGrid = new Object();

            listaMercadoriaGrid.Codigo = listaMercadoria.Codigo.val;
            listaMercadoriaGrid.CodigoCotacao = listaMercadoria.CodigoCotacao.val;
            listaMercadoriaGrid.CodigoProduto = listaMercadoria.CodigoProduto.val;
            listaMercadoriaGrid.CodigoCondicaoPagamento = listaMercadoria.CodigoCondicaoPagamento.val;
            listaMercadoriaGrid.CodigoFornecedor = listaMercadoria.CodigoFornecedor.val;
            listaMercadoriaGrid.Fornecedor = listaMercadoria.Fornecedor.val;
            listaMercadoriaGrid.Produto = listaMercadoria.Produto.val;
            listaMercadoriaGrid.QuantidadeOriginal = listaMercadoria.QuantidadeOriginal.val;
            listaMercadoriaGrid.QuantidadeRetorno = listaMercadoria.QuantidadeRetorno.val;
            listaMercadoriaGrid.ValorUnitarioOriginal = listaMercadoria.ValorUnitarioOriginal.val;
            listaMercadoriaGrid.ValorUnitarioRetorno = listaMercadoria.ValorUnitarioRetorno.val;
            listaMercadoriaGrid.ValorTotalItemOriginal = listaMercadoria.ValorTotalItemOriginal.val;
            listaMercadoriaGrid.ValorTotalRetorno = listaMercadoria.ValorTotalRetorno.val;
            listaMercadoriaGrid.BollGerarOrdemCompra = listaMercadoria.BollGerarOrdemCompra.val;
            listaMercadoriaGrid.GerarOrdemCompra = listaMercadoria.BollGerarOrdemCompra.val ? "Sim" : "Não";
            listaMercadoriaGrid.CondicaoPagamento = listaMercadoria.CondicaoPagamento.val;
            listaMercadoriaGrid.Observacao = listaMercadoria.Observacao.val;
            listaMercadoriaGrid.CodigoMercadoria = listaMercadoria.CodigoMercadoria.val;
            listaMercadoriaGrid.Marca = listaMercadoria.Marca.val;

            data.push(listaMercadoriaGrid);
        }
    });

    _gridRetornoProdutoFornecedor.CarregarGrid(data);
}


function SelecionarRetornoCotacaoClick(data) {
    LimparCamposRetornoProdutoFornecedor();
    PreencherListaFornecedoresRetorno();
    _modalRetornoMercadoriaCotacaoCompra.show();

    _retornoProdutoFornecedor.CodigoProduto.val(data.CodigoProduto);
    _retornoProdutoFornecedor.CodigoMercadoria.val(data.Codigo);
    _retornoProdutoFornecedor.CodigoCotacao.val(data.CodigoCotacao);
    _retornoProdutoFornecedor.QuantidadeOriginal.val(data.Quantidade);
    _retornoProdutoFornecedor.ValorUnitarioOriginal.val(data.ValorUnitario);
    _retornoProdutoFornecedor.ValorTotalItemOriginal.val(data.ValorTotal);
    _retornoProdutoFornecedor.Produto.val(data.Descricao);

    RegarregarGridRetornoProdutoFornecedor();
}

function FecharItemRetornoClick(e, sender) {
    LimparCamposRetornoProdutoFornecedor();
    _modalRetornoMercadoriaCotacaoCompra.hide();
}

function ValidarFornecedorSelecionado() {
    var fornecedorEncontrado = false;
    $.each(_cotacaoCompra.ListaFornecedor.list, function (i, listaFornecedor) {
        if (listaFornecedor.CodigoFornecedor.val == _retornoProdutoFornecedor.Fornecedor.codEntity())
            fornecedorEncontrado = true;
    });

    return fornecedorEncontrado;
}

function SalvarItemRetornoClick() {
    var tudoCerto = ValidarCamposObrigatorios(_retornoProdutoFornecedor);

    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var fornecedorLancadoAnterirormente = ValidarFornecedorSelecionado();
    if (!fornecedorLancadoAnterirormente) {
        _retornoProdutoFornecedor.Fornecedor.requiredClass("form-control is-invalid");
        exibirMensagem(tipoMensagem.atencao, "Fornecedor Inválido", "Fornecedor selecionado não foi localizado na lista anterior!");
        return;
    }

    if (_retornoProdutoFornecedor.Codigo.val() == "") {
        //novo
        var retornoProdutoFornecedor = new RetornoProdutoFornecedorMap();
        _retornoProdutoFornecedor.Codigo.val(guid());

        retornoProdutoFornecedor.Codigo.val = _retornoProdutoFornecedor.Codigo.val();
        retornoProdutoFornecedor.CodigoCotacao.val = _retornoProdutoFornecedor.CodigoCotacao.val();
        retornoProdutoFornecedor.CodigoFornecedor.val = _retornoProdutoFornecedor.Fornecedor.codEntity();
        retornoProdutoFornecedor.CodigoCondicaoPagamento.val = _retornoProdutoFornecedor.CondicaoPagamento.codEntity();
        retornoProdutoFornecedor.CodigoProduto.val = _retornoProdutoFornecedor.CodigoProduto.val();
        retornoProdutoFornecedor.CodigoMercadoria.val = _retornoProdutoFornecedor.CodigoMercadoria.val();

        retornoProdutoFornecedor.QuantidadeOriginal.val = _retornoProdutoFornecedor.QuantidadeOriginal.val();
        retornoProdutoFornecedor.ValorUnitarioOriginal.val = _retornoProdutoFornecedor.ValorUnitarioOriginal.val();
        retornoProdutoFornecedor.ValorTotalItemOriginal.val = _retornoProdutoFornecedor.ValorTotalItemOriginal.val();

        retornoProdutoFornecedor.Produto.val = _retornoProdutoFornecedor.Produto.val();
        retornoProdutoFornecedor.Fornecedor.val = _retornoProdutoFornecedor.Fornecedor.val();
        retornoProdutoFornecedor.CondicaoPagamento.val = _retornoProdutoFornecedor.CondicaoPagamento.val();
        retornoProdutoFornecedor.Observacao.val = _retornoProdutoFornecedor.Observacao.val();

        retornoProdutoFornecedor.QuantidadeRetorno.val = _retornoProdutoFornecedor.Quantidade.val();
        retornoProdutoFornecedor.ValorUnitarioRetorno.val = _retornoProdutoFornecedor.ValorUnitario.val();
        retornoProdutoFornecedor.ValorTotalRetorno.val = _retornoProdutoFornecedor.ValorTotalItem.val();
        retornoProdutoFornecedor.BollGerarOrdemCompra.val = _retornoProdutoFornecedor.GerarOrdemCompra.val();
        if (_retornoProdutoFornecedor.GerarOrdemCompra.val() === true)
            retornoProdutoFornecedor.GerarOrdemCompra.val = "Sim";
        else
            retornoProdutoFornecedor.GerarOrdemCompra.val = "Não";

        retornoProdutoFornecedor.CondicaoPagamento.val = _retornoProdutoFornecedor.CondicaoPagamento.val();
        retornoProdutoFornecedor.Observacao.val = _retornoProdutoFornecedor.Observacao.val();
        retornoProdutoFornecedor.CodigoMercadoria.val = _retornoProdutoFornecedor.CodigoMercadoria.val();
        retornoProdutoFornecedor.Marca.val = _retornoProdutoFornecedor.Marca.val();

        _cotacaoCompra.ListaRetornoProdutoFornecedor.list.push(retornoProdutoFornecedor);
    } else {
        //editando
        $.each(_cotacaoCompra.ListaRetornoProdutoFornecedor.list, function (i, fornecedor) {
            if (fornecedor.Codigo.val === _retornoProdutoFornecedor.Codigo.val()) {

                fornecedor.CodigoCotacao.val = _retornoProdutoFornecedor.CodigoCotacao.val();
                fornecedor.CodigoFornecedor.val = _retornoProdutoFornecedor.Fornecedor.codEntity();
                fornecedor.CodigoCondicaoPagamento.val = _retornoProdutoFornecedor.CondicaoPagamento.codEntity();
                fornecedor.CodigoProduto.val = _retornoProdutoFornecedor.CodigoProduto.val();
                fornecedor.CodigoMercadoria.val = _retornoProdutoFornecedor.CodigoMercadoria.val();

                fornecedor.QuantidadeOriginal.val = _retornoProdutoFornecedor.QuantidadeOriginal.val();
                fornecedor.ValorUnitarioOriginal.val = _retornoProdutoFornecedor.ValorUnitarioOriginal.val();
                fornecedor.ValorTotalItemOriginal.val = _retornoProdutoFornecedor.ValorTotalItemOriginal.val();

                fornecedor.Fornecedor.val = _retornoProdutoFornecedor.Fornecedor.val();
                fornecedor.CondicaoPagamento.val = _retornoProdutoFornecedor.CondicaoPagamento.val();
                fornecedor.Observacao.val = _retornoProdutoFornecedor.Observacao.val();

                fornecedor.QuantidadeRetorno.val = _retornoProdutoFornecedor.Quantidade.val();
                fornecedor.ValorUnitarioRetorno.val = _retornoProdutoFornecedor.ValorUnitario.val();
                fornecedor.ValorTotalRetorno.val = _retornoProdutoFornecedor.ValorTotalItem.val();
                fornecedor.BollGerarOrdemCompra.val = _retornoProdutoFornecedor.GerarOrdemCompra.val();
                if (_retornoProdutoFornecedor.GerarOrdemCompra.val() === true)
                    fornecedor.GerarOrdemCompra.val = "Sim";
                else
                    fornecedor.GerarOrdemCompra.val = "Não";

                fornecedor.CondicaoPagamento.val = _retornoProdutoFornecedor.CondicaoPagamento.val();
                fornecedor.Observacao.val = _retornoProdutoFornecedor.Observacao.val();
                fornecedor.CodigoMercadoria.val = _retornoProdutoFornecedor.CodigoMercadoria.val();
                fornecedor.Marca.val = _retornoProdutoFornecedor.Marca.val();

                return false;
            }
        });
    }

    RegarregarGridRetornoProdutoFornecedor();
    LimparCamposRetornoProdutoFornecedor();
}

function EditarRetornoProdutoFornecedorClick(data) {
    _retornoProdutoFornecedor.Codigo.val(data.Codigo);
    _retornoProdutoFornecedor.CodigoCotacao.val(data.CodigoCotacao);
    _retornoProdutoFornecedor.Fornecedor.codEntity(data.CodigoFornecedor);
    _retornoProdutoFornecedor.CondicaoPagamento.codEntity(data.CodigoCondicaoPagamento);
    _retornoProdutoFornecedor.CodigoProduto.val(data.CodigoProduto);
    _retornoProdutoFornecedor.CodigoMercadoria.val(data.CodigoMercadoria);

    _retornoProdutoFornecedor.QuantidadeOriginal.val(data.QuantidadeOriginal);
    _retornoProdutoFornecedor.ValorUnitarioOriginal.val(data.ValorUnitarioOriginal);
    _retornoProdutoFornecedor.ValorTotalItemOriginal.val(data.ValorTotalItemOriginal);

    _retornoProdutoFornecedor.Fornecedor.val(data.Fornecedor);
    _retornoProdutoFornecedor.CondicaoPagamento.val(data.CondicaoPagamento);
    _retornoProdutoFornecedor.Observacao.val(data.Observacao);
    _retornoProdutoFornecedor.Marca.val(data.Marca);

    _retornoProdutoFornecedor.Quantidade.val(data.QuantidadeRetorno);
    _retornoProdutoFornecedor.ValorUnitario.val(data.ValorUnitarioRetorno);
    _retornoProdutoFornecedor.ValorTotalItem.val(data.ValorTotalRetorno);
    //_retornoProdutoFornecedor.BollGerarOrdemCompra.val(data.BollGerarOrdemCompra);
    _retornoProdutoFornecedor.GerarOrdemCompra.val(data.BollGerarOrdemCompra);
}

function ExcluirRetornoProdutoFornecedorClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o retorno selecionado?", function () {
        var listaAtualizada = new Array();
        $.each(_cotacaoCompra.ListaRetornoProdutoFornecedor.list, function (i, retorno) {
            if (retorno.Codigo.val != data.Codigo) {
                listaAtualizada.push(retorno);
            }
        });
        _cotacaoCompra.ListaRetornoProdutoFornecedor.list = listaAtualizada;

        RegarregarGridRetornoProdutoFornecedor();
        LimparCamposRetornoProdutoFornecedor();
    });
}

function CalcularTotalItemRetorno(e, sender) {
    var quantidade = parseFloat(_retornoProdutoFornecedor.Quantidade.val().toString().replace(".", "").replace(",", ".")).toFixed(2);
    quantidade = parseFloat(quantidade);

    var valorUnitario = parseFloat(_retornoProdutoFornecedor.ValorUnitario.val().toString().replace(".", "").replace(",", ".")).toFixed(2);
    valorUnitario = parseFloat(valorUnitario);

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _retornoProdutoFornecedor.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
    }
}

function LimparCamposRetornoProdutoFornecedor() {
    _retornoProdutoFornecedor.Codigo.val(0);
    _retornoProdutoFornecedor.CodigoFornecedor.val("");

    LimparCampoEntity(_retornoProdutoFornecedor.Fornecedor);
    LimparCampoEntity(_retornoProdutoFornecedor.CondicaoPagamento);
    _retornoProdutoFornecedor.Observacao.val("");
    _retornoProdutoFornecedor.Marca.val("");

    _retornoProdutoFornecedor.Quantidade.val("");
    _retornoProdutoFornecedor.ValorUnitario.val("");
    _retornoProdutoFornecedor.ValorTotalItem.val("");
    _retornoProdutoFornecedor.GerarOrdemCompra.val(false);
    _retornoProdutoFornecedor.QuantidadeOriginal.val("");
    _retornoProdutoFornecedor.ValorUnitarioOriginal.val("");
    _retornoProdutoFornecedor.ValorTotalItemOriginal.val("");

    _retornoProdutoFornecedor.Fornecedor.requiredClass("form-control");
}