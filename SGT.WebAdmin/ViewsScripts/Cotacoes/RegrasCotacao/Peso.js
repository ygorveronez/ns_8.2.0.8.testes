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
/// <reference path="../../Enumeradores/EnumAplicacaoRegraCotacao.js" />
/// <reference path="../../enumeradores/enumcondicaoautorizaocotacao.js" />
/// <reference path="../../enumeradores/enumjuncaoautorizao.js" />
/// <reference path="RegrasCotacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasPeso;
var _peso;

var Peso = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumCondicaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumCondicaoAutorizao.E });
    this.Peso = PropertyEntity({ text: "Peso:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    this.OpcaoAplicacao = PropertyEntity({ val: ko.observable(EnumAplicacaoRegraCotacao.ExcluirTransportador), options: EnumAplicacaoRegraCotacao.obterOpcoes(), def: EnumAplicacaoRegraCotacao.ExcluirTransportador, text: "Informe a ação da regra: ", visible: ko.observable(true) });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Peso", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_peso, _gridRegrasPeso, "editarRegraPesoClick", typesKnockout.decimal);
    });

    // Controle de uso
    this.UsarRegraPorPeso = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por peso:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorPeso.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorPeso(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraPesoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraPesoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraPesoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraPesoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorPeso(usarRegra) {
    _peso.Visible.visibleFade(usarRegra);
    _peso.Regras.required(usarRegra);
}

function loadPeso() {
    _peso = new Peso();
    KoBindings(_peso, "knockoutRegraPeso");

    //-- Grid Regras
    _gridRegrasPeso = new GridReordering(_configRegras.infoTable, _peso.Regras.idGrid, GeraHeadTable("Peso"));
    _gridRegrasPeso.CarregarGrid();
    $("#" + _peso.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_peso);
    });
}

function editarRegraPesoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _peso.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _peso.Codigo.val(regra.Codigo);
        _peso.Ordem.val(regra.Ordem);
        _peso.Condicao.val(regra.Condicao);
        _peso.Juncao.val(regra.Juncao);
        _peso.Peso.val(regra.Valor);

        _peso.Adicionar.visible(false);
        _peso.Atualizar.visible(true);
        _peso.Excluir.visible(true);
        _peso.Cancelar.visible(true);
    }
}

function adicionarRegraPesoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_peso))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraPeso();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_peso);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _peso.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposPeso();
}

function atualizarRegraPesoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_peso))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraPeso();

    // Buscar todas regras
    var listaRegras = _peso.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _peso.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _peso.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposPeso();
}

function excluirRegraPesoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_peso);
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
    _peso.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposPeso();
}

function cancelarRegraPesoClick(e, sender) {
    LimparCamposPeso();
}



//*******MÉTODOS*******

function ObjetoRegraPeso() {
    var codigo = _peso.Codigo.val();
    var ordem = _peso.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasPeso.ObterOrdencao().length + 1,
        Juncao: _peso.Juncao.val(),
        Condicao: _peso.Condicao.val(),
        Valor: Globalize.parseFloat(_peso.Peso.val())
    };

    return regra;
}

function LimparCamposPeso() {
    _peso.Codigo.val(_peso.Codigo.def);
    _peso.Ordem.val(_peso.Ordem.def);
    _peso.Condicao.val(_peso.Condicao.def);
    _peso.Juncao.val(_peso.Juncao.def);
    _peso.Peso.val(_peso.Peso.def);

    _peso.Adicionar.visible(true);
    _peso.Atualizar.visible(false);
    _peso.Excluir.visible(false);
    _peso.Cancelar.visible(false);
}