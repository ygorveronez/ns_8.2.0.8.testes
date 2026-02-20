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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizao.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasValorAcrescimo;
var _valorAcrescimo;

var ValorAcrescimo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.ValorAcrescimo = PropertyEntity({ text: "Valor do Acréscimo:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Valor do Acrescimo", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorAcrescimo, _gridRegrasValorAcrescimo, "editarRegraValorAcrescimoClick", true);
    });

    // Controle de uso
    this.UsarRegraPorValorAcrescimo = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por Valor do Acréscimo:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorAcrescimo.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorAcrescimo(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorAcrescimoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorAcrescimoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorAcrescimoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorAcrescimoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorValorAcrescimo(usarRegra) {
    _valorAcrescimo.Visible.visibleFade(usarRegra);
    _valorAcrescimo.Alcadas.required(usarRegra);
}

function loadValorAcrescimo() {
    _valorAcrescimo = new ValorAcrescimo();
    KoBindings(_valorAcrescimo, "knockoutAlcadaValorAcrescimo");

    //-- Grid Regras
    _gridRegrasValorAcrescimo = new GridReordering(_configRegras.infoTable, _valorAcrescimo.Alcadas.idGrid, GeraHeadTable("Valor do Acréscimo"));
    _gridRegrasValorAcrescimo.CarregarGrid();
    $("#" + _valorAcrescimo.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasRegraContratoFreteTerceiro(_valorAcrescimo);
    });
}

function editarRegraValorAcrescimoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorAcrescimo.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorAcrescimo.Codigo.val(regra.Codigo);
        _valorAcrescimo.Ordem.val(regra.Ordem);
        _valorAcrescimo.Condicao.val(regra.Condicao);
        _valorAcrescimo.Juncao.val(regra.Juncao);

        _valorAcrescimo.ValorAcrescimo.val(regra.Valor);

        _valorAcrescimo.Adicionar.visible(false);
        _valorAcrescimo.Atualizar.visible(true);
        _valorAcrescimo.Excluir.visible(true);
        _valorAcrescimo.Cancelar.visible(true);
    }
}

function adicionarRegraValorAcrescimoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorAcrescimo))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorAcrescimo();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valorAcrescimo);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valorAcrescimo.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposValorAcrescimo();
}

function atualizarRegraValorAcrescimoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorAcrescimo))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorAcrescimo();

    // Buscar todas regras
    var listaRegras = _valorAcrescimo.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorAcrescimo.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valorAcrescimo.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposValorAcrescimo();
}

function excluirRegraValorAcrescimoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorAcrescimo);
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
    _valorAcrescimo.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposValorAcrescimo();
}

function cancelarRegraValorAcrescimoClick(e, sender) {
    LimparCamposValorAcrescimo();
}



//*******MÉTODOS*******
function ObjetoRegraValorAcrescimo() {
    var codigo = _valorAcrescimo.Codigo.val();
    var ordem = _valorAcrescimo.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorAcrescimo.ObterOrdencao().length + 1,
        Juncao: _valorAcrescimo.Juncao.val(),
        Condicao: _valorAcrescimo.Condicao.val(),
        Valor: Globalize.parseFloat(_valorAcrescimo.ValorAcrescimo.val())
    };

    return regra;
}

function LimparCamposValorAcrescimo() {
    _valorAcrescimo.Codigo.val(_valorAcrescimo.Codigo.def);
    _valorAcrescimo.Ordem.val(_valorAcrescimo.Ordem.def);
    _valorAcrescimo.Condicao.val(_valorAcrescimo.Condicao.def);
    _valorAcrescimo.Juncao.val(_valorAcrescimo.Juncao.def);
    _valorAcrescimo.ValorAcrescimo.val(_valorAcrescimo.ValorAcrescimo.def);

    _valorAcrescimo.Adicionar.visible(true);
    _valorAcrescimo.Atualizar.visible(false);
    _valorAcrescimo.Excluir.visible(false);
    _valorAcrescimo.Cancelar.visible(false);
}