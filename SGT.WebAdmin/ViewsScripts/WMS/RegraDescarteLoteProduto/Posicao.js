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


var _gridRegrasPosicao;
var _posicao;

var Posicao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Posicao = PropertyEntity({ text: "Posição:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Posição", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_posicao, _gridRegrasPosicao, "editarRegraPosicaoClick");
    });

    // Controle de uso
    this.UsarRegraPorPosicao = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por posição:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorPosicao.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorPosicao(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraPosicaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraPosicaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraPosicaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraPosicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorPosicao(usarRegra) {
    _posicao.Visible.visibleFade(usarRegra);
    _posicao.Alcadas.required(usarRegra);
}

function loadPosicao() {
    _posicao = new Posicao();
    KoBindings(_posicao, "knockoutRegraPosicao");

    //-- Busca
    new BuscarDepositoPosicao(_posicao.Posicao);

    //-- Grid Regras
    _gridRegrasPosicao = new GridReordering(_configRegras.infoTable, _posicao.Alcadas.idGrid, GeraHeadTable("Posição"));
    _gridRegrasPosicao.CarregarGrid();
    $("#" + _posicao.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasDescarteLoteProduto(_posicao);
    });
}

function editarRegraPosicaoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _posicao.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _posicao.Codigo.val(regra.Codigo);
        _posicao.Ordem.val(regra.Ordem);
        _posicao.Condicao.val(regra.Condicao);
        _posicao.Juncao.val(regra.Juncao);

        _posicao.Posicao.val(regra.Entidade.Descricao);
        _posicao.Posicao.codEntity(regra.Entidade.Codigo);

        _posicao.Adicionar.visible(false);
        _posicao.Atualizar.visible(true);
        _posicao.Excluir.visible(true);
        _posicao.Cancelar.visible(true);
    }
}

function adicionarRegraPosicaoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_posicao))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraPosicao();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_posicao);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _posicao.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposPosicao();
}

function atualizarRegraPosicaoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_posicao))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraPosicao();

    // Buscar todas regras
    var listaRegras = _posicao.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _posicao.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _posicao.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposPosicao();
}

function excluirRegraPosicaoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_posicao);
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
    _posicao.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposPosicao();
}

function cancelarRegraPosicaoClick(e, sender) {
    LimparCamposPosicao();
}



//*******MÉTODOS*******

function ObjetoRegraPosicao() {
    var codigo = _posicao.Codigo.val();
    var ordem = _posicao.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasPosicao.ObterOrdencao().length + 1,
        Juncao: _posicao.Juncao.val(),
        Condicao: _posicao.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_posicao.Posicao.codEntity()),
            Descricao: _posicao.Posicao.val()
        }
    };

    return regra;
}

function LimparCamposPosicao() {
    _posicao.Codigo.val(_posicao.Codigo.def);
    _posicao.Ordem.val(_posicao.Ordem.def);
    _posicao.Condicao.val(_posicao.Condicao.def);
    _posicao.Juncao.val(_posicao.Juncao.def);

    LimparCampoEntity(_posicao.Posicao);

    _posicao.Adicionar.visible(true);
    _posicao.Atualizar.visible(false);
    _posicao.Excluir.visible(false);
    _posicao.Cancelar.visible(false);
}