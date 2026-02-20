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
/// <reference path="../../Enumeradores/EnumBoletoAlteracaoEtapa.js" />
/// <reference path="BoletoAlteracao.js" />
/// <reference path="AlteracaoBoletoAlteracao.js" />
/// <reference path="ImpressaoBoletoAlteracao.js" />
/// <reference path="EmailBoletoAlteracao.js" />
/// <reference path="RemessaBoletoAlteracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaBoletoAlteracao;
var _etapaAtual;

var EtapaBoletoAlteracao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("20%") });

    this.Etapa1 = PropertyEntity({
        text: "Seleção de Boletos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaSelecaoClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde seleciona os boletos a serem alterados o seu vencimento."),
        tooltipTitle: ko.observable("Seleção de Boletos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Alteração de Dados", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaAlteracaoDadosClick,
        step: ko.observable(2),
        tooltip: ko.observable("Altere a data de vencimento e observação do boleto."),
        tooltipTitle: ko.observable("Alteração de Dados")
    });
    this.Etapa3 = PropertyEntity({
        text: "Impressão", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaImpressaoClick,
        step: ko.observable(3),
        tooltip: ko.observable("Realize o download do novo pdf do boleto."),
        tooltipTitle: ko.observable("Impressão")
    });
    this.Etapa4 = PropertyEntity({
        text: "Gerar Remessa", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaRemessaClick,
        step: ko.observable(4),
        tooltip: ko.observable("Gere a remessa de alteração de dados para envio ao banco."),
        tooltipTitle: ko.observable("Gerar Remessa")
    });
    this.Etapa5 = PropertyEntity({
        text: "E-mail", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaEmailClick,
        step: ko.observable(5),
        tooltip: ko.observable("Envie os novos boletos por e-mail aos clientes."),
        tooltipTitle: ko.observable("E-mail")
    });
}


//*******EVENTOS*******

function loadEtapaBoletoAlteracao() {
    _etapaBoletoAlteracao = new EtapaBoletoAlteracao();
    KoBindings(_etapaBoletoAlteracao, "knockoutEtapaBoletoAlteracao");

    $("#" + _etapaBoletoAlteracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoAlteracao.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaBoletoAlteracao.Etapa1.idTab).click();
    VerificarBotoes();
}

function etapaSelecaoClick(e, sender) {
    _etapaAtual = 1;
    VerificarBotoes();
}

function etapaAlteracaoDadosClick(e, sender) {
    _etapaAtual = 2;
    BuscarBoletosAlteracao();
    VerificarBotoes();
}

function etapaImpressaoClick(e, sender) {
    _etapaAtual = 3;
    BuscarBoletosImpressao();
    VerificarBotoes();
}

function etapaRemessaClick(e, sender) {
    _etapaAtual = 4;
    BuscarBoletosRemessa();
    VerificarBotoes();
}

function etapaEmailClick(e, sender) {
    _etapaAtual = 5;
    BuscarBoletosEmail();
    VerificarBotoes();
}
//*******MÉTODOS*******

function LimparOcultarAbas() {
    $("#" + _etapaBoletoAlteracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoAlteracao.Etapa1.idTab + " .step").attr("class", "step");

    $("#" + _etapaBoletoAlteracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoAlteracao.Etapa2.idTab + " .step").attr("class", "step");

    $("#" + _etapaBoletoAlteracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoAlteracao.Etapa3.idTab + " .step").attr("class", "step");

    $("#" + _etapaBoletoAlteracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoAlteracao.Etapa4.idTab + " .step").attr("class", "step");

    $("#" + _etapaBoletoAlteracao.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoAlteracao.Etapa5.idTab + " .step").attr("class", "step");
}

