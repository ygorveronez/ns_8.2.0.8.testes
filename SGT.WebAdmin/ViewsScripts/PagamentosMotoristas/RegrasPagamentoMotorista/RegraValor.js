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
/// <reference path="RegrasPagamentoMotorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasValor;
var _valor;

var Valor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.Valor = PropertyEntity({ text: "Valor do Pagamento:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Valor da Ocorrência", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valor, _gridRegrasValor, "editarRegraValorClick", true);
    });

    // Controle de uso
    this.RegraPorValor = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por valor do pagamento:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorValor.val.subscribe(function (novoValor) {
        SincronzarRegras();
        RegraPorValor(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorValor(usarRegra) {
    _valor.Visible.visibleFade(usarRegra);
    _valor.Regras.required(usarRegra);
}

function loadValor() {
    _valor = new Valor();
    KoBindings(_valor, "knockoutRegraPagamentoValor");

    //-- Grid Regras
    _gridRegrasValor = new GridReordering(_configRegras.infoTable, _valor.Regras.idGrid, GeraHeadTable("Valor do Pagamento"));
    _gridRegrasValor.CarregarGrid();
    $("#" + _valor.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadas(_valor);
    });
}

function editarRegraValorClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valor.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valor.Codigo.val(regra.Codigo);
        _valor.Ordem.val(regra.Ordem);
        _valor.Condicao.val(regra.Condicao);
        _valor.Juncao.val(regra.Juncao);
        _valor.Valor.val(regra.Valor);

        _valor.Adicionar.visible(false);
        _valor.Atualizar.visible(true);
        _valor.Excluir.visible(true);
        _valor.Cancelar.visible(true);
    }
}

function adicionarRegraValorClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valor))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValor();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valor);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valor.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValor();
}

function atualizarRegraValorClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valor))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValor();

    // Buscar todas regras
    var listaRegras = _valor.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valor.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valor.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValor();
}

function excluirRegraValorClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valor);
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
    _valor.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposValor();
}

function cancelarRegraValorClick(e, sender) {
    LimparCamposValor();
}



//*******MÉTODOS*******

function ObjetoRegraValor() {
    var codigo = _valor.Codigo.val();
    var ordem = _valor.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValor.ObterOrdencao().length + 1,
        Juncao: _valor.Juncao.val(),
        Condicao: _valor.Condicao.val(),
        Valor: Globalize.parseFloat(_valor.Valor.val())
    };

    return regra;
}

function LimparCamposValor() {
    _valor.Codigo.val(_valor.Codigo.def);
    _valor.Ordem.val(_valor.Ordem.def);
    _valor.Condicao.val(_valor.Condicao.def);
    _valor.Juncao.val(_valor.Juncao.def);
    _valor.Valor.val(_valor.Valor.def);

    _valor.Adicionar.visible(true);
    _valor.Atualizar.visible(false);
    _valor.Excluir.visible(false);
    _valor.Cancelar.visible(false);
}