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


var _gridRegrasValorCotacao;
var _valorCotacao;

var ValorCotacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumJuncaoAutorizao.E });
    this.Valor = PropertyEntity({ text: "Valor da Cotação:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Valor da Cotação", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorCotacao, _gridRegrasValorCotacao, "editarRegraValorCotacaoClick", typesKnockout.decimal);
    });


    // Controle de uso
    this.UsarRegraPorValorCotacao = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por Valor da Cotação:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorCotacao.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorCotacao(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorCotacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorCotacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorCotacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorCotacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

}


//*******EVENTOS*******
function UsarRegraPorValorCotacao(usarRegra) {
    _valorCotacao.Visible.visibleFade(usarRegra);
    _valorCotacao.Regras.required(usarRegra);
}

function loadValorCotacao() {
    _valorCotacao = new ValorCotacao();
    KoBindings(_valorCotacao, "knockoutRegraValorCotacao");
        
    //-- Grid Regras
    _gridRegrasValorCotacao = new GridReordering(_configRegras.infoTable, _valorCotacao.Regras.idGrid, GeraHeadTable("Valor da Cotação"));
    _gridRegrasValorCotacao.CarregarGrid();
    $("#" + _valorCotacao.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_valorCotacao);
    });

}

function editarRegraValorCotacaoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorCotacao.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorCotacao.Codigo.val(regra.Codigo);
        _valorCotacao.Ordem.val(regra.Ordem);
        _valorCotacao.Condicao.val(regra.Condicao);
        _valorCotacao.Juncao.val(regra.Juncao);
        _valorCotacao.Valor.val(regra.Valor);

        _valorCotacao.Adicionar.visible(false);
        _valorCotacao.Atualizar.visible(true);
        _valorCotacao.Excluir.visible(true);
        _valorCotacao.Cancelar.visible(true);
    }
}

function adicionarRegraValorCotacaoClick() {
    if (!ValidarCamposObrigatorios(_valorCotacao))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var regra = ObjetoRegraValorCotacao();

    var listaRegras = ObterRegrasOrdenadas(_valorCotacao);

    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valorCotacao.Regras.val(listaRegras);

    LimparCamposValorValorCotacao();
}

function atualizarRegraValorCotacaoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_valorCotacao))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var regra = ObjetoRegraValorCotacao();
    var listaRegras = _valorCotacao.Regras.val();
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorCotacao.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    _valorCotacao.Regras.val(listaRegras);

    LimparCamposValorValorCotacao();
}

function excluirRegraValorCotacaoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorCotacao);
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

    _valorCotacao.Regras.val(listaRegras);

    LimparCamposValorValorCotacao();
}

function cancelarRegraValorCotacaoClick(e, sender) {
    LimparCamposValorValorCotacao();
}



//*******MÉTODOS*******

function ObjetoRegraValorCotacao() {
    var codigo = _valorCotacao.Codigo.val();
    var ordem = _valorCotacao.Ordem.val();

    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorCotacao.ObterOrdencao().length + 1,
        Juncao: _valorCotacao.Juncao.val(),
        Condicao: _valorCotacao.Condicao.val(),
        Valor: Globalize.parseFloat(_valorCotacao.Valor.val())
    };

    return regra;
}

function LimparCamposValorValorCotacao() {
    _valorCotacao.Codigo.val(_valorCotacao.Codigo.def);
    _valorCotacao.Ordem.val(_valorCotacao.Ordem.def);
    _valorCotacao.Condicao.val(_valorCotacao.Condicao.def);
    _valorCotacao.Juncao.val(_valorCotacao.Juncao.def);
    _valorCotacao.Valor.val(_valorCotacao.Valor.def);

    _valorCotacao.Adicionar.visible(true);
    _valorCotacao.Atualizar.visible(false);
    _valorCotacao.Excluir.visible(false);
    _valorCotacao.Cancelar.visible(false);
}