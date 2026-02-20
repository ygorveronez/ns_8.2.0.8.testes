/// <reference path="IntegracaoBaixaTituloReceber.js" />
/// <reference path="../../Enumeradores/EnumBaixaTituloReceber.js" />
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
/// <reference path="CabecalhoBaixaTituloReceber.js" />
/// <reference path="NegociacaoBaixaTituloReceber.js" />
/// <reference path="BaixaTituloReceber.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaBaixaTituloReceber;
var _etapaAtual;

var EtapaBaixaTituloReceber = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("30%") });

    this.Etapa1 = PropertyEntity({
        text: "Quitação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(),
        eventClick: etapaQuitacaoClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia a baixa de um título a receber."),
        tooltipTitle: ko.observable("Quitação")
    });
    this.Etapa2 = PropertyEntity({
        text: "Negociação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(),
        eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Negocie o saldo devedor aplicando desconto e/ou acréscimos."),
        tooltipTitle: ko.observable("Negociação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(),
        eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Envie as informações da baixa do título e suas negociações."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapaBaixaTituloReceber() {
    _etapaBaixaTituloReceber = new EtapaBaixaTituloReceber();
    KoBindings(_etapaBaixaTituloReceber, "knockoutEtapaTituloReceber");

    Etapa1Liberada();
}

function etapaQuitacaoClick(e, sender) {
    _etapaAtual = 1;
    VerificarBotoes();
}

function etapaNegociacaoClick(e, sender) {
    _etapaAtual = 2;
    CarregarNegociacaoBaixa();
    VerificarBotoes();
    CarregarDocumentosNegociacaoBaixaTituloReceber();
}

function etapaIntegracaoClick(e, sender) {
    _etapaAtual = 3;
    CarregarIntegracaoBaixa();
    VerificarBotoes();
}

//***ETAPA 1***

function Etapa1Liberada() {
    _etapaBaixaTituloReceber.Etapa1.eventClick = etapaQuitacaoClick;

    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step lightgreen");

    Etapa2Desabilitada();
}

function Etapa1Aguardando() {
    _etapaBaixaTituloReceber.Etapa1.eventClick = etapaQuitacaoClick;

    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step yellow");

    Etapa2Desabilitada();
}

function Etapa1Sucesso() {
    _etapaBaixaTituloReceber.Etapa1.eventClick = etapaQuitacaoClick;

    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step green");
}

//***ETAPA 2***

function Etapa2Sucesso() {
    _etapaBaixaTituloReceber.Etapa2.eventClick = etapaNegociacaoClick;

    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step green");

    Etapa1Sucesso();
}

function Etapa2Liberada() {
    _etapaBaixaTituloReceber.Etapa2.eventClick = etapaNegociacaoClick;

    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step lightgreen");

    Etapa1Sucesso();
}

function Etapa2Aguardando() {
    _etapaBaixaTituloReceber.Etapa2.eventClick = etapaNegociacaoClick;

    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step yellow");

    Etapa1Sucesso();
    Etapa3Desabilitada();
}

function Etapa2Desabilitada() {
    _etapaBaixaTituloReceber.Etapa2.eventClick = function () { };

    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step");

    Etapa3Desabilitada();
}

//***ETAPA 3***

function Etapa3Sucesso() {
    _etapaBaixaTituloReceber.Etapa3.eventClick = etapaIntegracaoClick;

    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step green");

    Etapa2Sucesso();
}

function Etapa3Liberada() {
    _etapaBaixaTituloReceber.Etapa3.eventClick = etapaIntegracaoClick;

    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step lightgreen");

    Etapa2Sucesso();
}

function Etapa3Aguardando() {
    _etapaBaixaTituloReceber.Etapa3.eventClick = etapaIntegracaoClick;

    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step disabled");

    Etapa2Sucesso();
}

function Etapa3Desabilitada() {
    _etapaBaixaTituloReceber.Etapa3.eventClick = function () { };

    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step");
}

//*******MÉTODOS*******

function LimparOcultarAbas() {
    Etapa1Liberada();
}

function PosicionarEtapa(dado) {
    LimparOcultarAbas();

    _baixaTituloReceber.PercentualProcessadoGeracao.visible(false);
    _progressNegociacaoBaixaReceber.PercentualProcessadoFinalizacao.visible(false);

    if (dado.Etapa == EnumEtapasBaixaTituloReceber.EmGeracao) {
        Etapa1Aguardando();

        _baixaTituloReceber.PercentualProcessadoGeracao.visible(true);
        _baixaTituloReceber.CancelarBaixa.visible(false);
        _baixaTituloReceber.SalvarObservacao.visible(false);
        _baixaTituloReceber.SalvarDatas.visible(false);

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa1.idTab);
    } else if (dado.Etapa == EnumEtapasBaixaTituloReceber.Iniciada && _baixaTituloReceber.Codigo.val() == 0) {
        Etapa1Liberada();

        _baixaTituloReceber.CancelarBaixa.visible(false);
        _baixaTituloReceber.SalvarObservacao.visible(false);
        _baixaTituloReceber.SalvarDatas.visible(false);

        etapaQuitacaoClick();

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa1.idTab);

    } else if (dado.Etapa == EnumEtapasBaixaTituloReceber.Iniciada) {
        Etapa2Liberada();

        _baixaTituloReceber.CancelarBaixa.visible(true);
        _baixaTituloReceber.SalvarObservacao.visible(true);
        _baixaTituloReceber.SalvarDatas.visible(true);

        etapaNegociacaoClick();

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa2.idTab);

    } else if (dado.Etapa == EnumEtapasBaixaTituloReceber.EmNegociacao) {
        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step lightgreen");

        _baixaTituloReceber.CancelarBaixa.visible(true);
        _baixaTituloReceber.SalvarObservacao.visible(true);
        _baixaTituloReceber.SalvarDatas.visible(true);

        etapaIntegracaoClick();

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa3.idTab);

    } else if (dado.Etapa == EnumEtapasBaixaTituloReceber.Cancelada) {
        Etapa3Sucesso();

        _baixaTituloReceber.CancelarBaixa.visible(false);
        _baixaTituloReceber.SalvarObservacao.visible(false);
        _baixaTituloReceber.SalvarDatas.visible(false);

        etapaIntegracaoClick();

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa1.idTab);
    } else if (dado.Etapa == EnumEtapasBaixaTituloReceber.EmFinalizacao) {
        Etapa2Aguardando();

        _progressNegociacaoBaixaReceber.PercentualProcessadoFinalizacao.visible(true);
        _baixaTituloReceber.CancelarBaixa.visible(false);
        _baixaTituloReceber.SalvarObservacao.visible(false);
        _baixaTituloReceber.SalvarDatas.visible(false);
        _crudNegociacaoBaixa.FecharBaixa.enable(false);

        etapaNegociacaoClick();

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa2.idTab);
    } else {
        Etapa3Sucesso();

        _baixaTituloReceber.CancelarBaixa.visible(true);
        _baixaTituloReceber.SalvarObservacao.visible(true);
        _baixaTituloReceber.SalvarDatas.visible(true);

        etapaIntegracaoClick();

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa3.idTab);
    }

    VerificarBotoes();
}

