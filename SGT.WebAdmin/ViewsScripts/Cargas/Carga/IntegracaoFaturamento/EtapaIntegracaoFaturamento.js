var _HTMLDadosFaturamento = "";
var _integracaoFaturamento;
var _gridDadosFaturamento;

var EtapaIntegracaoFaturamento = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

};

function buscarDadosIntegracaoFaturamentoClick(e) {
    //executarReST("CargaIntegracao/ObterDadosIntegracoes", { Carga: e.Codigo.val() }, function (r) {
    //    if (r.Success) {
    //        _cargaAtual = e;

    //        if (r.Data.TiposIntegracoesCTe.length > 0 || r.Data.TiposIntegracoesEDI.length > 0 || r.Data.TiposIntegracoesCarga.length > 0) {
    $("#" + e.EtapaIntegracaoFaturamento.idGrid).html(_HTMLIntegracaoFaturamento.replace(/\b#divIntegracaoFaturamento\b/g, e.EtapaIntegracaoFaturamento.idGrid));

    _integracaoFaturamento = new EtapaIntegracaoFaturamento();
    _integracaoFaturamento.Carga.val(e.Codigo.val());

    //KoBindings(_integracaoFaturamento, "divIntegracaoFaturamento_" + e.EtapaIntegracaoFaturamento.idGrid);

    LocalizeCurrentPage();
    LoadIntegracaoFaturamentoEDI(e, "divIntegracaoFaturamentoEDI_" + e.EtapaIntegracaoFaturamento.idGrid);
    LoadIntegracaoFaturamentoFatura(e, "divIntegracaoFaturamentoFatura_" + e.EtapaIntegracaoFaturamento.idGrid);
    //        }
    //        else {
    //            $("#" + e.EtapaIntegracaoFaturamento.idGrid + " .divIntegracaoFaturamentoFatura").html('<p class="alert alert-success">' + Localization.Resources.Cargas.Carga.NaoExistemIntegracoesDisponiveisParaEstaCarga + '</p>');
    //        }
    //    } else {
    //        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
    //    }
    //});
}

//*******ETAPA*******

function EtapaIntegracaoFaturamentoDesabilitada(e) {
    $("#" + e.EtapaIntegracaoFaturamento.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaIntegracaoFaturamento.idTab + " .step").attr("class", "step");
    e.EtapaIntegracaoFaturamento.eventClick = function (e) { buscarDadosIntegracaoFaturamentoClick(e) };
}

function EtapaIntegracaoFaturamentoLiberada(e) {
    $("#" + e.EtapaIntegracaoFaturamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracaoFaturamento.idTab + " .step").attr("class", "step yellow");
    e.EtapaIntegracaoFaturamento.eventClick = function (e) { buscarDadosIntegracaoFaturamentoClick(e) };
}

function EtapaIntegracaoFaturamentoProblema(e) {
    $("#" + e.EtapaIntegracaoFaturamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracaoFaturamento.idTab + " .step").attr("class", "step red");
    e.EtapaIntegracaoFaturamento.eventClick = function (e) { buscarDadosIntegracaoFaturamentoClick(e) };
}

function EtapaIntegracaoFaturamentoAguardando(e) {
    $("#" + e.EtapaIntegracaoFaturamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracaoFaturamento.idTab + " .step").attr("class", "step yellow");
    e.EtapaIntegracaoFaturamento.eventClick = function (e) { buscarDadosIntegracaoFaturamentoClick(e) };
}

function EtapaIntegracaoFaturamentoAprovada(e) {
    $("#" + e.EtapaIntegracaoFaturamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracaoFaturamento.idTab + " .step").attr("class", "step green");
    e.EtapaIntegracaoFaturamento.eventClick = function (e) { buscarDadosIntegracaoFaturamentoClick(e) };
}