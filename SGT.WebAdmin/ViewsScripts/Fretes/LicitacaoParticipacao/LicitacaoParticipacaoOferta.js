/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLicitacaoParticipacao.js" />
/// <reference path="LicitacaoParticipacaoCadastro.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _licitacaoParticipacaoOferta;
var _tabelaFreteCliente;
var _tabelaFrete;
var _opcoesOferta = [];
var _opcaoSelecionada = false;

/*
 * Declaração das Classes
 */

var LicitacaoParticipacaoOferta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TabelaFrete = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.OpcoesOferta = PropertyEntity({ val: ko.observable(0), options: ko.observable(_opcoesOferta), text: "Tabela Frete: ", enable: ko.observable(true) });

    this.OpcoesOferta.val.subscribe(function () {
        if (_licitacaoParticipacaoOferta.OpcoesOferta.val() > 0 && _opcaoSelecionada)
            selecionarTabelaFrete();
    });
}

var TabelaFreteCliente = function () {
    this.CodigoLicitacaoParticipacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    
    this.Valores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.Observacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.ValoresMinimosGarantidos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.ValoresMaximos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.ValoresBases = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.ValoresExcedentes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.PercentuaisPagamentoAgregados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
}

/*
 * Declaração das Funções de Inicialização
 */


function loadLicitacaoParticipacaoOferta() {
    _licitacaoParticipacaoOferta = new LicitacaoParticipacaoOferta();
    _tabelaFreteCliente = new TabelaFreteCliente();
    loadValores();
    KoBindings(_licitacaoParticipacaoOferta, "knockoutLicitacaoParticipacaoOferta");
}

/*
 * Declaração das Funções
 */

function preencherOpcoesOferta(codigoSelecionado) {
    _opcoesOferta = [];

    executarReST("LicitacaoParticipacao/PesquisaOferta", RetornarObjetoPesquisa(_licitacaoParticipacaoOferta), function (retorno) {
        if (retorno.Data) {
            for (var i = 0; i < retorno.Data.ListaOferta.length; i++) {
                var novaOpcao = { value: retorno.Data.ListaOferta[i].Codigo, text: retorno.Data.ListaOferta[i].Descricao };
                _opcoesOferta.push(novaOpcao);
            }
        }
        _licitacaoParticipacaoOferta.OpcoesOferta.options(_opcoesOferta);

        if (_opcoesOferta.length > 0) {
            _opcaoSelecionada = true;
            _licitacaoParticipacaoOferta.OpcoesOferta.val(codigoSelecionado > 0 ? codigoSelecionado : _opcoesOferta[0].value);
            selecionarTabelaFrete();
        }
    });
}

function preencherOferta(dadosOferta) {
    if (dadosOferta)
        PreencherObjetoKnout(_licitacaoParticipacaoOferta, { Data: dadosOferta });
}

function limparOferta() {
    _opcaoSelecionada = false;
    LimparCampos(_licitacaoParticipacaoOferta);
    LimparCampos(_tabelaFreteCliente);
    $("#divValoresTabelaFrete").hide();
}

function selecionarTabelaFrete() {
    if (_licitacaoParticipacaoOferta.OpcoesOferta.val() == 0) {
        return;
    }

    _tabelaFreteCliente.Codigo.val(_licitacaoParticipacaoOferta.OpcoesOferta.val());

    BuscarPorCodigo(_tabelaFreteCliente, "TabelaFreteCliente/BuscarPorCodigo", function (arg) {
        buscarDadosTabelaFrete();
    }, null);
}

function buscarDadosTabelaFrete() {
    executarReST("TabelaFrete/BuscarPorCodigo", { Codigo: _licitacaoParticipacaoOferta.TabelaFrete.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _tabelaFrete = arg.Data;
                montarTabelaValores();
            }
        }
    });
}
