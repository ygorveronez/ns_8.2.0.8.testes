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
/// <reference path="RegrasAutorizacaoSimulacao.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasTipoSimulacao;
var _tipoSimulacao;

var TipoSimulacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoSimulacao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.TipoSimulacao = PropertyEntity({ text: "Tipo Simulação", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Tipo Simulaçao", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_tipoSimulacao, _gridRegrasTipoSimulacao, "editarRegraTipoSimulacaoClick");
    });

    // Controle de uso
    this.UsarRegraPorTipoSimulacao = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por tipo de simulação", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorTipoSimulacao.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorTipoSimulacao(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTipoSimulacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTipoSimulacaoClick, type: types.event, text:Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTipoSimulacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTipoSimulacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorTipoSimulacao(usarRegra) {
    _tipoSimulacao.Visible.visibleFade(usarRegra);
    _tipoSimulacao.Regras.required(usarRegra);
}

function loadTipoSimulacao() {
    console.log("Teste TipoSimulacao");
    _tipoSimulacao = new TipoSimulacao();
    KoBindings(_tipoSimulacao, "knockoutRegraTipoSimulacao");

    //-- Busca
    new BuscarTipoOcorrencia(_tipoSimulacao.TipoSimulacao, null, null, null, null, null, null, null, null, null, null, null, true);
    
    //-- Grid Regras
    _gridRegrasTipoSimulacao = new GridReordering(_configRegras.infoTable, _tipoSimulacao.Regras.idGrid, GeraHeadTable("Tipo Simulação"));
    _gridRegrasTipoSimulacao.CarregarGrid();
    $("#" + _tipoSimulacao.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_tipoSimulacao);
    });
}

function editarRegraTipoSimulacaoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _tipoSimulacao.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _tipoSimulacao.Codigo.val(regra.Codigo);
        _tipoSimulacao.Ordem.val(regra.Ordem);
        _tipoSimulacao.Condicao.val(regra.Condicao);
        _tipoSimulacao.Juncao.val(regra.Juncao);

        _tipoSimulacao.TipoSimulacao.val(regra.Entidade.Descricao);
        _tipoSimulacao.TipoSimulacao.codEntity(regra.Entidade.Codigo);

        _tipoSimulacao.Adicionar.visible(false);
        _tipoSimulacao.Atualizar.visible(true);
        _tipoSimulacao.Excluir.visible(true);
        _tipoSimulacao.Cancelar.visible(true);
    }
}

function adicionarRegraTipoSimulacaoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoSimulacao))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraTipoSimulacao();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_tipoSimulacao);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _tipoSimulacao.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoSimulacao();
}

function atualizarRegraTipoSimulacaoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoSimulacao))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraTipoSimulacao();

    // Buscar todas regras
    var listaRegras = _tipoSimulacao.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _tipoSimulacao.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _tipoSimulacao.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoSimulacao();
}

function excluirRegraTipoSimulacaoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_tipoSimulacao);
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
    _tipoSimulacao.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposTipoSimulacao();
}

function cancelarRegraTipoSimulacaoClick(e, sender) {
    LimparCamposTipoSimulacao();
}



//*******MÉTODOS*******

function ObjetoRegraTipoSimulacao() {
    var codigo = _tipoSimulacao.Codigo.val();
    var ordem = _tipoSimulacao.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTipoSimulacao.ObterOrdencao().length + 1,
        Juncao: _tipoSimulacao.Juncao.val(),
        Condicao: _tipoSimulacao.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_tipoSimulacao.TipoSimulacao.codEntity()),
            Descricao: _tipoSimulacao.TipoSimulacao.val()
        }
    };

    return regra;
}

function LimparCamposTipoSimulacao() {
    _tipoSimulacao.Codigo.val(_tipoSimulacao.Codigo.def);
    _tipoSimulacao.Ordem.val(_tipoSimulacao.Ordem.def);
    _tipoSimulacao.Condicao.val(_tipoSimulacao.Condicao.def);
    _tipoSimulacao.Juncao.val(_tipoSimulacao.Juncao.def);

    LimparCampoEntity(_tipoSimulacao.TipoSimulacao);

    _tipoSimulacao.Adicionar.visible(true);
    _tipoSimulacao.Atualizar.visible(false);
    _tipoSimulacao.Excluir.visible(false);
    _tipoSimulacao.Cancelar.visible(false);
}