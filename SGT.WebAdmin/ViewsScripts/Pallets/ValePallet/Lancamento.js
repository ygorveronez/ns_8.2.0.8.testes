/// <reference path="ValePallet.js" />
/// <reference path="../../Consultas/DevolucaoPallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoValePallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _lancamento;

/*
 * Declaração das Classes
 */

var Lancamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });    
    this.Chamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Chamado:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Devolucao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Devolução:", required: true, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.string, enable: false });
    this.Representante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Representante:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Responsável:" });
    this.Quantidade = PropertyEntity({ text: "*Quantidade:", enable: ko.observable(true), required: true, getType: typesKnockout.int, enable: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadLancamento(knoutCarga) {
    _lancamento = new Lancamento();
    KoBindings(_lancamento, "knockoutLancamento");

    new BuscarDevolucaoSemValePallet(_lancamento.Devolucao, null, knoutCarga);
    new BuscarRepresentante(_lancamento.Representante);

    _valePallet.Codigo.val.subscribe(function (val) {
        _lancamento.Codigo.val(val);
    });
}

/*
 * Declaração das Funções
 */

function DadosLancamento(dados) {
    PreencherObjetoKnout(_lancamento, { Data: dados.Lancamento });

    var habilitar = dados.Situacao == EnumSituacaoValePallet.Todas;

    controlarCamposDadosLancamentoHabilitados(habilitar);
}

function controlarCamposDadosLancamentoHabilitados(habilitar) {
    _lancamento.Devolucao.enable(habilitar);
    _lancamento.Representante.enable(habilitar);
    _lancamento.Quantidade.enable(habilitar);
}

function LimparCamposDadosLancamento() {
    LimparCampos(_lancamento);

    var habilitar = true;

    controlarCamposDadosLancamentoHabilitados(habilitar);
}