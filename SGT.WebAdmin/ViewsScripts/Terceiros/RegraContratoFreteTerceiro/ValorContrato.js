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


var _gridRegrasValorContrato;
var _valorContrato;

var ValorContrato = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.ValorContrato = PropertyEntity({ text: "Valor do Contrato:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Valor do Contrato", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorContrato, _gridRegrasValorContrato, "editarRegraValorContratoClick", true);
    });

    // Controle de uso
    this.UsarRegraPorValorContrato = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por Valor do Contrato:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorContrato.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorContrato(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorContratoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorContratoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorContratoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorContratoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorValorContrato(usarRegra) {
    _valorContrato.Visible.visibleFade(usarRegra);
    _valorContrato.Alcadas.required(usarRegra);
}

function loadValorContrato() {
    _valorContrato = new ValorContrato();
    KoBindings(_valorContrato, "knockoutAlcadaValorContrato");

    //-- Grid Regras
    _gridRegrasValorContrato = new GridReordering(_configRegras.infoTable, _valorContrato.Alcadas.idGrid, GeraHeadTable("Valor do Contrato"));
    _gridRegrasValorContrato.CarregarGrid();
    $("#" + _valorContrato.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasRegraContratoFreteTerceiro(_valorContrato);
    });
}

function editarRegraValorContratoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorContrato.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorContrato.Codigo.val(regra.Codigo);
        _valorContrato.Ordem.val(regra.Ordem);
        _valorContrato.Condicao.val(regra.Condicao);
        _valorContrato.Juncao.val(regra.Juncao);

        _valorContrato.ValorContrato.val(regra.Valor);

        _valorContrato.Adicionar.visible(false);
        _valorContrato.Atualizar.visible(true);
        _valorContrato.Excluir.visible(true);
        _valorContrato.Cancelar.visible(true);
    }
}

function adicionarRegraValorContratoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorContrato))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorContrato();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valorContrato);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valorContrato.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposValorContrato();
}

function atualizarRegraValorContratoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorContrato))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorContrato();

    // Buscar todas regras
    var listaRegras = _valorContrato.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorContrato.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valorContrato.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposValorContrato();
}

function excluirRegraValorContratoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorContrato);
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
    _valorContrato.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposValorContrato();
}

function cancelarRegraValorContratoClick(e, sender) {
    LimparCamposValorContrato();
}



//*******MÉTODOS*******
function ObjetoRegraValorContrato() {
    var codigo = _valorContrato.Codigo.val();
    var ordem = _valorContrato.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorContrato.ObterOrdencao().length + 1,
        Juncao: _valorContrato.Juncao.val(),
        Condicao: _valorContrato.Condicao.val(),
        Valor: Globalize.parseFloat(_valorContrato.ValorContrato.val()),
    };

    return regra;
}

function LimparCamposValorContrato() {
    _valorContrato.Codigo.val(_valorContrato.Codigo.def);
    _valorContrato.Ordem.val(_valorContrato.Ordem.def);
    _valorContrato.Condicao.val(_valorContrato.Condicao.def);
    _valorContrato.Juncao.val(_valorContrato.Juncao.def);
    _valorContrato.ValorContrato.val(_valorContrato.ValorContrato.def);

    _valorContrato.Adicionar.visible(true);
    _valorContrato.Atualizar.visible(false);
    _valorContrato.Excluir.visible(false);
    _valorContrato.Cancelar.visible(false);
}