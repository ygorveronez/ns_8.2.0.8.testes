/// <reference path="../../Enumeradores/EnumSituacaoFluxoCompra.js" />
/// <reference path="FluxoCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFluxoCompra;

var EtapaFluxoCompra = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("14%") });

    this.Etapa1 = PropertyEntity({
        text: "Requisição", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia o fluxo."),
        tooltipTitle: ko.observable("Requisição")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação da Requisição", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de aprovação da requisição."),
        tooltipTitle: ko.observable("Aprovação da Requisição")
    });
    this.Etapa3 = PropertyEntity({
        text: "Cotação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É a etapa de cotação."),
        tooltipTitle: ko.observable("Cotação")
    });
    this.Etapa4 = PropertyEntity({
        text: "Retorno da Cotação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("É a etapa de retorno da cotação."),
        tooltipTitle: ko.observable("Retorno da Cotação")
    });
    this.Etapa5 = PropertyEntity({
        text: "Ordem de Compra", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(5),
        tooltip: ko.observable("É a etapa de ordem de compra."),
        tooltipTitle: ko.observable("Ordem de Compra")
    });
    this.Etapa6 = PropertyEntity({
        text: "Aprovação Ordem de Compra", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(6),
        tooltip: ko.observable("É a etapa de aprovação da ordem de compra."),
        tooltipTitle: ko.observable("Aprovação Ordem de Compra")
    });
    this.Etapa7 = PropertyEntity({
        text: "Recebimento do Produto", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(7),
        tooltip: ko.observable("É a etapa de recebimento do produto."),
        tooltipTitle: ko.observable("Recebimento do Produto")
    });
};

//*******EVENTOS*******

function LoadEtapaFluxoCompra() {
    _etapaFluxoCompra = new EtapaFluxoCompra();
    KoBindings(_etapaFluxoCompra, "knockoutEtapaFluxoCompra");
    Etapa1LiberadaFluxoCompra();
}

function SetarEtapaInicioFluxoCompra() {
    DesabilitarTodasEtapasFluxoCompra();
    Etapa1LiberadaFluxoCompra();

    Global.ExibirStep(_etapaFluxoCompra.Etapa1.idTab);
}

function SetarEtapaFluxoCompra() {
    var etapaAtual = _fluxoCompra.EtapaAtual.val();
    var situacao = _fluxoCompra.Situacao.val();

    Etapa1AprovadaFluxoCompra();
    if (etapaAtual === EnumEtapaFluxoCompra.AprovacaoRequisicao) {
        if (situacao == EnumSituacaoFluxoCompra.Cancelado)
            Etapa2ReprovadaFluxoCompra();
        else
            Etapa2AguardandoFluxoCompra();
    }
    else if (etapaAtual === EnumEtapaFluxoCompra.Cotacao) {
        if (situacao == EnumSituacaoFluxoCompra.Cancelado)
            Etapa3ReprovadaFluxoCompra();
        else
            Etapa3AguardandoFluxoCompra();
    }
    else if (etapaAtual === EnumEtapaFluxoCompra.RetornoCotacao) {
        if (situacao == EnumSituacaoFluxoCompra.Cancelado)
            Etapa4ReprovadaFluxoCompra();
        else
            Etapa4AguardandoFluxoCompra();
    }
    else if (etapaAtual === EnumEtapaFluxoCompra.OrdemCompra) {
        if (situacao == EnumSituacaoFluxoCompra.Cancelado)
            Etapa5ReprovadaFluxoCompra();
        else
            Etapa5AguardandoFluxoCompra();
    }
    else if (etapaAtual === EnumEtapaFluxoCompra.AprovacaoOrdemCompra) {
        if (situacao == EnumSituacaoFluxoCompra.Cancelado)
            Etapa6ReprovadaFluxoCompra();
        else
            Etapa6AguardandoFluxoCompra();
    }
    else if (etapaAtual === EnumEtapaFluxoCompra.RecebimentoProduto) {
        if (situacao == EnumSituacaoFluxoCompra.Finalizado)
            Etapa7AprovadaFluxoCompra();
        else if (situacao == EnumSituacaoFluxoCompra.Cancelado)
            Etapa7ReprovadaFluxoCompra();
        else
            Etapa7AguardandoFluxoCompra();
    }
}

function DesabilitarTodasEtapasFluxoCompra() {
    Etapa2DesabilitadaFluxoCompra();
    Etapa3DesabilitadaFluxoCompra();
    Etapa4DesabilitadaFluxoCompra();
    Etapa5DesabilitadaFluxoCompra();
    Etapa6DesabilitadaFluxoCompra();
    Etapa7DesabilitadaFluxoCompra();
}

//*******Etapa 1*******

