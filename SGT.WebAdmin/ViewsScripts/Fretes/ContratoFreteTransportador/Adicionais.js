/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _adicionais;

var Adicionais = function () {
    this.ContratoFreteTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.PercentualRota = PropertyEntity({ text: ko.observable("Percentual Rota:"), getType: typesKnockout.decimal, val: ko.observable(0), def: 0, enable: ko.observable(true) });
    this.QuantidadeEntregas = PropertyEntity({ text: ko.observable("Quantidade Entregas:"), getType: typesKnockout.int, val: ko.observable(0), def: ko.observable(0), enable: ko.observable(true), required: ko.observable(false)});
    this.CapacidadeOTM = PropertyEntity({ text: ko.observable("Capacidade OTM:"), val: ko.observable(EnumSimNao.Sim), options: ko.observable(EnumSimNao.obterOpcoes()), def: ko.observable(EnumSimNao.Sim), enable: ko.observable(true), required: ko.observable(false) });
    this.DominioOTM = PropertyEntity({ text: ko.observable("Dominio OTM:"), val: ko.observable(EnumDominioOTM.SAO), options: EnumDominioOTM.obterOpcoes(), def: EnumDominioOTM.SAO, enable: ko.observable(true), required: ko.observable(false) });
    this.PontoPlanejamentoTransporte = PropertyEntity({ text: ko.observable("Ponto planejamento transporte:"), val: ko.observable(EnumPontoPlanejamentoTransporte.BR01), options: ko.observable(EnumPontoPlanejamentoTransporte.obterOpcoes()), def: ko.observable(EnumPontoPlanejamentoTransporte.BR01), enable: ko.observable(true) , required: ko.observable(false)});
    this.TipoIntegracao = PropertyEntity({ text: ko.observable("Tipo Integração:"), getType: ko.observable(EnumTipoIntegracaoUnilever.OTM), options: EnumTipoIntegracaoUnilever.obterOpcoes(), def: ko.observable(EnumTipoIntegracaoUnilever.OTM), enable: ko.observable(true) , required: ko.observable(false)}); 
    this.IDExterno = PropertyEntity({ text: ko.observable("ID Externo:"), getType: typesKnockout.int, val: ko.observable(0), enable: false });
    this.StatusAceiteContrato = PropertyEntity({ text: ko.observable("Status Aceite Contrato:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: false });
    this.GrupoCarga = PropertyEntity({ text: ko.observable("Grupo de Carga:"), val: ko.observable(EnumTipoGrupoCarga.Nenhum), options: ko.observable(EnumTipoGrupoCarga.obterOpcoes()), def: ko.observable(EnumTipoGrupoCarga.Nenhum), enable: ko.observable(true), required: ko.observable(false) });
    this.GerenciarCapacidade = PropertyEntity({ text: ko.observable("Gerenciar Capacidade:"), val: ko.observable(EnumSimNao.Sim), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Sim, enable: ko.observable(true) });
}


//*******EVENTOS*******

function LoadAdicionais() {
    _adicionais = new Adicionais();
    KoBindings(_adicionais, "knockoutAdicionais");

    new BuscarStatusAssinaturaContrato(_adicionais.StatusAceiteContrato);
}

//*******MÉTODOS*******
function EditarAdicionais(data) {
    LimparCamposAdicionais();
    _adicionais.ContratoFreteTransportador.val(_contratoFreteTransportador.Codigo.val());
    PreencherObjetoKnout(_adicionais, { Data: data });
}

function LimparCamposAdicionais() {
    LimparCampos(_adicionais);
}
