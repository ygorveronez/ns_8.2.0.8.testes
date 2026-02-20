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


var _gridRegrasValorOcorrencia;
var _valorOcorrencia;

var ValorOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Condicao.getFieldDescription(), issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Juncao.getFieldDescription(), issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoOcorrencia, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.ValorOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ValorOcorrencia.getFieldDescription(), type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ValorOcorrencia, type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorOcorrencia, _gridRegrasValorOcorrencia, "editarRegraValorOcorrenciaClick", typesKnockout.decimal);
    });

    // Controle de uso
    this.UsarRegraPorValorOcorrencia = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AtivarRegraAutorizacaoValorOcorrencia.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorOcorrencia.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorOcorrencia(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorValorOcorrencia(usarRegra) {
    _valorOcorrencia.Visible.visibleFade(usarRegra);
    _valorOcorrencia.Regras.required(usarRegra);
}

function loadValorOcorrencia() {
    _valorOcorrencia = new ValorOcorrencia();
    KoBindings(_valorOcorrencia, "knockoutRegraValorOcorrencia");

    //-- Grid Regras
    _gridRegrasValorOcorrencia = new GridReordering(_configRegras.infoTable, _valorOcorrencia.Regras.idGrid, GeraHeadTable(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ValorOcorrencia));
    _gridRegrasValorOcorrencia.CarregarGrid();
    $("#" + _valorOcorrencia.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_valorOcorrencia);
    });
}

function editarRegraValorOcorrenciaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorOcorrencia.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorOcorrencia.Codigo.val(regra.Codigo);
        _valorOcorrencia.Ordem.val(regra.Ordem);
        _valorOcorrencia.Condicao.val(regra.Condicao);
        _valorOcorrencia.Juncao.val(regra.Juncao);
        _valorOcorrencia.ValorOcorrencia.val(regra.Valor);

        _valorOcorrencia.Adicionar.visible(false);
        _valorOcorrencia.Atualizar.visible(true);
        _valorOcorrencia.Excluir.visible(true);
        _valorOcorrencia.Cancelar.visible(true);
    }
}

function adicionarRegraValorOcorrenciaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorOcorrencia))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraValorOcorrencia();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valorOcorrencia);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    listaRegras.push(regra);
    _valorOcorrencia.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorOcorrencia();
}

function atualizarRegraValorOcorrenciaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorOcorrencia))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraValorOcorrencia();

    // Buscar todas regras
    var listaRegras = _valorOcorrencia.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorOcorrencia.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valorOcorrencia.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorOcorrencia();
}

function excluirRegraValorOcorrenciaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorOcorrencia);
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
    _valorOcorrencia.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposValorOcorrencia();
}

function cancelarRegraValorOcorrenciaClick(e, sender) {
    LimparCamposValorOcorrencia();
}



//*******MÉTODOS*******

function ObjetoRegraValorOcorrencia() {
    var codigo = _valorOcorrencia.Codigo.val();
    var ordem = _valorOcorrencia.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorOcorrencia.ObterOrdencao().length + 1,
        Juncao: _valorOcorrencia.Juncao.val(),
        Condicao: _valorOcorrencia.Condicao.val(),
        Valor: Globalize.parseFloat(_valorOcorrencia.ValorOcorrencia.val())
    };

    return regra;
}

function LimparCamposValorOcorrencia() {
    _valorOcorrencia.Codigo.val(_valorOcorrencia.Codigo.def);
    _valorOcorrencia.Ordem.val(_valorOcorrencia.Ordem.def);
    _valorOcorrencia.Condicao.val(_valorOcorrencia.Condicao.def);
    _valorOcorrencia.Juncao.val(_valorOcorrencia.Juncao.def);
    _valorOcorrencia.ValorOcorrencia.val(_valorOcorrencia.ValorOcorrencia.def);

    _valorOcorrencia.Adicionar.visible(true);
    _valorOcorrencia.Atualizar.visible(false);
    _valorOcorrencia.Excluir.visible(false);
    _valorOcorrencia.Cancelar.visible(false);
}