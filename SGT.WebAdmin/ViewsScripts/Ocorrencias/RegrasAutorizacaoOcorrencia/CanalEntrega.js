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


var _gridRegrasCanalEntrega;
var _CanalEntrega;

var CanalEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Condicao.getFieldDescription(), issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Juncao.getFieldDescription(), issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoOcorrencia, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.CanalEntrega = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.CanalEntrega.getFieldDescription(), type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.CanalEntrega, type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_CanalEntrega, _gridRegrasCanalEntrega, "editarRegraCanalEntregaClick");
    });

    // Controle de uso
    this.UsarRegraPorCanalEntrega = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AtivarRegraAutorizacaoCanalEntrega.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorCanalEntrega.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorCanalEntrega(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraCanalEntregaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraCanalEntregaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraCanalEntregaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraCanalEntregaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorCanalEntrega(usarRegra) {
    _CanalEntrega.Visible.visibleFade(usarRegra);
    _CanalEntrega.Regras.required(usarRegra);
}

function loadCanalEntrega() {
    _CanalEntrega = new CanalEntrega();
    KoBindings(_CanalEntrega, "knockoutRegraCanalEntrega");

    //-- Busca
    new BuscarCanaisEntrega(_CanalEntrega.CanalEntrega);

    //-- Grid Regras
    _gridRegrasCanalEntrega = new GridReordering(_configRegras.infoTable, _CanalEntrega.Regras.idGrid, GeraHeadTable(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.CanalEntrega));
    _gridRegrasCanalEntrega.CarregarGrid();
    $("#" + _CanalEntrega.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_CanalEntrega);
    });
}

function editarRegraCanalEntregaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _CanalEntrega.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _CanalEntrega.Codigo.val(regra.Codigo);
        _CanalEntrega.Ordem.val(regra.Ordem);
        _CanalEntrega.Condicao.val(regra.Condicao);
        _CanalEntrega.Juncao.val(regra.Juncao);

        _CanalEntrega.CanalEntrega.val(regra.Entidade.Descricao);
        _CanalEntrega.CanalEntrega.codEntity(regra.Entidade.Codigo);

        _CanalEntrega.Adicionar.visible(false);
        _CanalEntrega.Atualizar.visible(true);
        _CanalEntrega.Excluir.visible(true);
        _CanalEntrega.Cancelar.visible(true);
    }
}

function adicionarRegraCanalEntregaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_CanalEntrega))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraCanalEntrega();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_CanalEntrega);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    listaRegras.push(regra);
    _CanalEntrega.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposCanalEntrega();
}

function atualizarRegraCanalEntregaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_CanalEntrega))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraCanalEntrega();

    // Buscar todas regras
    var listaRegras = _CanalEntrega.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _CanalEntrega.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _CanalEntrega.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposCanalEntrega();
}

function excluirRegraCanalEntregaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_CanalEntrega);
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
    _CanalEntrega.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposCanalEntrega();
}

function cancelarRegraCanalEntregaClick(e, sender) {
    LimparCamposCanalEntrega();
}



//*******MÉTODOS*******

function ObjetoRegraCanalEntrega() {
    var codigo = _CanalEntrega.Codigo.val();
    var ordem = _CanalEntrega.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasCanalEntrega.ObterOrdencao().length + 1,
        Juncao: _CanalEntrega.Juncao.val(),
        Condicao: _CanalEntrega.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_CanalEntrega.CanalEntrega.codEntity()),
            Descricao: _CanalEntrega.CanalEntrega.val()
        }
    };

    return regra;
}

function LimparCamposCanalEntrega() {
    _CanalEntrega.Codigo.val(_CanalEntrega.Codigo.def);
    _CanalEntrega.Ordem.val(_CanalEntrega.Ordem.def);
    _CanalEntrega.Condicao.val(_CanalEntrega.Condicao.def);
    _CanalEntrega.Juncao.val(_CanalEntrega.Juncao.def);

    LimparCampoEntity(_CanalEntrega.CanalEntrega);

    _CanalEntrega.Adicionar.visible(true);
    _CanalEntrega.Atualizar.visible(false);
    _CanalEntrega.Excluir.visible(false);
    _CanalEntrega.Cancelar.visible(false);
}