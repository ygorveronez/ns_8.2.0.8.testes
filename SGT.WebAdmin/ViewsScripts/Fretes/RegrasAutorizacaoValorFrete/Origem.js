/// <reference path="../../Consultas/Localidade.js" />
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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoValorFrete.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoTabelaFrete.js" />
/// <reference path="RegrasAutorizacaoValorFrete.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasOrigemFrete;
var _origemFrete;

var OrigemFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoValorFrete.IgualA), options: _condicaoAutorizaoTabelaFreteEntidade, def: EnumCondicaoAutorizaoValorFrete.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoTabelaFrete.E), options: _juncaoAutorizaoTabelaFrete, def: EnumJuncaoAutorizaoTabelaFrete.E });
    this.OrigemFrete = PropertyEntity({ text: "Origem: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Origem", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_origemFrete, _gridRegrasOrigemFrete, "editarRegraOrigemFreteClick");
    });

    // Controle de uso
    this.UsarRegraPorOrigemFrete = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por origem:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorOrigemFrete.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorOrigemFrete(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraOrigemFreteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraOrigemFreteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraOrigemFreteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraOrigemFreteClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorOrigemFrete(usarRegra) {
    _origemFrete.Visible.visibleFade(usarRegra);
    _origemFrete.Regras.required(usarRegra);
}

function loadOrigemFrete() {
    _origemFrete = new OrigemFrete();
    KoBindings(_origemFrete, "knockoutRegraOrigemFrete");

    //-- Busca
    new BuscarLocalidades(_origemFrete.OrigemFrete);

    //-- Grid Regras
    _gridRegrasOrigemFrete = new GridReordering(_configRegras.infoTable, _origemFrete.Regras.idGrid, GeraHeadTable("Origem"));
    _gridRegrasOrigemFrete.CarregarGrid();
    $("#" + _origemFrete.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasTabelaFrete(_origemFrete);
    });
}

function editarRegraOrigemFreteClick(codigo) {
    // Buscar todas regras
    var listaRegras = _origemFrete.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _origemFrete.Codigo.val(regra.Codigo);
        _origemFrete.Ordem.val(regra.Ordem);
        _origemFrete.Condicao.val(regra.Condicao);
        _origemFrete.Juncao.val(regra.Juncao);

        _origemFrete.OrigemFrete.val(regra.Entidade.Descricao);
        _origemFrete.OrigemFrete.codEntity(regra.Entidade.Codigo);

        _origemFrete.Adicionar.visible(false);
        _origemFrete.Atualizar.visible(true);
        _origemFrete.Excluir.visible(true);
        _origemFrete.Cancelar.visible(true);
    }
}

function adicionarRegraOrigemFreteClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_origemFrete))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraOrigemFrete();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_origemFrete);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _origemFrete.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposOrigemFrete();
}

function atualizarRegraOrigemFreteClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_origemFrete))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraOrigemFrete();

    // Buscar todas regras
    var listaRegras = _origemFrete.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _origemFrete.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _origemFrete.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposOrigemFrete();
}

function excluirRegraOrigemFreteClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_origemFrete);
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
    _origemFrete.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposOrigemFrete();
}

function cancelarRegraOrigemFreteClick(e, sender) {
    LimparCamposOrigemFrete();
}



//*******MÉTODOS*******

function ObjetoRegraOrigemFrete() {
    var codigo = _origemFrete.Codigo.val();
    var ordem = _origemFrete.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasOrigemFrete.ObterOrdencao().length + 1,
        Juncao: _origemFrete.Juncao.val(),
        Condicao: _origemFrete.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_origemFrete.OrigemFrete.codEntity()),
            Descricao: _origemFrete.OrigemFrete.val()
        }
    };

    return regra;
}

function LimparCamposOrigemFrete() {
    _origemFrete.Codigo.val(_origemFrete.Codigo.def);
    _origemFrete.Ordem.val(_origemFrete.Ordem.def);
    _origemFrete.Condicao.val(_origemFrete.Condicao.def);
    _origemFrete.Juncao.val(_origemFrete.Juncao.def);

    LimparCampoEntity(_origemFrete.OrigemFrete);

    _origemFrete.Adicionar.visible(true);
    _origemFrete.Atualizar.visible(false);
    _origemFrete.Excluir.visible(false);
    _origemFrete.Cancelar.visible(false);
}