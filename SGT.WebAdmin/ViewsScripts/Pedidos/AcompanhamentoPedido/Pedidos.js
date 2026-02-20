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
/// <reference path="../../Enumeradores/EnumEtapaAcompanhamentoPedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAcompanhamentoPedido.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaAcompanhamentoPedido;
var _listaEtapas = [];

var EtapaAcompanhamentoPedido = function (dadosPedido) {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.EtapaPedido = PropertyEntity({
        text: Localization.Resources.Cargas.ControleEntrega.Pedido, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idLI: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(Localization.Resources.Cargas.ControleEntrega.VisualizarDadosPedido),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.ControleEntrega.Pedido),
        stepClass: "",
        dataToggle: "tab",
        enable: ko.observable(false)
    });

    this.EtapaColeta = PropertyEntity({
        text: Localization.Resources.Cargas.ControleEntrega.Carregamento, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idLI: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable(Localization.Resources.Cargas.ControleEntrega.VisualizarDadosColeta),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.ControleEntrega.Carregamento),
        stepClass: "",
        dataToggle: "tab",
        enable: ko.observable(false)
    });

    this.EtapaTransporte = PropertyEntity({
        text: Localization.Resources.Cargas.ControleEntrega.Transporte, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idLI: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable(Localization.Resources.Cargas.ControleEntrega.VisualizarDadosTransporte),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.ControleEntrega.Transporte),
        stepClass: "",
        dataToggle: "tab",
        enable: ko.observable(false)
    });

    this.EtapaEntrega = PropertyEntity({
        text: Localization.Resources.Cargas.ControleEntrega.Entrega, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idLI: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable(Localization.Resources.Cargas.ControleEntrega.VisualizarDadosEntrega),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.ControleEntrega.Entrega),
        stepClass: "",
        dataToggle: "tab",
        enable: ko.observable(false)
    });

    this.CodigoPedido = PropertyEntity({});

    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Numero.getFieldDescription() });
    this.NotasFiscais = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.NotasFiscais.getFieldDescription() });
    this.SituacaoAcompanhamentoPedido = PropertyEntity({});
    this.DescricaoSituacaoAcompanhamentoPedido = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.DescricaoStatusMonitoramento = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.SituacaoMonitoramento.getFieldDescription() , visible: ko.observable(false) });

    this.Remetente = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription() });
    this.Destinatario = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription() });
    this.ValorFrete = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.ValorFrete.getFieldDescription() });
    this.Origem = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription() });
    this.Destino = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription() });
    this.TipoDeCarga = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.Produto.getFieldDescription()  });

    this.Tracao = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.Tracao.getFieldDescription() });
    this.Reboques = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.Reboque.getFieldDescription() });
    this.Motoristas = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription() });

    this.DataColeta = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.DataColeta.getFieldDescription() });

    this.PossuiPassagemAduanas = PropertyEntity({ val: ko.observable(false), def: false });

    if (dadosPedido) {
        PreencherObjetoKnout(this, { Data: dadosPedido });
        setarEtapaAcompanhamentoPedido(this, dadosPedido.SituacaoAcompanhamentoPedido);
    }
}


//*******EVENTOS*******

function RegistraComponenteAcompanhamentoPedidoDadosPedido() {
    if (ko.components.isRegistered('acompanhamento-pedido-dados-pedido'))
        return;

    ko.components.register('acompanhamento-pedido-dados-pedido', {
        viewModel: DadosPedido,
        template: {
            element: 'acompanhamento-pedido-dados-pedido-templete'
        }
    });
}

function RegistraComponenteAcompanhamentoPedidoDadosColeta() {
    if (ko.components.isRegistered('acompanhamento-pedido-dados-coleta'))
        return;

    ko.components.register('acompanhamento-pedido-dados-coleta', {
        viewModel: DadosColeta,
        template: {
            element: 'acompanhamento-pedido-dados-coleta-templete'
        }
    });
}

function RegistraComponenteAcompanhamentoPedidoDadosTransporte() {
    if (ko.components.isRegistered('acompanhamento-pedido-dados-transporte'))
        return;

    ko.components.register('acompanhamento-pedido-dados-transporte', {
        viewModel: DadosTransporte,
        template: {
            element: 'acompanhamento-pedido-dados-transporte-templete'
        }
    });
}

function RegistraComponenteAcompanhamentoPedidoDadosEntrega() {
    if (ko.components.isRegistered('acompanhamento-pedido-dados-entrega'))
        return;

    ko.components.register('acompanhamento-pedido-dados-entrega', {
        viewModel: DadosEntrega,
        template: {
            element: 'acompanhamento-pedido-dados-entrega-templete'
        }
    });
}

function loadEtapaAcompanhamentoPedido() {

    RegistraComponenteAcompanhamentoPedidoDadosPedido();
    RegistraComponenteAcompanhamentoPedidoDadosColeta();
    RegistraComponenteAcompanhamentoPedidoDadosTransporte();
    RegistraComponenteAcompanhamentoPedidoDadosEntrega();

    _etapaAcompanhamentoPedido = new EtapaAcompanhamentoPedido();
    KoBindings(_etapaAcompanhamentoPedido, "knockoutEtapaAcompanhamentoPedido");
}


