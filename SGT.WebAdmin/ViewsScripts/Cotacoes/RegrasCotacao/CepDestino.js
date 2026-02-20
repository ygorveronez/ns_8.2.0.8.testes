/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../enumeradores/enumcondicaoautorizaocotacao.js" />
/// <reference path="../../enumeradores/enumjuncaoautorizao.js" />
/// <reference path="Regrascotacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasCepDestino;
var _cepDestino;

var CepDestino = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumJuncaoAutorizao.E });
    this.CepDestino = PropertyEntity({ text: "Cep Destino:", type: types.string, getType: typesKnockout.string, required: ko.observable(true), def: "" });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Cep Destino", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_cepDestino, _gridRegrasCepDestino, "editarRegraCepDestinoClick", typesKnockout.string);
    });

    // Controle de uso
    this.UsarRegraPorCepDestino = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por Cep destino:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorCepDestino.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorCepDestino(novoValor);
    });


    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraCepDestinoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraCepDestinoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraCepDestinoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraCepDestinoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

}

//*******EVENTOS*******

function UsarRegraPorCepDestino(usarRegra) {
    _cepDestino.Visible.visibleFade(usarRegra);
    _cepDestino.Regras.required(usarRegra);
}

function loadCepDestino() {
    _cepDestino = new CepDestino();
    KoBindings(_cepDestino, "knockoutRegraCepDestino");

    //-- Grid Regras
    _gridRegrasCepDestino = new GridReordering(_configRegras.infoTable, _cepDestino.Regras.idGrid, GeraHeadTable("Cep Destino"));
    _gridRegrasCepDestino.CarregarGrid();
    $("#" + _cepDestino.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_cepDestino);
    });
}

function editarRegraCepDestinoClick(codigo) {
    var listaRegras = _cepDestino.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _cepDestino.Codigo.val(regra.Codigo);
        _cepDestino.Ordem.val(regra.Ordem);
        _cepDestino.Condicao.val(regra.Condicao);
        _cepDestino.Juncao.val(regra.Juncao);
        _cepDestino.CepDestino.val(regra.Valor);

        _cepDestino.Adicionar.visible(false);
        _cepDestino.Atualizar.visible(true);
        _cepDestino.Excluir.visible(true);
        _cepDestino.Cancelar.visible(true);
    }
}

function adicionarRegraCepDestinoClick() {
    if (!ValidarCamposObrigatorios(_cepDestino))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    var regra = ObjetoRegraCepDestino();
    var listaRegras = ObterRegrasOrdenadas(_cepDestino);

    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _cepDestino.Regras.val(listaRegras);

    LimparCamposCepDestino();
}

function atualizarRegraCepDestinoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_cepDestino))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var regra = ObjetoRegraCepDestino();
    var listaRegras = _cepDestino.Regras.val();
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _cepDestino.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }
    _cepDestino.Regras.val(listaRegras);
    LimparCamposCepDestino();
}

function excluirRegraCepDestinoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_cepDestino);
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

    _cepDestino.Regras.val(listaRegras);
    LimparCamposCepDestino();
}

function cancelarRegraCepDestinoClick(e, sender) {
    LimparCamposCepDestino();
}

//*******MÉTODOS*******

function ObjetoRegraCepDestino() {
    var codigo = _cepDestino.Codigo.val();
    var ordem = _cepDestino.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasCepDestino.ObterOrdencao().length + 1,
        Juncao: _cepDestino.Juncao.val(),
        Condicao: _cepDestino.Condicao.val(),
        Valor: _cepDestino.CepDestino.val()
    };

    return regra;
}

function LimparCamposCepDestino() {
    _cepDestino.Codigo.val(_cepDestino.Codigo.def);
    _cepDestino.Ordem.val(_cepDestino.Ordem.def);
    _cepDestino.Condicao.val(_cepDestino.Condicao.def);
    _cepDestino.Juncao.val(_cepDestino.Juncao.def);
    _cepDestino.CepDestino.val(_cepDestino.CepDestino.def);

    _cepDestino.Adicionar.visible(true);
    _cepDestino.Atualizar.visible(false);
    _cepDestino.Excluir.visible(false);
    _cepDestino.Cancelar.visible(false);
}
