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
/// <reference path="RegraDescarteLoteProduto.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasProdutoEmbarcador;
var _produtoEmbarcador;

var ProdutoEmbarcador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.ProdutoEmbarcador = PropertyEntity({ text: "Produto Embarcador:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Produto Embarcador", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_produtoEmbarcador, _gridRegrasProdutoEmbarcador, "editarRegraProdutoEmbarcadorClick");
    });

    // Controle de uso
    this.UsarRegraPorProdutoEmbarcador = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por produto embarcador:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorProdutoEmbarcador.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorProdutoEmbarcador(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraProdutoEmbarcadorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraProdutoEmbarcadorClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraProdutoEmbarcadorClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraProdutoEmbarcadorClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorProdutoEmbarcador(usarRegra) {
    _produtoEmbarcador.Visible.visibleFade(usarRegra);
    _produtoEmbarcador.Alcadas.required(usarRegra);
}

function loadProdutoEmbarcador() {
    _produtoEmbarcador = new ProdutoEmbarcador();
    KoBindings(_produtoEmbarcador, "knockoutRegraProdutoEmbarcador");

    //-- Busca
    new BuscarProdutos(_produtoEmbarcador.ProdutoEmbarcador);

    //-- Grid Regras
    _gridRegrasProdutoEmbarcador = new GridReordering(_configRegras.infoTable, _produtoEmbarcador.Alcadas.idGrid, GeraHeadTable("Produto Embarcador"));
    _gridRegrasProdutoEmbarcador.CarregarGrid();
    $("#" + _produtoEmbarcador.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasDescarteLoteProduto(_produtoEmbarcador);
    });
}

function editarRegraProdutoEmbarcadorClick(codigo) {
    // Buscar todas regras
    var listaRegras = _produtoEmbarcador.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _produtoEmbarcador.Codigo.val(regra.Codigo);
        _produtoEmbarcador.Ordem.val(regra.Ordem);
        _produtoEmbarcador.Condicao.val(regra.Condicao);
        _produtoEmbarcador.Juncao.val(regra.Juncao);

        _produtoEmbarcador.ProdutoEmbarcador.val(regra.Entidade.Descricao);
        _produtoEmbarcador.ProdutoEmbarcador.codEntity(regra.Entidade.Codigo);

        _produtoEmbarcador.Adicionar.visible(false);
        _produtoEmbarcador.Atualizar.visible(true);
        _produtoEmbarcador.Excluir.visible(true);
        _produtoEmbarcador.Cancelar.visible(true);
    }
}

function adicionarRegraProdutoEmbarcadorClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_produtoEmbarcador))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraProdutoEmbarcador();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_produtoEmbarcador);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _produtoEmbarcador.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposProdutoEmbarcador();
}

function atualizarRegraProdutoEmbarcadorClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_produtoEmbarcador))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraProdutoEmbarcador();

    // Buscar todas regras
    var listaRegras = _produtoEmbarcador.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _produtoEmbarcador.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _produtoEmbarcador.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposProdutoEmbarcador();
}

function excluirRegraProdutoEmbarcadorClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_produtoEmbarcador);
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
    _produtoEmbarcador.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposProdutoEmbarcador();
}

function cancelarRegraProdutoEmbarcadorClick(e, sender) {
    LimparCamposProdutoEmbarcador();
}



//*******MÉTODOS*******

function ObjetoRegraProdutoEmbarcador() {
    var codigo = _produtoEmbarcador.Codigo.val();
    var ordem = _produtoEmbarcador.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasProdutoEmbarcador.ObterOrdencao().length + 1,
        Juncao: _produtoEmbarcador.Juncao.val(),
        Condicao: _produtoEmbarcador.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_produtoEmbarcador.ProdutoEmbarcador.codEntity()),
            Descricao: _produtoEmbarcador.ProdutoEmbarcador.val()
        }
    };

    return regra;
}

function LimparCamposProdutoEmbarcador() {
    _produtoEmbarcador.Codigo.val(_produtoEmbarcador.Codigo.def);
    _produtoEmbarcador.Ordem.val(_produtoEmbarcador.Ordem.def);
    _produtoEmbarcador.Condicao.val(_produtoEmbarcador.Condicao.def);
    _produtoEmbarcador.Juncao.val(_produtoEmbarcador.Juncao.def);

    LimparCampoEntity(_produtoEmbarcador.ProdutoEmbarcador);

    _produtoEmbarcador.Adicionar.visible(true);
    _produtoEmbarcador.Atualizar.visible(false);
    _produtoEmbarcador.Excluir.visible(false);
    _produtoEmbarcador.Cancelar.visible(false);
}