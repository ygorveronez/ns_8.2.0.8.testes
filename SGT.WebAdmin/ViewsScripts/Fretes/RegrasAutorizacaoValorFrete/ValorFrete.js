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
/// <reference path="../../Enumeradores/EnumTipoAutorizaoTabelaFrete.js" />
/// <reference path="RegrasAutorizacaoValorFrete.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasValorFrete;
var _valorFrete;

var ValorFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoValorFrete.IgualA), options: _condicaoAutorizaoTabelaFreteEntidade, def: EnumCondicaoAutorizaoValorFrete.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoTabelaFrete.E), options: _juncaoAutorizaoTabelaFrete, def: EnumJuncaoAutorizaoTabelaFrete.E });
    this.TipoRegra = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoAutorizaoTabelaFrete.ValorFixo), options: _tipoAutorizaoTabelaFrete, def: EnumTipoAutorizaoTabelaFrete.ValorFixo });
    this.TipoRegra.val.subscribe(changeTipoRegra);
    this.ValorFrete = PropertyEntity({ text: ko.observable(""), getType: typesKnockout.decimal, required: ko.observable(true), def: "0.00" });
    
    // Controle de regra
    this.Regras = PropertyEntity({ text: "Valor do Frete", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorFrete, _gridRegrasValorFrete, "editarRegraValorFreteClick", true, true);
    });

    // Controle de uso
    this.UsarRegraPorValorFrete = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por valor do frete:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorFrete.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorFrete(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorFreteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorFreteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorFreteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorFreteClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorValorFrete(usarRegra) {
    _valorFrete.Visible.visibleFade(usarRegra);
    _valorFrete.Regras.required(usarRegra);
}

function changeTipoRegra() {
    var text = "";
    if (_valorFrete.TipoRegra.val() == EnumTipoAutorizaoTabelaFrete.ValorFixo)
        text = "Valor Fixo: ";
    else
        text = "Percentual Reajuste: ";
    _valorFrete.ValorFrete.text(text);
}

function loadValorFrete() {
    _valorFrete = new ValorFrete();
    KoBindings(_valorFrete, "knockoutRegraValorFrete");

    //-- Grid Regras
    _gridRegrasValorFrete = new GridReordering(_configRegras.infoTable, _valorFrete.Regras.idGrid, GeraHeadTable("Valor do Frete", true));
    _gridRegrasValorFrete.CarregarGrid();
    $("#" + _valorFrete.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasTabelaFrete(_valorFrete);
    });

    changeTipoRegra();
}

function editarRegraValorFreteClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorFrete.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorFrete.Codigo.val(regra.Codigo);
        _valorFrete.Ordem.val(regra.Ordem);
        _valorFrete.Condicao.val(regra.Condicao);
        _valorFrete.Juncao.val(regra.Juncao);
        _valorFrete.TipoRegra.val(regra.TipoRegra);
        _valorFrete.ValorFrete.val(regra.Valor);

        _valorFrete.Adicionar.visible(false);
        _valorFrete.Atualizar.visible(true);
        _valorFrete.Excluir.visible(true);
        _valorFrete.Cancelar.visible(true);
    }
}

function adicionarRegraValorFreteClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorFrete))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorFrete();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valorFrete);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valorFrete.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorFrete();
}

function atualizarRegraValorFreteClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorFrete))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorFrete();

    // Buscar todas regras
    var listaRegras = _valorFrete.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorFrete.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valorFrete.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorFrete();
}

function excluirRegraValorFreteClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorFrete);
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
    _valorFrete.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposValorFrete();
}

function cancelarRegraValorFreteClick(e, sender) {
    LimparCamposValorFrete();
}



//*******MÉTODOS*******

function ObjetoRegraValorFrete() {
    var codigo = _valorFrete.Codigo.val();
    var ordem = _valorFrete.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorFrete.ObterOrdencao().length + 1,
        Juncao: _valorFrete.Juncao.val(),
        Condicao: _valorFrete.Condicao.val(),
        TipoRegra: _valorFrete.TipoRegra.val(),
        Valor: Globalize.parseFloat(_valorFrete.ValorFrete.val().toString())
    };

    return regra;
}

function LimparCamposValorFrete() {
    _valorFrete.Codigo.val(_valorFrete.Codigo.def);
    _valorFrete.Ordem.val(_valorFrete.Ordem.def);
    _valorFrete.Condicao.val(_valorFrete.Condicao.def);
    _valorFrete.Juncao.val(_valorFrete.Juncao.def);
    _valorFrete.TipoRegra.val(_valorFrete.TipoRegra.def);
    _valorFrete.ValorFrete.val(_valorFrete.ValorFrete.def);

    _valorFrete.Adicionar.visible(true);
    _valorFrete.Atualizar.visible(false);
    _valorFrete.Excluir.visible(false);
    _valorFrete.Cancelar.visible(false);
}