function Etapa1LiberadaFluxoCompra() {
    $("#" + _etapaFluxoCompra.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa1.idTab + " .step").attr("class", "step yellow");

    Etapa2DesabilitadaFluxoCompra();
}

function Etapa1AprovadaFluxoCompra() {
    $("#" + _etapaFluxoCompra.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2DesabilitadaFluxoCompra() {
    _etapaFluxoCompra.Etapa2.eventClick = function () { };
    $("#" + _etapaFluxoCompra.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapaFluxoCompra.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3DesabilitadaFluxoCompra();
}

function Etapa2LiberadaFluxoCompra() {
    _etapaFluxoCompra.Etapa2.eventClick = CarregarAprovacaoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1AprovadaFluxoCompra();
}

function Etapa2AguardandoFluxoCompra() {
    _etapaFluxoCompra.Etapa2.eventClick = CarregarAprovacaoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa2.idTab + " .step").attr("class", "step yellow");

    CarregarAprovacaoFluxoCompra();
    Global.ExibirStep(_etapaFluxoCompra.Etapa2.idTab);

    Etapa1AprovadaFluxoCompra();
}

function Etapa2AprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa2.eventClick = CarregarAprovacaoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1AprovadaFluxoCompra();
}

function Etapa2ReprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa2.eventClick = CarregarAprovacaoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1AprovadaFluxoCompra();
}

//*******Etapa 3*******

function Etapa3DesabilitadaFluxoCompra() {
    _etapaFluxoCompra.Etapa3.eventClick = function () { };
    $("#" + _etapaFluxoCompra.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapaFluxoCompra.Etapa3.idTab + " .step").attr("class", "step");
    Etapa4DesabilitadaFluxoCompra();
}

function Etapa3LiberadaFluxoCompra() {
    _etapaFluxoCompra.Etapa3.eventClick = CarregarCotacaoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2AprovadaFluxoCompra();
}

function Etapa3AguardandoFluxoCompra() {
    _etapaFluxoCompra.Etapa3.eventClick = CarregarCotacaoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa3.idTab + " .step").attr("class", "step yellow");

    CarregarCotacaoFluxoCompra();
    Global.ExibirStep(_etapaFluxoCompra.Etapa3.idTab);

    Etapa2AprovadaFluxoCompra();
}

function Etapa3AprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa3.eventClick = CarregarCotacaoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2AprovadaFluxoCompra();
}

function Etapa3ReprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa3.eventClick = CarregarCotacaoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2AprovadaFluxoCompra();
}

//*******Etapa 4*******

function Etapa4DesabilitadaFluxoCompra() {
    _etapaFluxoCompra.Etapa4.eventClick = function () { };
    $("#" + _etapaFluxoCompra.Etapa4.idTab).prop("disabled", true);
    $("#" + _etapaFluxoCompra.Etapa4.idTab + " .step").attr("class", "step");
    Etapa5DesabilitadaFluxoCompra();
}

function Etapa4LiberadaFluxoCompra() {
    _etapaFluxoCompra.Etapa4.eventClick = CarregarCotacaoRetornoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3AprovadaFluxoCompra();
}

function Etapa4AguardandoFluxoCompra() {
    _etapaFluxoCompra.Etapa4.eventClick = CarregarCotacaoRetornoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa4.idTab + " .step").attr("class", "step yellow");

    CarregarCotacaoRetornoFluxoCompra();
    Global.ExibirStep(_etapaFluxoCompra.Etapa4.idTab);

    Etapa3AprovadaFluxoCompra();
}

function Etapa4AprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa4.eventClick = CarregarCotacaoRetornoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa4.idTab + " .step").attr("class", "step green");
    Etapa3AprovadaFluxoCompra();
}

function Etapa4ReprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa4.eventClick = CarregarCotacaoRetornoFluxoCompra;
    $("#" + _etapaFluxoCompra.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa4.idTab + " .step").attr("class", "step red");
    Etapa3AprovadaFluxoCompra();
}

//*******Etapa 5*******

function Etapa5DesabilitadaFluxoCompra() {
    _etapaFluxoCompra.Etapa5.eventClick = function () { };
    $("#" + _etapaFluxoCompra.Etapa5.idTab).prop("disabled", true);
    $("#" + _etapaFluxoCompra.Etapa5.idTab + " .step").attr("class", "step");
    Etapa6DesabilitadaFluxoCompra();
}

function Etapa5LiberadaFluxoCompra() {
    _etapaFluxoCompra.Etapa5.eventClick = CarregarFluxoCompraOrdemCompra;
    $("#" + _etapaFluxoCompra.Etapa5.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa5.idTab + " .step").attr("class", "step yellow");
    Etapa4AprovadaFluxoCompra();
}

