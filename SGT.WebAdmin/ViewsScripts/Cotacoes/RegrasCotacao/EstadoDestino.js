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
/// <reference path="../../enumeradores/enumcondicaoautorizaocotacao.js" />
/// <reference path="../../enumeradores/enumjuncaoautorizao.js" />
/// <reference path="RegrasCotacao.js.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasEstadoDestino;
var _estadoDestino;

var EstadoDestino = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumJuncaoAutorizao.E });
    this.EstadoDestino = PropertyEntity({ text: "Estado Destino:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Estado De Destino", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_estadoDestino, _gridRegrasEstadoDestino, "editarRegraEstadoDestinoClick");
    });

    // Controle de uso
    this.UsarRegraPorEstadoDestino = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por estado de destino:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorEstadoDestino.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorEstadoDestino(novoValor);
    });


    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraEstadoDestinoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraEstadoDestinoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraEstadoDestinoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraEstadoDestinoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorEstadoDestino(usarRegra) {
    _estadoDestino.Visible.visibleFade(usarRegra);
    _estadoDestino.Regras.required(usarRegra);
}

function loadEstadoDestino() {
    _estadoDestino = new EstadoDestino();
    KoBindings(_estadoDestino, "knockoutRegraEstadoDestino");

    new BuscarEstados(_estadoDestino.EstadoDestino);

    //-- Grid Regras
    _gridRegrasEstadoDestino = new GridReordering(_configRegras.infoTable, _estadoDestino.Regras.idGrid, GeraHeadTable("Estado de Destino"));
    _gridRegrasEstadoDestino.CarregarGrid();
    $("#" + _estadoDestino.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_estadoDestino);
    });
}


function editarRegraEstadoDestinoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _estadoDestino.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _estadoDestino.Codigo.val(regra.Codigo);
        _estadoDestino.Ordem.val(regra.Ordem);
        _estadoDestino.Condicao.val(regra.Condicao);
        _estadoDestino.Juncao.val(regra.Juncao);

        _estadoDestino.EstadoDestino.val(regra.Entidade.Descricao);
        _estadoDestino.EstadoDestino.codEntity(regra.Entidade.Codigo);

        _estadoDestino.Adicionar.visible(false);
        _estadoDestino.Atualizar.visible(true);
        _estadoDestino.Excluir.visible(true);
        _estadoDestino.Cancelar.visible(true);
    }
}

function adicionarRegraEstadoDestinoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_estadoDestino))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraEstadoDestino();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_estadoDestino);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _estadoDestino.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposEstadoDestino();
}

function atualizarRegraEstadoDestinoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_estadoDestino))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraEstadoDestino();

    // Buscar todas regras
    var listaRegras = _estadoDestino.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _estadoDestino.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _estadoDestino.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposEstadoDestino();
}

function excluirRegraEstadoDestinoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_estadoDestino);
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
    _estadoDestino.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposEstadoDestino();
}

function cancelarRegraEstadoDestinoClick(e, sender) {
    LimparCamposEstadoDestino();
}



//*******MÉTODOS*******

function ObjetoRegraEstadoDestino() {
    var codigo = _estadoDestino.Codigo.val();
    var ordem = _estadoDestino.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasEstadoDestino.ObterOrdencao().length + 1,
        Juncao: _estadoDestino.Juncao.val(),
        Condicao: _estadoDestino.Condicao.val(),
        Entidade: {
            Codigo: _estadoDestino.EstadoDestino.codEntity(),
            Descricao: _estadoDestino.EstadoDestino.val()
        }
    };

    console.log(regra);

    return regra;
}

function LimparCamposEstadoDestino() {
    _estadoDestino.Codigo.val(_estadoDestino.Codigo.def);
    _estadoDestino.Ordem.val(_estadoDestino.Ordem.def);
    _estadoDestino.Condicao.val(_estadoDestino.Condicao.def);
    _estadoDestino.Juncao.val(_estadoDestino.Juncao.def);

    LimparCampoEntity(_estadoDestino.EstadoDestino);

    _estadoDestino.Adicionar.visible(true);
    _estadoDestino.Atualizar.visible(false);
    _estadoDestino.Excluir.visible(false);
    _estadoDestino.Cancelar.visible(false);
}