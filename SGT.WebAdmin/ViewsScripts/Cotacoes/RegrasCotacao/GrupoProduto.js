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


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasGrupoProduto;
var _grupoProduto;

var GrupoProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumCondicaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumCondicaoAutorizao.E });
    this.GrupoProduto = PropertyEntity({ text: "Grupo Produto:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Grupo Produto", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_grupoProduto, _gridRegrasGrupoProduto, "editarRegraGrupoProdutoClick");
    });

    // Controle de uso
    this.UsarRegraPorGrupoProduto = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por grupo de produto:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorGrupoProduto.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorGrupoProduto(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraGrupoProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraGrupoProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraGrupoProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraGrupoProdutoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorGrupoProduto(usarRegra) {
    _grupoProduto.Visible.visibleFade(usarRegra);
    _grupoProduto.Regras.required(usarRegra);
}

function loadGrupoProduto() {
    _grupoProduto = new GrupoProduto();
    KoBindings(_grupoProduto, "knockoutRegraGrupoProduto");

    //-- Busca
    new BuscarGruposProdutos(_grupoProduto.GrupoProduto);

    //-- Grid Regras
    _gridRegrasGrupoProduto = new GridReordering(_configRegras.infoTable, _grupoProduto.Regras.idGrid, GeraHeadTable("Grupo Produto"));
    _gridRegrasGrupoProduto.CarregarGrid();
    $("#" + _grupoProduto.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_grupoProduto);
    });
}


function editarRegraGrupoProdutoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _grupoProduto.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _grupoProduto.Codigo.val(regra.Codigo);
        _grupoProduto.Ordem.val(regra.Ordem);
        _grupoProduto.Condicao.val(regra.Condicao);
        _grupoProduto.Juncao.val(regra.Juncao);

        _grupoProduto.GrupoProduto.val(regra.Entidade.Descricao);
        _grupoProduto.GrupoProduto.codEntity(regra.Entidade.Codigo);

        _grupoProduto.Adicionar.visible(false);
        _grupoProduto.Atualizar.visible(true);
        _grupoProduto.Excluir.visible(true);
        _grupoProduto.Cancelar.visible(true);
    }
}

function adicionarRegraGrupoProdutoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_grupoProduto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraGrupoProduto();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_grupoProduto);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _grupoProduto.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposGrupoProduto();
}

function atualizarRegraGrupoProdutoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_grupoProduto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraGrupoProduto();

    // Buscar todas regras
    var listaRegras = _grupoProduto.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _grupoProduto.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _grupoProduto.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposGrupoProduto();
}

function excluirRegraGrupoProdutoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_grupoProduto);
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
    _grupoProduto.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposGrupoProduto();
}

function cancelarRegraGrupoProdutoClick(e, sender) {
    LimparCamposGrupoProduto();
}



//*******MÉTODOS*******

function ObjetoRegraGrupoProduto() {
    var codigo = _grupoProduto.Codigo.val();
    var ordem = _grupoProduto.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasGrupoProduto.ObterOrdencao().length + 1,
        Juncao: _grupoProduto.Juncao.val(),
        Condicao: _grupoProduto.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_grupoProduto.GrupoProduto.codEntity()),
            Descricao: _grupoProduto.GrupoProduto.val()
        }
    };

    return regra;
}

function LimparCamposGrupoProduto() {
    _grupoProduto.Codigo.val(_grupoProduto.Codigo.def);
    _grupoProduto.Ordem.val(_grupoProduto.Ordem.def);
    _grupoProduto.Condicao.val(_grupoProduto.Condicao.def);
    _grupoProduto.Juncao.val(_grupoProduto.Juncao.def);

    LimparCampoEntity(_grupoProduto.GrupoProduto);

    _grupoProduto.Adicionar.visible(true);
    _grupoProduto.Atualizar.visible(false);
    _grupoProduto.Excluir.visible(false);
    _grupoProduto.Cancelar.visible(false);
}