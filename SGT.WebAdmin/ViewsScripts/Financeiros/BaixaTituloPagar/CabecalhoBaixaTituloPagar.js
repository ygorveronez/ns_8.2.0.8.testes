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
/// <reference path="EtapaBaixaTituloPagar.js" />
/// <reference path="BaixaTituloPagar.js" />
/// <reference path="NegociacaoBaixaTituloPagar.js" />
/// <reference path="IntegracaoBaixaTituloPagar.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cabecalhoBaixaTituloPagar;

var CabecalhoBaixaTituloPagar = function () {
    this.NumeroTitulo = PropertyEntity({ text: "Número do Título: ", getType: typesKnockout.int, val: ko.observable(""), visible: true });   
    this.DescricaoSituacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.NomePessoa = PropertyEntity({ text: "Pessoa: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.Detalhe = PropertyEntity({ text: "Observação/Detalhe: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.TituloDeAgrupamento = PropertyEntity({ text: ko.observable(""), getType: typesKnockout.string, val: ko.observable(""), visible: true });//Título gerado a partir de uma negociação
    this.LimparNovaBaixa = PropertyEntity({ eventClick: LimparNovaBaixaClick, type: types.event, text: " Limpar / Nova Baixa ", visible: ko.observable(true), enable: ko.observable(true) });
}


//*******EVENTOS*******

function LimparNovaBaixaClick(e, sender) {
    TipoPessoaTitulosPendentesChange();
    limparCamposBaixaTituloPagar();
    LimparCamposNegociacao();
    LimparCamposIntegracao();
    LimparOcultarAbas();
    LimparCampos(_cabecalhoBaixaTituloPagar);
    HabilitarTodosBotoes(true);

    $('#divParcelasNegociacal').show();

    _baixaTituloPagar.CancelarBaixa.visible(false);
    _baixaTituloPagar.SalvarObservacao.visible(false);

    Global.ResetarSteps();
}

function loadCabecalhoBaixaTituloPagar() {
    _cabecalhoBaixaTituloPagar = new CabecalhoBaixaTituloPagar();
    KoBindings(_cabecalhoBaixaTituloPagar, "knockoutCabecalhoTituloPagar");
}

//*******MÉTODOS*******

function CarregarDadosCabecalho(dados) {
    var dataCabecalho = { Data: dados };
    PreencherObjetoKnout(_cabecalhoBaixaTituloPagar, dataCabecalho);

    _baixaTituloPagar.Codigo.val(dataCabecalho.Data.Codigo);

    $("#knockoutCabecalhoTituloPagar").show();
}