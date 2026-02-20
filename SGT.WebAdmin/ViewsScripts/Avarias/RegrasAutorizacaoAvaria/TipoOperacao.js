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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoAvaria.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="RegrasAutorizacaoAvaria.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasTipoOperacao;
var _tipoOperacao;

var TipoOperacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoAvaria.IgualA), options: _condicaoAutorizaoAvariaEntidade, def: EnumCondicaoAutorizaoAvaria.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoAvaria.E), options: _juncaoAutorizaoAvaria, def: EnumJuncaoAutorizaoAvaria.E });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Tipo de Operação", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_tipoOperacao, _gridRegrasTipoOperacao, "editarRegraTipoOperacaoClick");
    });

    // Controle de uso
    this.UsarRegraPorTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por tipo de operação:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorTipoOperacao.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorTipoOperacao(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTipoOperacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTipoOperacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTipoOperacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTipoOperacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorTipoOperacao(usarRegra) {
    _tipoOperacao.Visible.visibleFade(usarRegra);
    _tipoOperacao.Regras.required(usarRegra);
}

function loadTipoOperacao() {
    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutRegraTipoOperacao");

    //-- Busca
    new BuscarTiposOperacao(_tipoOperacao.TipoOperacao);

    //-- Grid Regras
    _gridRegrasTipoOperacao = new GridReordering(_configRegras.infoTable, _tipoOperacao.Regras.idGrid, GeraHeadTable("Tipo de Operação"));
    _gridRegrasTipoOperacao.CarregarGrid();
    $("#" + _tipoOperacao.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasAvaria(_tipoOperacao);
    });
}

function editarRegraTipoOperacaoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _tipoOperacao.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _tipoOperacao.Codigo.val(regra.Codigo);
        _tipoOperacao.Ordem.val(regra.Ordem);
        _tipoOperacao.Condicao.val(regra.Condicao);
        _tipoOperacao.Juncao.val(regra.Juncao);

        _tipoOperacao.TipoOperacao.val(regra.Entidade.Descricao);
        _tipoOperacao.TipoOperacao.codEntity(regra.Entidade.Codigo);

        _tipoOperacao.Adicionar.visible(false);
        _tipoOperacao.Atualizar.visible(true);
        _tipoOperacao.Excluir.visible(true);
        _tipoOperacao.Cancelar.visible(true);
    }
}

function adicionarRegraTipoOperacaoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoOperacao))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTipoOperacao();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_tipoOperacao);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _tipoOperacao.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoOperacao();
}

function atualizarRegraTipoOperacaoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoOperacao))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTipoOperacao();

    // Buscar todas regras
    var listaRegras = _tipoOperacao.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _tipoOperacao.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _tipoOperacao.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoOperacao();
}

function excluirRegraTipoOperacaoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_tipoOperacao);
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
    _tipoOperacao.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposTipoOperacao();
}

function cancelarRegraTipoOperacaoClick(e, sender) {
    LimparCamposTipoOperacao();
}



//*******MÉTODOS*******

function ObjetoRegraTipoOperacao() {
    var codigo = _tipoOperacao.Codigo.val();
    var ordem = _tipoOperacao.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTipoOperacao.ObterOrdencao().length + 1,
        Juncao: _tipoOperacao.Juncao.val(),
        Condicao: _tipoOperacao.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_tipoOperacao.TipoOperacao.codEntity()),
            Descricao: _tipoOperacao.TipoOperacao.val()
        }
    };

    return regra;
}

function LimparCamposTipoOperacao() {
    _tipoOperacao.Codigo.val(_tipoOperacao.Codigo.def);
    _tipoOperacao.Ordem.val(_tipoOperacao.Ordem.def);
    _tipoOperacao.Condicao.val(_tipoOperacao.Condicao.def);
    _tipoOperacao.Juncao.val(_tipoOperacao.Juncao.def);

    LimparCampoEntity(_tipoOperacao.TipoOperacao);

    _tipoOperacao.Adicionar.visible(true);
    _tipoOperacao.Atualizar.visible(false);
    _tipoOperacao.Excluir.visible(false);
    _tipoOperacao.Cancelar.visible(false);
}