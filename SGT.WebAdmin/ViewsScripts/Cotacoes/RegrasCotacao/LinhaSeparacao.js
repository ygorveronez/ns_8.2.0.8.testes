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
/// <reference path="RegrasCotacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasLinhaSeparacao;
var _linhaSeparacao;

var LinhaSeparacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumJuncaoAutorizao.E });
    this.LinhaSeparacao = PropertyEntity({ text: "Linha Separação:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Linha Separação", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_linhaSeparacao, _gridRegrasLinhaSeparacao, "editarRegraLinhaSeparacaoClick");
    });

    // Controle de uso
    this.UsarRegraPorLinhaSeparacao = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por Linha Separação:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorLinhaSeparacao.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorLinhaSeparacao(novoValor);
    });


    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraLinhaSeparacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraLinhaSeparacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraLinhaSeparacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraLinhaSeparacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorLinhaSeparacao(usarRegra) {
    _linhaSeparacao.Visible.visibleFade(usarRegra);
    _linhaSeparacao.Regras.required(usarRegra);
}

function loadLinhaSeparacao() {
    _linhaSeparacao = new LinhaSeparacao();
    KoBindings(_linhaSeparacao, "knockoutRegraLinhaSeparacao");

    //-- Busca
    new BuscarLinhasSeparacao(_linhaSeparacao.LinhaSeparacao);

    //-- Grid Regras
    _gridRegrasLinhaSeparacao = new GridReordering(_configRegras.infoTable, _linhaSeparacao.Regras.idGrid, GeraHeadTable("Linhas Separação"));
    _gridRegrasLinhaSeparacao.CarregarGrid();
    $("#" + _linhaSeparacao.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_linhaSeparacao);
    });
}


function editarRegraLinhaSeparacaoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _linhaSeparacao.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _linhaSeparacao.Codigo.val(regra.Codigo);
        _linhaSeparacao.Ordem.val(regra.Ordem);
        _linhaSeparacao.Condicao.val(regra.Condicao);
        _linhaSeparacao.Juncao.val(regra.Juncao);

        _linhaSeparacao.LinhaSeparacao.val(regra.Entidade.Descricao);
        _linhaSeparacao.LinhaSeparacao.codEntity(regra.Entidade.Codigo);

        _linhaSeparacao.Adicionar.visible(false);
        _linhaSeparacao.Atualizar.visible(true);
        _linhaSeparacao.Excluir.visible(true);
        _linhaSeparacao.Cancelar.visible(true);
    }
}

function adicionarRegraLinhaSeparacaoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_linhaSeparacao))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraLinhaSeparacao();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_linhaSeparacao);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _linhaSeparacao.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposLinhaSeparacao();
}

function atualizarRegraLinhaSeparacaoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_linhaSeparacao))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraLinhaSeparacao();

    // Buscar todas regras
    var listaRegras = _linhaSeparacao.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _linhaSeparacao.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _linhaSeparacao.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposLinhaSeparacao();
}

function excluirRegraLinhaSeparacaoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_linhaSeparacao);
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
    _linhaSeparacao.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposLinhaSeparacao();
}

function cancelarRegraLinhaSeparacaoClick(e, sender) {
    LimparCamposLinhaSeparacao();
}



//*******MÉTODOS*******

function ObjetoRegraLinhaSeparacao() {
    var codigo = _linhaSeparacao.Codigo.val();
    var ordem = _linhaSeparacao.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasLinhaSeparacao.ObterOrdencao().length + 1,
        Juncao: _linhaSeparacao.Juncao.val(),
        Condicao: _linhaSeparacao.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_linhaSeparacao.LinhaSeparacao.codEntity()),
            Descricao: _linhaSeparacao.LinhaSeparacao.val()
        }
    };

    return regra;
}

function LimparCamposLinhaSeparacao() {
    _linhaSeparacao.Codigo.val(_linhaSeparacao.Codigo.def);
    _linhaSeparacao.Ordem.val(_linhaSeparacao.Ordem.def);
    _linhaSeparacao.Condicao.val(_linhaSeparacao.Condicao.def);
    _linhaSeparacao.Juncao.val(_linhaSeparacao.Juncao.def);

    LimparCampoEntity(_linhaSeparacao.LinhaSeparacao);

    _linhaSeparacao.Adicionar.visible(true);
    _linhaSeparacao.Atualizar.visible(false);
    _linhaSeparacao.Excluir.visible(false);
    _linhaSeparacao.Cancelar.visible(false);
}