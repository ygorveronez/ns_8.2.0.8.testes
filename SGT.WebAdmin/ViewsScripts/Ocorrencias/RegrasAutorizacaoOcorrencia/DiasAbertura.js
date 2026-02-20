/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
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

var _gridRegrasDiasAbertura;
var _diasAbertura;

var DiasAbertura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Condicao.getFieldDescription(), issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Juncao.getFieldDescription(), issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoOcorrencia, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.DiasAbertura = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DiasAbertura.getFieldDescription(), type: types.map, getType: typesKnockout.int, required: ko.observable(true), def: 0, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 5 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DiasAbertura, type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_diasAbertura, _gridRegrasDiasAbertura, "editarRegraDiasAberturaClick", typesKnockout.int);
    });

    // Controle de uso
    this.UsarRegraPorDiasAbertura = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AtivarRegraAutorizacaoPorDiasAbertura.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorDiasAbertura.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorDiasAbertura(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraDiasAberturaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraDiasAberturaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraDiasAberturaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraDiasAberturaClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Cancelar, visible: ko.observable(false) });
}

//*******EVENTOS*******

function UsarRegraPorDiasAbertura(usarRegra) {
    _diasAbertura.Visible.visibleFade(usarRegra);
    _diasAbertura.Regras.required(usarRegra);
}

function loadDiasAbertura() {
    _diasAbertura = new DiasAbertura();
    KoBindings(_diasAbertura, "knockoutRegraDiasAbertura");

    //-- Grid Regras
    _gridRegrasDiasAbertura = new GridReordering(_configRegras.infoTable, _diasAbertura.Regras.idGrid, GeraHeadTable(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DiasAbertura));
    _gridRegrasDiasAbertura.CarregarGrid();
    $("#" + _diasAbertura.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_diasAbertura);
    });
}

function editarRegraDiasAberturaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _diasAbertura.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _diasAbertura.Codigo.val(regra.Codigo);
        _diasAbertura.Ordem.val(regra.Ordem);
        _diasAbertura.Condicao.val(regra.Condicao);
        _diasAbertura.Juncao.val(regra.Juncao);
        _diasAbertura.DiasAbertura.val(regra.Valor);

        _diasAbertura.Adicionar.visible(false);
        _diasAbertura.Atualizar.visible(true);
        _diasAbertura.Excluir.visible(true);
        _diasAbertura.Cancelar.visible(true);
    }
}

function adicionarRegraDiasAberturaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_diasAbertura))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraDiasAbertura();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_diasAbertura);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    listaRegras.push(regra);
    _diasAbertura.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDiasAbertura();
}

function atualizarRegraDiasAberturaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_diasAbertura))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraDiasAbertura();

    // Buscar todas regras
    var listaRegras = _diasAbertura.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraDuplicada, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExisteUmaRegraIdentifca);

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _diasAbertura.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _diasAbertura.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDiasAbertura();
}

function excluirRegraDiasAberturaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_diasAbertura);
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
    _diasAbertura.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposDiasAbertura();
}

function cancelarRegraDiasAberturaClick(e, sender) {
    LimparCamposDiasAbertura();
}

//*******MÉTODOS*******

function ObjetoRegraDiasAbertura() {
    var codigo = _diasAbertura.Codigo.val();
    var ordem = _diasAbertura.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasDiasAbertura.ObterOrdencao().length + 1,
        Juncao: _diasAbertura.Juncao.val(),
        Condicao: _diasAbertura.Condicao.val(),
        Valor: _diasAbertura.DiasAbertura.val()
    };

    return regra;
}

function LimparCamposDiasAbertura() {
    _diasAbertura.Codigo.val(_diasAbertura.Codigo.def);
    _diasAbertura.Ordem.val(_diasAbertura.Ordem.def);
    _diasAbertura.Condicao.val(_diasAbertura.Condicao.def);
    _diasAbertura.Juncao.val(_diasAbertura.Juncao.def);
    _diasAbertura.DiasAbertura.val(_diasAbertura.DiasAbertura.def);

    _diasAbertura.Adicionar.visible(true);
    _diasAbertura.Atualizar.visible(false);
    _diasAbertura.Excluir.visible(false);
    _diasAbertura.Cancelar.visible(false);
}
