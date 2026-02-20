/// <reference path="IntegracaoBaixaTituloPagar.js" />
/// <reference path="NegociacaoBaixaTituloPagar.js" />
/// <reference path="../../Enumeradores/EnumBaixaTituloPagar.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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
/// <reference path="CabecalhoBaixaTituloPagar.js" />
/// <reference path="BaixaTituloPagar.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaBaixaTituloPagar;
var _etapaAtual;

var EtapaBaixaTituloPagar = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("30%") });

    this.Etapa1 = PropertyEntity({
        text: "Quitação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaQuitacaoClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia a baixa de um título a pagar."),
        tooltipTitle: ko.observable("Quitação")
    });
    this.Etapa2 = PropertyEntity({
        text: "Negociação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaNegociacaoClick,
        step: ko.observable(2),
        tooltip: ko.observable("Negocie o saldo devedor aplicando desconto e/ou acréscimos."),
        tooltipTitle: ko.observable("Negociação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaIntegracaoClick,
        step: ko.observable(3),
        tooltip: ko.observable("Envie as informações da baixa do título e suas negociações."),
        tooltipTitle: ko.observable("Integração")
    });
};


//*******EVENTOS*******

function loadEtapaBaixaTituloPagar() {
    _etapaBaixaTituloPagar = new EtapaBaixaTituloPagar();
    KoBindings(_etapaBaixaTituloPagar, "knockoutEtapaTituloPagar");

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
}

function etapaIntegracaoClick(e, sender) {
    _etapaAtual = 3;
    CarregarIntegracaoBaixa();
    VerificarBotoes();
}


//***ETAPA 1***

function Etapa1Liberada() {
    _etapaBaixaTituloPagar.Etapa1.eventClick = etapaQuitacaoClick;

    $("#" + _etapaBaixaTituloPagar.Etapa1.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloPagar.Etapa1.idTab + " .step").attr("class", "step lightgreen");

    Etapa2Desabilitada();
}

function Etapa1Aguardando() {
    _etapaBaixaTituloPagar.Etapa1.eventClick = etapaQuitacaoClick;

    $("#" + _etapaBaixaTituloPagar.Etapa1.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloPagar.Etapa1.idTab + " .step").attr("class", "step yellow");

    Etapa2Desabilitada();
}

function Etapa1Sucesso() {
    _etapaBaixaTituloPagar.Etapa1.eventClick = etapaQuitacaoClick;

    $("#" + _etapaBaixaTituloPagar.Etapa1.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloPagar.Etapa1.idTab + " .step").attr("class", "step green");
}

//***ETAPA 2***

function Etapa2Sucesso() {
    _etapaBaixaTituloPagar.Etapa2.eventClick = etapaNegociacaoClick;

    $("#" + _etapaBaixaTituloPagar.Etapa2.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloPagar.Etapa2.idTab + " .step").attr("class", "step green");

    Etapa1Sucesso();
}

function Etapa2Liberada() {
    _etapaBaixaTituloPagar.Etapa2.eventClick = etapaNegociacaoClick;

    $("#" + _etapaBaixaTituloPagar.Etapa2.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloPagar.Etapa2.idTab + " .step").attr("class", "step lightgreen");

    Etapa1Sucesso();
}

function Etapa2Aguardando() {
    _etapaBaixaTituloPagar.Etapa2.eventClick = etapaNegociacaoClick;

    $("#" + _etapaBaixaTituloPagar.Etapa2.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloPagar.Etapa2.idTab + " .step").attr("class", "step yellow");

    Etapa1Sucesso();
    Etapa3Desabilitada();
}

function Etapa2Desabilitada() {
    _etapaBaixaTituloPagar.Etapa2.eventClick = function () { };

    $("#" + _etapaBaixaTituloPagar.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapaBaixaTituloPagar.Etapa2.idTab + " .step").attr("class", "step");

    Etapa3Desabilitada();
}

//***ETAPA 3***

function Etapa3Sucesso() {
    _etapaBaixaTituloPagar.Etapa3.eventClick = etapaIntegracaoClick;

    $("#" + _etapaBaixaTituloPagar.Etapa3.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloPagar.Etapa3.idTab + " .step").attr("class", "step green");

    Etapa2Sucesso();
}

