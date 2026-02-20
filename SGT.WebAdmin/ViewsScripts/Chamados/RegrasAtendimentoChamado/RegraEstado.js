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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoAvaria.js" />
/// <reference path="RegrasAtendimentoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasEstado;
var _estado;

var Estado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoAvaria.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizaoAvaria.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoAvaria.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoAvaria.E });
    this.Estado = PropertyEntity({ text: "Estado: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Estado", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_estado, _gridRegrasEstado, "editarRegraEstadoClick");
    });

    // Controle de uso
    this.RegraPorEstado = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por Estado:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorEstado.val.subscribe(function (novoValor) {
        SincronzarRegras();
        RegraPorEstado(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraEstadoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraEstadoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraEstadoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraEstadoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorEstado(usarRegra) {
    _estado.Visible.visibleFade(usarRegra);
    _estado.Regras.required(usarRegra);
}

function loadRegraEstado() {
    _estado = new Estado();
    KoBindings(_estado, "knockoutRegraEstado");

    //-- Busca
    new BuscarEstados(_estado.Estado);

    //-- Grid Regras
    _gridRegrasEstado = new GridReordering(_configRegras.infoTable, _estado.Regras.idGrid, GeraHeadTable("Estado"));
    _gridRegrasEstado.CarregarGrid();
    $("#" + _estado.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadas(_estado);
    });
}

function editarRegraEstadoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _estado.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _estado.Codigo.val(regra.Codigo);
        _estado.Ordem.val(regra.Ordem);
        _estado.Condicao.val(regra.Condicao);
        _estado.Juncao.val(regra.Juncao);

        _estado.Estado.val(regra.Entidade.Descricao);
        _estado.Estado.codEntity(regra.Entidade.Codigo);

        _estado.Adicionar.visible(false);
        _estado.Atualizar.visible(true);
        _estado.Excluir.visible(true);
        _estado.Cancelar.visible(true);
    }
}

function adicionarRegraEstadoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_estado))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraEstado();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_estado);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _estado.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposEstado();
}

function atualizarRegraEstadoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_estado))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraEstado();

    // Buscar todas regras
    var listaRegras = _estado.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _estado.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _estado.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposEstado();
}

function excluirRegraEstadoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_estado);
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
    _estado.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposEstado();
}

function cancelarRegraEstadoClick(e, sender) {
    LimparCamposEstado();
}

//*******MÉTODOS*******

function ObjetoRegraEstado() {
    var codigo = _estado.Codigo.val();
    var ordem = _estado.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasEstado.ObterOrdencao().length + 1,
        Juncao: _estado.Juncao.val(),
        Condicao: _estado.Condicao.val(),
        Entidade: {
            Codigo: _estado.Estado.codEntity(),
            Descricao: _estado.Estado.val()
        }
    };

    return regra;
}

function LimparCamposEstado() {
    _estado.Codigo.val(_estado.Codigo.def);
    _estado.Ordem.val(_estado.Ordem.def);
    _estado.Condicao.val(_estado.Condicao.def);
    _estado.Juncao.val(_estado.Juncao.def);

    LimparCampoEntity(_estado.Estado);

    _estado.Adicionar.visible(true);
    _estado.Atualizar.visible(false);
    _estado.Excluir.visible(false);
    _estado.Cancelar.visible(false);
}