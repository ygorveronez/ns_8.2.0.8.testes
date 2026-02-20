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


var _gridRegrasValorPedagio;
var _valorPedagio;

var ValorPedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoValorFrete.IgualA), options: _condicaoAutorizaoTabelaFreteValor, def: EnumCondicaoAutorizaoValorFrete.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoTabelaFrete.E), options: _juncaoAutorizaoTabelaFrete, def: EnumJuncaoAutorizaoTabelaFrete.E });
    this.ValorPedagio = PropertyEntity({ text: "Valor do Pedágio: ", getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Valor do Pedágio", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorPedagio, _gridRegrasValorPedagio, "editarRegraValorPedagioClick", true);
    });

    // Controle de uso
    this.UsarRegraPorValorPedagio = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por valor do pedágio:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorPedagio.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorPedagio(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorPedagioClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorPedagioClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorPedagioClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorPedagioClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorValorPedagio(usarRegra) {
    _valorPedagio.Visible.visibleFade(usarRegra);
    _valorPedagio.Regras.required(usarRegra);
}

function loadValorPedagio() {
    _valorPedagio = new ValorPedagio();
    KoBindings(_valorPedagio, "knockoutRegraValorPedagio");

    //-- Grid Regras
    _gridRegrasValorPedagio = new GridReordering(_configRegras.infoTable, _valorPedagio.Regras.idGrid, GeraHeadTable("Valor do Pedágio"));
    _gridRegrasValorPedagio.CarregarGrid();
    $("#" + _valorPedagio.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasTabelaFrete(_valorPedagio);
    });
}

function editarRegraValorPedagioClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorPedagio.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorPedagio.Codigo.val(regra.Codigo);
        _valorPedagio.Ordem.val(regra.Ordem);
        _valorPedagio.Condicao.val(regra.Condicao);
        _valorPedagio.Juncao.val(regra.Juncao);
        _valorPedagio.ValorPedagio.val(regra.Valor);

        _valorPedagio.Adicionar.visible(false);
        _valorPedagio.Atualizar.visible(true);
        _valorPedagio.Excluir.visible(true);
        _valorPedagio.Cancelar.visible(true);
    }
}

function adicionarRegraValorPedagioClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorPedagio))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorPedagio();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valorPedagio);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valorPedagio.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorPedagio();
}

function atualizarRegraValorPedagioClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorPedagio))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorPedagio();

    // Buscar todas regras
    var listaRegras = _valorPedagio.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorPedagio.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valorPedagio.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorPedagio();
}

function excluirRegraValorPedagioClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorPedagio);
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
    _valorPedagio.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposValorPedagio();
}

function cancelarRegraValorPedagioClick(e, sender) {
    LimparCamposValorPedagio();
}



//*******MÉTODOS*******

function ObjetoRegraValorPedagio() {
    var codigo = _valorPedagio.Codigo.val();
    var ordem = _valorPedagio.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorPedagio.ObterOrdencao().length + 1,
        Juncao: _valorPedagio.Juncao.val(),
        Condicao: _valorPedagio.Condicao.val(),
        Valor: Globalize.parseFloat(_valorPedagio.ValorPedagio.val().toString())
    };

    return regra;
}

function LimparCamposValorPedagio() {
    _valorPedagio.Codigo.val(_valorPedagio.Codigo.def);
    _valorPedagio.Ordem.val(_valorPedagio.Ordem.def);
    _valorPedagio.Condicao.val(_valorPedagio.Condicao.def);
    _valorPedagio.Juncao.val(_valorPedagio.Juncao.def);
    _valorPedagio.ValorPedagio.val(_valorPedagio.ValorPedagio.def);

    _valorPedagio.Adicionar.visible(true);
    _valorPedagio.Atualizar.visible(false);
    _valorPedagio.Excluir.visible(false);
    _valorPedagio.Cancelar.visible(false);
}