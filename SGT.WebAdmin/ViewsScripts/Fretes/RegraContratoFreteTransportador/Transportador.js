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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizao.js" />
/// <reference path="RegraContratoFreteTransportador.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasTransportador;
var _transportador;

var Transportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Transportador = PropertyEntity({ text: ko.observable("Transportador:"), type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Transportador", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_transportador, _gridRegrasTransportador, "editarRegraTransportadorClick");
    });

    // Controle de uso
    this.UsarRegraPorTransportador = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable("Ativar Regra de autorização por Transportador:"), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorTransportador.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorTransportador(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTransportadorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTransportadorClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTransportadorClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTransportadorClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};


//*******EVENTOS*******
function UsarRegraPorTransportador(usarRegra) {
    _transportador.Visible.visibleFade(usarRegra);
    _transportador.Alcadas.required(usarRegra);
}

function loadTransportador() {
    _transportador = new Transportador();
    KoBindings(_transportador, "knockoutAlcadaTransportador");

    //-- Busca
    new BuscarTransportadores(_transportador.Transportador);

    var descricaoHeadTable = "Transportador";

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        descricaoHeadTable = "Empresa/Filial";

    //-- Grid Regras
    _gridRegrasTransportador = new GridReordering(_configRegras.infoTable, _transportador.Alcadas.idGrid, GeraHeadTable(descricaoHeadTable));
    _gridRegrasTransportador.CarregarGrid();
    $("#" + _transportador.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasRegraContratoFreteTransportador(_transportador);
    });

    AjustarLayoutRegraTransportadorPorTipoServico();
}

function AjustarLayoutRegraTransportadorPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _transportador.UsarRegraPorTransportador.text("Ativar Regra de autorização por Empresa/Filial:");
        _transportador.Transportador.text("Empresa/Filial:");
        $("#liTabAlcadaFilial").addClass("hidden");
        $("#spnDescricaoTabTransportador").text("Empresa/Filial");
    }
}

function editarRegraTransportadorClick(codigo) {
    // Buscar todas regras
    var listaRegras = _transportador.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _transportador.Codigo.val(regra.Codigo);
        _transportador.Ordem.val(regra.Ordem);
        _transportador.Condicao.val(regra.Condicao);
        _transportador.Juncao.val(regra.Juncao);

        _transportador.Transportador.val(regra.Entidade.Descricao);
        _transportador.Transportador.codEntity(regra.Entidade.Codigo);

        _transportador.Adicionar.visible(false);
        _transportador.Atualizar.visible(true);
        _transportador.Excluir.visible(true);
        _transportador.Cancelar.visible(true);
    }
}

function adicionarRegraTransportadorClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_transportador))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTransportador();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_transportador);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _transportador.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposTransportador();
}

function atualizarRegraTransportadorClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_transportador))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTransportador();

    // Buscar todas regras
    var listaRegras = _transportador.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _transportador.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _transportador.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposTransportador();
}

function excluirRegraTransportadorClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_transportador);
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
    _transportador.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposTransportador();
}

function cancelarRegraTransportadorClick(e, sender) {
    LimparCamposTransportador();
}



//*******MÉTODOS*******

function ObjetoRegraTransportador() {
    var codigo = _transportador.Codigo.val();
    var ordem = _transportador.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTransportador.ObterOrdencao().length + 1,
        Juncao: _transportador.Juncao.val(),
        Condicao: _transportador.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_transportador.Transportador.codEntity()),
            Descricao: _transportador.Transportador.val()
        }
    };

    return regra;
}

function LimparCamposTransportador() {
    _transportador.Codigo.val(_transportador.Codigo.def);
    _transportador.Ordem.val(_transportador.Ordem.def);
    _transportador.Condicao.val(_transportador.Condicao.def);
    _transportador.Juncao.val(_transportador.Juncao.def);

    LimparCampoEntity(_transportador.Transportador);

    _transportador.Adicionar.visible(true);
    _transportador.Atualizar.visible(false);
    _transportador.Excluir.visible(false);
    _transportador.Cancelar.visible(false);
}