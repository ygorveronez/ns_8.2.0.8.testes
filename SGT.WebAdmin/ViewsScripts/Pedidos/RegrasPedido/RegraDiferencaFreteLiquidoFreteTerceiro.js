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

var _gridRegrasDiferencaFreteLiquidoParaFreteTerceiro;
var _regraPorDiferencaFreteLiquidoParaFreteTerceiro;

var DiferencaFreteLiquidoParaFreteTerceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.DiferencaFreteLiquidoParaFreteTerceiro = PropertyEntity({ text: "Percentual de Diferença do Frete Líquido para o Frete com Terceiro:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Percentual de Diferença do Frete Líquido para o Frete com Terceiro", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_regraPorDiferencaFreteLiquidoParaFreteTerceiro, _gridRegrasDiferencaFreteLiquidoParaFreteTerceiro, "editarRegraDiferencaFreteLiquidoParaFreteTerceiroClick", true);
    });

    // Controle de uso
    this.RegraPorDiferencaFreteLiquidoParaFreteTerceiro = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por Percentual de Diferença do Frete Líquido para o Frete com Terceiro:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorDiferencaFreteLiquidoParaFreteTerceiro.val.subscribe(function (novaRegra) {
        SincronzarRegras();
        RegraPorDiferencaFreteLiquidoParaFreteTerceiro(novaRegra);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraDiferencaFreteLiquidoParaFreteTerceiroClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraDiferencaFreteLiquidoParaFreteTerceiroClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraDiferencaFreteLiquidoParaFreteTerceiroClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraDiferencaFreteLiquidoParaFreteTerceiroClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorDiferencaFreteLiquidoParaFreteTerceiro(usarRegra) {
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Visible.visibleFade(usarRegra);
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.required(usarRegra);
}

function loadRegraPorDiferencaFreteLiquidoParaFreteTerceiro() {
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro = new DiferencaFreteLiquidoParaFreteTerceiro();
    KoBindings(_regraPorDiferencaFreteLiquidoParaFreteTerceiro, "knockoutRegraDiferencaFreteLiquidoParaFreteTerceiro");

    //-- Grid Regras
    _gridRegrasDiferencaFreteLiquidoParaFreteTerceiro = new GridReordering(_configRegras.infoTable, _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.idGrid, GeraHeadTable("Percentual de Diferença do Frete Líquido para o Frete com Terceiro"));
    _gridRegrasDiferencaFreteLiquidoParaFreteTerceiro.CarregarGrid();
    $("#" + _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadas(_regraPorDiferencaFreteLiquidoParaFreteTerceiro);
    });
}

function editarRegraDiferencaFreteLiquidoParaFreteTerceiroClick(codigo) {
    // Buscar todas regras
    var listaRegras = _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Codigo.val(regra.Codigo);
        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Ordem.val(regra.Ordem);
        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Condicao.val(regra.Condicao);
        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Juncao.val(regra.Juncao);
        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.DiferencaFreteLiquidoParaFreteTerceiro.val(regra.Valor);

        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Adicionar.visible(false);
        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Atualizar.visible(true);
        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Excluir.visible(true);
        _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Cancelar.visible(true);
    }
}

function adicionarRegraDiferencaFreteLiquidoParaFreteTerceiroClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_regraPorDiferencaFreteLiquidoParaFreteTerceiro))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDiferencaFreteLiquidoParaFreteTerceiro();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_regraPorDiferencaFreteLiquidoParaFreteTerceiro);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposRegraDiferencaFreteLiquidoParaFreteTerceiro();
}

function atualizarRegraDiferencaFreteLiquidoParaFreteTerceiroClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_regraPorDiferencaFreteLiquidoParaFreteTerceiro))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDiferencaFreteLiquidoParaFreteTerceiro();
 
    // Buscar todas regras
    var listaRegras = _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os percentuais
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposRegraDiferencaFreteLiquidoParaFreteTerceiro();
}

function excluirRegraDiferencaFreteLiquidoParaFreteTerceiroClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_regraPorDiferencaFreteLiquidoParaFreteTerceiro);
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
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposRegraDiferencaFreteLiquidoParaFreteTerceiro();
}

function cancelarRegraDiferencaFreteLiquidoParaFreteTerceiroClick(e, sender) {
    LimparCamposRegraDiferencaFreteLiquidoParaFreteTerceiro();
}

//*******MÉTODOS*******

function ObjetoRegraDiferencaFreteLiquidoParaFreteTerceiro() {
    var codigo = _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Codigo.val();
    var ordem = _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasDiferencaFreteLiquidoParaFreteTerceiro.ObterOrdencao().length + 1,
        Juncao: _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Juncao.val(),
        Condicao: _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Condicao.val(),
        Valor: Globalize.parseFloat(_regraPorDiferencaFreteLiquidoParaFreteTerceiro.DiferencaFreteLiquidoParaFreteTerceiro.val())
    };

    return regra;
}

function LimparCamposRegraDiferencaFreteLiquidoParaFreteTerceiro() {
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Codigo.val(_regraPorDiferencaFreteLiquidoParaFreteTerceiro.Codigo.def);
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Ordem.val(_regraPorDiferencaFreteLiquidoParaFreteTerceiro.Ordem.def);
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Condicao.val(_regraPorDiferencaFreteLiquidoParaFreteTerceiro.Condicao.def);
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Juncao.val(_regraPorDiferencaFreteLiquidoParaFreteTerceiro.Juncao.def);
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.DiferencaFreteLiquidoParaFreteTerceiro.val(_regraPorDiferencaFreteLiquidoParaFreteTerceiro.DiferencaFreteLiquidoParaFreteTerceiro.def);

    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Adicionar.visible(true);
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Atualizar.visible(false);
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Excluir.visible(false);
    _regraPorDiferencaFreteLiquidoParaFreteTerceiro.Cancelar.visible(false);
}