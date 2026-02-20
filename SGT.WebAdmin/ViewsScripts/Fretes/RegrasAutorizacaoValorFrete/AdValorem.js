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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoValorFrete.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoTabelaFrete.js" />
/// <reference path="RegrasAutorizacaoValorFrete.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasAdValorem;
var _adValorem;

var AdValorem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ",issue: 1734,  val: ko.observable(EnumCondicaoAutorizaoValorFrete.IgualA), options: _condicaoAutorizaoTabelaFreteValor, def: EnumCondicaoAutorizaoValorFrete.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoTabelaFrete.E), options: _juncaoAutorizaoTabelaFrete, def: EnumJuncaoAutorizaoTabelaFrete.E });
    this.AdValorem = PropertyEntity({ text: "AdValorem: ", getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "AdValorem", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_adValorem, _gridRegrasAdValorem, "editarRegraAdValoremClick", true);
    });

    // Controle de uso
    this.UsarRegraPorAdValorem = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por AdValorem:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorAdValorem.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorAdValorem(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraAdValoremClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraAdValoremClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraAdValoremClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraAdValoremClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorAdValorem(usarRegra) {
    _adValorem.Visible.visibleFade(usarRegra);
    _adValorem.Regras.required(usarRegra);
}

function loadAdValorem() {
    _adValorem = new AdValorem();
    KoBindings(_adValorem, "knockoutRegraAdValorem");

    //-- Grid Regras
    _gridRegrasAdValorem = new GridReordering(_configRegras.infoTable, _adValorem.Regras.idGrid, GeraHeadTable("AdValorem"));
    _gridRegrasAdValorem.CarregarGrid();
    $("#" + _adValorem.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasTabelaFrete(_adValorem);
    });
}

function editarRegraAdValoremClick(codigo) {
    // Buscar todas regras
    var listaRegras = _adValorem.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _adValorem.Codigo.val(regra.Codigo);
        _adValorem.Ordem.val(regra.Ordem);
        _adValorem.Condicao.val(regra.Condicao);
        _adValorem.Juncao.val(regra.Juncao);
        _adValorem.AdValorem.val(regra.Valor);

        _adValorem.Adicionar.visible(false);
        _adValorem.Atualizar.visible(true);
        _adValorem.Excluir.visible(true);
        _adValorem.Cancelar.visible(true);
    }
}

function adicionarRegraAdValoremClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_adValorem))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraAdValorem();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_adValorem);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _adValorem.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposAdValorem();
}

function atualizarRegraAdValoremClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_adValorem))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraAdValorem();

    // Buscar todas regras
    var listaRegras = _adValorem.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _adValorem.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _adValorem.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposAdValorem();
}

function excluirRegraAdValoremClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_adValorem);
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
    _adValorem.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposAdValorem();
}

function cancelarRegraAdValoremClick(e, sender) {
    LimparCamposAdValorem();
}



//*******MÉTODOS*******

function ObjetoRegraAdValorem() {
    var codigo = _adValorem.Codigo.val();
    var ordem = _adValorem.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasAdValorem.ObterOrdencao().length + 1,
        Juncao: _adValorem.Juncao.val(),
        Condicao: _adValorem.Condicao.val(),
        Valor: Globalize.parseFloat(_adValorem.AdValorem.val().toString())
    };

    return regra;
}

function LimparCamposAdValorem() {
    _adValorem.Codigo.val(_adValorem.Codigo.def);
    _adValorem.Ordem.val(_adValorem.Ordem.def);
    _adValorem.Condicao.val(_adValorem.Condicao.def);
    _adValorem.Juncao.val(_adValorem.Juncao.def);
    _adValorem.AdValorem.val(_adValorem.AdValorem.def);

    _adValorem.Adicionar.visible(true);
    _adValorem.Atualizar.visible(false);
    _adValorem.Excluir.visible(false);
    _adValorem.Cancelar.visible(false);
}