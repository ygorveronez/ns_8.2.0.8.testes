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
/// <reference path="../../Enumeradores/EnumAplicacaoRegraCotacao.js" />
/// <reference path="../../enumeradores/enumcondicaoautorizaocotacao.js" />
/// <reference path="../../enumeradores/enumjuncaoautorizao.js" />
/// <reference path="RegrasCotacao.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasExpedidor;
var _expedidor;

var Expedidor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumCondicaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumCondicaoAutorizao.E });
    this.Expedidor = PropertyEntity({ text: "Expedidor:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    this.OpcaoAplicacao = PropertyEntity({ val: ko.observable(EnumAplicacaoRegraCotacao.ExcluirTransportador), options: EnumAplicacaoRegraCotacao.obterOpcoes(), def: EnumAplicacaoRegraCotacao.ExcluirTransportador, text: "Informe a ação da regra: ", visible: ko.observable(true) });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Expedidor", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_expedidor, _gridRegrasExpedidor, "editarRegraExpedidorClick");
    });

    // Controle de uso
    this.UsarRegraPorExpedidor = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por expedidor:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorExpedidor.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorExpedidor(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraExpedidorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraExpedidorClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraExpedidorClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraExpedidorClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorExpedidor(usarRegra) {
    _expedidor.Visible.visibleFade(usarRegra);
    _expedidor.Regras.required(usarRegra);
}

function loadExpedidor() {
    _expedidor = new Expedidor();
    KoBindings(_expedidor, "knockoutRegraExpedidor");

    //-- Busca
    new BuscarClientes(_expedidor.Expedidor);

    //-- Grid Regras
    _gridRegrasExpedidor = new GridReordering(_configRegras.infoTable, _expedidor.Regras.idGrid, GeraHeadTable("Expedidor"));
    _gridRegrasExpedidor.CarregarGrid();
    $("#" + _expedidor.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_expedidor);
    });
}

function editarRegraExpedidorClick(codigo) {
    // Buscar todas regras
    var listaRegras = _expedidor.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _expedidor.Codigo.val(regra.Codigo);
        _expedidor.Ordem.val(regra.Ordem);
        _expedidor.Condicao.val(regra.Condicao);
        _expedidor.Juncao.val(regra.Juncao);

        _expedidor.Expedidor.val(regra.Entidade.Descricao);
        _expedidor.Expedidor.codEntity(regra.Entidade.Codigo);

        _expedidor.Adicionar.visible(false);
        _expedidor.Atualizar.visible(true);
        _expedidor.Excluir.visible(true);
        _expedidor.Cancelar.visible(true);
    }
}

function adicionarRegraExpedidorClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_expedidor))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraExpedidor();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_expedidor);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _expedidor.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposExpedidor();
}

function atualizarRegraExpedidorClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_expedidor))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraExpedidor();

    // Buscar todas regras
    var listaRegras = _expedidor.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _expedidor.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _expedidor.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposExpedidor();
}

function excluirRegraExpedidorClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_expedidor);
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
    _expedidor.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposExpedidor();
}

function cancelarRegraExpedidorClick(e, sender) {
    LimparCamposExpedidor();
}



//*******MÉTODOS*******

function ObjetoRegraExpedidor() {
    var codigo = _expedidor.Codigo.val();
    var ordem = _expedidor.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasExpedidor.ObterOrdencao().length + 1,
        Juncao: _expedidor.Juncao.val(),
        Condicao: _expedidor.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_expedidor.Expedidor.codEntity()),
            Descricao: _expedidor.Expedidor.val()
        }
    };

    return regra;
}

function LimparCamposExpedidor() {
    _expedidor.Codigo.val(_expedidor.Codigo.def);
    _expedidor.Ordem.val(_expedidor.Ordem.def);
    _expedidor.Condicao.val(_expedidor.Condicao.def);
    _expedidor.Juncao.val(_expedidor.Juncao.def);

    LimparCampoEntity(_expedidor.Expedidor);

    _expedidor.Adicionar.visible(true);
    _expedidor.Atualizar.visible(false);
    _expedidor.Excluir.visible(false);
    _expedidor.Cancelar.visible(false);
}