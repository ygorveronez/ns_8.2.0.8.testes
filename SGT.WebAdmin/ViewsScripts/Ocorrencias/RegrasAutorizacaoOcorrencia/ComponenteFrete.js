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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="RegrasAutorizacaoOcorrencia.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasComponenteFrete;
var _componenteFrete;

var ComponenteFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Condicao.getFieldDescription(), issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Juncao.getFieldDescription(), issue: 1735,  val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoOcorrencia, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.ComponenteFrete = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ComponenteFrete.getFieldDescription(), type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ComponenteFrete, type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_componenteFrete, _gridRegrasComponenteFrete, "editarRegraComponenteFreteClick");
    });

    // Controle de uso
    this.UsarRegraPorComponenteFrete = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AtivarRegraAutorizacaoPorComponenteFrete.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorComponenteFrete.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorComponenteFrete(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraComponenteFreteClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraComponenteFreteClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraComponenteFreteClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraComponenteFreteClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorComponenteFrete(usarRegra) {
    _componenteFrete.Visible.visibleFade(usarRegra);
    _componenteFrete.Regras.required(usarRegra);
}

function loadComponenteFrete() {
    _componenteFrete = new ComponenteFrete();
    KoBindings(_componenteFrete, "knockoutRegraComponenteFrete");

    //-- Busca
    new BuscarComponentesDeFrete(_componenteFrete.ComponenteFrete);

    //-- Grid Regras
    _gridRegrasComponenteFrete = new GridReordering(_configRegras.infoTable, _componenteFrete.Regras.idGrid, GeraHeadTable(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ComponenteFrete));
    _gridRegrasComponenteFrete.CarregarGrid();
    $("#" + _componenteFrete.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_componenteFrete);
    });
}

function editarRegraComponenteFreteClick(codigo) {
    // Buscar todas regras
    var listaRegras = _componenteFrete.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _componenteFrete.Codigo.val(regra.Codigo);
        _componenteFrete.Ordem.val(regra.Ordem);
        _componenteFrete.Condicao.val(regra.Condicao);
        _componenteFrete.Juncao.val(regra.Juncao);

        _componenteFrete.ComponenteFrete.val(regra.Entidade.Descricao);
        _componenteFrete.ComponenteFrete.codEntity(regra.Entidade.Codigo);

        _componenteFrete.Adicionar.visible(false);
        _componenteFrete.Atualizar.visible(true);
        _componenteFrete.Excluir.visible(true);
        _componenteFrete.Cancelar.visible(true);
    }
}

function adicionarRegraComponenteFreteClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_componenteFrete))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraComponenteFrete();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_componenteFrete);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    listaRegras.push(regra);
    _componenteFrete.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposComponenteFrete();
}

function atualizarRegraComponenteFreteClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_componenteFrete))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraComponenteFrete();

    // Buscar todas regras
    var listaRegras = _componenteFrete.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _componenteFrete.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _componenteFrete.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposComponenteFrete();
}

function excluirRegraComponenteFreteClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_componenteFrete);
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
    _componenteFrete.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposComponenteFrete();
}

function cancelarRegraComponenteFreteClick(e, sender) {
    LimparCamposComponenteFrete();
}



//*******MÉTODOS*******

function ObjetoRegraComponenteFrete() {
    var codigo = _componenteFrete.Codigo.val();
    var ordem = _componenteFrete.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasComponenteFrete.ObterOrdencao().length + 1,
        Juncao: _componenteFrete.Juncao.val(),
        Condicao: _componenteFrete.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_componenteFrete.ComponenteFrete.codEntity()),
            Descricao: _componenteFrete.ComponenteFrete.val()
        }
    };

    return regra;
}

function LimparCamposComponenteFrete() {
    _componenteFrete.Codigo.val(_componenteFrete.Codigo.def);
    _componenteFrete.Ordem.val(_componenteFrete.Ordem.def);
    _componenteFrete.Condicao.val(_componenteFrete.Condicao.def);
    _componenteFrete.Juncao.val(_componenteFrete.Juncao.def);

    LimparCampoEntity(_componenteFrete.ComponenteFrete);

    _componenteFrete.Adicionar.visible(true);
    _componenteFrete.Atualizar.visible(false);
    _componenteFrete.Excluir.visible(false);
    _componenteFrete.Cancelar.visible(false);
}