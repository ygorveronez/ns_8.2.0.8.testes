/// <reference path="SolicitacaoAvaria.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoLancamentoNFSManual.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLEtapasFluxoColetaEntrega;

var EtapaFluxoColetaEntrega = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.int, eventClick: ObterDetalhesCargaFluxoClick });
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });
    this.NumeroCarregamento = PropertyEntity({});
    this.SituacaoEtapaFluxoColetaEntrega = PropertyEntity({});
    this.EtapaAtual = PropertyEntity({});
    this.Placas = PropertyEntity({});
    this.Destinatario = PropertyEntity({ visible: false });
    this.Remetente = PropertyEntity({});

    this.Agendamento = PropertyEntity({});
    this.NumeroPedido = PropertyEntity({});
    this.SenhaCarregamento = PropertyEntity({});
    this.NumeroPedidoCliente = PropertyEntity({});
    this.Motorista = PropertyEntity({});
    this.Coleta = PropertyEntity({});
    this.NumeroSM = PropertyEntity({});
    this.SituacaoChamado = PropertyEntity({ visible: ko.observable(false)} );
    this.CodigoCD = PropertyEntity({});
    
    this.DataAgSenha = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataAgPendenciaAlocarVeiculo = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataVeiculoAlocado = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataSaidaCD = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataIntegracao = PropertyEntity({ text: "Data Integração: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataChegadaFornecedor = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataEmissaoCTe = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataEmissaoMDFe = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataEmissaoCTeSubContratacao = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataSaidaFornecedor = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataChegadaCD = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataAgOcorrencia = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataFinalizacao = PropertyEntity({ text: "Data Ag Senha: ", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.Auditar = PropertyEntity({ eventClick: auditarFluxoColetaEntregaClick, type: types.event, text: "Auditar" });

    this.Placas = PropertyEntity({});

    this.EtapaAgSenha = PropertyEntity({
        text: "Aguardando Senha GA", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesAgSenhaFluxoColetaEntrega,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.AgSenha),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Aguardando Senha GA")
    });
    this.EtapaAgPendenciaAlocarVeiculo = PropertyEntity({
        text: "Pendencia Alocar Veículo", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesAgPendenciaAlocarVeiculoFluxoColetaEntrega,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.PendenciaAlocarVeiculo),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Pendencia Alocar Veículo")
    });
    this.EtapaVeiculoAlocado = PropertyEntity({
        text: "Veículo Alocado", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesVeiculoAlocadoFluxoColetaEntrega,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.VeiculoAlocado),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Veículo Alocado")
    });
    this.EtapaSaidaCD = PropertyEntity({
        text: "Saída da CD", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesSaidaCDFluxoColetaEntrega,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.SaidaCD),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Saída da CD")
    });
    this.EtapaIntegracao = PropertyEntity({
        text: "SM", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesEtapaIntegracao,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.Integracao),
        tooltip: ko.observable("Etapa de integração para solicitação de monitoriamento"),
        tooltipTitle: ko.observable("SM")
    });
    this.EtapaChegadaFornecedor = PropertyEntity({
        text: "Chegada ao Fornecedor", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesChegadaFornecedorFluxoColetaEntrega,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.ChegadaFornecedor),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Chegada ao Fornecedor")
    });
    this.EtapaEmissaoCTe = PropertyEntity({
        text: "CT-e", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesDocaCarregamentoFluxoPatio,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.EtapaEmissaoCTe),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("CT-e")
    });
    this.EtapaEmissaoMDFe = PropertyEntity({
        text: "MDF-e", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesDocaCarregamentoFluxoPatio,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.EtapaEmissaoMDFe),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("MDF-e")
    });
    this.EtapaEmissaoCTeSubContratacao = PropertyEntity({
        text: "CT-e Subcontratação", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesDocaCarregamentoFluxoPatio,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.CTeSubcontratacao),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("CT-e Subcontratação")
    });

    this.EtapaSaidaFornecedor = PropertyEntity({
        text: "Saída Fornecedor", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesSaidaFornecedorFluxoColetaEntrega,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.SaidaFornecedor),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Saída Fornecedor")
    });

    this.EtapaChegadaCD = PropertyEntity({
        text: "Chegada a CD", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesChegadaCDFluxoColetaEntrega,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.ChegadaCD),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Chegada a CD")
    });

    this.EtapaAgOcorrencia = PropertyEntity({
        text: "Na CD com Ocorrência", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesDocaCarregamentoFluxoPatio,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.Ocorrencia),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Na CD com Ocorrência")
    });

    this.EtapaFinalizacao = PropertyEntity({
        text: "Processo Finalizado", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        backEventClick: ExibirDetalhesProcessoFinalizadoFluxoColetaEntrega,
        cssClass: ko.observable("step"),
        step: ko.observable(EnumEtapaFluxoColetaEntrega.Finalizado),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Processo Finalizado")
    });
}

function ExibirDetalhesDocaCarregamentoFluxoPatio() {

}


//*******EVENTOS*******

function loadEtapasFluxoGestao(callback) {
    $.get("Content/Static/Carga/FluxoColetaEntrega.html?dyn=" + guid(), function (data) {
        _HTMLEtapasFluxoColetaEntrega = data;
        callback();
    });
}

function SetarEtapaFluxoDesabilitada(etapa) {
    etapa.cssClass("step");
    etapa.eventClick = function () { };
}

function SetarEtapaFluxoAprovada(etapa) {
    etapa.cssClass("step green");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoAguardando(etapa) {
    etapa.cssClass("step yellow");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoProblema(etapa) {
    etapa.cssClass("step red");
    etapa.eventClick = etapa.backEventClick;

}

function SetarEtapaFluxoLiberada(etapa) {
    etapa.cssClass("step yellow");
    etapa.eventClick = etapa.backEventClick;
}
