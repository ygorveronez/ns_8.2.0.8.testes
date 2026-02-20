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


var _gridRegrasTipoCarga;
var _TipoCarga;

var TipoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Condicao.getFieldDescription(), issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Juncao.getFieldDescription(), issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoOcorrencia, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoCarga.getFieldDescription(), type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoCarga, type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_TipoCarga, _gridRegrasTipoCarga, "editarRegraTipoCargaClick");
    });

    // Controle de uso
    this.UsarRegraPorTipoCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AtivarRegraAutorizacaoPorTipoCarga.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorTipoCarga.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorTipoCarga(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTipoCargaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTipoCargaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTipoCargaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTipoCargaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorTipoCarga(usarRegra) {
    _TipoCarga.Visible.visibleFade(usarRegra);
    _TipoCarga.Regras.required(usarRegra);
}

function loadTipoCarga() {
    _TipoCarga = new TipoCarga();
    KoBindings(_TipoCarga, "knockoutRegraTipoCarga");

    //-- Busca
    new BuscarTiposdeCarga(_TipoCarga.TipoCarga);

    //-- Grid Regras
    _gridRegrasTipoCarga = new GridReordering(_configRegras.infoTable, _TipoCarga.Regras.idGrid, GeraHeadTable(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoCarga));
    _gridRegrasTipoCarga.CarregarGrid();
    $("#" + _TipoCarga.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_TipoCarga);
    });
}

function editarRegraTipoCargaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _TipoCarga.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _TipoCarga.Codigo.val(regra.Codigo);
        _TipoCarga.Ordem.val(regra.Ordem);
        _TipoCarga.Condicao.val(regra.Condicao);
        _TipoCarga.Juncao.val(regra.Juncao);

        _TipoCarga.TipoCarga.val(regra.Entidade.Descricao);
        _TipoCarga.TipoCarga.codEntity(regra.Entidade.Codigo);

        _TipoCarga.Adicionar.visible(false);
        _TipoCarga.Atualizar.visible(true);
        _TipoCarga.Excluir.visible(true);
        _TipoCarga.Cancelar.visible(true);
    }
}

function adicionarRegraTipoCargaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_TipoCarga))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraTipoCarga();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_TipoCarga);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    listaRegras.push(regra);
    _TipoCarga.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoCarga();
}

function atualizarRegraTipoCargaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_TipoCarga))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraTipoCarga();

    // Buscar todas regras
    var listaRegras = _TipoCarga.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _TipoCarga.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _TipoCarga.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoCarga();
}

function excluirRegraTipoCargaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_TipoCarga);
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
    _TipoCarga.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposTipoCarga();
}

function cancelarRegraTipoCargaClick(e, sender) {
    LimparCamposTipoCarga();
}



//*******MÉTODOS*******

function ObjetoRegraTipoCarga() {
    var codigo = _TipoCarga.Codigo.val();
    var ordem = _TipoCarga.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTipoCarga.ObterOrdencao().length + 1,
        Juncao: _TipoCarga.Juncao.val(),
        Condicao: _TipoCarga.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_TipoCarga.TipoCarga.codEntity()),
            Descricao: _TipoCarga.TipoCarga.val()
        }
    };

    return regra;
}

function LimparCamposTipoCarga() {
    _TipoCarga.Codigo.val(_TipoCarga.Codigo.def);
    _TipoCarga.Ordem.val(_TipoCarga.Ordem.def);
    _TipoCarga.Condicao.val(_TipoCarga.Condicao.def);
    _TipoCarga.Juncao.val(_TipoCarga.Juncao.def);

    LimparCampoEntity(_TipoCarga.TipoCarga);

    _TipoCarga.Adicionar.visible(true);
    _TipoCarga.Atualizar.visible(false);
    _TipoCarga.Excluir.visible(false);
    _TipoCarga.Cancelar.visible(false);
}