//*******METODOS*******
function fecharDados() {
    var steps = document.querySelectorAll("#acompanhamentoPedidoStep > .active");

    [].forEach.call(steps, function (sp) {
        sp.classList.remove("active");
        sp.classList.remove("show");
    });
}

function removerClicksEtapas(etapaAcompanhamento) {
    etapaAcompanhamento.EtapaPedido.enable(false);
    etapaAcompanhamento.EtapaColeta.enable(false);
    etapaAcompanhamento.EtapaTransporte.enable(false);
    etapaAcompanhamento.EtapaEntrega.enable(false);
}



function setarEtapaAcompanhamentoPedido(etapaAcompanhamento, situacaoAcompanhamentoPedido) {
    removerClicksEtapas(etapaAcompanhamento);
    if (situacaoAcompanhamentoPedido == EnumSituacaoAcompanhamentoPedido.AgColeta) {
        liberarClick1(etapaAcompanhamento);
        etapaAcompanhamento.EtapaPedido.stepClass = 'yellow';
    } else if (situacaoAcompanhamentoPedido == EnumSituacaoAcompanhamentoPedido.ColetaAgendada) {
        liberarEtapas1(etapaAcompanhamento);
        liberarClick2(etapaAcompanhamento);
        etapaAcompanhamento.EtapaColeta.stepClass = 'yellow';
    } else if (situacaoAcompanhamentoPedido == EnumSituacaoAcompanhamentoPedido.ColetaRejeitada) {
        liberarEtapas1(etapaAcompanhamento);
        liberarClick2(etapaAcompanhamento);
        etapaAcompanhamento.EtapaColeta.stepClass = 'red';
    } else if (situacaoAcompanhamentoPedido == EnumSituacaoAcompanhamentoPedido.EmTransporte) {
        liberarClick3(etapaAcompanhamento);
        liberarEtapas2(etapaAcompanhamento);
        etapaAcompanhamento.EtapaTransporte.stepClass = 'yellow';
    } else if (situacaoAcompanhamentoPedido == EnumSituacaoAcompanhamentoPedido.ProblemaNoTransporte) {
        liberarClick3(etapaAcompanhamento);
        liberarEtapas2(etapaAcompanhamento);
        etapaAcompanhamento.EtapaTransporte.stepClass = 'red';
    } else if (situacaoAcompanhamentoPedido == EnumSituacaoAcompanhamentoPedido.SaiuParaEntrega) {
        liberarClick4(etapaAcompanhamento);
        liberarEtapas3(etapaAcompanhamento);
        etapaAcompanhamento.EtapaEntrega.stepClass = 'yellow';
    } else if (situacaoAcompanhamentoPedido == EnumSituacaoAcompanhamentoPedido.Entregue) {
        liberarClick4(etapaAcompanhamento);
        liberarEtapas3(etapaAcompanhamento);
        etapaAcompanhamento.EtapaEntrega.stepClass = 'green';
    } else if (situacaoAcompanhamentoPedido == EnumSituacaoAcompanhamentoPedido.EntregaRejeitada) {
        liberarClick4(etapaAcompanhamento);
        liberarEtapas3(etapaAcompanhamento);
        etapaAcompanhamento.EtapaEntrega.stepClass = 'red';
    }
}

function liberarEtapas3(etapaAcompanhamento) {
    etapaAcompanhamento.EtapaTransporte.stepClass = 'green';
    liberarEtapas2(etapaAcompanhamento);
}

function liberarEtapas2(etapaAcompanhamento) {
    etapaAcompanhamento.EtapaColeta.stepClass = 'green';
    liberarEtapas1(etapaAcompanhamento);
}

function liberarEtapas1(etapaAcompanhamento) {
    etapaAcompanhamento.EtapaPedido.stepClass = 'green';
    liberarClick1(etapaAcompanhamento);
}

function liberarClick1(etapaAcompanhamento) {
    etapaAcompanhamento.EtapaPedido.enable(true);
    etapaAcompanhamento.EtapaPedido.eventClick = exibirDadosEtapaPedido;
}

function liberarClick2(etapaAcompanhamento) {
    etapaAcompanhamento.EtapaColeta.enable(true);
    etapaAcompanhamento.EtapaColeta.eventClick = exibirDadosEtapaColeta;
    liberarClick1(etapaAcompanhamento);
}

function liberarClick3(etapaAcompanhamento) {
    etapaAcompanhamento.EtapaTransporte.enable(true);
    etapaAcompanhamento.EtapaTransporte.eventClick = exibirDadosEtapaTransporte;
    liberarClick2(etapaAcompanhamento);
}

function liberarClick4(etapaAcompanhamento) {
    etapaAcompanhamento.EtapaEntrega.enable(true);
    etapaAcompanhamento.EtapaEntrega.eventClick = exibirDadosEtapaEntrega;
    liberarClick3(etapaAcompanhamento);
}