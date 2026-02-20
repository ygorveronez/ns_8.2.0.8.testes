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


var _gridRegrasOrigem;
var _origem;

var Origem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoSimulacao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.Origem = PropertyEntity({ text: "Origem", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Origem", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_origem, _gridRegrasOrigem, "editarRegraOrigemClick");
    });

    // Controle de uso
    this.UsarRegraPorOrigem = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar regras autorização por origem", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorOrigem.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorOrigem(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraOrigemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraOrigemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraOrigemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraOrigemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorOrigem(usarRegra) {
    _origem.Visible.visibleFade(usarRegra);
    _origem.Regras.required(usarRegra);
}

function loadOrigem() {
    _origem = new Origem();
    KoBindings(_origem, "knockoutRegraOrigem");

    //-- Busca
    new BuscarLocalidades(_origem.Origem);

    //-- Grid Regras
    _gridRegrasOrigem = new GridReordering(_configRegras.infoTable, _origem.Regras.idGrid, GeraHeadTable("Origem"));
    _gridRegrasOrigem.CarregarGrid();
    $("#" + _origem.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_origem);
    });
}

function editarRegraOrigemClick(codigo) {
    // Buscar todas regras
    var listaRegras = _origem.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _origem.Codigo.val(regra.Codigo);
        _origem.Ordem.val(regra.Ordem);
        _origem.Condicao.val(regra.Condicao);
        _origem.Juncao.val(regra.Juncao);

        _origem.Origem.val(regra.Entidade.Descricao);
        _origem.Origem.codEntity(regra.Entidade.Codigo);

        _origem.Adicionar.visible(false);
        _origem.Atualizar.visible(true);
        _origem.Excluir.visible(true);
        _origem.Cancelar.visible(true);
    }
}

function adicionarRegraOrigemClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_origem))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraOrigem();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_origem);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _origem.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposOrigem();
}

function atualizarRegraOrigemClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_origem))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraOrigem();

    // Buscar todas regras
    var listaRegras = _origem.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _origem.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _origem.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposOrigem();
}

function excluirRegraOrigemClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_origem);
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
    _origem.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposOrigem();
}

function cancelarRegraOrigemClick(e, sender) {
    LimparCamposOrigem();
}



//*******MÉTODOS*******

function ObjetoRegraOrigem() {
    var codigo = _origem.Codigo.val();
    var ordem = _origem.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasOrigem.ObterOrdencao().length + 1,
        Juncao: _origem.Juncao.val(),
        Condicao: _origem.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_origem.Origem.codEntity()),
            Descricao: _origem.Origem.val()
        }
    };

    return regra;
}

function LimparCamposOrigem() {
    _origem.Codigo.val(_origem.Codigo.def);
    _origem.Ordem.val(_origem.Ordem.def);
    _origem.Condicao.val(_origem.Condicao.def);
    _origem.Juncao.val(_origem.Juncao.def);

    LimparCampoEntity(_origem.Origem);

    _origem.Adicionar.visible(true);
    _origem.Atualizar.visible(false);
    _origem.Excluir.visible(false);
    _origem.Cancelar.visible(false);
}