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


var _gridRegrasValorDesconto;
var _valorDesconto;

var ValorDesconto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.ValorDesconto = PropertyEntity({ text: "Valor do Desconto:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Valor do Desconto", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorDesconto, _gridRegrasValorDesconto, "editarRegraValorDescontoClick", true);
    });

    // Controle de uso
    this.UsarRegraPorValorDesconto = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por Valor do Desconto:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorDesconto.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorDesconto(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorDescontoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorDescontoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorDescontoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorDescontoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorValorDesconto(usarRegra) {
    _valorDesconto.Visible.visibleFade(usarRegra);
    _valorDesconto.Alcadas.required(usarRegra);
}

function loadValorDesconto() {
    _valorDesconto = new ValorDesconto();
    KoBindings(_valorDesconto, "knockoutAlcadaValorDesconto");

    //-- Grid Regras
    _gridRegrasValorDesconto = new GridReordering(_configRegras.infoTable, _valorDesconto.Alcadas.idGrid, GeraHeadTable("Valor do Desconto"));
    _gridRegrasValorDesconto.CarregarGrid();
    $("#" + _valorDesconto.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasRegraContratoFreteTerceiro(_valorDesconto);
    });
}

function editarRegraValorDescontoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorDesconto.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorDesconto.Codigo.val(regra.Codigo);
        _valorDesconto.Ordem.val(regra.Ordem);
        _valorDesconto.Condicao.val(regra.Condicao);
        _valorDesconto.Juncao.val(regra.Juncao);

        _valorDesconto.ValorDesconto.val(regra.Valor);

        _valorDesconto.Adicionar.visible(false);
        _valorDesconto.Atualizar.visible(true);
        _valorDesconto.Excluir.visible(true);
        _valorDesconto.Cancelar.visible(true);
    }
}

function adicionarRegraValorDescontoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorDesconto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorDesconto();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valorDesconto);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valorDesconto.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposValorDesconto();
}

function atualizarRegraValorDescontoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorDesconto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorDesconto();

    // Buscar todas regras
    var listaRegras = _valorDesconto.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorDesconto.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valorDesconto.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposValorDesconto();
}

function excluirRegraValorDescontoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorDesconto);
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
    _valorDesconto.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposValorDesconto();
}

function cancelarRegraValorDescontoClick(e, sender) {
    LimparCamposValorDesconto();
}



//*******MÉTODOS*******
function ObjetoRegraValorDesconto() {
    var codigo = _valorDesconto.Codigo.val();
    var ordem = _valorDesconto.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorDesconto.ObterOrdencao().length + 1,
        Juncao: _valorDesconto.Juncao.val(),
        Condicao: _valorDesconto.Condicao.val(),
        Valor: Globalize.parseFloat(_valorDesconto.ValorDesconto.val())
    };

    return regra;
}

function LimparCamposValorDesconto() {
    _valorDesconto.Codigo.val(_valorDesconto.Codigo.def);
    _valorDesconto.Ordem.val(_valorDesconto.Ordem.def);
    _valorDesconto.Condicao.val(_valorDesconto.Condicao.def);
    _valorDesconto.Juncao.val(_valorDesconto.Juncao.def);
    _valorDesconto.ValorDesconto.val(_valorDesconto.ValorDesconto.def);

    _valorDesconto.Adicionar.visible(true);
    _valorDesconto.Atualizar.visible(false);
    _valorDesconto.Excluir.visible(false);
    _valorDesconto.Cancelar.visible(false);
}