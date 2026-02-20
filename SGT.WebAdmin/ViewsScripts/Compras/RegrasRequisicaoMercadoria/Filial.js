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
/// <reference path="RegrasRequisicaoMercadoria.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasFilial;
var _filial;

var Filial = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Filial = PropertyEntity({ text: "Filial:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Filial", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_filial, _gridRegrasFilial, "editarRegraFilialClick");
    });

    // Controle de uso
    this.UsarRegraPorFilial = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por filial:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorFilial.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorFilial(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraFilialClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraFilialClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraFilialClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraFilialClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorFilial(usarRegra) {
    _filial.Visible.visibleFade(usarRegra);
    _filial.Alcadas.required(usarRegra);
}

function loadFilial() {
    _filial = new Filial();
    KoBindings(_filial, "knockoutRegraFilial");

    //-- Busca
    new BuscarTransportadores(_filial.Filial);

    //-- Grid Regras
    _gridRegrasFilial = new GridReordering(_configRegras.infoTable, _filial.Alcadas.idGrid, GeraHeadTable("Filial"));
    _gridRegrasFilial.CarregarGrid();
    $("#" + _filial.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasDescarteLoteProduto(_filial);
    });
}

function editarRegraFilialClick(codigo) {
    // Buscar todas regras
    var listaRegras = _filial.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _filial.Codigo.val(regra.Codigo);
        _filial.Ordem.val(regra.Ordem);
        _filial.Condicao.val(regra.Condicao);
        _filial.Juncao.val(regra.Juncao);

        _filial.Filial.val(regra.Entidade.Descricao);
        _filial.Filial.codEntity(regra.Entidade.Codigo);

        _filial.Adicionar.visible(false);
        _filial.Atualizar.visible(true);
        _filial.Excluir.visible(true);
        _filial.Cancelar.visible(true);
    }
}

function adicionarRegraFilialClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_filial))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraFilial();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_filial);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _filial.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposFilial();
}

function atualizarRegraFilialClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_filial))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraFilial();

    // Buscar todas regras
    var listaRegras = _filial.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _filial.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _filial.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposFilial();
}

function excluirRegraFilialClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_filial);
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
    _filial.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposFilial();
}

function cancelarRegraFilialClick(e, sender) {
    LimparCamposFilial();
}



//*******MÉTODOS*******

function ObjetoRegraFilial() {
    var codigo = _filial.Codigo.val();
    var ordem = _filial.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasFilial.ObterOrdencao().length + 1,
        Juncao: _filial.Juncao.val(),
        Condicao: _filial.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_filial.Filial.codEntity()),
            Descricao: _filial.Filial.val()
        }
    };

    return regra;
}

function LimparCamposFilial() {
    _filial.Codigo.val(_filial.Codigo.def);
    _filial.Ordem.val(_filial.Ordem.def);
    _filial.Condicao.val(_filial.Condicao.def);
    _filial.Juncao.val(_filial.Juncao.def);

    LimparCampoEntity(_filial.Filial);

    _filial.Adicionar.visible(true);
    _filial.Atualizar.visible(false);
    _filial.Excluir.visible(false);
    _filial.Cancelar.visible(false);
}