function Etapa5AguardandoFluxoCompra() {
    _etapaFluxoCompra.Etapa5.eventClick = CarregarFluxoCompraOrdemCompra;
    $("#" + _etapaFluxoCompra.Etapa5.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa5.idTab + " .step").attr("class", "step yellow");

    CarregarFluxoCompraOrdemCompra();
    Global.ExibirStep(_etapaFluxoCompra.Etapa5.idTab);

    Etapa4AprovadaFluxoCompra();
}

function Etapa5AprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa5.eventClick = CarregarFluxoCompraOrdemCompra;
    $("#" + _etapaFluxoCompra.Etapa5.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa5.idTab + " .step").attr("class", "step green");
    Etapa4AprovadaFluxoCompra();
}

function Etapa5ReprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa5.eventClick = CarregarFluxoCompraOrdemCompra;
    $("#" + _etapaFluxoCompra.Etapa5.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa5.idTab + " .step").attr("class", "step red");
    Etapa4AprovadaFluxoCompra();
}

//*******Etapa 6*******

function Etapa6DesabilitadaFluxoCompra() {
    _etapaFluxoCompra.Etapa6.eventClick = function () { };
    $("#" + _etapaFluxoCompra.Etapa6.idTab).prop("disabled", true);
    $("#" + _etapaFluxoCompra.Etapa6.idTab + " .step").attr("class", "step");
    Etapa7DesabilitadaFluxoCompra();
}

function Etapa6LiberadaFluxoCompra() {
    _etapaFluxoCompra.Etapa6.eventClick = CarregarAprovacaoOrdemCompra;
    $("#" + _etapaFluxoCompra.Etapa6.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa6.idTab + " .step").attr("class", "step yellow");
    Etapa5AprovadaFluxoCompra();
}

function Etapa6AguardandoFluxoCompra() {
    _etapaFluxoCompra.Etapa6.eventClick = CarregarAprovacaoOrdemCompra;
    $("#" + _etapaFluxoCompra.Etapa6.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa6.idTab + " .step").attr("class", "step yellow");

    CarregarAprovacaoOrdemCompra();
    Global.ExibirStep(_etapaFluxoCompra.Etapa6.idTab);

    Etapa5AprovadaFluxoCompra();
}

function Etapa6AprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa6.eventClick = CarregarAprovacaoOrdemCompra;
    $("#" + _etapaFluxoCompra.Etapa6.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa6.idTab + " .step").attr("class", "step green");
    Etapa5AprovadaFluxoCompra();
}

function Etapa6ReprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa6.eventClick = CarregarAprovacaoOrdemCompra;
    $("#" + _etapaFluxoCompra.Etapa6.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa6.idTab + " .step").attr("class", "step red");
    Etapa5AprovadaFluxoCompra();
}

//*******Etapa 7*******

function Etapa7DesabilitadaFluxoCompra() {
    _etapaFluxoCompra.Etapa7.eventClick = function () { };
    $("#" + _etapaFluxoCompra.Etapa7.idTab).prop("disabled", true);
    $("#" + _etapaFluxoCompra.Etapa7.idTab + " .step").attr("class", "step");
}

function Etapa7LiberadaFluxoCompra() {
    _etapaFluxoCompra.Etapa7.eventClick = CarregarRecebimentoProduto;
    $("#" + _etapaFluxoCompra.Etapa7.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa7.idTab + " .step").attr("class", "step yellow");
    Etapa6AprovadaFluxoCompra();
}

function Etapa7AguardandoFluxoCompra() {
    _etapaFluxoCompra.Etapa7.eventClick = CarregarRecebimentoProduto;
    $("#" + _etapaFluxoCompra.Etapa7.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa7.idTab + " .step").attr("class", "step yellow");

    CarregarRecebimentoProduto();
    Global.ExibirStep(_etapaFluxoCompra.Etapa7.idTab);

    Etapa6AprovadaFluxoCompra();
}

function Etapa7AprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa7.eventClick = CarregarRecebimentoProduto;
    $("#" + _etapaFluxoCompra.Etapa7.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa7.idTab + " .step").attr("class", "step green");

    CarregarRecebimentoProduto();
    Global.ExibirStep(_etapaFluxoCompra.Etapa7.idTab);

    Etapa6AprovadaFluxoCompra();
}

function Etapa7ReprovadaFluxoCompra() {
    _etapaFluxoCompra.Etapa7.eventClick = CarregarRecebimentoProduto;
    $("#" + _etapaFluxoCompra.Etapa7.idTab).prop("disabled", false);
    $("#" + _etapaFluxoCompra.Etapa7.idTab + " .step").attr("class", "step red");
    Etapa6AprovadaFluxoCompra();
}