function PosicionarEtapa(dado) {
    LimparOcultarAbas();

    if (dado.Etapa == EnumBoletoAlteracaoEtapa.Selecao) {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").hide();
        $("#knockoutImpressaoBoletoAlteracao").hide();
        $("#knockoutRemessaBoletoAlteracao").hide();
        $("#knockoutEmailBoletoAlteracao").hide();

        $("#" + _etapaBoletoAlteracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa1.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa2.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa3.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa4.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa5.idTab + " .step").attr("class", "step lightgreen");

        //$("#" + _etapaBoletoAlteracao.Etapa1.idTab).click();
        Global.ExibirStep(_etapaBoletoAlteracao.Etapa1.idTab);

    } else if (dado.Etapa == EnumBoletoAlteracaoEtapa.Alteracao) {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").show();
        $("#knockoutImpressaoBoletoAlteracao").hide();
        $("#knockoutRemessaBoletoAlteracao").hide();
        $("#knockoutEmailBoletoAlteracao").hide();

        $("#" + _etapaBoletoAlteracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa2.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa3.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa4.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa5.idTab + " .step").attr("class", "step lightgreen");

        //$("#" + _etapaBoletoAlteracao.Etapa2.idTab).click();
        Global.ExibirStep(_etapaBoletoAlteracao.Etapa2.idTab);

    } else if (dado.Etapa == EnumBoletoAlteracaoEtapa.Impressao) {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").show();
        $("#knockoutImpressaoBoletoAlteracao").show();
        $("#knockoutRemessaBoletoAlteracao").hide();
        $("#knockoutEmailBoletoAlteracao").hide();

        $("#" + _etapaBoletoAlteracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa3.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa4.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa5.idTab + " .step").attr("class", "step lightgreen");

        //$("#" + _etapaBoletoAlteracao.Etapa3.idTab).click();
        Global.ExibirStep(_etapaBoletoAlteracao.Etapa3.idTab);

    } else if (dado.Etapa == EnumBoletoAlteracaoEtapa.Remessa) {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").show();
        $("#knockoutImpressaoBoletoAlteracao").show();
        $("#knockoutRemessaBoletoAlteracao").show();
        $("#knockoutEmailBoletoAlteracao").hide();

        $("#" + _etapaBoletoAlteracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa3.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa4.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBoletoAlteracao.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa5.idTab + " .step").attr("class", "step lightgreen");

        //$("#" + _etapaBoletoAlteracao.Etapa4.idTab).click();
        Global.ExibirStep(_etapaBoletoAlteracao.Etapa4.idTab);
    } else if (dado.Etapa == EnumBoletoAlteracaoEtapa.Email) {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").show();
        $("#knockoutImpressaoBoletoAlteracao").show();
        $("#knockoutRemessaBoletoAlteracao").show();
        $("#knockoutEmailBoletoAlteracao").show();

        $("#" + _etapaBoletoAlteracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa3.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa4.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoAlteracao.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoAlteracao.Etapa5.idTab + " .step").attr("class", "step lightgreen");

        //$("#" + _etapaBoletoAlteracao.Etapa5.idTab).click();
        Global.ExibirStep(_etapaBoletoAlteracao.Etapa5.idTab);
    }

    VerificarBotoes();
}

function VerificarBotoes() {
    if (_boletoAlteracao.Etapa.val() == EnumBoletoAlteracaoEtapa.Selecao) {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").hide();
        $("#knockoutImpressaoBoletoAlteracao").hide();
        $("#knockoutRemessaBoletoAlteracao").hide();
        $("#knockoutEmailBoletoAlteracao").hide();
    } else if (_boletoAlteracao.Etapa.val() == EnumBoletoAlteracaoEtapa.Alteracao) {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").show();
        $("#knockoutImpressaoBoletoAlteracao").hide();
        $("#knockoutRemessaBoletoAlteracao").hide();
        $("#knockoutEmailBoletoAlteracao").hide();
    } else if (_boletoAlteracao.Etapa.val() == EnumBoletoAlteracaoEtapa.Impressao) {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").show();
        $("#knockoutImpressaoBoletoAlteracao").show();
        $("#knockoutRemessaBoletoAlteracao").hide();
        $("#knockoutEmailBoletoAlteracao").hide();
    } else if (_boletoAlteracao.Etapa.val() == EnumBoletoAlteracaoEtapa.Remessa) {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").show();
        $("#knockoutImpressaoBoletoAlteracao").show();
        $("#knockoutRemessaBoletoAlteracao").show();
        $("#knockoutEmailBoletoAlteracao").hide();
    }
    else {
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").show();
        $("#knockoutImpressaoBoletoAlteracao").show();
        $("#knockoutRemessaBoletoAlteracao").show();
        $("#knockoutEmailBoletoAlteracao").show();
    }
}