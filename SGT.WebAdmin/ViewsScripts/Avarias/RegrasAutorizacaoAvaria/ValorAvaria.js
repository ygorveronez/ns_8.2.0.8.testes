/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoAvaria.js" />
/// <reference path="RegrasAutorizacaoAvaria.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasValorAvaria;
var _valorAvaria;

var ValorAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoAvaria.IgualA), options: _condicaoAutorizaoAvariaValor, def: EnumCondicaoAutorizaoAvaria.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoAvaria.E), options: _juncaoAutorizaoAvaria, def: EnumJuncaoAutorizaoAvaria.E });
    this.ValorAvaria = PropertyEntity({ text: "Valor da Avaria:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Valor da Avaria", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorAvaria, _gridRegrasValorAvaria, "editarRegraValorAvariaClick", true);
    });

    // Controle de uso
    this.UsarRegraPorValorAvaria = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por valor:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorAvaria.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorAvaria(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorAvariaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorAvariaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorAvariaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorAvariaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};


//*******EVENTOS*******
function UsarRegraPorValorAvaria(usarRegra) {
    _valorAvaria.Visible.visibleFade(usarRegra);
    _valorAvaria.Regras.required(usarRegra);
}

function loadValorAvaria() {
    _valorAvaria = new ValorAvaria();
    KoBindings(_valorAvaria, "knockoutRegraValorAvaria");

    //-- Grid Regras
    _gridRegrasValorAvaria = new GridReordering(_configRegras.infoTable, _valorAvaria.Regras.idGrid, GeraHeadTable("Valor da Avaria"));
    _gridRegrasValorAvaria.CarregarGrid();
    $("#" + _valorAvaria.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasAvaria(_valorAvaria);
    });
}

function editarRegraValorAvariaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorAvaria.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorAvaria.Codigo.val(regra.Codigo);
        _valorAvaria.Ordem.val(regra.Ordem);
        _valorAvaria.Condicao.val(regra.Condicao);
        _valorAvaria.Juncao.val(regra.Juncao);
        _valorAvaria.ValorAvaria.val(regra.Valor);

        _valorAvaria.Adicionar.visible(false);
        _valorAvaria.Atualizar.visible(true);
        _valorAvaria.Excluir.visible(true);
        _valorAvaria.Cancelar.visible(true);
    }
}

function adicionarRegraValorAvariaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorAvaria))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorAvaria();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valorAvaria);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valorAvaria.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorAvaria();
}

function atualizarRegraValorAvariaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorAvaria))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorAvaria();

    // Buscar todas regras
    var listaRegras = _valorAvaria.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorAvaria.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valorAvaria.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorAvaria();
}

function excluirRegraValorAvariaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorAvaria);
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
    _valorAvaria.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposValorAvaria();
}

function cancelarRegraValorAvariaClick(e, sender) {
    LimparCamposValorAvaria();
}



//*******MÉTODOS*******

function ObjetoRegraValorAvaria() {
    var codigo = _valorAvaria.Codigo.val();
    var ordem = _valorAvaria.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorAvaria.ObterOrdencao().length + 1,
        Juncao: _valorAvaria.Juncao.val(),
        Condicao: _valorAvaria.Condicao.val(),
        Valor: Globalize.parseFloat(_valorAvaria.ValorAvaria.val().toString())
    };

    return regra;
}

function LimparCamposValorAvaria() {
    _valorAvaria.Codigo.val(_valorAvaria.Codigo.def);
    _valorAvaria.Ordem.val(_valorAvaria.Ordem.def);
    _valorAvaria.Condicao.val(_valorAvaria.Condicao.def);
    _valorAvaria.Juncao.val(_valorAvaria.Juncao.def);
    _valorAvaria.ValorAvaria.val(_valorAvaria.ValorAvaria.def);

    _valorAvaria.Adicionar.visible(true);
    _valorAvaria.Atualizar.visible(false);
    _valorAvaria.Excluir.visible(false);
    _valorAvaria.Cancelar.visible(false);
}