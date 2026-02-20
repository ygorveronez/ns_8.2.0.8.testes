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
/// <reference path="Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoComposicao, _gridComposicaoProduto;

var ProdutoComposicao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.ProdutoKIT = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Este produto é um KIT?", enable: ko.observable(true), visible: ko.observable(true) });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.Quantidade = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "*Quantidade: ", required: true, getType: typesKnockout.decimal, maxlength: 20, configDecimal: { precision: 4, allowZero: false, allowNegative: false } });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarComposicaoProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarComposicaoProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirComposicaoProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposComposicaoProdutoAlteracao, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.ProdutoKIT.val.subscribe(function (novoValor) {
        if (!novoValor) {
            LimparCamposComposicaoProduto();
        } else if (novoValor) {
            $("#liInsumoProduto").show();
        }
    });
}

//*******EVENTOS*******

function loadProdutoComposicao() {
    _produtoComposicao = new ProdutoComposicao();
    KoBindings(_produtoComposicao, "knockoutComposicaoProduto");

    new BuscarProdutoTMS(_produtoComposicao.Produto);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarComposicaoProdutoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Produto", title: "Produto", width: "60%" },
        { data: "Quantidade", title: "Quantidade", width: "20%" }
    ];

    _gridComposicaoProduto = new BasicDataTable(_produtoComposicao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    $("#liInsumoProduto").hide();
    RecarregarGridComposicaoProduto();
}

//*******MÉTODOS*******

function RecarregarGridComposicaoProduto() {
    var data = new Array();

    $.each(_produto.Composicoes.list, function (i, composicao) {
        var composicaoGrid = new Object();

        composicaoGrid.Codigo = composicao.Codigo.val;
        composicaoGrid.Quantidade = composicao.Quantidade.val;
        composicaoGrid.Produto = composicao.Produto.val;

        data.push(composicaoGrid);
    });

    _gridComposicaoProduto.CarregarGrid(data);
}

function EditarComposicaoProdutoClick(data) {
    for (var i = 0; i < _produto.Composicoes.list.length; i++) {
        if (data.Codigo == _produto.Composicoes.list[i].Codigo.val) {
            var composicao = _produto.Composicoes.list[i];

            _produtoComposicao.Codigo.val(composicao.Codigo.val);
            _produtoComposicao.Quantidade.val(composicao.Quantidade.val);
            _produtoComposicao.Produto.val(composicao.Produto.val);
            _produtoComposicao.Produto.codEntity(composicao.Produto.codEntity);

            _produtoComposicao.Adicionar.visible(false);
            _produtoComposicao.Atualizar.visible(true);
            _produtoComposicao.Excluir.visible(true);
            _produtoComposicao.Cancelar.visible(true);
        }
    }
}

function ExcluirComposicaoProdutoClick() {
    for (var i = 0; i < _produto.Composicoes.list.length; i++) {
        if (_produtoComposicao.Codigo.val() == _produto.Composicoes.list[i].Codigo.val) {
            _produto.Composicoes.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridComposicaoProduto();
    LimparCamposComposicaoProdutoAlteracao();
}

function AdicionarComposicaoProdutoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_produtoComposicao);

    if (valido) {
        _produtoComposicao.Codigo.val(guid());
        _produto.Composicoes.list.push(SalvarListEntity(_produtoComposicao));

        RecarregarGridComposicaoProduto();
        LimparCamposComposicaoProdutoAlteracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarComposicaoProdutoClick() {
    var valido = ValidarCamposObrigatorios(_produtoComposicao);

    if (valido) {
        for (var i = 0; i < _produto.Composicoes.list.length; i++) {
            if (_produtoComposicao.Codigo.val() == _produto.Composicoes.list[i].Codigo.val) {
                _produto.Composicoes.list[i] = SalvarListEntity(_produtoComposicao);
                break;
            }
        }

        RecarregarGridComposicaoProduto();
        LimparCamposComposicaoProdutoAlteracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function validaCamposObrigatoriosComposicao() {
    var tudoCerto = true;
    if (_produtoComposicao.ProdutoKIT.val() == true && _produto.Composicoes.list.length == 0)
        tudoCerto = false;
    return tudoCerto;
}

function LimparCamposComposicaoProdutoAlteracao() {
    LimparCampoEntity(_produtoComposicao.Produto);
    _produtoComposicao.Quantidade.val("0,0000");
    _produtoComposicao.Adicionar.visible(true);
    _produtoComposicao.Atualizar.visible(false);
    _produtoComposicao.Excluir.visible(false);
    _produtoComposicao.Cancelar.visible(false);
}

function LimparCamposComposicaoProduto() {
    LimparCampos(_produtoComposicao);
    _produto.Composicoes.list = new Array();
    $("#liInsumoProduto").hide();
    RecarregarGridComposicaoProduto();
}