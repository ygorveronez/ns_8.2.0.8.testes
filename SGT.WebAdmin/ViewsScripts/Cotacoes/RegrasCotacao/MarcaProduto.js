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


var _gridRegrasMarcaProduto;
var _marcaProduto;

var MarcaProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumCondicaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumCondicaoAutorizao.E });
    this.MarcaProduto = PropertyEntity({ text: "Marca de Produto:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Marca Produto", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_marcaProduto, _gridRegrasMarcaProduto, "editarRegraMarcaProdutoClick");
    });

    // Controle de uso
    this.UsarRegraPorMarcaProduto = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por marca do produto:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorMarcaProduto.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorMarcaProduto(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraMarcaProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraMarcaProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraMarcaProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraMarcaProdutoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorMarcaProduto(usarRegra) {
    _marcaProduto.Visible.visibleFade(usarRegra);
    _marcaProduto.Regras.required(usarRegra);
}

function loadMarcaProduto() {
    _marcaProduto = new MarcaProduto();
    KoBindings(_marcaProduto, "knockoutRegraMarcaProduto");

    //-- Busca
    new BuscarMarcaProduto(_marcaProduto.MarcaProduto);

    //-- Grid Regras
    _gridRegrasMarcaProduto = new GridReordering(_configRegras.infoTable, _marcaProduto.Regras.idGrid, GeraHeadTable("Marca Produto"));
    _gridRegrasMarcaProduto.CarregarGrid();
    $("#" + _marcaProduto.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_marcaProduto);
    });
}

function editarRegraMarcaProdutoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _marcaProduto.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _marcaProduto.Codigo.val(regra.Codigo);
        _marcaProduto.Ordem.val(regra.Ordem);
        _marcaProduto.Condicao.val(regra.Condicao);
        _marcaProduto.Juncao.val(regra.Juncao);

        _marcaProduto.MarcaProduto.val(regra.Entidade.Descricao);
        _marcaProduto.MarcaProduto.codEntity(regra.Entidade.Codigo);

        _marcaProduto.Adicionar.visible(false);
        _marcaProduto.Atualizar.visible(true);
        _marcaProduto.Excluir.visible(true);
        _marcaProduto.Cancelar.visible(true);
    }
}

function adicionarRegraMarcaProdutoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_marcaProduto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMarcaProduto();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_marcaProduto);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _marcaProduto.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposMarcaProduto();
}

function atualizarRegraMarcaProdutoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_marcaProduto))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMarcaProduto();

    // Buscar todas regras
    var listaRegras = _marcaProduto.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _marcaProduto.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _marcaProduto.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposMarcaProduto();
}

function excluirRegraMarcaProdutoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_marcaProduto);
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
    _marcaProduto.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposMarcaProduto();
}

function cancelarRegraMarcaProdutoClick(e, sender) {
    LimparCamposMarcaProduto();
}



//*******MÉTODOS*******

function ObjetoRegraMarcaProduto() {
    var codigo = _marcaProduto.Codigo.val();
    var ordem = _marcaProduto.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasMarcaProduto.ObterOrdencao().length + 1,
        Juncao: _marcaProduto.Juncao.val(),
        Condicao: _marcaProduto.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_marcaProduto.MarcaProduto.codEntity()),
            Descricao: _marcaProduto.MarcaProduto.val()
        }
    };

    return regra;
}

function LimparCamposMarcaProduto() {
    _marcaProduto.Codigo.val(_marcaProduto.Codigo.def);
    _marcaProduto.Ordem.val(_marcaProduto.Ordem.def);
    _marcaProduto.Condicao.val(_marcaProduto.Condicao.def);
    _marcaProduto.Juncao.val(_marcaProduto.Juncao.def);

    LimparCampoEntity(_marcaProduto.MarcaProduto);

    _marcaProduto.Adicionar.visible(true);
    _marcaProduto.Atualizar.visible(false);
    _marcaProduto.Excluir.visible(false);
    _marcaProduto.Cancelar.visible(false);
}