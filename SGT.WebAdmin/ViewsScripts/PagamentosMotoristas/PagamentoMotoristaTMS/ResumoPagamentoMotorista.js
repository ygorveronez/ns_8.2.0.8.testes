/// <reference path="../../../js/libs/jquery-2.1.1.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoMotorista.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="PagamentoMotoristaTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="EtapasPagamentoMotorista.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _resumoPagamentoMotorista;

var ResumoPagamentoMotorista = function () {

    this.NumeroPagamentoMotorista = PropertyEntity({ text: "Número Pagamento: ", visible: ko.observable(true) });
    this.NumeroCarga = PropertyEntity({ text: "Carga: ", visible: ko.observable(true) });
    this.DataPagamentoMotorista = PropertyEntity({ text: "Data do Pagamento: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.TipoPagamentoMotorista = PropertyEntity({ text: "Tipo do Pagamento: ", visible: ko.observable(true) });
    this.ValorPagamentoMotorista = PropertyEntity({ text: "Valor da Ocorrência: ", getType: typesKnockout.decimal, visible: ko.observable(true), visible: ko.observable(false) });
    this.Motorista = PropertyEntity({ text: "Motorista: ", visible: ko.observable(true) });

}


//*******EVENTOS*******

function loadResumoPagamentoMotorista() {
    _resumoPagamentoMotorista = new ResumoPagamentoMotorista();
    KoBindings(_resumoPagamentoMotorista, "knockoutResumoPagamentoMotorista");

}

//*******MÉTODOS*******

function preecherResumoPagamentoMotorista() {
    _resumoPagamentoMotorista.NumeroPagamentoMotorista.visible(true);
    _resumoPagamentoMotorista.ValorPagamentoMotorista.visible(true);

    _resumoPagamentoMotorista.NumeroPagamentoMotorista.val(_pagamentoMotorista.NumeroPagamentoMotorista.val());
    _resumoPagamentoMotorista.NumeroCarga.val(_pagamentoMotorista.Carga.val());
    _resumoPagamentoMotorista.DataPagamentoMotorista.val(_pagamentoMotorista.DataPagamentoMotorista.val());
    _resumoPagamentoMotorista.Situacao.val(_pagamentoMotorista.DescricaoSituacao.val());    
    _resumoPagamentoMotorista.TipoPagamentoMotorista.val(_pagamentoMotorista.TipoPagamentoMotorista.val());    
    _resumoPagamentoMotorista.ValorPagamentoMotorista.val(_pagamentoMotorista.Valor.val());    
    _resumoPagamentoMotorista.Motorista.val(_pagamentoMotorista.Motorista.val());
}

function limparResumoPagamentoMotorista() {
    _resumoPagamentoMotorista.NumeroPagamentoMotorista.visible(false);
    LimparCampos(_resumoPagamentoMotorista);
}