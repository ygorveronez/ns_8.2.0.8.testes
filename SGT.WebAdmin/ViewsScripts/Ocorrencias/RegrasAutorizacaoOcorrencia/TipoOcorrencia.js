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


var _gridRegrasTipoOcorrencia;
var _tipoOcorrencia;

var TipoOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Condicao.getFieldDescription(), issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Juncao.getFieldDescription(), issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoOcorrencia, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.TipoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOcorrencia.getFieldDescription(), type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOcorrencia, type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_tipoOcorrencia, _gridRegrasTipoOcorrencia, "editarRegraTipoOcorrenciaClick");
    });

    // Controle de uso
    this.UsarRegraPorTipoOcorrencia = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AtivarRegraAutorizacaoPorTipoOcorrencia.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorTipoOcorrencia.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorTipoOcorrencia(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTipoOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTipoOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTipoOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTipoOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorTipoOcorrencia(usarRegra) {
    _tipoOcorrencia.Visible.visibleFade(usarRegra);
    _tipoOcorrencia.Regras.required(usarRegra);
}

function loadTipoOcorrencia() {
    _tipoOcorrencia = new TipoOcorrencia();
    KoBindings(_tipoOcorrencia, "knockoutRegraTipoOcorrencia");

    //-- Busca
    new BuscarTipoOcorrencia(_tipoOcorrencia.TipoOcorrencia, null, null, null, null, null, null, null, null, null, null, null, true);
    
    //-- Grid Regras
    _gridRegrasTipoOcorrencia = new GridReordering(_configRegras.infoTable, _tipoOcorrencia.Regras.idGrid, GeraHeadTable(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOcorrencia));
    _gridRegrasTipoOcorrencia.CarregarGrid();
    $("#" + _tipoOcorrencia.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_tipoOcorrencia);
    });
}

function editarRegraTipoOcorrenciaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _tipoOcorrencia.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _tipoOcorrencia.Codigo.val(regra.Codigo);
        _tipoOcorrencia.Ordem.val(regra.Ordem);
        _tipoOcorrencia.Condicao.val(regra.Condicao);
        _tipoOcorrencia.Juncao.val(regra.Juncao);

        _tipoOcorrencia.TipoOcorrencia.val(regra.Entidade.Descricao);
        _tipoOcorrencia.TipoOcorrencia.codEntity(regra.Entidade.Codigo);

        _tipoOcorrencia.Adicionar.visible(false);
        _tipoOcorrencia.Atualizar.visible(true);
        _tipoOcorrencia.Excluir.visible(true);
        _tipoOcorrencia.Cancelar.visible(true);
    }
}

function adicionarRegraTipoOcorrenciaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoOcorrencia))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraTipoOcorrencia();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_tipoOcorrencia);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    listaRegras.push(regra);
    _tipoOcorrencia.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoOcorrencia();
}

function atualizarRegraTipoOcorrenciaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoOcorrencia))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraTipoOcorrencia();

    // Buscar todas regras
    var listaRegras = _tipoOcorrencia.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _tipoOcorrencia.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _tipoOcorrencia.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoOcorrencia();
}

function excluirRegraTipoOcorrenciaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_tipoOcorrencia);
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
    _tipoOcorrencia.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposTipoOcorrencia();
}

function cancelarRegraTipoOcorrenciaClick(e, sender) {
    LimparCamposTipoOcorrencia();
}



//*******MÉTODOS*******

function ObjetoRegraTipoOcorrencia() {
    var codigo = _tipoOcorrencia.Codigo.val();
    var ordem = _tipoOcorrencia.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTipoOcorrencia.ObterOrdencao().length + 1,
        Juncao: _tipoOcorrencia.Juncao.val(),
        Condicao: _tipoOcorrencia.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_tipoOcorrencia.TipoOcorrencia.codEntity()),
            Descricao: _tipoOcorrencia.TipoOcorrencia.val()
        }
    };

    return regra;
}

function LimparCamposTipoOcorrencia() {
    _tipoOcorrencia.Codigo.val(_tipoOcorrencia.Codigo.def);
    _tipoOcorrencia.Ordem.val(_tipoOcorrencia.Ordem.def);
    _tipoOcorrencia.Condicao.val(_tipoOcorrencia.Condicao.def);
    _tipoOcorrencia.Juncao.val(_tipoOcorrencia.Juncao.def);

    LimparCampoEntity(_tipoOcorrencia.TipoOcorrencia);

    _tipoOcorrencia.Adicionar.visible(true);
    _tipoOcorrencia.Atualizar.visible(false);
    _tipoOcorrencia.Excluir.visible(false);
    _tipoOcorrencia.Cancelar.visible(false);
}