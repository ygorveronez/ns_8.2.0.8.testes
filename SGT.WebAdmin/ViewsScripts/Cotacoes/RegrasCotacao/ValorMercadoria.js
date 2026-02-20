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


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasValorMercadoria;
var _valorMercadoria;

var ValorMercadoria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumCondicaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumCondicaoAutorizao.E });
    this.ValorMercadoria = PropertyEntity({ text: "Valor da Mercadoria:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Valor da mercadoria", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorMercadoria, _gridRegrasValorMercadoria, "editarRegraValorMercadoriaClick", typesKnockout.decimal);
    });

    // Controle de uso
    this.UsarRegraPorValorMercadoria = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por valor da mercadoria:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorMercadoria.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorMercadoria(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorMercadoriaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorMercadoriaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorMercadoriaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorMercadoriaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorValorMercadoria(usarRegra) {
    _valorMercadoria.Visible.visibleFade(usarRegra);
    _valorMercadoria.Regras.required(usarRegra);
}

function loadValorMercadoria() {
    _valorMercadoria = new ValorMercadoria();
    KoBindings(_valorMercadoria, "knockoutRegraValorMercadoria");

    //-- Grid Regras
    _gridRegrasValorMercadoria = new GridReordering(_configRegras.infoTable, _valorMercadoria.Regras.idGrid, GeraHeadTable("Valor da Ocorrência"));
    _gridRegrasValorMercadoria.CarregarGrid();
    $("#" + _valorMercadoria.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_valorMercadoria);
    });

}


function editarRegraValorMercadoriaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorMercadoria.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorMercadoria.Codigo.val(regra.Codigo);
        _valorMercadoria.Ordem.val(regra.Ordem);
        _valorMercadoria.Condicao.val(regra.Condicao);
        _valorMercadoria.Juncao.val(regra.Juncao);
        _valorMercadoria.ValorMercadoria.val(regra.Valor);

        _valorMercadoria.Adicionar.visible(false);
        _valorMercadoria.Atualizar.visible(true);
        _valorMercadoria.Excluir.visible(true);
        _valorMercadoria.Cancelar.visible(true);
    }
}

function adicionarRegraValorMercadoriaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorMercadoria))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorMercadoria();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valorMercadoria);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valorMercadoria.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorMercadoria();
}

function atualizarRegraValorMercadoriaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorMercadoria))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorMercadoria();

    // Buscar todas regras
    var listaRegras = _valorMercadoria.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorMercadoria.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valorMercadoria.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorMercadoria();
}

function excluirRegraValorMercadoriaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorMercadoria);
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
    _valorMercadoria.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposValorMercadoria();
}

function cancelarRegraValorMercadoriaClick(e, sender) {
    LimparCamposValorMercadoria();
}



//*******MÉTODOS*******

function ObjetoRegraValorMercadoria() {
    var codigo = _valorMercadoria.Codigo.val();
    var ordem = _valorMercadoria.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorMercadoria.ObterOrdencao().length + 1,
        Juncao: _valorMercadoria.Juncao.val(),
        Condicao: _valorMercadoria.Condicao.val(),
        Valor: Globalize.parseFloat(_valorMercadoria.ValorMercadoria.val())
    };

    return regra;
}

function LimparCamposValorMercadoria() {
    _valorMercadoria.Codigo.val(_valorMercadoria.Codigo.def);
    _valorMercadoria.Ordem.val(_valorMercadoria.Ordem.def);
    _valorMercadoria.Condicao.val(_valorMercadoria.Condicao.def);
    _valorMercadoria.Juncao.val(_valorMercadoria.Juncao.def);
    _valorMercadoria.ValorMercadoria.val(_valorMercadoria.ValorMercadoria.def);

    _valorMercadoria.Adicionar.visible(true);
    _valorMercadoria.Atualizar.visible(false);
    _valorMercadoria.Excluir.visible(false);
    _valorMercadoria.Cancelar.visible(false);
}