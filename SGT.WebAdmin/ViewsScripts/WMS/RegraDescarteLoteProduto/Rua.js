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


var _gridRegrasRua;
var _rua;

var Rua = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Rua = PropertyEntity({ text: "Rua:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Rua", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_rua, _gridRegrasRua, "editarRegraRuaClick");
    });

    // Controle de uso
    this.UsarRegraPorRua = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por rua:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorRua.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorRua(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraRuaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraRuaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraRuaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraRuaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorRua(usarRegra) {
    _rua.Visible.visibleFade(usarRegra);
    _rua.Alcadas.required(usarRegra);
}

function loadRua() {
    _rua = new Rua();
    KoBindings(_rua, "knockoutRegraRua");

    //-- Busca
    new BuscarDepositoRua(_rua.Rua);

    //-- Grid Regras
    _gridRegrasRua = new GridReordering(_configRegras.infoTable, _rua.Alcadas.idGrid, GeraHeadTable("Rua"));
    _gridRegrasRua.CarregarGrid();
    $("#" + _rua.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasDescarteLoteProduto(_rua);
    });
}

function editarRegraRuaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _rua.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _rua.Codigo.val(regra.Codigo);
        _rua.Ordem.val(regra.Ordem);
        _rua.Condicao.val(regra.Condicao);
        _rua.Juncao.val(regra.Juncao);

        _rua.Rua.val(regra.Entidade.Descricao);
        _rua.Rua.codEntity(regra.Entidade.Codigo);

        _rua.Adicionar.visible(false);
        _rua.Atualizar.visible(true);
        _rua.Excluir.visible(true);
        _rua.Cancelar.visible(true);
    }
}

function adicionarRegraRuaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_rua))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraRua();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_rua);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _rua.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposRua();
}

function atualizarRegraRuaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_rua))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraRua();

    // Buscar todas regras
    var listaRegras = _rua.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _rua.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _rua.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposRua();
}

function excluirRegraRuaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_rua);
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
    _rua.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposRua();
}

function cancelarRegraRuaClick(e, sender) {
    LimparCamposRua();
}



//*******MÉTODOS*******

function ObjetoRegraRua() {
    var codigo = _rua.Codigo.val();
    var ordem = _rua.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasRua.ObterOrdencao().length + 1,
        Juncao: _rua.Juncao.val(),
        Condicao: _rua.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_rua.Rua.codEntity()),
            Descricao: _rua.Rua.val()
        }
    };

    return regra;
}

function LimparCamposRua() {
    _rua.Codigo.val(_rua.Codigo.def);
    _rua.Ordem.val(_rua.Ordem.def);
    _rua.Condicao.val(_rua.Condicao.def);
    _rua.Juncao.val(_rua.Juncao.def);

    LimparCampoEntity(_rua.Rua);

    _rua.Adicionar.visible(true);
    _rua.Atualizar.visible(false);
    _rua.Excluir.visible(false);
    _rua.Cancelar.visible(false);
}