function VerificarBotoes() {
    if ((_baixaTituloReceber.Etapa.val() == EnumEtapasBaixaTituloReceber.EmNegociacao || _baixaTituloReceber.Etapa.val() == EnumEtapasBaixaTituloReceber.Iniciada) && !_FormularioSomenteLeitura && _baixaTituloReceber.Codigo.val() > 0) {
        HabilitarTodosBotoes(true);
        _baixaTituloReceber.GerarBaixa.enable(false);
    } else {
        HabilitarTodosBotoes(false);
    }
}

function HabilitarTodosBotoes(v) {
    if (_FormularioSomenteLeitura)
        v = false;

    if (_baixaTituloReceber.Codigo.val() <= 0 && v == false && !_FormularioSomenteLeitura) {
        _baixaTituloReceber.DataBaixa.enable(true);
        _baixaTituloReceber.DataBase.enable(true);
        _baixaTituloReceber.ValorBaixado.enable(true);
        _baixaTituloReceber.Observacao.enable(true);
        _baixaTituloReceber.GerarBaixa.enable(true);
        _baixaTituloReceber.CancelarBaixa.enable(true);
        _baixaTituloReceber.SalvarObservacao.enable(true);
        _baixaTituloReceber.SalvarDatas.enable(true);
        DesabilitaCamposTitulosPendentes(true);

        v = false;
        if (_negociacaoBaixa != undefined && _negociacaoBaixa != null) {

            _parcelaNegociacaoBaixa.QuantidadeParcelas.enable(v);
            _parcelaNegociacaoBaixa.IntervaloDeDias.enable(v);
            _parcelaNegociacaoBaixa.DataPrimeiroVencimento.enable(v);
            _parcelaNegociacaoBaixa.DataEmissao.enable(v);
            _parcelaNegociacaoBaixa.TipoArredondamento.enable(v);
            _parcelaNegociacaoBaixa.GerarParcelas.enable(v);

            _crudNegociacaoBaixa.FecharBaixa.enable(v);
        }

        if (_detalheParcela != null) {
            //_detalheParcela.Valor.enable(v);
            //_detalheParcela.ValorDesconto.enable(v);
            _detalheParcela.DataVencimento.enable(v);
            _detalheParcela.SalvarParcela.enable(v);
        }

    } else {
        _baixaTituloReceber.DataBaixa.enable(v);
        _baixaTituloReceber.DataBase.enable(v);
        _baixaTituloReceber.ValorBaixado.enable(v);
        _baixaTituloReceber.GerarBaixa.enable(v);
        DesabilitaCamposTitulosPendentes(v);

        if (_negociacaoBaixa != undefined && _negociacaoBaixa != null) {

            _parcelaNegociacaoBaixa.QuantidadeParcelas.enable(v);
            _parcelaNegociacaoBaixa.IntervaloDeDias.enable(v);
            _parcelaNegociacaoBaixa.DataPrimeiroVencimento.enable(v);
            _parcelaNegociacaoBaixa.DataEmissao.enable(v);
            _parcelaNegociacaoBaixa.TipoArredondamento.enable(v);
            _parcelaNegociacaoBaixa.GerarParcelas.enable(v);

            _crudNegociacaoBaixa.FecharBaixa.enable(v);
        }

        if (_documentoNegociacaoBaixaTituloReceber != null) {
            _documentoNegociacaoBaixaTituloReceber.Documento.enable(v);
            _documentoNegociacaoBaixaTituloReceber.AcrescimoDesconto.enable(v);
            _documentoNegociacaoBaixaTituloReceber.RatearValorPagoEntreDocumentos.enable(v);
        }

        if (_chequeBaixa != null) {
            _chequeBaixa.AdicionarCheque.enable(v);
            _chequeBaixa.ChequeBaixa.enable(v);
        }

        if (_detalheParcela != null) {
            //_detalheParcela.Valor.enable(v);
            //_detalheParcela.ValorDesconto.enable(v);
            _detalheParcela.DataVencimento.enable(v);
            _detalheParcela.SalvarParcela.enable(v);
        }
    }
}