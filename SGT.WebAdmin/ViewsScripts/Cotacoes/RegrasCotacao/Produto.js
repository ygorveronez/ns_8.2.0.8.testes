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
/// <reference path="../../enumeradores/enumcondicaoautorizaocotacao.js" />
/// <reference path="../../enumeradores/enumjuncaoautorizao.js" />
/// <reference path="RegrasCotacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasProduto;
var _produto;

var Produto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumJuncaoAutorizao.E });
    this.Produto = PropertyEntity({ text: "Produto:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Produto", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_produto, _gridRegrasProduto, "editarRegraProdutoClick");
    });

    // Controle de uso
    this.UsarRegraPorProduto = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por produto:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorProduto.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorProduto(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraProdutoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorProduto(usarRegra) {
    _produto.Visible.visibleFade(usarRegra);
    _produto.Regras.required(usarRegra);
}

function loadProduto() {
    _produto = new Produto();
    KoBindings(_produto, "knockoutRegraProduto");

    //-- Busca
    new BuscarProdutos(_produto.Produto);

    //-- Grid Regras
    _gridRegrasProduto = new GridReordering(_configRegras.infoTable, _produto.Regras.idGrid, GeraHeadTable("Produto"));
    _gridRegrasProduto.CarregarGrid();
    $("#" + _produto.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_produto);
    });
}


function editarRegraProdutoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _produto.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _produto.Codigo.val(regra.Codigo);
        _produto.Ordem.val(regra.Ordem);
        _produto.Condicao.val(regra.Condicao);
        _produto.Juncao.val(regra.Juncao);

        _produto.Produto.val(regra.Entidade.Descricao);
        _produto.Produto.codEntity(regra.Entidade.Codigo);

        _produto.Adicionar.visible(false);
        _produto.Atualizar.visible(true);
        _produto.Excluir.visible(true);
        _produto.Cancelar.visible(true);
    }
}

function adicionarRegraProdutoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_produto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraProduto();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_produto);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _produto.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposProduto();
}

function atualizarRegraProdutoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_produto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraProduto();

    // Buscar todas regras
    var listaRegras = _produto.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _produto.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _produto.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposProduto();
}

function excluirRegraProdutoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_produto);
    var index = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == e.Codigo.val()) {
            index = parseInt(i);
            break;
        }
    }

    // Remove a regra especifica
    listaRegras.splice(index, 1);

    // Itera para corrigir o numero da ordem
    for (i = 1; i <= listaRegras.length; i++)
        listaRegras[i - 1].Ordem = i;

    // Atuliza o componente de regras
    _produto.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposProduto();
}

function cancelarRegraProdutoClick(e, sender) {
    LimparCamposProduto();
}



//*******MÉTODOS*******

function ObjetoRegraProduto() {
    var codigo = _produto.Codigo.val();
    var ordem = _produto.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasProduto.ObterOrdencao().length + 1,
        Juncao: _produto.Juncao.val(),
        Condicao: _produto.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_produto.Produto.codEntity()),
            Descricao: _produto.Produto.val()
        }
    };

    return regra;
}

function LimparCamposProduto() {
    _produto.Codigo.val(_produto.Codigo.def);
    _produto.Ordem.val(_produto.Ordem.def);
    _produto.Condicao.val(_produto.Condicao.def);
    _produto.Juncao.val(_produto.Juncao.def);

    LimparCampoEntity(_produto.Produto);

    _produto.Adicionar.visible(true);
    _produto.Atualizar.visible(false);
    _produto.Excluir.visible(false);
    _produto.Cancelar.visible(false);
}