function Etapa3Liberada() {
    _etapaBaixaTituloPagar.Etapa3.eventClick = etapaIntegracaoClick;

    $("#" + _etapaBaixaTituloPagar.Etapa3.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloPagar.Etapa3.idTab + " .step").attr("class", "step lightgreen");

    Etapa2Sucesso();
}

function Etapa3Aguardando() {
    _etapaBaixaTituloPagar.Etapa3.eventClick = etapaIntegracaoClick;

    $("#" + _etapaBaixaTituloPagar.Etapa3.idTab).removeAttr("disabled");
    $("#" + _etapaBaixaTituloPagar.Etapa3.idTab + " .step").attr("class", "step disabled");

    Etapa2Sucesso();
}

function Etapa3Desabilitada() {
    _etapaBaixaTituloPagar.Etapa3.eventClick = function () { };

    $("#" + _etapaBaixaTituloPagar.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapaBaixaTituloPagar.Etapa3.idTab + " .step").attr("class", "step");
}

//*******MÉTODOS*******

function LimparOcultarAbas() {
    Etapa1Liberada();
}

function PosicionarEtapa(dado) {
    if (dado.Etapa === EnumEtapasBaixaTituloPagar.Iniciada && _baixaTituloPagar.Codigo.val() == 0) {
        Etapa1Liberada();

        _baixaTituloPagar.CancelarBaixa.visible(false);
        _baixaTituloPagar.SalvarObservacao.visible(false);

        etapaQuitacaoClick();

        Global.ExibirStep(_etapaBaixaTituloPagar.Etapa1.idTab);

    } else if (dado.Etapa === EnumEtapasBaixaTituloPagar.Iniciada) {
        Etapa2Liberada();

        _baixaTituloPagar.CancelarBaixa.visible(true);
        _baixaTituloPagar.SalvarObservacao.visible(true);

        etapaNegociacaoClick();

        Global.ExibirStep(_etapaBaixaTituloPagar.Etapa2.idTab);

    } else if (dado.Etapa === EnumEtapasBaixaTituloPagar.EmNegociacao) {
        Etapa2Liberada();

        _baixaTituloPagar.CancelarBaixa.visible(true);
        _baixaTituloPagar.SalvarObservacao.visible(true);

        etapaNegociacaoClick();

        Global.ExibirStep(_etapaBaixaTituloPagar.Etapa2.idTab);

    } else if (dado.Etapa === EnumEtapasBaixaTituloPagar.Cancelada) {
        Etapa3Sucesso();

        _baixaTituloPagar.CancelarBaixa.visible(false);
        _baixaTituloPagar.SalvarObservacao.visible(false);

        etapaQuitacaoClick();

        Global.ExibirStep(_etapaBaixaTituloPagar.Etapa1.idTab);
    }
    else {
        Etapa3Sucesso();

        _baixaTituloPagar.CancelarBaixa.visible(true);
        _baixaTituloPagar.SalvarObservacao.visible(true);

        etapaIntegracaoClick();

        Global.ExibirStep(_etapaBaixaTituloPagar.Etapa3.idTab);
    }

    VerificarBotoes();
}

function VerificarBotoes() {
    if (_negociacaoBaixa != undefined && _negociacaoBaixa != null)
        _negociacaoBaixa.ImprimirRecibo.visible(false);
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.EmNegociacao && !_FormularioSomenteLeitura && _baixaTituloPagar.Codigo.val() > 0) {
        HabilitarTodosBotoes(true);
    } else if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Iniciada && !_FormularioSomenteLeitura && _baixaTituloPagar.Codigo.val() > 0) {
        HabilitarTodosBotoes(true);
    } else if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada && !_FormularioSomenteLeitura && _baixaTituloPagar.Codigo.val() > 0) {
        HabilitarTodosBotoes(false);
    } else if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada && !_FormularioSomenteLeitura && _baixaTituloPagar.Codigo.val() > 0) {
        HabilitarTodosBotoes(false);
        if (_negociacaoBaixa != undefined && _negociacaoBaixa != null)
            _negociacaoBaixa.ImprimirRecibo.visible(true);
    }
    else {
        HabilitarTodosBotoes(false);
    }
}

