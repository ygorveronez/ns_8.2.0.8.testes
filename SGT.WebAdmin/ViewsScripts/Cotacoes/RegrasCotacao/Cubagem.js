/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../enumeradores/enumcondicaoautorizaocotacao.js" />
/// <reference path="../../enumeradores/enumjuncaoautorizao.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Regrascotacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasCubagem;
var _cubagem;

var Cubagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumJuncaoAutorizao.E });
    this.Cubagem = PropertyEntity({ text: "Cubagem:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Cubagem", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_cubagem, _gridRegrasCubagem, "editarRegraCubagemClick", typesKnockout.decimal);
    });


    // Controle de uso
    this.UsarRegraPorCubagem = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por cubagem:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorCubagem.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorCubagem(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraCubagemClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraCubagemClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraCubagemClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraCubagemClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

}


//*******EVENTOS*******
function UsarRegraPorCubagem(usarRegra) {
    _cubagem.Visible.visibleFade(usarRegra);
    _cubagem.Regras.required(usarRegra);
}

function loadCubagem() {
    _cubagem = new Cubagem();
    KoBindings(_cubagem, "knockoutRegraCubagem");
        
    //-- Grid Regras
    _gridRegrasCubagem = new GridReordering(_configRegras.infoTable, _cubagem.Regras.idGrid, GeraHeadTable("Cubagem"));
    _gridRegrasCubagem.CarregarGrid();
    $("#" + _cubagem.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_cubagem);
    });

}

function editarRegraCubagemClick(codigo) {
    // Buscar todas regras
    var listaRegras = _cubagem.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _cubagem.Codigo.val(regra.Codigo);
        _cubagem.Ordem.val(regra.Ordem);
        _cubagem.Condicao.val(regra.Condicao);
        _cubagem.Juncao.val(regra.Juncao);
        _cubagem.Cubagem.val(regra.Valor);

        _cubagem.Adicionar.visible(false);
        _cubagem.Atualizar.visible(true);
        _cubagem.Excluir.visible(true);
        _cubagem.Cancelar.visible(true);
    }
}

function adicionarRegraCubagemClick() {
    if (!ValidarCamposObrigatorios(_cubagem))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var regra = ObjetoRegraCubagem();

    var listaRegras = ObterRegrasOrdenadas(_cubagem);

    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _cubagem.Regras.val(listaRegras);

    LimparCamposValorCubagem();
}

function atualizarRegraCubagemClick(e, sender) {

    if (!ValidarCamposObrigatorios(_cubagem))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var regra = ObjetoRegraCubagem();
    var listaRegras = _cubagem.Regras.val();
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _cubagem.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    _cubagem.Regras.val(listaRegras);

    LimparCamposValorCubagem();
}

function excluirRegraCubagemClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_cubagem);
    var index = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == e.Codigo.val()) {
            index = parseInt(i);
            break;
        }
    }

    listaRegras.splice(index, 1);

    for (i = 1; i <= listaRegras.length; i++)
        listaRegras[i - 1].Ordem = i;

    _cubagem.Regras.val(listaRegras);

    LimparCamposValorCubagem();
}

function cancelarRegraCubagemClick(e, sender) {
    LimparCamposValorCubagem();
}



//*******MÉTODOS*******

function ObjetoRegraCubagem() {
    var codigo = _cubagem.Codigo.val();
    var ordem = _cubagem.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasCubagem.ObterOrdencao().length + 1,
        Juncao: _cubagem.Juncao.val(),
        Condicao: _cubagem.Condicao.val(),
        Valor: Globalize.parseFloat(_cubagem.Cubagem.val())
    };

    return regra;
}

function LimparCamposValorCubagem() {
    _cubagem.Codigo.val(_cubagem.Codigo.def);
    _cubagem.Ordem.val(_cubagem.Ordem.def);
    _cubagem.Condicao.val(_cubagem.Condicao.def);
    _cubagem.Juncao.val(_cubagem.Juncao.def);
    _cubagem.Cubagem.val(_cubagem.Cubagem.def);

    _cubagem.Adicionar.visible(true);
    _cubagem.Atualizar.visible(false);
    _cubagem.Excluir.visible(false);
    _cubagem.Cancelar.visible(false);
}