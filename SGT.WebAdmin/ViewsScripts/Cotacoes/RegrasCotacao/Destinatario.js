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


var _gridRegrasDestinatario;
var _destinatario;

var Destinatario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumCondicaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumCondicaoAutorizao.E });
    this.Destinatario = PropertyEntity({ text: "Destinatário:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    this.OpcaoAplicacao = PropertyEntity({ val: ko.observable(EnumAplicacaoRegraCotacao.ExcluirTransportador), options: EnumAplicacaoRegraCotacao.obterOpcoes(), def: EnumAplicacaoRegraCotacao.ExcluirTransportador, text: "Informe a ação da regra: ", visible: ko.observable(true) });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Destinatário", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_destinatario, _gridRegrasDestinatario, "editarRegraDestinatarioClick");
    });

    // Controle de uso
    this.UsarRegraPorDestinatario = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por destinatário:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorDestinatario.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorDestinatario(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraDestinatarioClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraDestinatarioClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraDestinatarioClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraDestinatarioClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorDestinatario(usarRegra) {
    _destinatario.Visible.visibleFade(usarRegra);
    _destinatario.Regras.required(usarRegra);
}

function loadDestinatario() {
    _destinatario = new Destinatario();
    KoBindings(_destinatario, "knockoutRegraDestinatario");

    new BuscarClientes(_destinatario.Destinatario);

    //-- Grid Regras
    _gridRegrasDestinatario = new GridReordering(_configRegras.infoTable, _destinatario.Regras.idGrid, GeraHeadTable("Destinatario"));
    _gridRegrasDestinatario.CarregarGrid();
    $("#" + _destinatario.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_destinatario);
    });
}

function editarRegraDestinatarioClick(codigo) {
    // Buscar todas regras
    var listaRegras = _destinatario.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _destinatario.Codigo.val(regra.Codigo);
        _destinatario.Ordem.val(regra.Ordem);
        _destinatario.Condicao.val(regra.Condicao);
        _destinatario.Juncao.val(regra.Juncao);

        _destinatario.Destinatario.val(regra.Entidade.Descricao);
        _destinatario.Destinatario.codEntity(regra.Entidade.Codigo);

        _destinatario.Adicionar.visible(false);
        _destinatario.Atualizar.visible(true);
        _destinatario.Excluir.visible(true);
        _destinatario.Cancelar.visible(true);
    }
}

function adicionarRegraDestinatarioClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_destinatario))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDestinatario();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_destinatario);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _destinatario.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDestinatario();
}

function atualizarRegraDestinatarioClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_destinatario))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDestinatario();

    // Buscar todas regras
    var listaRegras = _destinatario.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _destinatario.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _destinatario.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDestinatario();
}

function excluirRegraDestinatarioClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_destinatario);
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
    _destinatario.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposDestinatario();
}

function cancelarRegraDestinatarioClick(e, sender) {
    LimparCamposDestinatario();
}

//*******MÉTODOS*******

function ObjetoRegraDestinatario() {
    var codigo = _destinatario.Codigo.val();
    var ordem = _destinatario.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasDestinatario.ObterOrdencao().length + 1,
        Juncao: _destinatario.Juncao.val(),
        Condicao: _destinatario.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_destinatario.Destinatario.codEntity()),
            Descricao: _destinatario.Destinatario.val()
        }
    };

    return regra;
}

function LimparCamposDestinatario() {
    _destinatario.Codigo.val(_destinatario.Codigo.def);
    _destinatario.Ordem.val(_destinatario.Ordem.def);
    _destinatario.Condicao.val(_destinatario.Condicao.def);
    _destinatario.Juncao.val(_destinatario.Juncao.def);

    LimparCampoEntity(_destinatario.Destinatario);

    _destinatario.Adicionar.visible(true);
    _destinatario.Atualizar.visible(false);
    _destinatario.Excluir.visible(false);
    _destinatario.Cancelar.visible(false);
}