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
/// <reference path="RegraDescarteLoteProduto.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasQuantidade;
var _quantidade;

var Quantidade = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Quantidade = PropertyEntity({ text: "Quantidade:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Quantidade", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_quantidade, _gridRegrasQuantidade, "editarRegraQuantidadeClick", true);
    });

    // Controle de uso
    this.UsarRegraPorQuantidade = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por quantidade:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorQuantidade.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorQuantidade(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraQuantidadeClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraQuantidadeClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraQuantidadeClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraQuantidadeClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorQuantidade(usarRegra) {
    _quantidade.Visible.visibleFade(usarRegra);
    _quantidade.Alcadas.required(usarRegra);
}

function loadQuantidade() {
    _quantidade = new Quantidade();
    KoBindings(_quantidade, "knockoutRegraQuantidade");

    //-- Grid Regras
    _gridRegrasQuantidade = new GridReordering(_configRegras.infoTable, _quantidade.Alcadas.idGrid, GeraHeadTable("Quantidade"));
    _gridRegrasQuantidade.CarregarGrid();
    $("#" + _quantidade.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasDescarteLoteProduto(_quantidade);
    });
}

function editarRegraQuantidadeClick(codigo) {
    // Buscar todas regras
    var listaRegras = _quantidade.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _quantidade.Codigo.val(regra.Codigo);
        _quantidade.Ordem.val(regra.Ordem);
        _quantidade.Condicao.val(regra.Condicao);
        _quantidade.Juncao.val(regra.Juncao);
        _quantidade.Quantidade.val(regra.Quantidade);

        _quantidade.Adicionar.visible(false);
        _quantidade.Atualizar.visible(true);
        _quantidade.Excluir.visible(true);
        _quantidade.Cancelar.visible(true);
    }
}

function adicionarRegraQuantidadeClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_quantidade))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraQuantidade();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_quantidade);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _quantidade.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposQuantidade();
}

function atualizarRegraQuantidadeClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_quantidade))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraQuantidade();

    // Buscar todas regras
    var listaRegras = _quantidade.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _quantidade.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _quantidade.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposQuantidade();
}

function excluirRegraQuantidadeClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_quantidade);
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
    _quantidade.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposQuantidade();
}

function cancelarRegraQuantidadeClick(e, sender) {
    LimparCamposQuantidade();
}



//*******MÉTODOS*******

function ObjetoRegraQuantidade() {
    var codigo = _quantidade.Codigo.val();
    var ordem = _quantidade.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasQuantidade.ObterOrdencao().length + 1,
        Juncao: _quantidade.Juncao.val(),
        Condicao: _quantidade.Condicao.val(),
        Valor: Globalize.parseFloat(_quantidade.Quantidade.val())
    };

    return regra;
}

function LimparCamposQuantidade() {
    _quantidade.Codigo.val(_quantidade.Codigo.def);
    _quantidade.Ordem.val(_quantidade.Ordem.def);
    _quantidade.Condicao.val(_quantidade.Condicao.def);
    _quantidade.Juncao.val(_quantidade.Juncao.def);
    _quantidade.Quantidade.val(_quantidade.Quantidade.def);

    _quantidade.Adicionar.visible(true);
    _quantidade.Atualizar.visible(false);
    _quantidade.Excluir.visible(false);
    _quantidade.Cancelar.visible(false);
}