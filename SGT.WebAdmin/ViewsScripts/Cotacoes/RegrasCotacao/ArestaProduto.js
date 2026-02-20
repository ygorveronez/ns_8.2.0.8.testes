/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../enumeradores/enumcondicaoautorizaocotacao.js" />
/// <reference path="../../enumeradores/enumjuncaoautorizao.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Regrascotacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasArestaProduto;
var _arestaProduto;

var ArestaProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumJuncaoAutorizao.E });
    this.ArestaProduto = PropertyEntity({ text: "Aresta:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Aresta", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_arestaProduto, _gridRegrasArestaProduto, "editarRegraArestaProdutoClick", typesKnockout.decimal);
    });


    // Controle de uso
    this.UsarRegraPorArestaProduto = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por Aresta Produto:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorArestaProduto.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorArestaProduto(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraArestaProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraArestaProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraArestaProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraArestaProdutoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

}


//*******EVENTOS*******
function UsarRegraPorArestaProduto(usarRegra) {
    _arestaProduto.Visible.visibleFade(usarRegra);
    _arestaProduto.Regras.required(usarRegra);
}

function loadArestaProduto() {
    _arestaProduto = new ArestaProduto();
    KoBindings(_arestaProduto, "knockoutRegraArestaProduto");

    //-- Grid Regras
    _gridRegrasArestaProduto = new GridReordering(_configRegras.infoTable, _arestaProduto.Regras.idGrid, GeraHeadTable("ArestaProduto"));
    _gridRegrasArestaProduto.CarregarGrid();
    $("#" + _arestaProduto.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_arestaProduto);
    });

}

function editarRegraArestaProdutoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _arestaProduto.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _arestaProduto.Codigo.val(regra.Codigo);
        _arestaProduto.Ordem.val(regra.Ordem);
        _arestaProduto.Condicao.val(regra.Condicao);
        _arestaProduto.Juncao.val(regra.Juncao);
        _arestaProduto.ArestaProduto.val(regra.Valor);

        _arestaProduto.Adicionar.visible(false);
        _arestaProduto.Atualizar.visible(true);
        _arestaProduto.Excluir.visible(true);
        _arestaProduto.Cancelar.visible(true);
    }
}

function adicionarRegraArestaProdutoClick() {
    if (!ValidarCamposObrigatorios(_arestaProduto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var regra = ObjetoRegraArestaProduto();

    var listaRegras = ObterRegrasOrdenadas(_arestaProduto);

    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _arestaProduto.Regras.val(listaRegras);

    LimparCamposValorArestaProduto();
}

function atualizarRegraArestaProdutoClick(e, sender) {

    if (!ValidarCamposObrigatorios(_arestaProduto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var regra = ObjetoRegraArestaProduto();
    var listaRegras = _arestaProduto.Regras.val();
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _arestaProduto.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    _arestaProduto.Regras.val(listaRegras);

    LimparCamposValorArestaProduto();
}

function excluirRegraArestaProdutoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_arestaProduto);
    var index = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == e.Codigo.val()) {
            index = parseInt(i);
            break;
        }
    }

    listaRegras.splice(index, 1);

    for (i = 1; i <= listaRegras.length; i++)
        listaRegras[i - 1].Ordem = i;

    _arestaProduto.Regras.val(listaRegras);

    LimparCamposValorArestaProduto();
}

function cancelarRegraArestaProdutoClick(e, sender) {
    LimparCamposValorArestaProduto();
}



//*******MÉTODOS*******

function ObjetoRegraArestaProduto() {
    var codigo = _arestaProduto.Codigo.val();
    var ordem = _arestaProduto.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasArestaProduto.ObterOrdencao().length + 1,
        Juncao: _arestaProduto.Juncao.val(),
        Condicao: _arestaProduto.Condicao.val(),
        Valor: Globalize.parseFloat(_arestaProduto.ArestaProduto.val())
    };

    return regra;
}

function LimparCamposValorArestaProduto() {
    _arestaProduto.Codigo.val(_arestaProduto.Codigo.def);
    _arestaProduto.Ordem.val(_arestaProduto.Ordem.def);
    _arestaProduto.Condicao.val(_arestaProduto.Condicao.def);
    _arestaProduto.Juncao.val(_arestaProduto.Juncao.def);
    _arestaProduto.ArestaProduto.val(_arestaProduto.ArestaProduto.def);

    _arestaProduto.Adicionar.visible(true);
    _arestaProduto.Atualizar.visible(false);
    _arestaProduto.Excluir.visible(false);
    _arestaProduto.Cancelar.visible(false);
}