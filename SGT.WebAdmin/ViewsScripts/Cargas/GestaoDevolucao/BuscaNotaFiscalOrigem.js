/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />

//#region Objetos Globais do Arquivo
var _gridNotaFiscalOrigem;
var _notasOrigem;
var _emitenteSelecionado;
// #endregion Objetos Globais do Arquivo

//#region Classes
var NotaFiscalOrigemDevolucao = function () {
    this.CodigoGestaoDevolucaoImportacao = PropertyEntity({ val: ko.observable(0) });
    this.TipoEmissao = PropertyEntity({ val: ko.observable(1), def: 1, text: Localization.Resources.Consultas.NotaFiscalEletronica.TipoEmissao.getFieldDescription(), visible: false });
    this.NumeroNotaFiscal = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: "Número", visible: ko.observable(true) });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.Serie = PropertyEntity({ val: ko.observable(""), text: "Série", visible: ko.observable(true) });
    this.Chave = PropertyEntity({ val: ko.observable(""), text: "Chave", visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário", idBtnSearch: guid() });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            loadGridNotaFiscalOrigem();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Cancelar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            limparLinhasSelecionadas();
            LimparCampos(_notasOrigem);
            Global.fecharModal("divModalBuscaNotasOrigemDevolucao");
        }, text: "Cancelar", idGrid: guid(), visible: ko.observable(true)
    });
}
//#endregion Classes

// #region Funções de Inicialização
function loadBuscaNotasFiscaisDeOrigem() {
    _notasOrigem = new NotaFiscalOrigemDevolucao();
    KoBindings(_notasOrigem, "knockoutNotaFiscalOrigemDevolucao", false, _notasOrigem.Pesquisar.id);

    loadGridNotaFiscalOrigem();

    BuscarClientes(_notasOrigem.Remetente);
    BuscarClientes(_notasOrigem.Destinatario);
}

function loadGridNotaFiscalOrigem() {
    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: null,
        callbackNaoSelecionado: null,
        callbackSelecionado: callbackSelecionado,
        callbackSelecionarTodos: null,
        somenteLeitura: false
    };

    _gridNotaFiscalOrigem = new GridView(_notasOrigem.Pesquisar.idGrid, "XMLNotaFiscal/PesquisaNotasFiscaisSaida", _notasOrigem, null, null, 10, null, null, null, multiplaEscolha);
    _gridNotaFiscalOrigem.CarregarGrid();
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos


function callbackSelecionado(argumentoNulo, registro) {
    var registrosSelecionados = _gridNotaFiscalOrigem.ObterMultiplosSelecionados();

    if (registrosSelecionados.length == 1) {
        _emitenteSelecionado = registro.CNPJEmitente;
    }

    if (registro.CNPJEmitente != _emitenteSelecionado) {
        document.getElementById(registro.Codigo.toString()).click();
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Os emitentes das notas selecionadas precisam ser iguais.")
        return;
    }
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

// #endregion Funções Públicas

// #region Funções Privadas
function limparLinhasSelecionadas() {
    var registrosSelecionados = _gridNotaFiscalOrigem.ObterMultiplosSelecionados();
    _emitenteSelecionado = "";

    for (var i = 0; i < registrosSelecionados.length; i++) {
        $('#' + registrosSelecionados[i].Codigo).removeClass('selected');
    }
}
// #endregion Funções Privadas