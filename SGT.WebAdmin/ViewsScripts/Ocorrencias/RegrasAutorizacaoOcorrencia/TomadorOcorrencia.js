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


var _gridRegrasTomadorOcorrencia;
var _tomadorOcorrencia;

var TomadorOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Condicao.getFieldDescription(), issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Juncao.getFieldDescription(), issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoOcorrencia, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.TomadorOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TomadorOcorrencia.getFieldDescription(), type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TomadorOcorrencia, type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_tomadorOcorrencia, _gridRegrasTomadorOcorrencia, "editarRegraTomadorOcorrenciaClick");
    });

    // Controle de uso
    this.UsarRegraPorTomadorOcorrencia = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AtivarRegraAutorizacaoPorTomadorOcorrencia.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorTomadorOcorrencia.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorTomadorOcorrencia(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTomadorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTomadorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTomadorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTomadorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorTomadorOcorrencia(usarRegra) {
    _tomadorOcorrencia.Visible.visibleFade(usarRegra);
    _tomadorOcorrencia.Regras.required(usarRegra);
}

function loadTomadorOcorrencia() {
    _tomadorOcorrencia = new TomadorOcorrencia();
    KoBindings(_tomadorOcorrencia, "knockoutRegraTomadorOcorrencia");

    //-- Busca
    new BuscarClientes(_tomadorOcorrencia.TomadorOcorrencia);

    //-- Grid Regras
    _gridRegrasTomadorOcorrencia = new GridReordering(_configRegras.infoTable, _tomadorOcorrencia.Regras.idGrid, GeraHeadTable(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TomadorOcorrencia));
    _gridRegrasTomadorOcorrencia.CarregarGrid();
    $("#" + _tomadorOcorrencia.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_tomadorOcorrencia);
    });
}

function editarRegraTomadorOcorrenciaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _tomadorOcorrencia.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _tomadorOcorrencia.Codigo.val(regra.Codigo);
        _tomadorOcorrencia.Ordem.val(regra.Ordem);
        _tomadorOcorrencia.Condicao.val(regra.Condicao);
        _tomadorOcorrencia.Juncao.val(regra.Juncao);

        _tomadorOcorrencia.TomadorOcorrencia.val(regra.Entidade.Descricao);
        _tomadorOcorrencia.TomadorOcorrencia.codEntity(regra.Entidade.Codigo);

        _tomadorOcorrencia.Adicionar.visible(false);
        _tomadorOcorrencia.Atualizar.visible(true);
        _tomadorOcorrencia.Excluir.visible(true);
        _tomadorOcorrencia.Cancelar.visible(true);
    }
}

function adicionarRegraTomadorOcorrenciaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tomadorOcorrencia))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraTomadorOcorrencia();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_tomadorOcorrencia);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    listaRegras.push(regra);
    _tomadorOcorrencia.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTomadorOcorrencia();
}

function atualizarRegraTomadorOcorrenciaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tomadorOcorrencia))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraTomadorOcorrencia();

    // Buscar todas regras
    var listaRegras = _tomadorOcorrencia.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _tomadorOcorrencia.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _tomadorOcorrencia.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTomadorOcorrencia();
}

function excluirRegraTomadorOcorrenciaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_tomadorOcorrencia);
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
    _tomadorOcorrencia.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposTomadorOcorrencia();
}

function cancelarRegraTomadorOcorrenciaClick(e, sender) {
    LimparCamposTomadorOcorrencia();
}



//*******MÉTODOS*******

function ObjetoRegraTomadorOcorrencia() {
    var codigo = _tomadorOcorrencia.Codigo.val();
    var ordem = _tomadorOcorrencia.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTomadorOcorrencia.ObterOrdencao().length + 1,
        Juncao: _tomadorOcorrencia.Juncao.val(),
        Condicao: _tomadorOcorrencia.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_tomadorOcorrencia.TomadorOcorrencia.codEntity()),
            Descricao: _tomadorOcorrencia.TomadorOcorrencia.val()
        }
    };

    return regra;
}

function LimparCamposTomadorOcorrencia() {
    _tomadorOcorrencia.Codigo.val(_tomadorOcorrencia.Codigo.def);
    _tomadorOcorrencia.Ordem.val(_tomadorOcorrencia.Ordem.def);
    _tomadorOcorrencia.Condicao.val(_tomadorOcorrencia.Condicao.def);
    _tomadorOcorrencia.Juncao.val(_tomadorOcorrencia.Juncao.def);

    LimparCampoEntity(_tomadorOcorrencia.TomadorOcorrencia);

    _tomadorOcorrencia.Adicionar.visible(true);
    _tomadorOcorrencia.Atualizar.visible(false);
    _tomadorOcorrencia.Excluir.visible(false);
    _tomadorOcorrencia.Cancelar.visible(false);
}