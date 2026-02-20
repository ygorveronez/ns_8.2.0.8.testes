/// <reference path="Infracao.js" />
/// <reference path="../../Consultas/TipoInfracao.js" />
/// <reference path="../../Consultas/CargaCte.js" />
/// <reference path="../../Consultas/Seguradora.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoInfracao.js" />
/// <reference path="../../Enumeradores/EnumTipoInfracaoTransito.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dadosSinistro;

/*
 * Declaração das Classes
 */

var DadosSinistro = function () {

    //Dados Sinistro
    this.DataSinistro = PropertyEntity({ text: "*Data do Sinistro: ", getType: typesKnockout.date, required: false, enable: ko.observable(false), visible: ko.observable(true) });
    this.DataEmbarque = PropertyEntity({ text: "*Data do Embarque: ", getType: typesKnockout.date, required: false, enable: ko.observable(false), visible: ko.observable(true) });
    this.NumeroNotaFiscal = PropertyEntity({ val: ko.observable(""), def: "", text: "Nº Nota Fiscal Embarcador: ", enable: ko.observable(false), configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(true) });
    this.CargaCte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Emitente Nota Fiscal:", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário nota Fiscal:", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });
    this.Segurado = PropertyEntity({ val: ko.observable(""), def: false, enable: ko.observable(false), getType: typesKnockout.bool, text: "Estava segurado?" });
    this.LimpezaPista = PropertyEntity({ val: ko.observable(""), def: false, enable: ko.observable(false), getType: typesKnockout.bool, text: "Ocorreu limpeza da pista?" });
    this.Seguradora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Seguradora:", enable: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });
    this.ProdutoCarga = PropertyEntity({ text: "*Produto Carga:", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false), required: false, maxlength: 100, visible: ko.observable(true) });
    this.ValorNotaFiscal = PropertyEntity({ text: "Valor Nota Fiscal:", getType: typesKnockout.decimal, val: ko.observable(""), enable: ko.observable(false), visible: ko.observable(true) });
    this.ValorEstimadoPrejuizo = PropertyEntity({ text: "Valor Estimado do Prejuizo:", getType: typesKnockout.decimal, enable: ko.observable(false), val: ko.observable(""), visible: ko.observable(true) });
    this.ClassificacaoSinistro = PropertyEntity({ val: ko.observable(""), options: EnumClassificacaoSinistro.obterOpcoes(), enable: ko.observable(false), text: "Classificação Sinistro: ", visible: ko.observable(true) });
    this.CausaSinistro = PropertyEntity({ text: "Causa do Sinistro:", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false), maxlength: 500, visible: ko.observable(true) });

};


function loadDadosSinistro() {
    _dadosSinistro = new DadosSinistro();
    KoBindings(_dadosSinistro, "knockoutDadosSinistro");

    new BuscarCargaCTe(_dadosSinistro.CargaCte);
    new BuscarSeguradoras(_dadosSinistro.Seguradora);
    new BuscarCargas(_dadosSinistro.Carga);
    new BuscarClientes(_dadosSinistro.Emitente);
    new BuscarClientes(_dadosSinistro.Destinatario);
    loadAnexo();
}

function preecherDadosInfracao() {
    _dadosInfracao.DataSinistro.val(_dadosSinistro.DataSinistro.val());
    _dadosInfracao.DataEmbarque.val(_dadosSinistro.DataEmbarque.val());
    _dadosInfracao.NumeroNotaFiscal.val(_dadosSinistro.NumeroNotaFiscal.val());
    _dadosInfracao.Segurado.val(_dadosSinistro.Segurado.val());
    _dadosInfracao.LimpezaPista.val(_dadosSinistro.LimpezaPista.val());
    _dadosInfracao.CargaCte.val(_dadosSinistro.CargaCte.codEntity());
    _dadosInfracao.Emitente.val(_dadosSinistro.Emitente.codEntity());
    _dadosInfracao.Destinatario.val(_dadosSinistro.Destinatario.codEntity());
    _dadosInfracao.Seguradora.val(_dadosSinistro.Seguradora.codEntity());
    _dadosInfracao.Carga.val(_dadosSinistro.Carga.codEntity());
    _dadosInfracao.ProdutoCarga.val(_dadosSinistro.ProdutoCarga.val());
    _dadosInfracao.ValorNotaFiscal.val(_dadosSinistro.ValorNotaFiscal.val());
    _dadosInfracao.ValorEstimadoPrejuizo.val(_dadosSinistro.ValorEstimadoPrejuizo.val());
    _dadosInfracao.ClassificacaoSinistro.val(_dadosSinistro.ClassificacaoSinistro.val());
    _dadosInfracao.CausaSinistro.val(_dadosSinistro.CausaSinistro.val());
}

function limparCamposDadosSinistro() {
    LimparCampos(_dadosSinistro);
}