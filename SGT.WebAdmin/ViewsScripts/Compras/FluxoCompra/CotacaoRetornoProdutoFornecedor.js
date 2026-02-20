/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/CondicaoPagamento.js" />
/// <reference path="CotacaoRetorno.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _retornoProdutoFornecedor;
var _gridRetornoProdutoFornecedor;

var CotacaoRetornoProdutoFornecedor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.QuantidadeOriginal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ValorUnitarioOriginal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ValorTotalItemOriginal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.QuantidadeRetorno = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ValorUnitarioRetorno = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ValorTotalRetorno = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Fornecedor:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.CondicaoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Condição Pagamento:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 2000, enable: ko.observable(true), required: false });
    this.Marca = PropertyEntity({ text: "Marca:", maxlength: 500, enable: ko.observable(true), required: false });

    this.Quantidade = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.decimal, required: ko.observable(true), text: ko.observable("*Quantidade:"), maxlength: 10, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ getType: typesKnockout.decimal, required: ko.observable(true), text: ko.observable("*Val. Unit.:"), maxlength: 18, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: false, allowNegative: false } });
    this.ValorTotalItem = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(""), def: "", required: ko.observable(false), text: ko.observable("*Total:"), maxlength: 10, visible: ko.observable(true), enable: false });
    this.GerarOrdemCompra = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar O.C. para este retorno?", enable: ko.observable(true) });

    this.ValorUnitario.val.subscribe(function () {
        CalcularTotalItemRetorno();
    });

    this.Quantidade.val.subscribe(function () {
        CalcularTotalItemRetorno();
    });

    this.SalvarItem = PropertyEntity({ type: types.event, eventClick: SalvarItemRetornoClick, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Fechar = PropertyEntity({ type: types.event, eventClick: FecharItemRetornoClick, text: "Fechar", visible: ko.observable(true) });

    this.RetornoMercadoria = PropertyEntity({ type: types.local });
};

//*******EVENTOS*******

function LoadCotacaoRetornoProdutoFornecedor() {
    $.get("Content/Static/Compras/CotacaoModais.html?dyn=" + guid(), function (htmlCotacaoModais) {
        $("#CotacaoModais").html(htmlCotacaoModais);

        _retornoProdutoFornecedor = new CotacaoRetornoProdutoFornecedor();
        KoBindings(_retornoProdutoFornecedor, "knoutRetornoMercadoria");

        new BuscarClientes(_retornoProdutoFornecedor.Fornecedor, null, null, null, null, null, null, null, null, null, null, _cotacaoRetorno.Fornecedores);
        new BuscarCondicaoPagamento(_retornoProdutoFornecedor.CondicaoPagamento);

        LoadGridCotacaoRetornoProdutoFornecedor();
    });
}

function SalvarItemRetornoClick() {
    var tudoCerto = ValidarCamposObrigatorios(_retornoProdutoFornecedor);

    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    _retornoProdutoFornecedor.QuantidadeRetorno.val(_retornoProdutoFornecedor.Quantidade.val());
    _retornoProdutoFornecedor.ValorUnitarioRetorno.val(_retornoProdutoFornecedor.ValorUnitario.val());
    _retornoProdutoFornecedor.ValorTotalRetorno.val(_retornoProdutoFornecedor.ValorTotalItem.val());

    _retornoProdutoFornecedor.Produto.codEntity(_cotacaoRetorno.Produto.codEntity());
    _retornoProdutoFornecedor.Produto.val(_cotacaoRetorno.Produto.val());
    _retornoProdutoFornecedor.QuantidadeOriginal.val(_cotacaoRetorno.QuantidadeOriginal.val());
    _retornoProdutoFornecedor.ValorUnitarioOriginal.val(_cotacaoRetorno.ValorUnitarioOriginal.val());
    _retornoProdutoFornecedor.ValorTotalItemOriginal.val(_cotacaoRetorno.ValorTotalItemOriginal.val());

    if (_retornoProdutoFornecedor.Codigo.val() == 0) {
        _retornoProdutoFornecedor.Codigo.val(guid());
        _cotacaoRetorno.Retornos.list.push(SalvarListEntity(_retornoProdutoFornecedor));
    }
    else {
        $.each(_cotacaoRetorno.Retornos.list, function (i, retorno) {
            if (retorno.Codigo.val === _retornoProdutoFornecedor.Codigo.val()) {
                _cotacaoRetorno.Retornos.list[i] = SalvarListEntity(_retornoProdutoFornecedor);
                return false;
            }
        });
    }

    RecarregarGridRetornoProdutoFornecedor();
    LimparCamposRetornoProdutoFornecedor();
}

function FecharItemRetornoClick() {
    LimparCamposRetornoProdutoFornecedor();
    Global.fecharModal('divRetornoMercadoria');
}

////*******MÉTODOS*******

function LoadGridCotacaoRetornoProdutoFornecedor() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarCotacaoRetornoProdutoFornecedorClick, tamanho: "10", icone: "" };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: ExcluirCotacaoRetornoProdutoFornecedorClick, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, excluir], descricao: "Opções", tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
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

    _gridRetornoProdutoFornecedor = new BasicDataTable(_retornoProdutoFornecedor.RetornoMercadoria.id, header, menuOpcoes);
    RecarregarGridRetornoProdutoFornecedor();
}