function HabilitarTodosBotoes(v) {
    if (_FormularioSomenteLeitura)
        v = false;

    if (_baixaTituloPagar.Codigo.val() <= 0 && v == false && !_FormularioSomenteLeitura) {
        _baixaTituloPagar.DataBaixa.enable(true);
        _baixaTituloPagar.ValorBaixado.enable(true);
        _baixaTituloPagar.Observacao.enable(true);
        _baixaTituloPagar.BaixarTitulo.enable(true);
        _baixaTituloPagar.CancelarBaixa.enable(true);
        _baixaTituloPagar.SalvarObservacao.enable(true);

        v = false;
        if (_negociacaoBaixa != null) {
            //_negociacaoBaixa.ValorTipoPagamento.enable(v);
            //_negociacaoBaixa.AdicionarTipoDePagamento.enable(v);
            _negociacaoBaixa.TipoDePagamento.enable(v);
            _negociacaoBaixa.AdicionarAcrescimoDesconto.enable(v);
            _negociacaoBaixa.AcrescimosDescontos.enable(v);

            _negociacaoBaixa.QuantidadeParcelas.enable(v);
            _negociacaoBaixa.IntervaloDeDias.enable(v);
            _negociacaoBaixa.DataPrimeiroVencimento.enable(v);
            _negociacaoBaixa.DataEmissao.enable(v);
            _negociacaoBaixa.TipoArredondamento.enable(v);
            _negociacaoBaixa.GerarParcelas.enable(v);
            if (_negociacaoBaixa.PessoaNegociacao != undefined)
                _negociacaoBaixa.PessoaNegociacao.enable(v);
            _negociacaoBaixa.FecharBaixa.enable(v);
        }

        if (_detalheParcela != null) {
            _detalheParcela.Valor.enable(v);
            _detalheParcela.ValorDesconto.enable(v);
            _detalheParcela.DataVencimento.enable(v);
            _detalheParcela.SalvarParcela.enable(v);
            _detalheParcela.FormaTitulo.enable(v);
            //_detalheParcela.NumeroBoleto.enable(v);
            //_detalheParcela.Portador.enable(v);
            //_detalheParcela.ValorOriginalMoedaEstrangeira.enable(v);
        }

    } else {
        _baixaTituloPagar.Documento.visible(v);
        _baixaTituloPagar.DataBaixa.enable(v);
        _baixaTituloPagar.ValorBaixado.enable(v);
        _baixaTituloPagar.BaixarTitulo.enable(v);

        if (_negociacaoBaixa != null) {
            _negociacaoBaixa.ValorTipoPagamento.enable(v);
            _negociacaoBaixa.AdicionarTipoDePagamento.enable(v);
            _negociacaoBaixa.TipoDePagamento.enable(v);
            _negociacaoBaixa.AdicionarAcrescimoDesconto.enable(v);
            _negociacaoBaixa.AcrescimosDescontos.enable(v);

            _negociacaoBaixa.QuantidadeParcelas.enable(v);
            _negociacaoBaixa.IntervaloDeDias.enable(v);
            _negociacaoBaixa.DataPrimeiroVencimento.enable(v);
            _negociacaoBaixa.DataEmissao.enable(v);
            _negociacaoBaixa.TipoArredondamento.enable(v);
            _negociacaoBaixa.GerarParcelas.enable(v);
            if (_negociacaoBaixa.PessoaNegociacao != undefined)
                _negociacaoBaixa.PessoaNegociacao.enable(v);
            _negociacaoBaixa.FecharBaixa.enable(v);
        }

        if (_detalheParcela != null) {
            _detalheParcela.Valor.enable(v);
            _detalheParcela.ValorDesconto.enable(v);
            _detalheParcela.DataVencimento.enable(v);
            _detalheParcela.SalvarParcela.enable(v);
            _detalheParcela.FormaTitulo.enable(v);
            _detalheParcela.NumeroBoleto.enable(v);
            _detalheParcela.Portador.enable(v);
            _detalheParcela.ValorOriginalMoedaEstrangeira.enable(v);
        }
    }
}