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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="EtapaFatura.js" />
/// <reference path="FechamentoFatura.js" />
/// <reference path="Fatura.js" />
/// <reference path="CargaFatura.js" />
/// <reference path="IntegracaoFatura.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _cabecalhoFatura;

var CabecalhoFatura = function () {
    this.NumeroFatura = PropertyEntity({ text: "Número da Fatura: ", getType: typesKnockout.int, val: ko.observable(""), visible: true });
    this.DescricaoPeriodo = PropertyEntity({ text: "Período: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescricaoSituacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DataInicial = PropertyEntity({ text: "Data Inicio: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.NomePessoa = PropertyEntity({ text: "Pessoa: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.DescricaoGrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.MotivoCancelamento = PropertyEntity({ text: "Motivo do Cancelamento: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.PercentualProcessadoCancelamento = PropertyEntity({ val: ko.observable("0%"), def: "0%", text: ko.observable("Cancelando esta fatura"), visible: ko.observable(false) });

    this.LimparNovaFatura = PropertyEntity({ eventClick: LimparNovaFaturaClick, type: types.event, text: " Limpar / Nova Fatura ", visible: ko.observable(true), enable: ko.observable(true) });
};


//*******EVENTOS*******

function loadCabecalhoFatura() {
    _cabecalhoFatura = new CabecalhoFatura();
    KoBindings(_cabecalhoFatura, "knockoutCabecalhoFatura");
}

function LimparNovaFaturaClick(e, sender) {
    LimparCabecalhoFatura();
    LimparInegracaoFatura();
    LimparFechamentoFatura();
    limparCamposFatura();    
    LimparCargaFatura();
    LimparEtapaFatura();
}

//*******MÉTODOS*******

function LimparCabecalhoFatura() {
    LimparCampos(_cabecalhoFatura);
    $("#knockoutCabecalhoFatura").hide();
}


function CarregarDadosCabecalho(dados) {
    var dataCabecalho = { Data: dados };
    PreencherObjetoKnout(_cabecalhoFatura, dataCabecalho);

    _cabecalhoFatura.NomePessoa.visible(_cabecalhoFatura.NomePessoa.val() != "");
    _cabecalhoFatura.DescricaoGrupoPessoa.visible(_cabecalhoFatura.DescricaoGrupoPessoa.val() != "");

    _fatura.NumeroFatura.val(dataCabecalho.Data.NumeroFatura);
    _fatura.Codigo.val(dataCabecalho.Data.Codigo);

    if (dados.Situacao == EnumSituacoesFatura.Cancelado)
        _cabecalhoFatura.MotivoCancelamento.visible(true);
    else 
        _cabecalhoFatura.MotivoCancelamento.visible(false);

    $("#knockoutCabecalhoFatura").show();
}