function EditarCotacaoRetornoProdutoFornecedorClick(data) {
    for (var i = 0; i < _cotacaoRetorno.Retornos.list.length; i++) {
        if (data.Codigo == _cotacaoRetorno.Retornos.list[i].Codigo.val) {

            var item = _cotacaoRetorno.Retornos.list[i];

            _retornoProdutoFornecedor.Codigo.val(item.Codigo.val);
            _retornoProdutoFornecedor.Fornecedor.val(item.Fornecedor.val);
            _retornoProdutoFornecedor.Fornecedor.codEntity(item.Fornecedor.codEntity);
            _retornoProdutoFornecedor.CondicaoPagamento.val(item.CondicaoPagamento.val);
            _retornoProdutoFornecedor.CondicaoPagamento.codEntity(item.CondicaoPagamento.codEntity);

            _retornoProdutoFornecedor.Quantidade.val(item.QuantidadeRetorno.val);
            _retornoProdutoFornecedor.ValorUnitario.val(item.ValorUnitarioRetorno.val);
            _retornoProdutoFornecedor.ValorTotalItem.val(item.ValorTotalRetorno.val);

            _retornoProdutoFornecedor.Observacao.val(item.Observacao.val);
            _retornoProdutoFornecedor.Marca.val(item.Marca.val);
            _retornoProdutoFornecedor.GerarOrdemCompra.val(item.GerarOrdemCompra.val);

            break;
        }
    }
}

function ExcluirCotacaoRetornoProdutoFornecedorClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o retorno selecionado?", function () {
        for (var i = 0; i < _cotacaoRetorno.Retornos.list.length; i++) {
            if (data.Codigo == _cotacaoRetorno.Retornos.list[i].Codigo.val) {
                _cotacaoRetorno.Retornos.list.splice(i, 1);
                break;
            }
        }

        RecarregarGridRetornoProdutoFornecedor();
        LimparCamposRetornoProdutoFornecedor();
    });
}

function RecarregarGridRetornoProdutoFornecedor() {
    var data = new Array();

    $.each(_cotacaoRetorno.Retornos.list, function (i, listaMercadoria) {
        if (listaMercadoria.Produto.codEntity == _cotacaoRetorno.Produto.codEntity()) {
            var listaMercadoriaGrid = new Object();

            listaMercadoriaGrid.Codigo = listaMercadoria.Codigo.val;
            listaMercadoriaGrid.Fornecedor = listaMercadoria.Fornecedor.val;
            listaMercadoriaGrid.Produto = listaMercadoria.Produto.val;
            listaMercadoriaGrid.QuantidadeOriginal = listaMercadoria.QuantidadeOriginal.val;
            listaMercadoriaGrid.QuantidadeRetorno = listaMercadoria.QuantidadeRetorno.val;
            listaMercadoriaGrid.ValorUnitarioOriginal = listaMercadoria.ValorUnitarioOriginal.val;
            listaMercadoriaGrid.ValorUnitarioRetorno = listaMercadoria.ValorUnitarioRetorno.val;
            listaMercadoriaGrid.ValorTotalItemOriginal = listaMercadoria.ValorTotalItemOriginal.val;
            listaMercadoriaGrid.ValorTotalRetorno = listaMercadoria.ValorTotalRetorno.val;
            listaMercadoriaGrid.GerarOrdemCompra = listaMercadoria.GerarOrdemCompra.val ? "Sim" : "Não";
            listaMercadoriaGrid.CondicaoPagamento = listaMercadoria.CondicaoPagamento.val;
            listaMercadoriaGrid.Marca = listaMercadoria.Marca.val;

            data.push(listaMercadoriaGrid);
        }
    });

    _gridRetornoProdutoFornecedor.CarregarGrid(data);
}

function CalcularTotalItemRetorno() {
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
    LimparCampos(_retornoProdutoFornecedor);
}