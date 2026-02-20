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

var _gridRegrasDistanciaFrete;
var _distancia;

var Distancia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.Distancia = PropertyEntity({ text: "Distancia (KM):", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Distância", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_distancia, _gridRegrasDistanciaFrete, "editarRegraDistanciaClick", true);
    });

    // Controle de uso
    this.RegraPorDistancia = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por distância:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorDistancia.val.subscribe(function (novoDistancia) {
        SincronzarRegras();
        RegraPorDistancia(novoDistancia);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraDistanciaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraDistanciaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraDistanciaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraDistanciaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorDistancia(usarRegra) {
    _distancia.Visible.visibleFade(usarRegra);
    _distancia.Regras.required(usarRegra);
}

function loadDistancia() {
    _distancia = new Distancia();
    KoBindings(_distancia, "knockoutRegraDistancia");

    //-- Grid Regras
    _gridRegrasDistanciaFrete = new GridReordering(_configRegras.infoTable, _distancia.Regras.idGrid, GeraHeadTable("Distância"));
    _gridRegrasDistanciaFrete.CarregarGrid();
    $("#" + _distancia.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadas(_distancia);
    });
}

function editarRegraDistanciaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _distancia.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _distancia.Codigo.val(regra.Codigo);
        _distancia.Ordem.val(regra.Ordem);
        _distancia.Condicao.val(regra.Condicao);
        _distancia.Juncao.val(regra.Juncao);
        _distancia.Distancia.val(regra.Valor);

        _distancia.Adicionar.visible(false);
        _distancia.Atualizar.visible(true);
        _distancia.Excluir.visible(true);
        _distancia.Cancelar.visible(true);
    }
}

function adicionarRegraDistanciaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_distancia))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDistancia();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_distancia);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _distancia.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDistancia();
}

function atualizarRegraDistanciaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_distancia))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDistancia();

    // Buscar todas regras
    var listaRegras = _distancia.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _distancia.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os distanciaes
    _distancia.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDistancia();
}

function excluirRegraDistanciaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_distancia);
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
    _distancia.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposDistancia();
}

function cancelarRegraDistanciaClick(e, sender) {
    LimparCamposDistancia();
}



//*******MÉTODOS*******

function ObjetoRegraDistancia() {
    var codigo = _distancia.Codigo.val();
    var ordem = _distancia.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasDistanciaFrete.ObterOrdencao().length + 1,
        Juncao: _distancia.Juncao.val(),
        Condicao: _distancia.Condicao.val(),
        Valor: Globalize.parseFloat(_distancia.Distancia.val())
    };

    return regra;
}

function LimparCamposDistancia() {
    _distancia.Codigo.val(_distancia.Codigo.def);
    _distancia.Ordem.val(_distancia.Ordem.def);
    _distancia.Condicao.val(_distancia.Condicao.def);
    _distancia.Juncao.val(_distancia.Juncao.def);
    _distancia.Distancia.val(_distancia.Distancia.def);

    _distancia.Adicionar.visible(true);
    _distancia.Atualizar.visible(false);
    _distancia.Excluir.visible(false);
    _distancia.Cancelar.visible(false);
}