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


var _gridRegrasDestino;
var _destino;

var Destino = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoOcorrenciaEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizaoSimulacao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.Destino = PropertyEntity({ text: "Destino", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Destino", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_destino, _gridRegrasDestino, "editarRegraDestinoClick");
    });

    // Controle de uso
    this.UsarRegraPorDestino = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por tipo de operação", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorDestino.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorDestino(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraDestinoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraDestinoClick, type: types.event, text:Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraDestinoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({eventClick: cancelarRegraDestinoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false)
    });
}


//*******EVENTOS*******
function UsarRegraPorDestino(usarRegra) {
    _destino.Visible.visibleFade(usarRegra);
    _destino.Regras.required(usarRegra);
}

function loadDestino() {
    _destino = new Destino();
    KoBindings(_destino, "knockoutRegraDestino");

    //-- Busca
    new BuscarLocalidades(_destino.Destino);

    //-- Grid Regras
    _gridRegrasDestino = new GridReordering(_configRegras.infoTable, _destino.Regras.idGrid, GeraHeadTable("Destino"));
    _gridRegrasDestino.CarregarGrid();
    $("#" + _destino.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasOcorrencia(_destino);
    });
}

function editarRegraDestinoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _destino.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _destino.Codigo.val(regra.Codigo);
        _destino.Ordem.val(regra.Ordem);
        _destino.Condicao.val(regra.Condicao);
        _destino.Juncao.val(regra.Juncao);

        _destino.Destino.val(regra.Entidade.Descricao);
        _destino.Destino.codEntity(regra.Entidade.Codigo);

        _destino.Adicionar.visible(false);
        _destino.Atualizar.visible(true);
        _destino.Excluir.visible(true);
        _destino.Cancelar.visible(true);
    }
}

function adicionarRegraDestinoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_destino))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraDestino();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_destino);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _destino.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDestino();
}

function atualizarRegraDestinoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_destino))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    // Codigo da regra
    var regra = ObjetoRegraDestino();

    // Buscar todas regras
    var listaRegras = _destino.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _destino.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _destino.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDestino();
}

function excluirRegraDestinoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_destino);
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
    _destino.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposDestino();
}

function cancelarRegraDestinoClick(e, sender) {
    LimparCamposDestino();
}



//*******MÉTODOS*******

function ObjetoRegraDestino() {
    var codigo = _destino.Codigo.val();
    var ordem = _destino.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasDestino.ObterOrdencao().length + 1,
        Juncao: _destino.Juncao.val(),
        Condicao: _destino.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_destino.Destino.codEntity()),
            Descricao: _destino.Destino.val()
        }
    };

    return regra;
}

function LimparCamposDestino() {
    _destino.Codigo.val(_destino.Codigo.def);
    _destino.Ordem.val(_destino.Ordem.def);
    _destino.Condicao.val(_destino.Condicao.def);
    _destino.Juncao.val(_destino.Juncao.def);

    LimparCampoEntity(_destino.Destino);

    _destino.Adicionar.visible(true);
    _destino.Atualizar.visible(false);
    _destino.Excluir.visible(false);
    _destino.Cancelar.visible(false);
}