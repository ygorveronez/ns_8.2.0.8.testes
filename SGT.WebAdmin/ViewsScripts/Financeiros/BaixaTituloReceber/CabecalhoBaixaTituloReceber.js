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
/// <reference path="EtapaBaixaTituloReceber.js" />
/// <reference path="BaixaTituloReceber.js" />
/// <reference path="NegociacaoBaixaTituloReceber.js" />
/// <reference path="IntegracaoBaixaTituloReceber.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cabecalhoBaixaTituloReceber;

var CabecalhoBaixaTituloReceber = function () {
    this.NumeroTitulo = PropertyEntity({ text: "Número do Título: ", getType: typesKnockout.int, val: ko.observable(""), visible: true });
    this.NumeroFatura = PropertyEntity({ text: "Número da Fatura: ", getType: typesKnockout.int, val: ko.observable(""), visible: true });
    this.DescricaoSituacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TituloDeAgrupamento = PropertyEntity({ text: ko.observable(""), getType: typesKnockout.string, val: ko.observable(""), visible: true });//Título gerado a partir de uma negociação

    this.NomePessoa = PropertyEntity({ text: "Pessoa: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.DescricaoGrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.LimparNovaBaixa = PropertyEntity({ eventClick: LimparNovaBaixaClick, type: types.event, text: " Limpar / Nova Baixa ", visible: ko.observable(true), enable: ko.observable(true) });
}


//*******EVENTOS*******

function loadCabecalhoBaixaTituloReceber() {
    _cabecalhoBaixaTituloReceber = new CabecalhoBaixaTituloReceber();
    KoBindings(_cabecalhoBaixaTituloReceber, "knockoutCabecalhoTituloReceber");
}

function LimparNovaBaixaClick(e, sender) {
    limparCamposBaixaTituloReceber();
    LimparCamposNegociacao();
    LimparCamposIntegracao();
    LimparOcultarAbas();
    LimparCampos(_cabecalhoBaixaTituloReceber);
    $('#divParcelasNegociacal').show();

    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step lightgreen");

    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step");

    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step");

    _baixaTituloReceber.CancelarBaixa.visible(false);
    _baixaTituloReceber.SalvarObservacao.visible(false);

    Global.ExibirStep(_etapaBaixaTituloReceber.Etapa1.idTab);
    etapaQuitacaoClick();

    $("#knockoutCabecalhoTituloReceber").hide();
}

//*******MÉTODOS*******

function CarregarDadosCabecalho(dados) {
    var dataCabecalho = { Data: dados };
    PreencherObjetoKnout(_cabecalhoBaixaTituloReceber, dataCabecalho);

    _cabecalhoBaixaTituloReceber.NomePessoa.visible(_cabecalhoBaixaTituloReceber.NomePessoa.val() != "");
    _cabecalhoBaixaTituloReceber.DescricaoGrupoPessoa.visible(_cabecalhoBaixaTituloReceber.DescricaoGrupoPessoa.val() != "");

    $("#divSelecaoPessoa").attr("class", "col-12 col-md-4");
    $("#divSelecaoGrupoPessoa").attr("class", "col-12 col-md-4");

    _baixaTituloReceber.Codigo.val(dataCabecalho.Data.Codigo);

    $("#knockoutCabecalhoTituloReceber").show();
}