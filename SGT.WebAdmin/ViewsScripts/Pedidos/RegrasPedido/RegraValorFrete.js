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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="RegrasPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasValorFreteFrete;
var _valorFrete;

var ValorFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.ValorFrete = PropertyEntity({ text: "Valor do Frete:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Valor do Frete", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorFrete, _gridRegrasValorFreteFrete, "editarRegraValorFreteClick", true);
    });

    // Controle de uso
    this.RegraPorValorFrete = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por valor do frete:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorValorFrete.val.subscribe(function (novoValorFrete) {
        SincronzarRegras();
        RegraPorValorFrete(novoValorFrete);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorFreteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorFreteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorFreteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorFreteClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorValorFrete(usarRegra) {
    _valorFrete.Visible.visibleFade(usarRegra);
    _valorFrete.Regras.required(usarRegra);
}

function loadValorFrete() {
    _valorFrete = new ValorFrete();
    KoBindings(_valorFrete, "knockoutRegraValorFrete");

    //-- Grid Regras
    _gridRegrasValorFreteFrete = new GridReordering(_configRegras.infoTable, _valorFrete.Regras.idGrid, GeraHeadTable("Valor do Frete"));
    _gridRegrasValorFreteFrete.CarregarGrid();
    $("#" + _valorFrete.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadas(_valorFrete);
    });
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
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
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
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorFrete.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valorFretees
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
        Ordem: ordem != 0 ? ordem : _gridRegrasValorFreteFrete.ObterOrdencao().length + 1,
        Juncao: _valorFrete.Juncao.val(),
        Condicao: _valorFrete.Condicao.val(),
        Valor: Globalize.parseFloat(_valorFrete.ValorFrete.val())
    };

    return regra;
}

function LimparCamposValorFrete() {
    _valorFrete.Codigo.val(_valorFrete.Codigo.def);
    _valorFrete.Ordem.val(_valorFrete.Ordem.def);
    _valorFrete.Condicao.val(_valorFrete.Condicao.def);
    _valorFrete.Juncao.val(_valorFrete.Juncao.def);
    _valorFrete.ValorFrete.val(_valorFrete.ValorFrete.def);

    _valorFrete.Adicionar.visible(true);
    _valorFrete.Atualizar.visible(false);
    _valorFrete.Excluir.visible(false);
    _valorFrete.Cancelar.visible(false);
}