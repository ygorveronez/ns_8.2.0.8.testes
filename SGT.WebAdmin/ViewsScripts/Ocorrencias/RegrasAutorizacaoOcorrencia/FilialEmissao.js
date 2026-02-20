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


var _gridRegrasFilialEmissao;
var _filialEmissao;

var FilialEmissao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Condicao.getFieldDescription(), issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Juncao.getFieldDescription(), issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoOcorrencia, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.FilialEmissao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.FilialEmissao.getFieldDescription(), type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.FilialEmissao, type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_filialEmissao, _gridRegrasFilialEmissao, "editarRegraFilialEmissaoClick");
    });

    // Controle de uso
    this.UsarRegraPorFilialEmissao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AtivarRegraAutorizacaoPorFilialEmissao.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorFilialEmissao.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorFilialEmissao(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraFilialEmissaoClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraFilialEmissaoClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraFilialEmissaoClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraFilialEmissaoClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorFilialEmissao(usarRegra) {
    _filialEmissao.Visible.visibleFade(usarRegra);
    _filialEmissao.Regras.required(usarRegra);
}

function loadFilialEmissao() {
    _filialEmissao = new FilialEmissao();
    KoBindings(_filialEmissao, "knockoutRegraFilialEmissao");

    //-- Busca
    new BuscarFilial(_filialEmissao.FilialEmissao);

    //-- Grid Regras
    _gridRegrasFilialEmissao = new GridReordering(_configRegras.infoTable, _filialEmissao.Regras.idGrid, GeraHeadTable(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.FilialEmissao));
    _gridRegrasFilialEmissao.CarregarGrid();
    $("#" + _filialEmissao.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_filialEmissao);
    });
}

function editarRegraFilialEmissaoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _filialEmissao.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _filialEmissao.Codigo.val(regra.Codigo);
        _filialEmissao.Ordem.val(regra.Ordem);
        _filialEmissao.Condicao.val(regra.Condicao);
        _filialEmissao.Juncao.val(regra.Juncao);

        _filialEmissao.FilialEmissao.val(regra.Entidade.Descricao);
        _filialEmissao.FilialEmissao.codEntity(regra.Entidade.Codigo);

        _filialEmissao.Adicionar.visible(false);
        _filialEmissao.Atualizar.visible(true);
        _filialEmissao.Excluir.visible(true);
        _filialEmissao.Cancelar.visible(true);
    }
}

function adicionarRegraFilialEmissaoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_filialEmissao))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraFilialEmissao();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_filialEmissao);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    listaRegras.push(regra);
    _filialEmissao.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposFilialEmissao();
}

function atualizarRegraFilialEmissaoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_filialEmissao))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraFilialEmissao();

    // Buscar todas regras
    var listaRegras = _filialEmissao.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _filialEmissao.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _filialEmissao.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposFilialEmissao();
}

function excluirRegraFilialEmissaoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_filialEmissao);
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
    _filialEmissao.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposFilialEmissao();
}

function cancelarRegraFilialEmissaoClick(e, sender) {
    LimparCamposFilialEmissao();
}



//*******MÉTODOS*******

function ObjetoRegraFilialEmissao() {
    var codigo = _filialEmissao.Codigo.val();
    var ordem = _filialEmissao.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasFilialEmissao.ObterOrdencao().length + 1,
        Juncao: _filialEmissao.Juncao.val(),
        Condicao: _filialEmissao.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_filialEmissao.FilialEmissao.codEntity()),
            Descricao: _filialEmissao.FilialEmissao.val()
        }
    };

    return regra;
}

function LimparCamposFilialEmissao() {
    _filialEmissao.Codigo.val(_filialEmissao.Codigo.def);
    _filialEmissao.Ordem.val(_filialEmissao.Ordem.def);
    _filialEmissao.Condicao.val(_filialEmissao.Condicao.def);
    _filialEmissao.Juncao.val(_filialEmissao.Juncao.def);

    LimparCampoEntity(_filialEmissao.FilialEmissao);

    _filialEmissao.Adicionar.visible(true);
    _filialEmissao.Atualizar.visible(false);
    _filialEmissao.Excluir.visible(false);
    _filialEmissao.Cancelar.visible(false);
}