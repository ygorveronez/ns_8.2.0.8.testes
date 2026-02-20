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


var _gridRegrasDeposito;
var _deposito;

var Deposito = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Deposito = PropertyEntity({ text: "Depósito Armazenamento:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Depósito Armazenamento", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_deposito, _gridRegrasDeposito, "editarRegraDepositoClick");
    });

    // Controle de uso
    this.UsarRegraPorDeposito = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por depósito armazenamento:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorDeposito.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorDeposito(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraDepositoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraDepositoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraDepositoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraDepositoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorDeposito(usarRegra) {
    _deposito.Visible.visibleFade(usarRegra);
    _deposito.Alcadas.required(usarRegra);
}

function loadDeposito() {
    _deposito = new Deposito();
    KoBindings(_deposito, "knockoutRegraDeposito");

    //-- Busca
    new BuscarDeposito(_deposito.Deposito);

    //-- Grid Regras
    _gridRegrasDeposito = new GridReordering(_configRegras.infoTable, _deposito.Alcadas.idGrid, GeraHeadTable("Depósito Armazenamento"));
    _gridRegrasDeposito.CarregarGrid();
    $("#" + _deposito.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasDescarteLoteProduto(_deposito);
    });
}

function editarRegraDepositoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _deposito.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _deposito.Codigo.val(regra.Codigo);
        _deposito.Ordem.val(regra.Ordem);
        _deposito.Condicao.val(regra.Condicao);
        _deposito.Juncao.val(regra.Juncao);

        _deposito.Deposito.val(regra.Entidade.Descricao);
        _deposito.Deposito.codEntity(regra.Entidade.Codigo);

        _deposito.Adicionar.visible(false);
        _deposito.Atualizar.visible(true);
        _deposito.Excluir.visible(true);
        _deposito.Cancelar.visible(true);
    }
}

function adicionarRegraDepositoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_deposito))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDeposito();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_deposito);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _deposito.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposDeposito();
}

function atualizarRegraDepositoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_deposito))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDeposito();

    // Buscar todas regras
    var listaRegras = _deposito.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _deposito.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _deposito.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposDeposito();
}

function excluirRegraDepositoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_deposito);
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
    _deposito.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposDeposito();
}

function cancelarRegraDepositoClick(e, sender) {
    LimparCamposDeposito();
}



//*******MÉTODOS*******

function ObjetoRegraDeposito() {
    var codigo = _deposito.Codigo.val();
    var ordem = _deposito.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasDeposito.ObterOrdencao().length + 1,
        Juncao: _deposito.Juncao.val(),
        Condicao: _deposito.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_deposito.Deposito.codEntity()),
            Descricao: _deposito.Deposito.val()
        }
    };

    return regra;
}

function LimparCamposDeposito() {
    _deposito.Codigo.val(_deposito.Codigo.def);
    _deposito.Ordem.val(_deposito.Ordem.def);
    _deposito.Condicao.val(_deposito.Condicao.def);
    _deposito.Juncao.val(_deposito.Juncao.def);

    LimparCampoEntity(_deposito.Deposito);

    _deposito.Adicionar.visible(true);
    _deposito.Atualizar.visible(false);
    _deposito.Excluir.visible(false);
    _deposito.Cancelar.visible(false);
}